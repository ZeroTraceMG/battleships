# Iron Tide: Naval Supremacy — Art Direction Bible

**Version:** 1.0 | **Status:** Approved | **Date:** 2026-02-27

---

## 1. Identity Statement

> **"Heavy industry meets deep ocean science."**

Iron Tide is set in a world where Prohibition-era diesel engineering collided with alien bio-ocean
technology. The aesthetic is **dieselpunk meets deep-sea sci-fi**: battered factory trawlers
retrofitted with plasma cannons, bio-energy reactors humming beneath corroded hulls, and a
bioluminescent ocean that glows with something older than humanity.

**Tone keywords:** Industrial brutalism · Wet metal · Amber heat · Cyan cold · Diesel grit ·
Alien wonder

**Not:** Steampunk whimsy · Clean sci-fi chrome · Fantasy medieval · WW2 realism

---

## 2. Color Palette (Canonical Hex Values)

All artists must use these exact values. Reference this table before creating any asset.

### 2.1 Hull Colors

| Name | Hex | Usage |
|------|-----|-------|
| Deep Steel | `#1A2332` | Ship body (darkest base coat) |
| Oxidised Metal | `#2E3D4F` | Mid panels, secondary surfaces |
| Brushed Iron | `#4A5568` | Edge highlights, wear scratches, rivets |

### 2.2 Energy Accents — Allied / Weapons

| Name | Hex | Usage |
|------|-----|-------|
| Plasma Amber | `#D97706` | Cannon charge glow, muzzle flash base |
| Engine Gold | `#F59E0B` | Exhaust vents, energy conduit lines |
| Reactor Core | `#FCD34D` | Critical energy state, overcharge VFX |

### 2.3 Ocean / Sci-Fi

| Name | Hex | Usage |
|------|-----|-------|
| Biolum Cyan | `#06B6D4` | Water sheen, torpedo trails, sonar rings |
| Deep Teal | `#0D9488` | Ocean depth gradient, underwater fog |
| Void Navy | `#0A1628` | Water base (darkest), deep shadow |

### 2.4 Danger

| Name | Hex | Usage |
|------|-----|-------|
| Threat Red | `#DC2626` | Enemy team color, low-HP warning, hostile UI |
| Warning Orange | `#EA580C` | Caution states, "under attack" ring |

### 2.5 UI Chrome

| Name | Hex | Usage |
|------|-----|-------|
| Brass Gold | `#92400E` | Panel borders, rivet accents, separators |
| Aged Copper | `#78350F` | Secondary chrome, background detail |
| HUD Glass | `rgba(6,182,212,0.15)` | Panel backgrounds (semi-transparent cyan tint) |

---

## 3. Ship Visual Language

Each ship archetype has a **mandatory silhouette profile**, **material palette**, and **VFX signature**.
These rules ensure ships are immediately readable in combat at any distance.

---

### 3.1 Corvette — Wraith-class

**Role:** Scout / Harasser

| Property | Specification |
|----------|--------------|
| Silhouette | Narrow, low profile. Swept-back superstructure barely above waterline. No external turrets. |
| Beam/Length ratio | ~1:8 (very long relative to width) |
| Hull material | Matte Dark Steel (`#1A2332`) with minimal surface variation |
| Accent | Cyan engine vent glow at stern (pair of slotted exhausts) |
| Deck detail | Minimal: flush hatches, no railings, one low periscope mast |
| No-nos | No visible turrets. No tall superstructure. No wake foam at speed (cuts cleanly). |

**VFX signature:**
- Twin cyan engine trails (thin, tubular, fade after 0.3s)
- No bow foam (hull is submerged profile)
- Speed blur distortion on max throttle (screen-space post effect, subtle)

---

### 3.2 Frigate — Bulwark-class

**Role:** Balanced Brawler

| Property | Specification |
|----------|--------------|
| Silhouette | Medium beam, single rotating gun turret amidships, prominent enclosed bridge |
| Beam/Length ratio | ~1:5 |
| Hull material | Riveted steel panels, visible weld seams, light surface rust on lower hull |
| Accent | Amber searchlight mounted on bridge, brass fittings at portholes |
| Deck detail | Railing sections, lifebuoy rings, radar dish (non-functional prop) |

**VFX signature:**
- Amber muzzle flash ×2 (barrel pair) — wide flash, 3-frame hold, smoke trail follows
- White foam V-wake at bow, dissipates after 1.5s
- Searchlight beam (volumetric cone, amber `#D97706`, sweeps in idle)

---

### 3.3 Destroyer — Specter-class

**Role:** Torpedo Striker

| Property | Specification |
|----------|--------------|
| Silhouette | Long and narrow with 4 visible torpedo tube mounts angled outward at 30° |
| Beam/Length ratio | ~1:9 (second longest after Corvette) |
| Hull material | Dark matte (`#1A2332` + `#2E3D4F`) — no shine, combat-worn |
| Accent | Cyan torpedo bay glow (interior light visible through slotted tube openings) |
| Deck detail | Four torpedo launchers (2 per side), low profile bridge, no large superstructure |

**VFX signature:**
- Cyan torpedo wake streak (tight bioluminescent line, lingers 1.0s)
- Bubble trail on launch (white/cyan burst from tube mouth, 0.3s)
- Impact: deep cyan burst expanding ring + white foam blast

---

### 3.4 Ironclad — Leviathan-class

**Role:** Siege / Anchor

| Property | Specification |
|----------|--------------|
| Silhouette | Wide blunt bow, three turrets (fore × 1, amidships × 1, aft × 1), smokestack cluster (3 pipes) |
| Beam/Length ratio | ~1:4 (widest ship) |
| Hull material | Heavy riveted armour — thick overlapping plates, prominent rust staining on lower hull |
| Accent | Brass porthole trim (warm gold ring around each circular window), orange cannon flash glow |
| Deck detail | Anchor chain at bow, heavy cable runs between stacks, anti-air gun props (non-functional) |

**VFX signature:**
- Black diesel smoke from stacks (always-on idle, thickens under acceleration)
- Deep orange cannon flash (`#EA580C` core → `#D97706` fringe) — large, 5-frame hold
- Shockwave ring on broadside fire (subtle ground-level ripple, radius 3m)
- Deep bow wake + churned foam (heaviest wake of any ship)

---

## 4. Environment Art Direction

### 4.1 Water Shader

The water is the most visible surface in the game. It must read as **living and alien**, not realistic.

**ShaderGraph implementation:**
- 2× scrolling normal maps at different scale and speed ratios (1.0× speed, 0.6× speed, 60° angle offset)
- Depth fog: samples `SceneDepth`, lerps `Void Navy (#0A1628)` → `Biolum Cyan (#06B6D4)` over 4m depth
- Foam at object edges: Fresnel + intersection thickness fade
- Bioluminescent sparkle: VFX Graph particle layer (always-on, low density, additive blend)
- Night ambient: cool blue directional light + warm amber point lights near harbor structures

**Performance note:** Mobile uses 1 normal map only. VFX Graph sparkle disabled on low-end devices.

---

### 4.2 Islands and Rocks

| Property | Specification |
|----------|--------------|
| Rock type | Dark volcanic basalt — near-black with blue-grey undertone (`#1A2332` family) |
| Bio-growth | Cyan/green bio-moss in crevices (emission enabled, subtle glow `#06B6D4` at 0.3 intensity) |
| Edge treatment | Worn, rounded — no sharp geological fractures |
| Wet surfaces | Emission rim light samples water color below (dynamic, baked fallback on mobile) |
| Scale reference | Smallest rock: 3m wide. Impassable island: 20–50m wide |

---

### 4.3 Harbor Structures

| Property | Specification |
|----------|--------------|
| Material | Concrete + weathered steel + corroded pipe networks |
| Color | Concrete: mid grey `#4A5568`. Steel: `#2E3D4F` with rust patches |
| Lighting | Team-color searchlights sweep the water (slow, rhythmic sweep, 6s cycle) |
| Signage | Neon tube signs in Art Deco lettering — amber tubes for neutral, team-color for team base |
| Smokestacks | Emit gentle idle exhaust (grey, low density, constant) |
| Dock | Wooden planks darkened with diesel spill, iron bollards, hanging chains |

---

## 5. UI Art Direction — Art Deco Industrial

### 5.1 Typography

| Role | Font | Weight | Notes |
|------|------|--------|-------|
| Headers / Ship names | Rajdhani | Bold | All-caps preferred. Download: Google Fonts |
| Body text / tooltips | Exo 2 | Regular | Mix of caps/lower, best at 12–14pt |
| Numbers (HP, gold, timer) | Rajdhani | SemiBold | Tabular figures, right-align in columns |

Both fonts are free under the SIL Open Font License (OFL). Embed in project at `Assets/_Game/UI/Fonts/`.

---

### 5.2 Panel Style

| Element | Specification |
|---------|--------------|
| Background | HUD Glass: `rgba(6,182,212,0.15)` — semi-transparent dark with cyan tint |
| Border | 2px solid `Brass Gold (#92400E)` with 4px circular rivet marks at each corner |
| Inner separator | 1px solid `Oxidised Metal (#2E3D4F)` horizontal lines |
| Corner radius | 2px max — sharp-cornered industrial look |

---

### 5.3 HUD Elements

**HP Bar:**
- Pressure-gauge style (horizontal fill, not just a flat bar)
- Fill color: `Threat Red (#DC2626)`, empties left-to-right
- Needle indicator on current value (vertical tick mark)
- Background: `#1A2332` dark trough

**Gold Display:**
- Brass coin icon (circular, `Brass Gold (#92400E)` tint) to the left
- Rajdhani SemiBold number in `Engine Gold (#F59E0B)`
- Earn event: brief amber pulse animation (scale 1.0→1.15→1.0 over 0.3s)

**Cooldown:**
- Circular arc fill in `Biolum Cyan (#06B6D4)` (sweeps clockwise)
- Icon desaturates + darkens when on cooldown (not disabled — still visible)
- Cooldown complete: brief cyan flash ring

**Minimap:**
- Porthole circle frame (circular mask)
- Brass rim (`Brass Gold #92400E`), 4px wide
- Background: `Void Navy (#0A1628)`
- Ally ship dots: `Engine Gold (#F59E0B)`
- Enemy ship dots: `Threat Red (#DC2626)`
- Map objects (islands, objectives): `Brushed Iron (#4A5568)`

**Kill Feed:**
- Right side of screen, vertical stack
- Each entry: skull icon + attacker ship name + "→" + victim ship name
- Fades out after 5 seconds (alpha fade, 0.5s)
- Font: Exo 2 Regular, white text

---

### 5.4 Ability Buttons (Mobile)

- **Shape:** Hexagonal (flat-top orientation)
- **Fill:** Team color at 70% opacity when ready
- **Cooldown overlay:** Clock-wipe (sweep from 12 o'clock, dark overlay)
- **On press:** Scale punch (1.0→0.9→1.0 over 0.15s)
- **Disabled:** Full grey desaturate + lock icon

---

## 6. VFX Palette

Every visual effect must map to this reference. Do not use arbitrary colors.

| Effect | Primary Color | Secondary | Notes |
|--------|--------------|-----------|-------|
| Cannon fire — flash | `Plasma Amber #D97706` | White core | 3–5 frame hold, additive blend |
| Cannon — shell trail | `Engine Gold #F59E0B` | Faint amber | Thin line renderer, fades 0.2s |
| Cannon — explosion | `Warning Orange #EA580C` | Black smoke | Volumetric smoke overlay |
| Torpedo — launch | `Biolum Cyan #06B6D4` | White bubble | Sphere burst from tube |
| Torpedo — trail | `Biolum Cyan #06B6D4` | `Deep Teal #0D9488` | Submerged line, biolum blend |
| Torpedo — impact | `Biolum Cyan #06B6D4` | White foam | Expanding ring + upward splash |
| Smoke Screen | Diesel black `#111827` | `Biolum Cyan #06B6D4` wisps | Particle blend, lingers 4s |
| Sonar ping | `Biolum Cyan #06B6D4` | Transparent | Expanding ring, fades at edge |
| Repair field | `#22C55E` green | Circuit line pattern | Pulse 0.5Hz, on hull surface |
| Harbor win explosion | Multi-stage | — | Diesel smoke → fire column → shockwave |
| Water wake | White foam | `Biolum Cyan` sparkle | V-shape behind hull |
| Respawn | `Brushed Iron #4A5568` | `Biolum Cyan #06B6D4` | Ship rises from below + energy ripple |

---

## 7. Audio Mood Reference

Audio direction is defined here for concept alignment. See `ASSET_PIPELINE.md §5` for file specs.

### 7.1 Ship Engine Audio

| Ship | Sound Character |
|------|----------------|
| Corvette (Wraith) | High-pitch turbine whine + rhythmic clicks at idle |
| Frigate (Bulwark) | Medium diesel thrum, rhythmic mechanical chug |
| Destroyer (Specter) | Low rumble, hydraulic hiss from torpedo tube hydraulics |
| Ironclad (Leviathan) | Deep diesel roar, metal groaning and structural creak on turns |

### 7.2 Weapon Audio

| Weapon | Sound Profile |
|--------|--------------|
| Cannon | Deep BOOM with short reverb tail. Brass shell ejection clink (separate layer) |
| Torpedo | Hiss-splash on launch → underwater whoosh (low-pass filtered) → deep THUD on hit |
| Smoke Screen | Pressurized hiss (valve open) → soft sustained whoosh |
| Sonar | Classic PING, underwater echo with natural reverb decay |
| Repair | Electrical hum, ascending tone (pitch rises while active) |

### 7.3 Environment Audio

| Layer | Description |
|-------|-------------|
| Ocean (base) | Layered wave loops at 3 distances (near/mid/far) |
| Harbor | Dock creak, water lap against pylons, steam release, distant machinery drone |
| Wind | Light high-frequency layer on open water, volume follows ship speed |

### 7.4 Music Direction

| State | Style |
|-------|-------|
| Gameplay Phase 1 | Industrial percussion + brass ostinato (tense, sparse) |
| Gameplay Phase 2 | Full orchestral + synthesizer layer (driving, action) |
| Victory | Short brass fanfare, 8-bar, major key resolution |
| Defeat | Descending brass phrase, slow fade, minor key |
| Lobby/Menu | Ambient industrial drone + slow arpeggiated brass (not distracting) |

---

## 8. Concept Art Reference Process

1. Generate mood boards in **Midjourney v6** using prompts referencing: diesel engines, deep sea biology, industrial decay, bioluminescence, Art Deco lettering.
2. Generate tileable texture candidates in **Stable Diffusion** (ComfyUI) for: rusted metal, riveted steel, bio-organic growth, wet rock.
3. All AI-generated art is **reference only** — final assets are original geometry + hand-painted texture maps.
4. Ship silhouettes must be recognizable at 64×64 px resolution (minimap scale). Test every new ship model at this size before proceeding.

---

*This document is the canonical visual authority for Iron Tide. All asset creators must read and confirm understanding before beginning production work.*
