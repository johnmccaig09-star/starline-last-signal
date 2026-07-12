using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

namespace STARLINE.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private GameState _currentState = GameState.Menu;
        private CurrentMission _currentMission;
        private List<Player> _activePlayers = new();
        private float _missionElapsedTime;
        private bool _isPaused;

        public GameState CurrentState => _currentState;
        public CurrentMission CurrentMission => _currentMission;
        public List<Player> ActivePlayers => _activePlayers;
        public float MissionElapsedTime => _missionElapsedTime;
        public bool IsPaused => _isPaused;

        public event Action<GameState> OnGameStateChanged;
        public event Action<CurrentMission> OnMissionStarted;
        public event Action<MissionResult> OnMissionCompleted;
        public event Action<FailureReason> OnMissionFailed;
        public event Action OnGamePaused;
        public event Action OnGameResumed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystems();
        }

        private void InitializeSystems()
        {
            AudioManager.Initialize();
            PersistenceManager.Initialize();
        }

        private void Update()
        {
            if (_currentState == GameState.InMission && !_isPaused)
            {
                _missionElapsedTime += Time.deltaTime;
            }
        }

        public void StartMission(MissionData missionData)
        {
            if (_currentState != GameState.Menu && _currentState != GameState.MissionFailed && _currentState != GameState.MissionComplete)
            {
                Debug.LogWarning("Cannot start mission while already in mission");
                return;
            }

            _currentMission = new CurrentMission(missionData);
            _missionElapsedTime = 0f;
            _isPaused = false;
            ChangeGameState(GameState.Loading);
            
            OnMissionStarted?.Invoke(_currentMission);
            SceneManager.LoadScene("MissionScene", LoadSceneMode.Single);
            ChangeGameState(GameState.InMission);
        }

        public void CompleteMission(MissionResult result)
        {
            if (_currentState != GameState.InMission && _currentState != GameState.Extraction)
            {
                Debug.LogWarning("Cannot complete mission");
                return;
            }

            ChangeGameState(GameState.MissionComplete);
            PersistenceManager.SaveMissionResult(result);
            OnMissionCompleted?.Invoke(result);
        }

        public void FailMission(FailureReason reason)
        {
            if (_currentState != GameState.InMission && _currentState != GameState.Extraction)
                return;

            ChangeGameState(GameState.MissionFailed);
            OnMissionFailed?.Invoke(reason);
        }

        public void PauseMission()
        {
            if (_currentState != GameState.InMission || _isPaused)
                return;

            _isPaused = true;
            Time.timeScale = 0f;
            ChangeGameState(GameState.Paused);
            OnGamePaused?.Invoke();
        }

        public void ResumeMission()
        {
            if (!_isPaused)
                return;

            _isPaused = false;
            Time.timeScale = 1f;
            ChangeGameState(GameState.InMission);
            OnGameResumed?.Invoke();
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            _isPaused = false;
            ChangeGameState(GameState.Menu);
            SceneManager.LoadScene("MenuScene");
        }

        public void InitiateExtraction()
        {
            if (_currentState != GameState.InMission)
                return;

            ChangeGameState(GameState.Extraction);
        }

        public void RegisterPlayer(Player player)
        {
            if (!_activePlayers.Contains(player))
            {
                _activePlayers.Add(player);
            }
        }

        public void UnregisterPlayer(Player player)
        {
            _activePlayers.Remove(player);
            
            if (_activePlayers.Count == 0 && _currentState == GameState.InMission)
            {
                FailMission(FailureReason.AllPlayersDefeated);
            }
        }

        private void ChangeGameState(GameState newState)
        {
            if (_currentState == newState)
                return;

            _currentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
    }

    public enum GameState { Menu, Loading, InMission, Paused, Extraction, MissionComplete, MissionFailed }
    public enum FailureReason { AllPlayersDefeated, ObjectivesFailed, ExtractionTimeout, EnvironmentalHazard, BossDefeated }
    public enum ObjectiveType { Collect, Defend, Eliminate, Explore }
    public enum PlanetType { Grassland, Volcanic, Arctic, Desert, Oceanic, Crystal, Swamp, Urban, Jungle, Artificial }
    public enum DifficultyLevel { Easy, Normal, Hard, Nightmare, Impossible }
    public enum AlienRaceType { Myr, KhorDominion, VeyCollective, Eclipsed, Titans }

    [System.Serializable]
    public class CurrentMission
    {
        public MissionData Data { get; }
        public List<Objective> ActiveObjectives { get; } = new();
        public Dictionary<string, int> CollectedResources { get; } = new();
        public int EnemiesDefeated { get; set; }
        public float MissionScore { get; set; }

        public CurrentMission(MissionData data)
        {
            Data = data;
            InitializeObjectives();
        }

        private void InitializeObjectives()
        {
            foreach (var objectiveData in Data.Objectives)
            {
                Objective objective = objectiveData.Type switch
                {
                    ObjectiveType.Collect => new CollectResourceObjective(objectiveData),
                    ObjectiveType.Defend => new DefenseObjective(objectiveData),
                    ObjectiveType.Eliminate => new EliminationObjective(objectiveData),
                    ObjectiveType.Explore => new ExploreObjective(objectiveData),
                    _ => null
                };

                if (objective != null)
                    ActiveObjectives.Add(objective);
            }
        }

        public bool AreAllCriticalObjectivesComplete()
        {
            foreach (var objective in ActiveObjectives)
            {
                if (objective.IsCritical && !objective.IsComplete)
                    return false;
            }
            return true;
        }
    }

    [System.Serializable]
    public class MissionData
    {
        public string MissionName;
        public PlanetType PlanetType;
        public DifficultyLevel Difficulty;
        public ObjectiveData[] Objectives;
        public int RewardCredits;
        public float TimeLimit;
        public AlienRaceType PrimaryEnemy;
    }

    [System.Serializable]
    public class ObjectiveData
    {
        public ObjectiveType Type;
        public string Description;
        public bool IsCritical;
        public int TargetCount;
        public float TimeLimit;
    }

    [System.Serializable]
    public class Player
    {
        public int Id;
        public string Name;
        public int Level;
        public int Health;
        public Vector3 Position;
        public int Score;
    }

    public abstract class Objective
    {
        public string Description { get; set; }
        public bool IsCritical { get; set; }
        public bool IsComplete { get; set; }
        public float Progress { get; set; }
        public abstract void UpdateProgress();
    }

    public class CollectResourceObjective : Objective
    {
        public string ResourceType { get; set; }
        public int TargetAmount { get; set; }
        public int CurrentAmount { get; set; }
        public override void UpdateProgress() => Progress = (float)CurrentAmount / TargetAmount;
    }

    public class DefenseObjective : Objective
    {
        public float DurationRequired { get; set; }
        public float ElapsedTime { get; set; }
        public override void UpdateProgress() => Progress = ElapsedTime / DurationRequired;
    }

    public class EliminationObjective : Objective
    {
        public int TargetCount { get; set; }
        public int CurrentCount { get; set; }
        public override void UpdateProgress() => Progress = (float)CurrentCount / TargetCount;
    }

    public class ExploreObjective : Objective
    {
        public int PointsToDiscover { get; set; }
        public int PointsDiscovered { get; set; }
        public override void UpdateProgress() => Progress = (float)PointsDiscovered / PointsToDiscover;
    }
}
