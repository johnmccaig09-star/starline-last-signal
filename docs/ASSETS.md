# Asset Pipeline - STARLINE: LAST SIGNAL

## Asset Categories

### 3D Models

#### Characters
- **Player Models**: Customizable armor frames with modular parts
- **Enemies**: 5 alien races with multiple variants per race
- **NPCs**: Crew members, medics, engineers
- **Bosses**: Mini-bosses and raid bosses

#### Environments
- **Biome Tilesets**: Modular pieces for rapid level creation
- **Structures**: Bases, ruins, facilities
- **Vehicles**: Dropship, rovers, transport vessels
- **Props**: Crates, barrels, debris, collectibles

#### Effects
- **Particle Systems**: Weapon effects, explosions, environmental effects
- **Impact Decals**: Bullet holes, blast marks, burn marks
- **VFX**: Ability effects, energy effects, environmental hazards

### Textures & Materials

#### Specification
- **Resolution**: 2K (2048x2048) for hero assets, 1K for props
- **Format**: PNG for lossless (authoring), converted to platform-specific for builds
- **Maps**: Albedo, Normal, Roughness, Metallic, AO, Emission
- **Platforms**: Optimized variants for console vs PC

#### Organization
```
Assets/
├── Textures/
│   ├── Characters/
│   ├── Environments/
│   ├── Weapons/
│   └── UI/
```

### Audio

#### Music
- **Ambience**: Per-biome atmospheric tracks
- **Combat**: Dynamic music layers
- **Boss Themes**: Memorable boss music
- **Menu**: Title and lobby themes

#### Sound Effects
- **Weapons**: Unique sounds per weapon type
- **Abilities**: Whoosh, impact, activation sounds
- **Enemies**: Vocalizations, footsteps, attacks
- **UI**: Click, select, confirm sounds
- **Environment**: Ambient loops, hazard warnings

#### Voice
- **Mission Briefing**: VO for mission objectives
- **Radio Chatter**: Squad communication
- **NPC Dialogue**: Crew interactions

### Animations

#### Character
- **Movement**: Idle, walk, run, sprint, slide, jump variants
- **Combat**: Attack animations per weapon type, reload, ability cast
- **Emotes**: Social animations, celebration, pain reactions
- **Interactions**: Door opening, switch activation, object pickup

#### Enemies
- **Locomotion**: Walk, run, strafe, attack approach
- **Combat**: Attack variations, death animations
- **Special**: Ability animations, transformation sequences

### UI Assets

#### Sprite Sheets
- **HUD Elements**: Health bars, ammo counters, minimap
- **FieldLink-9**: UI icons, menu buttons
- **Icons**: Weapons, items, abilities, status effects
- **Fonts**: Clear sci-fi font for readability

### Configuration Files

#### Scriptable Objects
```csharp
// WeaponConfig.asset
[CreateAssetMenu]
public class WeaponConfig : ScriptableObject
{
    public string weaponName;
    public Mesh model;
    public Material[] materials;
    public AudioClip fireSound;
    public ParticleSystem muzzleFlash;
    public float damage;
    public float fireRate;
}
```

## Asset Pipeline Workflow

### Creation Phase

1. **Concept Art**: Design passes, style guide adherence
2. **Modeling**: High-poly sculpts, topology optimization
3. **Texturing**: PBR workflow, consistency across assets
4. **Rigging**: Skeleton setup, weight painting
5. **Animation**: Motion capture reference, keyframe animation

### Implementation Phase

1. **Import**: Configure import settings in Unity
2. **Optimization**: LOD creation, texture atlasing
3. **Integration**: Prefabs, material assignment, effect setup
4. **Testing**: Performance review, visual passes

### Quality Assurance

1. **Visual QA**: Consistency, visual fidelity
2. **Performance QA**: Polycount, texture memory, draw calls
3. **Audio QA**: Balance, quality, compression
4. **Animation QA**: Smoothness, responsiveness, transition quality

## Asset Standards

### 3D Model Standards

**Player Character**
- Polycount: 30K-50K LOD 0, 15K-20K LOD 1, 5K-10K LOD 2
- Rig: 75-100 bones
- Animation count: 40+ core animations

**Enemy Characters**
- Polycount: 20K-40K per enemy type
- Rig: 50-80 bones
- Animation count: 20-30 per enemy type

**Environmental Props**
- Polycount: 500-5000 per prop
- Texture atlasing: Up to 8 props per 2K atlas

### Texture Standards

**Resolution Hierarchy**
- Hero Assets: 2K
- Secondary Assets: 1K
- Tertiary Props: 512x512

**Compression**
- PC: RGB Compressed (BC7)
- Console: Platform-optimized compression

### Animation Standards

**Movement Animations**
- Frame rate: 24 fps gameplay
- Blend tree transitions: 0.1-0.3 seconds
- Root motion: Where appropriate for movement

**Combat Animations**
- Attack duration: 0.3-1.0 seconds
- Recovery frames: 0.2-0.5 seconds
- Combo windows: 0.1-0.2 seconds

## Version Control for Assets

### Using Git LFS

```bash
# Track large binary files
git lfs track "Assets/**/*.fbx"
git lfs track "Assets/**/*.png"
git lfs track "Assets/**/*.wav"

# Lock files during editing to prevent conflicts
git lfs lock "Assets/Models/Player.fbx"
```

### Directory Structure

```
Assets/
├── Characters/
│   ├── Player/
│   │   ├── Models/
│   │   ├── Rigs/
│   │   ├── Animations/
│   │   └── Materials/
│   └── Enemies/
│       └── [AlienRace]/
├── Environments/
│   ├── [BiomeType]/
│   │   ├── Models/
│   │   ├── Materials/
│   │   └── Textures/
├── Weapons/
│   ├── Models/
│   ├── Materials/
│   └── Effects/
├── Audio/
│   ├── Music/
│   ├── SFX/
│   └── Voice/
├── UI/
│   ├── Icons/
│   ├── Fonts/
│   └── Sprites/
├── Effects/
│   ├── Particles/
│   └── PostProcessing/
└── Prefabs/
    ├── Characters/
    ├── Enemies/
    └── Interactive/
```

## Export Standards

### 3D Model Export

**Format**: FBX 2020
**Settings**:
- Include: Geometry, Materials, Textures, Animations, Deformed Models
- Tangent and Binormal: YES
- Smooth Groups: YES
- Split Per Vertex Normals: YES
- Bake Animation: NO (for rigged models)

### Texture Export

**Format**: PNG (24-bit or 32-bit for transparency)
**Naming Convention**:
- `[AssetName]_[MapType]_[Variation].png`
- Example: `plasma_rifle_Albedo.png`, `plasma_rifle_Normal.png`

### Audio Export

**Format**: WAV (24-bit, 48kHz for music, 16-bit 44.1kHz for SFX)
**Compression**: OPUS or platform-specific within Unity
**Naming**: `[EffectType]_[Variant]_[Intensity].wav`

## Performance Targets

| Asset Type | Target | Maximum |
|------------|--------|----------|
| Player Polycount | 40K | 60K |
| Enemy Polycount | 25K | 40K |
| Environment Polycount | 2M | 2.5M |
| Texture Memory | 200MB | 350MB |
| Audio Memory | 50MB | 100MB |
| Draw Calls | < 500 | < 700 |

## Tools & Software

**Modeling**
- Autodesk Maya
- Blender (free alternative)

**Texturing**
- Substance Painter
- Marmoset Toolbag

**Animation**
- Maya Animation
- Blender Animation
- MotionBuilder (for mocap)

**Audio**
- FMOD Studio
- Wwise
- Audacity (free alternative)

**Version Control**
- Git with Git LFS
- Perforce (optional for large teams)

## Asset Review Checklist

- [ ] Model topology is clean and optimized
- [ ] Textures follow PBR guidelines
- [ ] Materials are properly assigned
- [ ] Animations are smooth and properly looped
- [ ] Audio levels are normalized
- [ ] All files follow naming conventions
- [ ] Documentation is up to date
- [ ] Asset meets polycount targets
- [ ] LODs are created and tested
- [ ] Performance impact is acceptable
