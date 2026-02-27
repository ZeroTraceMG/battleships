# Iron Tide: Naval Supremacy — Asset Pipeline

**Version:** 1.0 | **Status:** Approved | **Date:** 2026-02-27

---

## 1. Pipeline Overview

```
Concept (Midjourney/Sketch)
    ↓
3D Modeling (Blender 4.x)
    ↓
UV Unwrap (Blender / RizomUV)
    ↓
Texturing (Armor Paint / Substance Painter)
    ↓
Export FBX → Import Unity
    ↓
Material Assignment (URP/Lit or custom ShaderGraph)
    ↓
LOD Setup + Prefab Assembly
    ↓
VFX / Audio attach
    ↓
Playtest → Iterate
```

All assets pass through this pipeline in sequence. No asset skips a step.

---

## 2. Naming Convention

Consistent naming is required. Unity's asset database is case-sensitive. Follow exactly.

### 2.1 3D Meshes (FBX)

```
SHIP_[Archetype]_[LOD].fbx           SHIP_Frigate_LOD0.fbx
SHIP_[Archetype]_[LOD].fbx           SHIP_Frigate_LOD1.fbx
WPN_[Type]_[Variant].fbx             WPN_Cannon_Heavy.fbx
ENV_[Category]_[Name].fbx            ENV_Island_VolcanicA.fbx
ENV_[Category]_[Name].fbx            ENV_Harbor_DockSectionA.fbx
PROP_[Name].fbx                      PROP_Bollard_Iron.fbx
```

### 2.2 Textures (PNG)

```
T_[Mesh]_Albedo.png                  T_SHIP_Frigate_Albedo.png
T_[Mesh]_Normal.png                  T_SHIP_Frigate_Normal.png
T_[Mesh]_MetallicSmooth.png          T_SHIP_Frigate_MetallicSmooth.png
T_[Mesh]_Emission.png                T_SHIP_Frigate_Emission.png
T_[Mesh]_Albedo_Damaged.png          T_SHIP_Frigate_Albedo_Damaged.png
```

Packed texture channel convention:
- `MetallicSmooth`: **R** = Metallic, **A** = Smoothness (Unity URP standard)
- `OcclusionEmission`: **R** = Occlusion, **G** = Emission mask (where applicable)

### 2.3 Materials, Prefabs, Effects, Audio

```
M_[Mesh]_[Variant].mat               M_SHIP_Frigate_Default.mat
M_[Mesh]_[Variant].mat               M_SHIP_Frigate_Damaged.mat
PFB_[Category]_[Name].prefab         PFB_Ship_Frigate.prefab
PFB_[Category]_[Name].prefab         PFB_Projectile_Torpedo.prefab
VFX_[Name].prefab                    VFX_CannonExplosion.prefab
VFX_[Name].prefab                    VFX_TorpedoImpact.prefab
SFX_[Category]_[Name].wav            SFX_Weapon_CannonFire.wav
SFX_[Category]_[Name].wav            SFX_Ship_FrigateEngine.wav
MUS_[Name].ogg                       MUS_Gameplay_Combat.ogg
```

---

## 3. Texture Specifications

### 3.1 Resolution by Asset Category

| Asset Type | Resolution | Maps Required |
|------------|-----------|--------------|
| Ship LOD0 | 2048×2048 | Albedo, Normal, MetallicSmooth, Emission (optional) |
| Ship LOD1 | 1024×1024 | Albedo, Normal, MetallicSmooth |
| Environment (hero) | 2048×2048 | Albedo, Normal, MetallicSmooth |
| Environment (tiled) | 1024×1024 | Albedo, Normal |
| UI elements | Max 512×512 | Albedo only (no normal) |
| VFX sprite sheet | 256×256 per frame | Albedo + Alpha |
| VFX sprite atlas | Max 2048×2048 | Combined sprite sheet |

All textures must be power-of-2 dimensions.

### 3.2 Compression Settings by Platform

| Platform | Format | Quality Setting |
|----------|--------|----------------|
| PC (Windows) | BC7 | High quality |
| Android | ASTC 6×6 | Standard |
| iOS | ASTC 6×6 | Standard |
| Editor (development) | None / uncompressed | — |

Override per-platform in Unity Texture Importer. Do not use DXT1/DXT5 for new assets.

### 3.3 Color Space

- All **Albedo / Emission** textures: **sRGB** color space
- All **Normal / MetallicSmooth / Occlusion** textures: **Linear** color space
- Set this in Unity Texture Importer (`sRGB (Color Texture)` toggle)

---

## 4. Blender Export Settings

Use these exact settings every time. Save as a Blender export preset named `IronTide_FBX`.

### 4.1 FBX Export Panel Settings

```
Include:
  Object Types:   Mesh (+ Armature only if animated)
  Custom Props:   OFF

Transform:
  Scale:          0.01         ← Blender m → Unity units
  Apply Scalings: FBX All
  Forward:        -Z Forward
  Up:             Y Up
  Apply Transform: YES         ← Apply all rotations before export

Geometry:
  Smoothing:      Face         ← hard edges via split normals
  Export Subdiv:  OFF
  Apply Modifiers: YES
  Tangent Space:  YES          ← required for normal map baking
  Triangulate Faces: OFF       ← Unity importer handles triangulation

Armatures: (only for animated meshes)
  Only Deform Bones: YES
  Add Leaf Bones: OFF
```

### 4.2 Scene Setup Before Export

1. Apply all transforms: `Object → Apply → All Transforms` (Ctrl+A → All)
2. Set origin to geometry center (or to specific pivot point as agreed per asset)
3. Remove all modifiers not needed in Unity (Subdivision Surface → apply at LOD0 level)
4. Verify mesh has no N-gons in topology areas that deform or tile
5. Name the mesh object exactly as the target filename (without `.fbx` extension)

### 4.3 LOD Mesh Requirements

| LOD | Trigger Distance | Max Triangle Count |
|-----|-----------------|-------------------|
| LOD0 | 0–20m | Ship: 8,000 tris. Prop: 2,000 tris |
| LOD1 | 20–60m | Ship: 2,500 tris. Prop: 500 tris |
| Culled | >60m | Fully culled |

Export LOD meshes as **separate FBX files** (`SHIP_Frigate_LOD0.fbx`, `SHIP_Frigate_LOD1.fbx`).
Unity's LOD Group component will merge them into a single prefab.

---

## 5. Unity Import Settings

### 5.1 Ships

In the FBX Importer Inspector:

```
Model tab:
  Scale Factor:          1          (FBX already has correct scale from Blender)
  Mesh Compression:      Medium
  Read/Write Enabled:    OFF        (GPU-only, saves memory)
  Optimize Mesh:         Everything
  Generate Colliders:    OFF        (use separate collider mesh)
  Normals:               Import
  Normals Mode:          Import
  Tangents:              Calculate Mikktspace

Rig tab:
  Animation Type:        None       (unless animated)

Materials tab:
  Material Creation:     None       (create materials manually)
```

After import, assemble the LOD Group:
1. Create empty GameObject named `PFB_Ship_[Archetype]`
2. Add LOD0 mesh as child (`SHIP_Frigate_LOD0`)
3. Add LOD1 mesh as child (`SHIP_Frigate_LOD1`)
4. Add `LOD Group` component to root — assign LOD0 (0–20m), LOD1 (20–60m), Culled (>60m)
5. Assign Layer: **Ships** (Layer 6)
6. Add a separate invisible child `Collider` with simplified BoxCollider or custom mesh collider

### 5.2 Environment (Static)

```
Model tab:
  Same as ships, plus:
  Generate Lightmap UVs: YES       (UV2 for baked lighting)

Static Flags (on GameObject):
  ✓ Batching Static
  ✓ Occluder Static
  ✓ Occludee Static
  ✓ Navigation Static    (for pathfinding exclusion zones)
  ✗ Lightmap Static      (dynamic time-of-day — baked ambient only)

Layer:
  Navigable terrain:     MapObjects (Layer 8)
  Decorative props:      Default
```

### 5.3 VFX Textures

```
Texture Type:   Sprite (2D and UI) for sprite sheets
               Default for tileable textures

Sprite Mode:    Multiple (sprite sheet) — define Sprite Editor slices

Wrap Mode:      Clamp     (sprite sheets)
               Repeat    (tileable textures)
Filter Mode:    Bilinear

Alpha Source:   From Grayscale (where texture has no alpha channel)
Alpha Is Transparent: YES
```

### 5.4 Audio

```
All SFX files:
  Load Type:              Compressed In Memory
  Compression Format:     Vorbis
  Quality:                70
  Force To Mono:          YES    (3D spatial audio — stereo wastes memory)
  Normalize:              YES

Music files (.ogg):
  Load Type:              Streaming
  Compression Format:     Vorbis
  Quality:                80
  Force To Mono:          NO     (music stays stereo)
```

---

## 6. Shader Assignments

| Asset Category | Shader | Notes |
|----------------|--------|-------|
| Ships | `Universal Render Pipeline/Lit` | Metallic workflow, custom `_DamageBlend` keyword |
| Water | `Custom/Water` (ShaderGraph) | `Water.shadergraph` — see Water spec in ART_DIRECTION §4.1 |
| Environment | `Universal Render Pipeline/Lit` | Detail normal for close-up rock/concrete |
| UI elements | `UI/Default` or `Universal Render Pipeline/Unlit` | No lighting calculations |
| VFX particles | `Universal Render Pipeline/Particles/Unlit` | Additive blend for energy effects |
| VFX trails | `Universal Render Pipeline/Particles/Unlit` | Alpha blend for trails |

### 6.1 Ship Material Variants

Each ship has two material variants set up from day one:

| Variant | When Used | Changes |
|---------|-----------|---------|
| `M_SHIP_[Archetype]_Default.mat` | Normal gameplay | Full color, emission at normal intensity |
| `M_SHIP_[Archetype]_Damaged.mat` | Below 30% HP | Darker albedo, rust/char overlays, emission reduced |

Switch triggered via `ShipVisualController.cs` by subscribing to the HP threshold event.

---

## 7. Folder Structure

```
Assets/
└── _Game/
    ├── Art/
    │   ├── Ships/
    │   │   ├── Frigate/
    │   │   │   ├── Meshes/          ← FBX files
    │   │   │   ├── Textures/        ← PNG files
    │   │   │   └── Materials/       ← .mat files
    │   │   ├── Corvette/
    │   │   ├── Destroyer/
    │   │   └── Ironclad/
    │   ├── Environment/
    │   │   ├── Water/               ← Water.shadergraph + textures
    │   │   ├── Islands/
    │   │   └── Harbor/
    │   ├── UI/
    │   │   ├── Fonts/               ← Rajdhani + Exo 2 TTF files
    │   │   ├── Icons/               ← SVG/PNG icons
    │   │   ├── Atlases/             ← Sprite atlas assets
    │   │   └── Textures/            ← UI background patches
    │   └── VFX/
    │       ├── Textures/            ← VFX sprite sheets
    │       └── Prefabs/             ← VFX_*.prefab
    ├── Audio/
    │   ├── SFX/
    │   │   ├── Ships/
    │   │   ├── Weapons/
    │   │   └── Environment/
    │   └── Music/
    └── Prefabs/
        ├── Ships/                   ← PFB_Ship_*.prefab
        ├── Projectiles/             ← PFB_Projectile_*.prefab
        └── Environment/             ← PFB_Harbor_*.prefab
```

---

## 8. Phase-by-Phase Art Milestones

Art production runs parallel to code development. Milestone gates below must be met before
advancing to the next art phase.

| Phase | Code State | Art Deliverable | Gate |
|-------|-----------|----------------|------|
| Phase 1 (grey-box) | Prototype playable | Placeholder cubes/cylinders only. No textures. | Game loops run with placeholders |
| Phase 2 (block art) | MVP vs AI | Final silhouettes, base materials (flat colors). No fine detail textures. | All 4 ship archetypes distinguishable at 20m |
| Phase 3 (alpha art) | PvP Alpha | Full LOD0 textures, VFX first pass, audio first pass | QA-approved visual pass |
| Phase 4 (polish) | Dedicated Server | Damage states, cosmetic variant slots, final UI skin | Store screenshot quality |
| Phase 5 (ship) | Ranked Beta | Cosmetic variant skins, seasonal assets, store icons | Platform submission ready |

### Phase 2 Block Art Checklist

Before considering Phase 2 art complete:
- [ ] All 4 ships have final silhouette geometry (no placeholder cubes)
- [ ] All 4 ships have base flat-color materials matching palette (`#1A2332` hull, correct accents)
- [ ] Ship silhouettes are distinguishable at 64×64 resolution
- [ ] Water shader has at least 1 scrolling normal map (no flat blue fill)
- [ ] Harbor has basic dock geometry (no floating ships)
- [ ] At least 1 island blocking element per lane

---

## 9. Quality Checklist (Per Asset)

Run this before marking any asset as complete:

- [ ] Named correctly (matches naming convention in §2)
- [ ] FBX exported with correct settings (§4.1 settings applied)
- [ ] All texture channels correct (sRGB vs Linear, §3.3)
- [ ] Compression set per platform (§3.2)
- [ ] LOD Group configured with correct distances (§5.1)
- [ ] Material variants exist (Default + Damaged for ships)
- [ ] Layer assigned correctly (§5.1)
- [ ] Asset reviewed at minimap scale (64×64 visibility test)
- [ ] No floating geometry or inverted normals
- [ ] Prefab assembled and saved to correct folder (§7)

---

*Cross-reference `ART_DIRECTION.md` for exact hex values, material spec, and VFX colors.*
*Cross-reference `TOOLS_PLAN.md` for tool versions and license info.*
