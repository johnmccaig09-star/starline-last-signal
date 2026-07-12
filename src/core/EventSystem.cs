using UnityEngine;
using UnityEngine.Events;
using System;

namespace STARLINE.Core
{
    public static class GameEvents
    {
        public static UnityEvent<Player> PlayerSpawned = new();
        public static UnityEvent<Player> PlayerDied = new();
        public static UnityEvent<Player, float> PlayerHealthChanged = new();
        public static UnityEvent<Enemy, float> EnemyTookDamage = new();
        public static UnityEvent<Enemy> EnemyDefeated = new();
        public static UnityEvent<Enemy> EnemySpawned = new();
        public static UnityEvent<Weapon> WeaponFired = new();
        public static UnityEvent<LootObject, Player> LootCollected = new();
        public static UnityEvent<LootObject> LootSpawned = new();
        public static UnityEvent<Objective> ObjectiveCompleted = new();
        public static UnityEvent<Objective> ObjectiveFailed = new();
        public static UnityEvent<int> ScoreUpdated = new();
        public static UnityEvent OnExtractionStarted = new();
        public static UnityEvent OnExtractionComplete = new();
        public static UnityEvent<float> ExtractionProgressChanged = new();
        public static UnityEvent<string> NotificationDisplayed = new();
        public static UnityEvent<Weapon> WeaponEquipped = new();
        public static UnityEvent<Ability> AbilityActivated = new();
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        private float _masterVolume = 1f;
        private float _sfxVolume = 0.8f;
        private float _musicVolume = 0.6f;
        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        public static void Initialize()
        {
            if (Instance == null)
            {
                GameObject obj = new("AudioManager");
                Instance = obj.AddComponent<AudioManager>();
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
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip, _sfxVolume * volume * _masterVolume);
        }

        public void PlayMusic(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            _musicSource.clip = clip;
            _musicSource.volume = _musicVolume * volume * _masterVolume;
            _musicSource.Play();
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
        }
    }
}
