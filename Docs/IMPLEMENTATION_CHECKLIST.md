# Iron Tide: Naval Supremacy — Master Implementation Checklist

**Version:** 1.0 | **Status:** Active | **Date:** 2026-02-27

---

## How to Read This Checklist

Each task entry follows this format:
```
- [ ] Task description
      Tool:   [tool or command to use]
      Output: [file/asset/result created]
      Check:  [acceptance criterion — how to verify it's done]
      Blocks: [which later tasks this must complete before]
```

Mark tasks `[x]` when complete. Do not mark in-progress tasks as complete.

---

## Phase 1 — Prototype (Weeks 1–3, ~15 dev-days)

**Goal:** Playable grey-box loop. One ship, one cannon, water visible. No networking.

---

### P1.1 — Project Setup

- [ ] Create Unity project via Unity Hub, Unity 6 LTS, URP template
      Tool:   Unity Hub
      Output: `C:\game\battleships\` Unity project (with URP pre-configured)
      Check:  Editor opens without errors. URP asset assigned in Project Settings → Graphics.
      Blocks: Everything

- [ ] Install all required packages
      Tool:   Package Manager (Window → Package Manager)
      Output: Mirror, Input System, Cinemachine, TextMeshPro, ProBuilder, VFX Graph installed
      Check:  `Packages/manifest.json` contains all packages. No import errors in Console.
      Blocks: P1.3, P1.4, P1.7

- [ ] Create scene structure
      Tool:   Unity Editor
      Output: `Assets/_Game/Scenes/Bootstrap.unity`, `Assets/_Game/Scenes/DevSandbox.unity`
      Check:  Both scenes in Build Settings. Bootstrap loads first (Build Index 0).
      Blocks: P1.5, P1.6

- [ ] Create folder structure
      Tool:   Unity Editor or OS file system
      Output: `Assets/_Game/{Scripts,Tests,Data,Prefabs,Scenes,Art,Audio}` folders
      Check:  All folders visible in Project window. `.gitkeep` in empty folders.
      Blocks: P1.2

- [ ] Add `.gitignore` and `.gitattributes` for Unity + Git LFS
      Tool:   Text editor / Claude Code
      Output: `.gitignore` (Unity standard), `.gitattributes` (LFS for .fbx .png .wav etc.)
      Check:  `git status` does not show Library/ or Temp/ folders.
      Blocks: First commit

---

### P1.2 — Core Architecture

- [ ] Implement `ServiceLocator.cs`
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Core/ServiceLocator.cs`
      Check:  EditMode test passes: register a service, retrieve it, verify type safety.
      Blocks: P1.3, P1.4, P1.5, P1.6

- [ ] Implement `EventBus.cs`
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Core/EventBus.cs`
      Check:  EditMode test: publish event with payload, subscriber receives correct data.
      Blocks: P1.5, P1.6, P1.7

- [ ] Implement `LayerMaskConstants.cs`
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Utils/LayerMaskConstants.cs`
      Check:  Constants match Unity Layer setup (Ships=6, Projectiles=7, MapObjects=8, Water=9).
      Blocks: P1.4, P1.5

- [ ] Configure Unity Layers and Tags
      Tool:   Unity Editor (Edit → Project Settings → Tags and Layers)
      Output: Layers: Ships(6), Projectiles(7), MapObjects(8), Water(9), HarborZone(10)
      Check:  Layers visible in Inspector dropdowns. `LayerMaskConstants` values match.
      Blocks: P1.4, P1.5

---

### P1.3 — ScriptableObjects (Core)

- [ ] Create `ShipClassData.cs` ScriptableObject
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Data/ShipClassData.cs`
      Check:  Create Asset → _Game/Data/Ships/ → verify all fields populate in Inspector.
      Blocks: P1.4

- [ ] Create `WeaponData.cs` ScriptableObject
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Data/WeaponData.cs`
      Check:  Create asset in Data/Weapons/ → verify damage, range, cooldown fields visible.
      Blocks: P1.5

- [ ] Create ShipClassData assets for all 4 archetypes
      Tool:   Unity Editor + Claude Code (for stat values)
      Output: `Data/Ships/SC_Corvette.asset`, `SC_Frigate.asset`, `SC_Destroyer.asset`, `SC_Ironclad.asset`
      Check:  Stats match BUILDPLAN.md archetype specs. All referenced WeaponData assigned.
      Blocks: P1.4

---

### P1.4 — Ship Controller (Single Player)

- [ ] Implement `ShipController.cs` (movement only)
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Ship/ShipController.cs`
      Check:  Placeholder ship cube moves with WASD/joystick. Speed matches ShipClassData.
      Blocks: P1.5

- [ ] Implement `ShipStats.cs` (HP, ownership, death)
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Ship/ShipStats.cs`
      Check:  `TakeDamage()` reduces HP. `OnDeath` event fires at 0 HP. EditMode test passes.
      Blocks: P1.5, P1.6

- [ ] Create Frigate placeholder prefab
      Tool:   Unity Editor
      Output: `Assets/_Game/Prefabs/Ships/PFB_Ship_Frigate.prefab` (placeholder cube)
      Check:  Prefab has ShipController + ShipStats + Rigidbody (kinematic). Loads in DevSandbox.
      Blocks: P1.5, P1.6

---

### P1.5 — Weapon System (Basic)

- [ ] Implement `WeaponMount.cs`
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Weapons/WeaponMount.cs`
      Check:  Fires on trigger. Cooldown enforced. Reads stats from WeaponData SO.
      Blocks: P1.6

- [ ] Implement `ProjectileBase.cs`
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Weapons/ProjectileBase.cs`
      Check:  Spawned projectile travels at correct speed, calls `TakeDamage()` on collision.
      Blocks: P1.6

- [ ] Create CannonShell placeholder prefab
      Tool:   Unity Editor
      Output: `Assets/_Game/Prefabs/Projectiles/PFB_Projectile_CannonShell.prefab`
      Check:  Small sphere, correct layer (Projectiles=7), destroys on terrain hit.
      Blocks: P1.6

---

### P1.6 — DevSandbox Playable Loop

- [ ] Set up DevSandbox scene
      Tool:   Unity Editor + ProBuilder
      Output: `DevSandbox.unity` with water plane, two ProBuilder platforms (harbor areas)
      Check:  Scene plays without errors. Ship spawns. Cannon fires. Hit registers.
      Blocks: P1.7

- [ ] Wire up kill / respawn in DevSandbox
      Tool:   Unity Editor
      Output: `DevSandboxManager.cs` — respawns ship after death (3s delay, same position)
      Check:  Kill ship → black screen 3s → ship returns. Repeatable without restart.
      Blocks: Phase 2

---

### P1.7 — Water Shader (Phase 1)

- [ ] Create basic water ShaderGraph
      Tool:   Unity ShaderGraph editor
      Output: `Assets/_Game/Art/Environment/Water/Water.shadergraph`
      Check:  Water plane visible with Void Navy base color + 1 scrolling normal map.
      Blocks: Phase 2 art

---

### P1.8 — Camera

- [ ] Implement top-down Cinemachine camera
      Tool:   Unity Editor + Cinemachine
      Output: `Assets/_Game/Scripts/Camera/GameCamera.cs` + Cinemachine Virtual Camera config
      Check:  Camera follows player ship with configurable offset. Edge scrolling on PC.
      Blocks: Phase 2

---

### P1.9 — CI / Tests Foundation

- [ ] Write EditMode tests for ServiceLocator and EventBus
      Tool:   Unity Test Framework
      Output: `Assets/_Game/Tests/EditMode/CoreSystemTests.cs`
      Check:  All tests pass in Test Runner window.
      Blocks: Phase 2

- [ ] Create GitHub Actions `test.yml` workflow
      Tool:   GitHub Actions + Unity Test Runner
      Output: `.github/workflows/test.yml`
      Check:  PR to develop triggers test run. Green check on passing tests.
      Blocks: Phase 2 CI

---

## Phase 2 — MVP vs AI (Weeks 4–10, ~35 dev-days)

**Goal:** Full match vs bots. All 4 ships, all abilities, shop, HUD, minimap, harbor objectives.

---

### P2.1 — All Ship Abilities

- [ ] Implement Corvette: Smoke Screen ability
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Abilities/SmokescreenAbility.cs`
      Check:  Area spawned, blocks vision of enemies in zone. Respects WeaponData cooldown.
      Blocks: P2.6

- [ ] Implement Corvette: Sonar Ping ability
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Abilities/SonarPingAbility.cs`
      Check:  Expanding ring reveals stealthed/fogged enemies in radius for 5s.
      Blocks: P2.6

- [ ] Implement Frigate: Twin Cannon Salvo
      Tool:   Rider/VS Code
      Output: `WeaponMount` salvo mode in `WeaponData`
      Check:  2 shells fired in 0.2s burst. Single cooldown for both. Both deal damage.
      Blocks: P2.6

- [ ] Implement Destroyer: Torpedo (skillshot)
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Weapons/TorpedoProjectile.cs`
      Check:  Aimed torpedoes travel in straight line, high damage on hit, miss-able.
      Blocks: P2.6

- [ ] Implement Ironclad: Heavy Salvo + Area Denial
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Abilities/HeavySalvoAbility.cs`
      Check:  3-shot burst over 1.5s. Area denial mine spawns at target location.
      Blocks: P2.6

---

### P2.2 — Economy System

- [ ] Implement gold system
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Economy/GoldManager.cs`
      Check:  Kill → tier×50g. Assist (>10% dmg in last 10s) → 40% kill gold. Passive tick.
      Blocks: P2.3

- [ ] Create `UpgradeData.cs` ScriptableObject
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Data/UpgradeData.cs`
      Check:  Fields: category (Hull/Weapon/Engine), level (I–V), cost, stat delta.
      Blocks: P2.3

- [ ] Create upgrade cost assets
      Tool:   Unity Editor
      Output: Upgrade assets in `Data/Upgrades/` (Level I–V per category, costs per MEMORY.md)
      Check:  Level I: 150g, II: 300g, III: 500g, IV: 800g, V: 1200g. Total per category: 2950g.
      Blocks: P2.3

- [ ] Implement `ShipUpgradeSystem.cs`
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Economy/ShipUpgradeSystem.cs`
      Check:  Purchase upgrade → deduct gold → stat applies to ship → upgrade persists to match end.
      Blocks: P2.3

---

### P2.3 — Shop UI

- [ ] Design shop UI layout in Figma
      Tool:   Figma
      Output: Figma frame: shop panel mockup (upgrade categories, level indicators, buy button)
      Check:  Design matches ART_DIRECTION §5 color palette and panel style.
      Blocks: P2.3 Unity impl

- [ ] Implement shop UI panel in Unity
      Tool:   Unity Editor + TextMeshPro + Claude Code
      Output: `Assets/_Game/Scripts/UI/ShopPanel.cs` + `PFB_UI_ShopPanel.prefab`
      Check:  Opens on hotkey. Shows current upgrades. Buy button deducts gold and closes.
      Blocks: P2.6

---

### P2.4 — HUD

- [ ] Implement HP bar (pressure gauge style)
      Tool:   Unity UI + Claude Code
      Output: `Assets/_Game/Scripts/UI/HpBarUI.cs`
      Check:  Reflects ShipStats.CurrentHp in real time. Color shifts at 30% threshold.
      Blocks: P2.6

- [ ] Implement gold display
      Tool:   Unity UI
      Output: `Assets/_Game/Scripts/UI/GoldDisplayUI.cs`
      Check:  Updates on every gold change. Earn animation plays on gain.
      Blocks: P2.6

- [ ] Implement ability cooldown UI
      Tool:   Unity UI
      Output: `Assets/_Game/Scripts/UI/AbilityCooldownUI.cs`
      Check:  Circular arc sweeps correctly for each ability. Icon greys on cooldown.
      Blocks: P2.6

- [ ] Implement minimap
      Tool:   Unity UI (RawImage + RenderTexture) + Claude Code
      Output: `Assets/_Game/Scripts/UI/MinimapUI.cs`
      Check:  Porthole frame. Ally dots gold, enemy dots red. Updates position every 0.2s.
      Blocks: P2.6

- [ ] Implement kill feed
      Tool:   Unity UI
      Output: `Assets/_Game/Scripts/UI/KillFeedUI.cs`
      Check:  Entries appear on kill/death. Fade after 5s. Max 5 visible entries.
      Blocks: P2.6

---

### P2.5 — Map & Objectives

- [ ] Design full DevSandbox map using ProBuilder
      Tool:   Unity ProBuilder
      Output: `DevSandbox.unity` — two lanes, center, two harbors, 3 islands per lane
      Check:  All ships can navigate all lanes. Harbor zones trigger team-color effects.
      Blocks: P2.5 objectives

- [ ] Implement `HarborObjective.cs`
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Objectives/HarborObjective.cs`
      Check:  Capture timer. Captured harbor regenerates HP for nearby team ships. Recapturable.
      Blocks: P2.5 camps

- [ ] Implement `NeutralCamp.cs`
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Objectives/NeutralCamp.cs`
      Check:  Bot guard spawns. Kill guard → gold reward. Camp respawns after 60s.
      Blocks: P2.6

- [ ] Implement `MatchManager.cs` (win condition)
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Match/MatchManager.cs`
      Check:  Destroy enemy main harbor → victory. Timer shown. Score tracked.
      Blocks: P2.6

---

### P2.6 — Bot AI

- [ ] Create `BotTuningData.cs` ScriptableObject
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Data/BotTuningData.cs`
      Check:  Fields: aggressionLevel, goldSpendThreshold, lanePreference, aimAccuracy (0–1).
      Blocks: P2.6 AI

- [ ] Implement `BotController.cs` (lane navigation + attack)
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/AI/BotController.cs`
      Check:  Bot navigates lane, fires at enemies in range, retreats at low HP, buys upgrades.
      Blocks: P2.6 full match

- [ ] Full 3v3 bot match playable
      Tool:   Unity Editor
      Output: `DevSandbox.unity` configured for 3v3 (1 player + 2 bots vs 3 bots)
      Check:  Match starts, progresses, ends with win/loss. No null-ref errors over 10-min run.
      Blocks: Phase 3

---

### P2.7 — Art Track: Block Art

- [ ] Model final silhouettes for all 4 ships in Blender
      Tool:   Blender 4.x + Hard Ops
      Output: `SHIP_Corvette_LOD0.fbx`, `SHIP_Frigate_LOD0.fbx`, `SHIP_Destroyer_LOD0.fbx`, `SHIP_Ironclad_LOD0.fbx`
      Check:  Silhouettes readable at 64×64 px. Archetype instantly recognizable.
      Blocks: P2.7 materials

- [ ] Apply base flat-color materials (no textures)
      Tool:   Unity Editor
      Output: `M_SHIP_[Archetype]_Default.mat` with flat Deep Steel `#1A2332` hull
      Check:  Ships look correct under URP directional light. Accent colors visible (no texture).
      Blocks: Phase 3 art

- [ ] Phase 2 water shader upgrade (2 normal maps)
      Tool:   Unity ShaderGraph
      Output: Updated `Water.shadergraph` with depth fog + 2 normal maps
      Check:  Depth fog transitions Void Navy → Biolum Cyan. No performance drop on mobile target.
      Blocks: Phase 3 art

- [ ] Basic harbor geometry
      Tool:   Blender 4.x + ProBuilder
      Output: `ENV_Harbor_DockSectionA.fbx` — dock platform, bollards
      Check:  Ships can approach dock area. No geometry overlap. Correct scale.
      Blocks: Phase 3 art

---

### P2.8 — Audio Track: Placeholder SFX

- [ ] Generate placeholder SFX for all weapons
      Tool:   jsfxr.com
      Output: `SFX_Weapon_CannonFire.wav`, `SFX_Weapon_TorpedoLaunch.wav`, `SFX_Weapon_TorpedoHit.wav`
      Check:  Audio plays on fire/hit events. No missing reference warnings.
      Blocks: Phase 3 audio

- [ ] Add placeholder engine loops for all 4 ships
      Tool:   jsfxr.com / Audacity
      Output: `SFX_Ship_[Archetype]Engine.wav` × 4
      Check:  Engine loop plays while ship moves, stops on halt. Pitch matches ship type roughly.
      Blocks: Phase 3 audio

---

## Phase 3 — PvP Alpha (Weeks 11–16, ~25 dev-days)

**Goal:** Online 3v3 PvP. Lobby → match → result. Lag compensation. Disconnect handling.

---

### P3.1 — Mirror Networking Core

- [ ] Set up `NetworkManager` scene and configuration
      Tool:   Unity Editor + Mirror docs
      Output: `Bootstrap.unity` with NetworkManager + KCP Transport configured
      Check:  Host starts server. Client connects. No timeout on LAN.
      Blocks: P3.2

- [ ] Implement `NetworkShip.cs`
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Networking/NetworkShip.cs`
      Check:  Ship position synced. Owner client controls movement. Server validates speed cap.
      Blocks: P3.3

- [ ] Implement `NetworkProjectile.cs`
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Networking/NetworkProjectile.cs`
      Check:  Server spawns projectile. All clients see it. Hit resolved server-side only.
      Blocks: P3.3

- [ ] Implement server-authoritative damage + gold
      Tool:   Rider/VS Code + Claude Code
      Output: `[Command]` and `[ClientRpc]` methods in NetworkShip and GoldManager
      Check:  Client cannot fake damage. Gold never desyncs between host and clients.
      Blocks: P3.4

---

### P3.2 — Lobby UI

- [ ] Implement lobby scene and NetworkRoomManager
      Tool:   Unity Editor + Mirror
      Output: `Assets/_Game/Scenes/Lobby.unity`
      Check:  Host → shows room. Client → joins room. Ready button shows to all players.
      Blocks: P3.2 player list

- [ ] Implement player list UI in lobby
      Tool:   Unity UI + Mirror
      Output: `Assets/_Game/Scripts/UI/LobbyPlayerListUI.cs`
      Check:  List updates live when players join/leave. Ready state visible per player.
      Blocks: P3.2 countdown

- [ ] Implement match start countdown
      Tool:   Rider/VS Code
      Output: Server starts 5s countdown when all ready → loads GameScene for all clients.
      Check:  All clients transition simultaneously. No client left in lobby.
      Blocks: P3.4

---

### P3.3 — Lag Compensation

- [ ] Implement client-side prediction for ship movement
      Tool:   Rider/VS Code + Claude Code
      Output: Updated `NetworkShip.cs` — local movement applied immediately, server correction interpolated
      Check:  At 100ms simulated latency, ship still feels responsive. No rubber-banding over 200ms.
      Blocks: P3.4

- [ ] Implement ping display
      Tool:   Unity UI + Mirror
      Output: `Assets/_Game/Scripts/UI/PingDisplayUI.cs`
      Check:  Shows RTT in ms, updated every 2s. Green <80ms, Yellow <150ms, Red >150ms.
      Blocks: P3.4

---

### P3.4 — Disconnect Handling

- [ ] Implement disconnect and reconnect logic
      Tool:   Rider/VS Code + Mirror
      Output: `Assets/_Game/Scripts/Networking/ConnectionManager.cs`
      Check:  Client disconnect → ship frozen → 30s reconnect window → removed if timeout.
      Blocks: Phase 4

- [ ] Implement host migration (basic)
      Tool:   Rider/VS Code + Mirror
      Output: Mirror's `NetworkRoomManager` host migration configured
      Check:  Host disconnects → another client promoted → match continues.
      Blocks: Phase 4

---

### P3.5 — Art Track: Full Textures

- [ ] UV unwrap and texture all 4 ships (LOD0)
      Tool:   Blender + Armor Paint
      Output: `T_SHIP_[Archetype]_Albedo.png`, `T_SHIP_[Archetype]_Normal.png`, `T_SHIP_[Archetype]_MetallicSmooth.png`
      Check:  All textures 2048×2048. No UV seams visible at normal play distance.
      Blocks: P3.5 damaged variants

- [ ] Create damaged material variants for all ships
      Tool:   Armor Paint
      Output: `T_SHIP_[Archetype]_Albedo_Damaged.png` × 4 + `M_SHIP_[Archetype]_Damaged.mat` × 4
      Check:  Damaged appearance triggers at 30% HP. Rust/char visible. Distinct from default.
      Blocks: Phase 4 art

- [ ] Full VFX pass: cannon, torpedo, smoke, sonar, repair
      Tool:   Unity VFX Graph
      Output: `VFX_CannonExplosion.prefab`, `VFX_TorpedoImpact.prefab`, `VFX_SmokeBurst.prefab`, etc.
      Check:  Each VFX plays on correct event. Colors match ART_DIRECTION §6 palette exactly.
      Blocks: Phase 4 art

- [ ] Audio first pass: real SFX replacing placeholders
      Tool:   Audacity + Freesound.org
      Output: All SFX in `Audio/SFX/` replaced with real recordings/edits
      Check:  No jsfxr placeholder sounds remain. All SFX at -18 LUFS normalized.
      Blocks: Phase 4 audio

---

## Phase 4 — Dedicated Server (Weeks 17–20, ~15 dev-days)

**Goal:** Headless server deployable on Linux. Matchmaker REST API. Load tested.

---

### P4.1 — Headless Server Build

- [ ] Audit and guard all client-only code with `#if !UNITY_SERVER`
      Tool:   Rider/VS Code + Grep search
      Output: All camera, audio, UI code wrapped in server build guards
      Check:  Server build (Linux headless) compiles with zero errors.
      Blocks: P4.2

- [ ] Configure Linux Headless build target in Unity
      Tool:   Unity Build Settings
      Output: Build target: Linux 64-bit, Dedicated Server, IL2CPP
      Check:  `.x86_64` executable produced. Runs without GPU on headless machine.
      Blocks: P4.2

- [ ] Create Dockerfile for server image
      Tool:   Docker + Claude Code
      Output: `Server/Dockerfile` — Ubuntu 22.04 base + server binary
      Check:  `docker build` succeeds. Container starts, accepts KCP connection from test client.
      Blocks: P4.3

---

### P4.2 — Matchmaker API

- [ ] Design matchmaker REST API (simple)
      Tool:   Claude Code + Markdown design doc
      Output: API spec: `POST /queue`, `GET /queue/{id}`, `DELETE /queue/{id}`, `GET /servers`
      Check:  Spec reviewed and approved before implementation.
      Blocks: P4.2 impl

- [ ] Implement matchmaker server (Node.js or Go)
      Tool:   Claude Code + Node.js / Go
      Output: `Server/Matchmaker/` — simple queue, match players by region, assign server
      Check:  Two clients queue → matched → server connection info returned within 10s.
      Blocks: P4.3

- [ ] Integrate Unity client with matchmaker
      Tool:   Rider/VS Code + Unity `UnityWebRequest`
      Output: `Assets/_Game/Scripts/Networking/MatchmakerClient.cs`
      Check:  Client queues, polls for match, connects to assigned server IP:port automatically.
      Blocks: P4.3

---

### P4.3 — Load Testing

- [ ] Load test: 6 simultaneous clients on server
      Tool:   Unity with bot clients + server instance
      Output: Performance log from 10-minute bot match on server
      Check:  Server CPU <80%. No player desync. Tick rate stable at 30Hz.
      Blocks: Phase 5

---

### P4.4 — Art / Audio Polish

- [ ] Harbor win explosion VFX
      Tool:   Unity VFX Graph
      Output: `VFX_HarborExplosion.prefab` — multi-stage: smoke → fire → shockwave
      Check:  Plays on harbor destroy. Multi-stage timing correct. No performance spike >5ms.
      Blocks: Phase 5

- [ ] Implement FMOD integration and migrate audio events
      Tool:   FMOD Studio + FMOD Unity package
      Output: FMOD project + all SFX migrated to FMOD Events. `AudioService.cs` updated.
      Check:  All audio plays via FMOD. Engine parameter drives pitch/intensity correctly.
      Blocks: Phase 5 audio

---

## Phase 5 — Ranked Beta (Weeks 21–26, ~20 dev-days)

**Goal:** ELO/MMR, ranked queue, seasons, cosmetics, platform store submissions.

---

### P5.1 — Ranked System

- [ ] Design MMR/ELO calculation
      Tool:   Claude Code + spreadsheet
      Output: `Server/Matchmaker/Elo.cs` (or equivalent) — K-factor 32, provisional 10 games
      Check:  Win → gain MMR. Loss → lose MMR. Equal opponents → near-zero net change.
      Blocks: P5.2

- [ ] Implement ranked queue with MMR matchmaking
      Tool:   Rider/VS Code + matchmaker
      Output: Queue filters by MMR ±150 range. Expands ±50 every 30s.
      Check:  Two evenly-matched players queue → matched within 2 minutes on average.
      Blocks: P5.3

- [ ] Design season structure and reset schedule
      Tool:   Claude Code + doc
      Output: Season spec: 3-month seasons. Soft reset (MMR × 0.75) on season end.
      Check:  Spec approved. Season 1 start/end dates defined.
      Blocks: P5.3

---

### P5.2 — Cosmetics System

- [ ] Create `CosmeticData.cs` ScriptableObject
      Tool:   Rider/VS Code
      Output: `Assets/_Game/Scripts/Data/CosmeticData.cs` — fields: skin name, ship override materials, VFX override prefabs
      Check:  Asset created. Can swap ship materials and VFX at runtime.
      Blocks: P5.2 shop

- [ ] Implement cosmetic inventory and equip
      Tool:   Rider/VS Code + Claude Code
      Output: `Assets/_Game/Scripts/Economy/CosmeticInventory.cs`
      Check:  Player equips cosmetic → persists to next match. Only visual change, no stat change.
      Blocks: P5.3

- [ ] Create Season 1 cosmetic variant (1 ship)
      Tool:   Blender + Armor Paint
      Output: `M_SHIP_Frigate_Season1.mat` + unique hull texture
      Check:  Equippable in game. Visible to all clients in match.
      Blocks: Store submission

---

### P5.3 — Store Submission Prep

- [ ] Meet Android (Google Play) submission requirements
      Tool:   Unity Build Settings + Android SDK
      Output: Signed APK / AAB. Target API Level 33+. 64-bit only.
      Check:  Pre-launch report passes. No policy violations. Store listing complete.
      Blocks: Launch

- [ ] Meet iOS (App Store) submission requirements
      Tool:   Unity Cloud Build + Xcode
      Output: IPA signed with distribution cert. Privacy manifest complete.
      Check:  TestFlight build accepted. No binary rejection from Apple.
      Blocks: Launch

- [ ] PC (Steam or itch.io) build and listing
      Tool:   Unity Windows Build + Steamworks
      Output: Windows IL2CPP build. Steam depot uploaded (or itch.io zip).
      Check:  Build launches on clean Windows install (no Unity required). Steam page live.
      Blocks: Launch

---

## Art Track — Runs Parallel to All Phases

| Phase | Art Milestone | Acceptance Criterion |
|-------|--------------|---------------------|
| Phase 1 | Placeholder geometry only | Game loop runs with cubes/capsules |
| Phase 2 | Final silhouettes + base materials | All 4 ships instantly recognizable at 20m |
| Phase 3 | Full LOD0 textures + VFX first pass | QA approves visual quality |
| Phase 4 | Damage states + cosmetic slots + final UI | Screenshot-quality for store listing |
| Phase 5 | Season cosmetic variants + store assets | Platform submission approved |

---

## Verification: Definition of Done per Phase

### Phase 1 Complete when:
- [ ] DevSandbox loads and plays without errors
- [ ] Ship moves, cannon fires, hit deals damage, death respawns
- [ ] EditMode tests pass in CI
- [ ] All core scripts committed to `develop` branch

### Phase 2 Complete when:
- [ ] 3v3 bot match plays to win/loss condition without errors
- [ ] All 4 ship archetypes playable with correct abilities
- [ ] Economy, shop, upgrades all functional
- [ ] HUD shows HP, gold, cooldowns, minimap, kill feed
- [ ] Block art pass: ship silhouettes final

### Phase 3 Complete when:
- [ ] 3v3 PvP over internet (not LAN) playable with all features
- [ ] Lobby → match → result flow works end-to-end
- [ ] Reconnect window works on disconnect
- [ ] Lag compensation tested up to 200ms without rubber-banding
- [ ] Full texture pass complete on all ships

### Phase 4 Complete when:
- [ ] Dedicated server runs headless on Linux in Docker
- [ ] Matchmaker assigns players to servers automatically
- [ ] Load test: 6 players, 10 minutes, no desync
- [ ] FMOD audio integrated

### Phase 5 Complete when:
- [ ] Ranked queue with MMR matching works
- [ ] Season 1 cosmetic available and equippable
- [ ] All three platform builds submitted to stores
- [ ] Soft launch approved

---

*This checklist is the ground truth for project progress. Update checkbox status as tasks complete.*
*Cross-reference `BUILDPLAN.md` for game design context and `TECHNICAL_PLAN.md` for architecture detail.*
