# Iron Tide: Naval Supremacy — Tools Plan

**Version:** 1.0 | **Status:** Approved | **Date:** 2026-02-27

---

## 1. Unity Engine & Core Packages

### 1.1 Engine

| Tool | Version | License | Role |
|------|---------|---------|------|
| Unity | 6 LTS (6000.0.x) | Unity Personal / Pro | Game engine, build system, editor |
| Unity Hub | Latest | Free | Version management, project launcher |

**Installation:** Download Unity Hub → install Unity 6 LTS via Hub → add Android Build Support, Windows Build Support (IL2CPP), iOS Build Support modules.

### 1.2 Unity Packages (via Package Manager)

| Package | Version | License | Role |
|---------|---------|---------|------|
| Universal Render Pipeline (URP) | 17.0.x | Unity | Rendering, ShaderGraph, VFX Graph |
| Unity Input System | 1.8.x | Unity | Cross-platform input (keyboard/mouse/touch) |
| Cinemachine | 3.x | Unity | Camera follow, cinematic transitions |
| TextMeshPro | 3.x | Unity | High-quality text rendering, UI |
| ProBuilder | Latest | Unity | Rapid in-editor map prototyping |
| VFX Graph | 17.0.x | Unity | GPU particle systems (explosions, trails) |
| Unity Test Framework | 1.4.x | Unity | EditMode + PlayMode automated tests |
| Device Simulator | Latest | Unity | Mobile screen size preview in Editor |
| Unity Profiler | Built-in | Unity | CPU/GPU/memory performance analysis |
| Burst Compiler | Latest | Unity | DOTS-aligned math optimizations |
| Collections | Latest | Unity | NativeArray, NativeList for performance code |

### 1.3 Third-Party Unity Packages

| Package | Version | License | Source | Role |
|---------|---------|---------|--------|------|
| Mirror Networking | Latest stable | MIT | `https://github.com/MirrorNetworking/Mirror` | Multiplayer networking framework |
| KCP Transport | Bundled with Mirror | MIT | Mirror package | UDP transport layer (low latency) |

**Mirror install:** Add via Package Manager → `+ Add package from git URL` → `https://github.com/MirrorNetworking/Mirror.git#latest`

---

## 2. AI & Copilot Tools

| Tool | Version | License / Cost | Role in Project |
|------|---------|---------------|----------------|
| GitHub Copilot | Current | $10/mo Individual | Inline code autocomplete in VS Code / Rider |
| Claude Code | claude-sonnet-4-6 | Anthropic subscription | Architecture design, debugging, SO data gen, balance tuning |
| Midjourney | v6 | $10–30/mo | Concept art: ship silhouettes, mood boards, UI mockups |
| Stable Diffusion | SDXL 1.0 | Free (local) | Texture generation: tiling metal, rust, bio-organic patterns |
| ComfyUI | Latest | Free | Stable Diffusion workflow UI (runs locally) |
| Adobe Firefly | Current | Adobe CC subscription | Background removal, texture variation, upscaling |
| Suno.ai | v4 | Free tier / $10/mo | AI music prototyping before composer hand-off |

### 2.1 AI Usage Policy

- AI-generated art is **reference and prototype only** — no AI art ships in final builds.
- AI-generated code is **reviewed by a human** before commit — never commit Copilot output blind.
- Claude Code outputs are **architectural guidance** — verify logic before implementation.
- All final assets must be **original work** free of third-party IP contamination.

---

## 3. 3D Modeling & Texturing

### 3.1 Primary 3D Tool

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| Blender | 4.x (4.2 LTS or newer) | Free / GPL | Primary 3D modeling, UV unwrap, export to FBX |
| Hard Ops | 0.99.x | ~€20 (Blender Market) | Hard surface modeling accelerator |
| BoxCutter | 7.x | ~€20 (Blender Market) | Boolean cuts, panel lines on ship hulls |
| BlenderKit | Latest | Free tier | Asset library for kitbashing reference |

**Download:** [blender.org](https://www.blender.org/) | Add-ons via Edit → Preferences → Add-ons

### 3.2 Texturing

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| Armor Paint | 0.9.x | Free (itch.io) | **Primary texturing** — PBR painting, free path |
| Substance Painter | 2024.x | €37/mo (Adobe) | **Alternative texturing** — industry standard, superior bakers |
| Quixel Mixer | Latest | Free (Epic) | Layered material blending, quick texture prototyping |
| Quixel Bridge | Latest | Free (Epic) | Access to Megascans library (rock, metal, wood surfaces) |

**Strategy:** Use Armor Paint for Phase 1–2 (free). Evaluate upgrade to Substance Painter at Phase 3 if texture quality needs improvement.

### 3.3 UV Unwrapping

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| Blender (built-in) | 4.x | Free | Standard UV unwrap (adequate for most assets) |
| RizomUV | 2024.x | ~€100 one-time | Optional: complex UV unwrapping for hero ship models |

---

## 4. 2D, UI & Font Tools

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| Figma | Current | Free tier | UI layout design, style guide, interactive prototypes |
| Inkscape | 1.3.x | Free / GPL | Vector icons: ship icons, ability icons, logo |
| TexturePacker | 7.x | €35 one-time | Sprite atlas packing for UI elements and VFX |
| Google Fonts | — | Free / OFL | Rajdhani + Exo 2 font families |

### 4.1 Font Details

| Font | Weights Used | Download URL | License |
|------|-------------|-------------|---------|
| Rajdhani | Bold, SemiBold | fonts.google.com/specimen/Rajdhani | SIL OFL |
| Exo 2 | Regular, Medium | fonts.google.com/specimen/Exo+2 | SIL OFL |

Place font `.ttf` files in `Assets/_Game/Art/UI/Fonts/` and create TMP Font Assets via `Window → TextMeshPro → Font Asset Creator`.

---

## 5. Audio Tools

### 5.1 Phase 1–2: Unity Audio (Built-in)

No additional audio tools needed. Use Unity's built-in `AudioSource` + `AudioClip` system.

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| Unity Audio | Built-in | Free | Basic SFX + music playback, 3D spatial audio |
| Audacity | 3.x | Free / GPL | SFX editing: trim, normalize, noise removal, format convert |
| jsfxr.com | Web | Free | Instant placeholder SFX generation (retro-style) |
| Freesound.org | Web | CC-licensed | Library: ocean, machinery, explosions (attribute required) |

### 5.2 Phase 3+: FMOD Studio

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| FMOD Studio | 2.02.x | Free (indie ≤$200K revenue) | Adaptive music, event-based SFX, parameter-driven audio |
| FMOD Unity Integration | 2.02.x | Free | FMOD → Unity integration package |

**FMOD indie license:** Free for projects with <$200K annual revenue. See [fmod.com/licensing](https://www.fmod.com/licensing).

**Migration plan (Phase 2→3):**
1. Replace `AudioSource.PlayOneShot()` calls with `FmodEventEmitter` components
2. Convert all SFX to FMOD Events in FMOD Studio project
3. Create parameter: `shipSpeed` (0–1 float) → drives engine loop pitch/intensity
4. Create parameter: `hpRatio` (0–1 float) → drives damage audio layer blend
5. Create Snapshot: `Gameplay_Muffled` → applied when ship is sunk (POV effect)

---

## 6. Development Environment

### 6.1 IDE

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| JetBrains Rider | 2024.x | €7/mo or €69/yr | **Recommended** — best Unity support, built-in debugger |
| VS Code | 1.90.x | Free | Alternative — add C# Dev Kit + Unity extension |

**Rider setup:** Enable `Preferences → Unity → Use JetBrains Rider as external editor` in Unity.

### 6.2 Version Control

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| Git | 2.44.x | Free | Local version control |
| GitHub | — | Free (public/private) | Remote repository hosting |
| GitHub Desktop | Latest | Free | GUI for Git (optional, for non-CLI users) |
| Git LFS | 3.5.x | Free | Large File Storage for textures, audio, FBX files |

**Git LFS tracked extensions** (add to `.gitattributes`):
```
*.fbx filter=lfs diff=lfs merge=lfs -text
*.png filter=lfs diff=lfs merge=lfs -text
*.psd filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.tga filter=lfs diff=lfs merge=lfs -text
```

### 6.3 Branching Strategy (GitHub Flow)

```
main          ← always deployable, tagged releases only
develop       ← integration branch, CI builds on push
feature/*     ← individual feature branches (merge to develop via PR)
hotfix/*      ← urgent production fixes (merge to main + develop)
art/*         ← art asset branches (merge to develop when milestone complete)
```

### 6.4 Project Management

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| GitHub Projects | — | Free | Kanban board linked to GitHub issues |
| GitHub Issues | — | Free | Bug tracking, feature requests, tasks |

**Board columns:** Backlog → In Progress → Review → Done

---

## 7. CI/CD & Build Automation

### 7.1 GitHub Actions

| Workflow | Trigger | Action | Runtime |
|---------|---------|--------|---------|
| `build-pc.yml` | Push to `develop` | Build Windows IL2CPP | ~15 min |
| `test.yml` | Every PR | Run EditMode + PlayMode tests | ~8 min |
| `build-android.yml` | Tag push (`v*.*.*`) | Build Android APK | ~20 min |
| `build-server.yml` | Tag push (`server-*`) | Build headless Linux server | ~12 min |

**GitHub Actions free tier:** 2,000 minutes/month for private repos. ~100 full CI runs/month.

### 7.2 Unity Cloud Build (Optional)

| Use Case | When | Cost |
|----------|------|------|
| iOS Build | Phase 3+ | Requires Unity Cloud Build (needs Mac runner) |
| Automated iOS cert management | Phase 5 | Part of Unity Build Automation |

Use Unity Cloud Build only for iOS (requires macOS) — all other platforms via GitHub Actions.

### 7.3 Docker (Phase 4 — Dedicated Server)

| Tool | Version | License / Cost | Role |
|------|---------|---------------|------|
| Docker Desktop | 4.x | Free (personal) | Container build and local testing |
| Docker Hub | — | Free (public) | Container registry for server image |

**Server image:** Ubuntu 22.04 base + Unity headless Linux build. Phase 4 spec in `IMPLEMENTATION_CHECKLIST.md §Phase4`.

### 7.4 Build Targets Summary

| Target | Platform | Architecture | Use |
|--------|---------|-------------|-----|
| Windows Client | PC | x86_64, IL2CPP | Primary development + release |
| Android Client | Android | ARM64, IL2CPP | Mobile release |
| iOS Client | iOS | ARM64, IL2CPP | Mobile release (Mac runner required) |
| Linux Headless | Linux | x86_64, IL2CPP | Dedicated server |

---

## 8. Documentation

| Tool | Role |
|------|------|
| Markdown (`.md` files) | All project documentation (versioned with code) |
| Mermaid diagrams | Architecture diagrams embedded in Markdown |
| These files in `Docs/` | Authoritative source — update alongside code |

---

## 9. License Summary

| Tool | License Type | Cost |
|------|-------------|------|
| Unity 6 (Personal) | Proprietary, free ≤$200K revenue | Free |
| Mirror Networking | MIT | Free |
| Blender | GPL | Free |
| Armor Paint | MIT / proprietary | Free |
| Audacity | GPL | Free |
| Inkscape | GPL | Free |
| GitHub (Actions, Projects) | Proprietary, free tier | Free |
| FMOD Studio (Indie) | Proprietary, free ≤$200K revenue | Free |
| Google Fonts (Rajdhani, Exo 2) | SIL OFL | Free |
| Hard Ops + BoxCutter | Proprietary (Blender Market) | ~€40 one-time |
| TexturePacker | Proprietary | €35 one-time |
| GitHub Copilot | Proprietary | $10/mo |
| Midjourney | Proprietary | $10–30/mo |
| JetBrains Rider | Proprietary | €7/mo |
| Substance Painter | Proprietary | €37/mo (if adopted) |

**Minimum viable cost (Phase 1):** Hard Ops + BoxCutter (~€40) + GitHub Copilot ($10/mo) + Rider (€7/mo) = ~€57/mo + €40 one-time.

---

*All version numbers reflect 2026-02 availability. Check official sources for updates before installation.*
