# Technical Architecture - STARLINE: LAST SIGNAL

## Engine & Tech Stack

- **Engine**: Unity 2022 LTS
- **Primary Language**: C#
- **Networking**: Mirror or Photon 2 (TBD)
- **Build Targets**: PC, PlayStation 5, Xbox Series X/S

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────┐
│              User Interface Layer                    │
│  (Menus, HUD, FieldLink-9, Squad Management)       │
└─────────────────────────────────────────────────────────┘
                        ↕
┌─────────────────────────────────────────────────────────┐
│           Game Logic Layer                          │
│  (Missions, Combat, Movement, Building, Economy)   │
└─────────────────────────────────────────────────────────┘
                        ↕
┌─────────────────────────────────────────────────────────┐
│          Systems Layer                              │
│  (Physics, Audio, Networking, Events, Persistence) │
└─────────────────────────────────────────────────────────┘
                        ↕
┌─────────────────────────────────────────────────────────┐
│              Unity Engine                           │
│  (Rendering, Physics, Input, Asset Management)     │
└─────────────────────────────────────────────────────────┘
```

## Core Systems

### 1. Game Manager

**Responsibility**: Central orchestration of game state and lifecycle.

```csharp
public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState { get; private set; }
    
    public void StartMission(MissionData missionData)
    public void CompleteMission(MissionResult result)
    public void FailMission(FailureReason reason)
    public void PauseMission()
    public void ResumeMission()
}

public enum GameState
{
    Menu,
    Loading,
    InMission,
    Paused,
    Extraction,
    MissionComplete,
    MissionFailed
}
```

### 2. Player System

**Components**:
- **PlayerController**: Input handling and top-level control
- **MovementSystem**: Fluid parkour mechanics
- **CombatSystem**: Damage, weapons, abilities
- **BuildSystem**: Equipment and loadout management
- **HealthSystem**: Damage, healing, death

```csharp
public class PlayerController : MonoBehaviour
{
    private MovementSystem _movementSystem;
    private CombatSystem _combatSystem;
    private BuildSystem _buildSystem;
    private HealthSystem _healthSystem;
    
    private void Update() { /* Input polling */ }
    private void FixedUpdate() { /* Physics */ }
    
    public void TakeDamage(float amount, DamageType type)
    public void Die()
    public void Revive()
}
```

### 3. Movement System

**Features**:
- Momentum-based physics
- Chainable movement abilities
- Environmental interaction
- Animation blending

```csharp
public class MovementSystem : MonoBehaviour
{
    private float _currentSpeed;
    private float _currentVerticalVelocity;
    private MovementState _state;
    
    // Core abilities
    public void Slide(Vector3 direction)
    public void WallRun(Collider wall)
    public void DoubleJump()
    public void TripleJump()
    public void AirDash(Vector3 direction)
    public void GroundSlam()
    public void GrappleHook(Transform target)
    
    // Helpers
    private void ApplyMomentumPreservation()
    private void ChainMovementAbility(MovementAbility next)
    private bool CanPerformAbility(MovementAbility ability)
}

public enum MovementState
{
    Idle,
    Running,
    Sliding,
    Airborne,
    WallRunning,
    Grappling,
    Stunned
}
```

### 4. Combat System

**Components**:
- Weapon management
- Ability system
- Targeting and aim assist
- Damage calculations

```csharp
public class CombatSystem : MonoBehaviour
{
    private Weapon _primaryWeapon;
    private Weapon _secondaryWeapon;
    private Weapon _meleeWeapon;
    private Ability _suitAbility;
    private Ability _ultimateAbility;
    
    public void Fire(bool isPrimary = true)
    public void AltFire(bool isPrimary = true)
    public void MeleeAttack()
    public void ActivateSuitAbility()
    public void ActivateUltimate()
    
    public void OnDamageReceived(DamageInfo damage)
}

public class Weapon
{
    public WeaponType Type { get; }
    public float Damage { get; }
    public float FireRate { get; }
    public int AmmoCapacity { get; }
    public int CurrentAmmo { get; private set; }
    
    public void Fire(Vector3 origin, Vector3 direction)
    public void Reload()
    public void AltFire(Vector3 origin, Vector3 direction)
}
```

### 5. Mission System

**Responsibility**: Mission generation, objectives, state tracking.

```csharp
public class MissionManager : Singleton<MissionManager>
{
    private CurrentMission _currentMission;
    private List<Objective> _activeObjectives;
    
    public void StartMission(MissionDifficulty difficulty, PlanetData planet)
    public void CompleteObjective(Objective objective)
    public void FailObjective(Objective objective)
    public bool CanExtract() => /* All critical objectives complete */
    public void InitiateExtraction()
}

public abstract class Objective
{
    public string Description { get; protected set; }
    public bool IsComplete { get; protected set; }
    public bool IsCritical { get; protected set; }
    
    public abstract void OnUpdate();
    public abstract void OnComplete();
}

public class CollectResourceObjective : Objective
{
    private ResourceType _resourceType;
    private int _targetAmount;
    private int _currentAmount;
    
    public override void OnUpdate() { /* Check collection progress */ }
}

public class DefenseObjective : Objective
{
    private Transform _defensePoint;
    private float _duration;
    
    public override void OnUpdate() { /* Track defense time */ }
}
```

### 6. Loot & Extraction System

**Key Features**:
- Physical loot objects with weight
- Teamwork requirements for large items
- Limited dropship capacity
- Risk vs. reward decisions

```csharp
public class LootSystem : MonoBehaviour
{
    private List<LootObject> _availableLoot;
    private List<LootObject> _collectedLoot;
    
    public void SpawnLoot(ResourceType type, int quantity)
    public void CollectLoot(LootObject loot, PlayerController collector)
    public void DropLoot(LootObject loot)
    public bool CanFitInDropship(LootObject loot) => /* Capacity check */
}

public class LootObject : MonoBehaviour
{
    public ResourceType Type { get; }
    public float Weight { get; }
    public int Value { get; }
    public bool RequiresTeamwork { get; }
    
    public void OnPickup(PlayerController player)
    public void OnDrop()
}

public class ExtractionManager : MonoBehaviour
{
    private Dropship _dropship;
    public float CargoCapacity { get; } = 1000f;
    public float CurrentCargo { get; private set; }
    
    public bool TryLoadLoot(LootObject loot)
    public void UnloadLoot()
    public void Extract(Action onComplete)
}
```

### 7. Enemy AI System

**Architecture**:
- Behavior trees for decision-making
- Squad-based coordination
- Alien race-specific behaviors

```csharp
public abstract class AIController : MonoBehaviour
{
    protected BehaviorTree _behaviorTree;
    protected Squad _squad;
    protected TargetingSystem _targeting;
    
    public virtual void OnUpdate()
    public virtual void OnSquadCommand(SquadCommand command)
    public abstract void OnPlayerDetected(PlayerController player)
}

public class MyrAIController : AIController
{
    // Swarm coordination
    private List<MyrAIController> _nestmates;
    
    public override void OnUpdate()
    {
        // Use hive mind behavior
        CoordinateWithNest();
        SearchForBiomass();
    }
    
    private void CoordinateWithNest() { }
    private void SearchForBiomass() { }
}

public class TitanAIController : AIController
{
    // Individual decision making
    private BehaviorTree _complexTree;
    
    public override void OnUpdate()
    {
        // Advanced tactical decisions
        _behaviorTree.Update();
    }
}
```

### 8. Persistence System

**Manages**:
- Player progression
- Ship upgrades
- Planet occupation
- Squad inventory

```csharp
public class PersistenceManager : Singleton<PersistenceManager>
{
    public void SaveGame(GameSaveData data)
    public GameSaveData LoadGame()
    public void SaveMissionResult(MissionResult result)
    public void UpdatePlanetOccupation(Planet planet, float occupationChange)
}

public class GameSaveData
{
    public PlayerProgressionData PlayerData;
    public ShipData ShipData;
    public Dictionary<Planet, PlanetData> GalacticState;
    public long Timestamp;
}
```

### 9. Event System

**Pattern**: Event-driven architecture for loose coupling.

```csharp
public class GameEvents
{
    public static UnityEvent<PlayerController> PlayerDied = new();
    public static UnityEvent<PlayerController, int> LootCollected = new();
    public static UnityEvent<Mission> MissionStarted = new();
    public static UnityEvent<Mission> MissionCompleted = new();
    public static UnityEvent<Enemy> EnemySpawned = new();
    public static UnityEvent<Enemy> EnemyDefeated = new();
}

// Usage:
GameEvents.PlayerDied.AddListener(OnPlayerDeath);
GameEvents.LootCollected.AddListener((player, value) => OnLootCollected(player, value));
```

## Networking Architecture

**TBD - To be determined based on testing**

Options:
- Mirror: Open-source, good for indie games
- Photon 2: Commercial, good for console support

**Requirements**:
- Up to 4 players per mission
- < 100ms latency tolerance
- Server authority for anti-cheat
- Player state synchronization

## Memory Management

### Object Pooling

For frequently created/destroyed objects:

```csharp
public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _availableObjects;
    private List<T> _activeObjects;
    
    public T GetObject()
    public void ReturnObject(T obj)
}

// Usage:
private ObjectPool<Bullet> _bulletPool;

public void Fire(Vector3 origin, Vector3 direction)
{
    var bullet = _bulletPool.GetObject();
    bullet.Initialize(origin, direction);
}
```

### Resource Management

- LOD (Level of Detail) for distant enemies
- Texture atlasing
- Audio streaming
- Unload unused assets

## Data Structures

### Scriptable Objects for Configuration

```csharp
[CreateAssetMenu(fileName = "WeaponConfig", menuName = "STARLINE/Weapons/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    public string WeaponName;
    public float Damage;
    public float FireRate;
    public int AmmoCapacity;
    public AudioClip FireSound;
}
```

## Performance Targets

| Metric | Target |
|--------|--------|
| FPS (Console) | 60 |
| FPS (PC High-End) | 120 |
| FPS (PC Mid-Range) | 60 |
| Frame Time | < 16.67ms (60fps) |
| Memory | < 4GB active |
| Draw Calls | < 2000 |
| Batch Calls | < 500 |
| Polygons (Outdoor) | 2M |
| Polygons (Indoor) | 500K |
