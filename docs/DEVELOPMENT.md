# Development Guide - STARLINE: LAST SIGNAL

## Project Setup

### Prerequisites
- Unity 2022 LTS or later
- C# 9.0+
- Git
- Git LFS (for large binary files)

### Initial Setup

```bash
# Clone the repository
git clone https://github.com/johnmccaig09-star/starline-last-signal.git
cd starline-last-signal

# Initialize Git LFS
git lfs install

# Install dependencies
# (Instructions pending)
```

## Development Structure

### Folder Organization

```
src/
├── core/                    # Core game systems
│   ├── GameManager.cs
│   ├── PersistenceManager.cs
│   └── EventSystem.cs
├── gameplay/                # Gameplay mechanics
│   ├── player/
│   │   ├── PlayerController.cs
│   │   ├── MovementSystem.cs
│   │   ├── CombatSystem.cs
│   │   └── BuildSystem.cs
│   ├── enemy/
│   │   ├── EnemyBase.cs
│   │   └── alien_races/
│   ├── extraction/
│   │   ├── LootSystem.cs
│   │   ├── ExtractionManager.cs
│   │   └── DropshipUI.cs
│   ├── missions/
│   │   ├── MissionManager.cs
│   │   ├── MissionGenerator.cs
│   │   └── ObjectiveSystem.cs
│   └── planets/
│       ├── PlanetData.cs
│       ├── BiomeGenerator.cs
│       └── OccupationSystem.cs
├── ui/                      # User interface
│   ├── menus/
│   │   ├── MainMenu.cs
│   │   ├── MissionSelect.cs
│   │   └── SquadLoadout.cs
│   ├── hud/
│   │   ├── HUD.cs
│   │   ├── ObjectiveTracker.cs
│   │   └── ResourceCounter.cs
│   └── fieldlink/
│       ├── FieldLink9.cs
│       ├── TacticalMenu.cs
│       └── FieldLink9UI.cs
├── ai/                      # Enemy AI systems
│   ├── AIController.cs
│   ├── decision_systems/
│   └── alien_behaviors/
│       ├── MyrBehavior.cs
│       ├── KhorBehavior.cs
│       ├── VeyBehavior.cs
│       ├── EclipsedBehavior.cs
│       └── TitanBehavior.cs
├── audio/                   # Audio systems
│   ├── AudioManager.cs
│   └── SoundEffects.cs
└── utils/                   # Utility classes
    ├── ObjectPool.cs
    ├── MathHelpers.cs
    └── Constants.cs
```

## Coding Standards

### Naming Conventions

- **Classes**: PascalCase (e.g., `PlayerController`)
- **Methods**: PascalCase (e.g., `GetPlayerHealth()`)
- **Properties**: PascalCase (e.g., `CurrentAmmo`)
- **Private Fields**: _camelCase (e.g., `_playerHealth`)
- **Constants**: UPPER_SNAKE_CASE (e.g., `MAX_SQUAD_SIZE`)
- **Interfaces**: IPascalCase (e.g., `IExtractionTarget`)

### Code Style

```csharp
// Use explicit access modifiers
public class PlayerController : MonoBehaviour
{
    // Private fields at top
    private float _health = 100f;
    private Rigidbody _rigidbody;
    
    // Public properties
    public float Health => _health;
    public bool IsAlive { get; private set; } = true;
    
    // Method structure
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        // Update logic
    }
    
    public void TakeDamage(float amount)
    {
        _health -= amount;
        if (_health <= 0) Die();
    }
    
    private void Die()
    {
        IsAlive = false;
        // Death logic
    }
}
```

## Version Control

### Branch Naming

- `main` - Production-ready code
- `develop` - Integration branch
- `feature/feature-name` - New features
- `bugfix/bug-name` - Bug fixes
- `docs/doc-name` - Documentation updates

### Commit Messages

Use clear, concise commit messages:

```
[CATEGORY] Brief description

Optional longer description explaining the why and how.

Category options:
- FEAT: New feature
- FIX: Bug fix
- REFACTOR: Code refactoring
- DOCS: Documentation
- STYLE: Formatting
- TEST: Adding tests
- CHORE: Maintenance
```

Example:
```
[FEAT] Implement fluid movement system

Added core movement abilities:
- Slide with momentum preservation
- Wall run and wall jump
- Double and triple jump
- Air dash for directional control

Movement chains together seamlessly for battlefield traversal.
```

## Testing

### Unit Tests

Create unit tests for critical systems:

```csharp
[TestFixture]
public class MovementSystemTests
{
    [Test]
    public void Slide_ReducesVerticalVelocity()
    {
        // Arrange
        var player = CreateTestPlayer();
        player.Velocity = new Vector3(10, 5, 0);
        
        // Act
        player.Slide();
        
        // Assert
        Assert.Less(player.Velocity.y, 5);
    }
}
```

### Integration Tests

Test systems working together:

```csharp
[UnityTest]
public IEnumerator ExtractionFlow_CompletesSuccessfully()
{
    // Setup
    var mission = CreateTestMission();
    var player = CreateTestPlayer();
    var loot = CreateTestLoot();
    
    // Player collects loot
    player.PickupLoot(loot);
    
    // Player reaches dropship
    player.Move(dropshipPosition);
    yield return new WaitForSeconds(2f);
    
    // Verify
    Assert.True(mission.IsCompleted);
}
```

## Performance Guidelines

### Target Specifications

- **Target FPS**: 60 FPS on console, 120 FPS on high-end PC
- **Memory**: Optimize for 8GB+ available
- **Multiplayer**: Support up to 4 players per mission
- **Draw Calls**: < 2000 per frame
- **Polygon Budget**: ~2M polys for outdoor areas, ~500K for indoor

### Optimization Checklist

- [ ] Use object pooling for frequently created/destroyed objects
- [ ] Implement LOD (Level of Detail) for distant objects
- [ ] Use static batching where appropriate
- [ ] Profile with Profiler regularly
- [ ] Cache frequently accessed components
- [ ] Use coroutines instead of Update for non-critical logic

## Building and Deployment

### Build Configurations

```bash
# Development build (debug symbols, dev UI)
unity -buildPath build/dev -scriptingBackend mono -buildOptions Development

# Release build (optimized, no debug symbols)
unity -buildPath build/release -scriptingBackend IL2CPP
```

## Documentation Standards

### File Headers

```csharp
/// <summary>
/// Manages player movement, abilities, and traversal.
/// Handles fluid parkour mechanics inspired by Warframe and ULTRAKILL.
/// </summary>
public class MovementSystem : MonoBehaviour
{
    /// <summary>
    /// Performs a slide, reducing vertical velocity and gaining horizontal speed.
    /// </summary>
    /// <param name="direction">Direction to slide in</param>
    /// <returns>True if slide was successful</returns>
    public bool Slide(Vector3 direction)
    {
        // Implementation
    }
}
```

## Useful Resources

- [Unity Documentation](https://docs.unity.com)
- [C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [Game Design Patterns](https://www.informit.com/articles/article.aspx?p=1623551)

## Getting Help

For questions or issues:
1. Check existing documentation
2. Search GitHub issues
3. Create a new issue with detailed description
4. Contact the development team
