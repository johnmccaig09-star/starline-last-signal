using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace STARLINE.Core
{
    public class PersistenceManager : MonoBehaviour
    {
        public static PersistenceManager Instance { get; private set; }
        private GameSaveData _currentSaveData;
        private string _savePath;
        private const string SAVE_FILE = "starline_save.json";

        public GameSaveData CurrentSaveData => _currentSaveData;

        public static void Initialize()
        {
            if (Instance == null)
            {
                GameObject obj = new("PersistenceManager");
                Instance = obj.AddComponent<PersistenceManager>();
                DontDestroyOnLoad(obj);
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE);
            LoadGame();
        }

        public void SaveGame()
        {
            try
            {
                string json = JsonUtility.ToJson(_currentSaveData, true);
                File.WriteAllText(_savePath, json);
                Debug.Log($"Game saved to {_savePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        public void LoadGame()
        {
            try
            {
                if (File.Exists(_savePath))
                {
                    string json = File.ReadAllText(_savePath);
                    _currentSaveData = JsonUtility.FromJson<GameSaveData>(json);
                    Debug.Log("Game loaded successfully");
                }
                else
                {
                    CreateNewGame();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                CreateNewGame();
            }
        }

        public void CreateNewGame()
        {
            _currentSaveData = new GameSaveData
            {
                PlayerLevel = 1,
                Credits = 5000,
                TotalMissionsCompleted = 0,
                TotalScore = 0,
                PlayTime = 0,
                LastSaveTime = DateTime.Now.ToString()
            };
            SaveGame();
        }

        public void SaveMissionResult(MissionResult result)
        {
            _currentSaveData.TotalMissionsCompleted++;
            _currentSaveData.Credits += result.CreditsEarned;
            _currentSaveData.TotalScore += result.Score;
            _currentSaveData.LastSaveTime = DateTime.Now.ToString();
            SaveGame();
        }

        public void UpdatePlanetOccupation(string planetName, float occupationChange)
        {
            if (!_currentSaveData.PlanetOccupation.ContainsKey(planetName))
            {
                _currentSaveData.PlanetOccupation[planetName] = 100f;
            }

            _currentSaveData.PlanetOccupation[planetName] = Mathf.Clamp(
                _currentSaveData.PlanetOccupation[planetName] + occupationChange,
                0f,
                100f
            );

            SaveGame();
        }
    }

    [System.Serializable]
    public class GameSaveData
    {
        public int PlayerLevel;
        public int Credits;
        public int TotalMissionsCompleted;
        public float TotalScore;
        public float PlayTime;
        public string LastSaveTime;
        public Dictionary<string, float> PlanetOccupation = new();
        public List<string> UnlockedWeapons = new();
        public Dictionary<string, int> ShipUpgrades = new();
    }

    [System.Serializable]
    public class MissionResult
    {
        public bool Success;
        public int Score;
        public int CreditsEarned;
        public Dictionary<string, int> ResourcesCollected = new();
        public int EnemiesDefeated;
        public float MissionDuration;
    }
}
