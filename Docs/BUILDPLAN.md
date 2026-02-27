# IRON TIDE: NAVAL SUPREMACY — Complete Unity Build Plan

> **Assumption flags** (labeled [A#] throughout): made where spec was silent.
> wc3bs.com was unreachable; design proceeds from user spec + wiki data.

---

## A) HIGH-LEVEL DESIGN SUMMARY

### Concept
**Iron Tide: Naval Supremacy** is a team-based 3v3 (→5v5) naval brawler.
Art direction: Dieselpunk / sci-fi ocean — riveted steel hulls, exhaust-belching
stacks, bioluminescent water, Art Deco UI panels. Zero Warcraft assets, names, or IP.

### Core Loop
```
Spawn at Harbor
  → Navigate 2-lane + flank map
  → Earn Gold (kills / assists / neutral camps / objective ticks)
  → Recall to Harbor → Buy upgrades / new ship class
  → Push objectives → Destroy enemy Harbor/Mothership → WIN
```

### Match Format
| Parameter        | MVP                     | Future            |
|-----------------|-------------------------|-------------------|
| Teams           | 3v3                     | 5v5               |
| Match length    | 15–30 min               | same              |
| Map             | 1 map: dual-lane + flanks | additional maps  |
| Respawn timer   | 8s base + 2s per tier    | same              |

### Four Ship Archetypes (original names)
| Archetype      | Role              | Signature                        |
|---------------|-------------------|----------------------------------|
| **Corvette**  | Scout / Harasser  | Smoke Screen + Sonar ping        |
| **Frigate**   | Balanced Brawler  | Twin cannon salvo, durable       |
| **Destroyer** | Torpedo Striker   | High-damage torpedoes, fragile   |
| **Ironclad**  | Siege / Anchor    | Heavy salvo, area denial, slow   |

### Weapons / Abilities
- **Primary Cannons** — arcing shells, travel time, splash AoE
- **Torpedoes** — skillshot, linear travel, mild passive drift, no lock-on
- **Smoke Screen** — vision blocker + accuracy debuff in zone
- **Sonar/Radar** — reveals enemy positions in radius for team
- **Repair Module** — HoT (heal-over-time) active, short duration

### Economy Tier Curve (tunable via ScriptableObjects)
| Level | Cost  | Cumulative | Stat Gain (example) |
|-------|-------|-----------|---------------------|
| I     | 150g  | 150g      | +8% base stat       |
| II    | 300g  | 450g      | +8%                 |
| III   | 500g  | 950g      | +10%                |
| IV    | 800g  | 1750g     | +10%                |
| V     | 1200g | 2950g     | +12%                |

Ship class upgrades (Harbor purchase, one-way upgrade [A1]):
- Corvette → Frigate: 500g | Frigate → Destroyer: 900g | → Ironclad: 1500g

### Win Condition
Destroy the enemy **Harbor** (acts as spawn point + shop). Harbor has a large HP
pool and must be damaged directly; it cannot be backdoored past lane objectives
without contesting them first [A2].

---

## B) TECHNICAL ARCHITECTURE

### Stack
| Layer             | Choice                                      | Reason                                      |
|------------------|---------------------------------------------|---------------------------------------------|
| Engine           | Unity 6 LTS (6000.0.x)                      | Stable, long-term support, mobile proven    |
| Render           | URP (2 quality tiers: PC / Mobile)          | Required for mobile performance             |
| Input            | Unity Input System (new)                    | Unified PC + mobile                         |
| Networking       | **Mirror + KCP transport**                  | OSS, headless-ready, KCP handles mobile NAT |
| Camera           | Cinemachine + custom IsometricCameraController | Smooth follow + zoom + rotation            |
| UI               | UI Toolkit + TextMeshPro (hybrid [A3])      | Performance-first                           |
| Audio            | Unity Audio + custom AudioManager           | No middleware for MVP                       |
| Physics          | Unity PhysX (kinematic Rigidbodies for ships) | Predictable, perf-friendly                 |

**Why Mirror over alternatives:**
- FishNet has better raw CCU performance but smaller community + docs
- Photon costs money early; Mirror is free with pay-nothing-until-scale path
- Mirror headless builds are well-documented; KCP transport handles UDP on mobile NAT
- Migration path to FishNet is feasible post-Phase 3 if CCU demands it

### Authority Model (summary — full spec in Section E)
- **Server-authoritative**: projectile spawn, hit detection, damage, gold, objectives, death/respawn
- **Client-authoritative with server validation**: movement input (speed-capped server-side)
- **Client-side only**: visuals, particles, camera, local UI

### Folder Structure
```
Assets/
  _Game/
    Art/
      Ships/           # mesh + materials per archetype
      Environment/     # water, islands, harbor art
      Effects/         # VFX prefabs (muzzle, splash, smoke, torpedo wake)
      UI/              # sprites, atlases, fonts
    Audio/
      SFX/
      Music/
    Data/              # ALL ScriptableObject assets (.asset files)
      Ships/           # ShipClassData per archetype + tier
      Weapons/         # WeaponData per weapon type
      Modules/         # ModuleData
      Upgrades/        # UpgradeData per category per level (I–V)
      Objectives/      # MapObjectiveData
      Bots/            # BotTuningData per difficulty
      Cosmetics/       # CosmeticData
    Prefabs/
      Ships/
      Projectiles/
      Effects/
      Map/
      UI/
    Scenes/
      Bootstrap        # DontDestroyOnLoad systems, init
      MainMenu
      Lobby
      GameplayMVP      # Primary match scene
      DevSandbox       # Rapid iteration, no match state
    Scripts/
      Core/
        GameManager.cs
        MatchStateMachine.cs
        ServiceLocator.cs
        EventBus.cs
      Ships/
        ShipController.cs
        ShipStats.cs
        ShipHealth.cs
        ShipVisuals.cs
        ShipUpgradeHandler.cs
      Weapons/
        WeaponBase.cs
        CannonWeapon.cs
        TorpedoWeapon.cs
        ProjectileBase.cs
        CannonShell.cs
        TorpedoProjectile.cs
        ProjectileHitValidator.cs
      Abilities/
        AbilityBase.cs
        SmokeScreenAbility.cs
        SonarAbility.cs
        RepairAbility.cs
      Economy/
        GoldManager.cs
        KillTracker.cs
        ShopManager.cs
      Map/
        LaneObjective.cs
        NeutralCamp.cs
        Harbor.cs
        MinimapController.cs
      Networking/
        NetworkManagerGame.cs
        NetworkShip.cs
        NetworkProjectile.cs
        NetworkGoldSync.cs
        ServerBotManager.cs
      UI/
        HUD/
          HUDController.cs
          GoldDisplay.cs
          HealthBar.cs
          AbilityCooldownUI.cs
          KillFeedUI.cs
        Shop/
          ShopUI.cs
          UpgradeSlotUI.cs
        Minimap/
          MinimapUI.cs
        MainMenu/
          MainMenuUI.cs
          LobbyUI.cs
      Camera/
        IsometricCameraController.cs
      Input/
        InputRouter.cs
        PCInputHandler.cs
        MobileInputHandler.cs
      Bots/
        BotBrain.cs
        BotSteering.cs
        BotCombatModule.cs
        BotEconomyModule.cs
      Audio/
        AudioManager.cs
        SoundBank.cs
      Utils/
        ObjectPool.cs
        MathUtils.cs
        LayerMaskConstants.cs
    Tests/
      EditMode/        # Pure logic, no scene
      PlayMode/        # Scene-dependent
  Plugins/
    Mirror/            # imported via Package Manager
```

---

## C) MILESTONE PLAN

### PHASE 1 — Prototype (Weeks 1–3 | ~15 dev-days)
**Goal:** Movement + camera + aiming + one weapon + one ship, playable sandbox.

| Task | Detail |
|------|--------|
| Unity project init | Unity 6 LTS, URP, Input System, Mirror, Cinemachine, TMP |
| URP quality tiers | PC (shadows, reflection probe) / Mobile (baked, no tessellation) |
| IsometricCameraController | Zoom (scroll/pinch), rotation (mid-drag/2-finger), follow target |
| InputRouter | Runtime detect PC vs Mobile, route to handlers |
| ShipController | WASD + mouse-aim (PC), twin-stick (mobile), kinematic Rigidbody |
| CannonWeapon + CannonShell | Arc trajectory, travel time, splash AoE, object pool |
| WaterShader (ShaderGraph) | Scrolling normals, depth fog, foam edge; mobile LOD variant |
| DevSandbox scene | One ship, one cannon, one target dummy, camera |
| Git init | .gitignore (Unity), initial commit, branch strategy |

**Acceptance Criteria:**
- Ship moves at design speed; turn rate matches ShipClassData on both input modes
- Camera: zoom [3m–40m], full 360° rotation, mobile pinch/swipe work
- Cannon fires arc shell with visible travel time; splash damages dummy in radius
- 60 fps stable on mid-range Android (Profiler session, mobile quality tier)

**Risks:**
| Risk | Mitigation |
|------|-----------|
| Water shader cost on mobile | Profile early Day 5; have flat-color fallback ready |
| Camera feel on touch | Cinemachine + manual tuning pass; budget 1 extra day |

---

### PHASE 2 — MVP vs AI (Weeks 4–10 | ~35 dev-days)
**Goal:** Full offline 3v3 match vs bots, all 4 ships, shop, gold, objectives, minimap.

| Task | Detail |
|------|--------|
| All 4 ship prefabs | Corvette, Frigate, Destroyer, Ironclad — placeholder meshes |
| All SO data assets | ShipClassData ×4 tiers, WeaponData ×4, ModuleData, UpgradeData ×5 levels ×5 categories, MapObjectiveData ×5, BotTuningData ×3 difficulties, CosmeticData stubs |
| TorpedoWeapon + TorpedoProjectile | Skillshot, kinematic, mild drift via steeringFactor |
| SmokeScreenAbility | Zone prefab, vision mask on minimap, accuracy debuff |
| SonarAbility | Radius reveal, team broadcast, ping UI |
| RepairAbility | HoT component, cooldown, visual pulse |
| ShipHealth + death/respawn | HP pool, armor reduction, death event, respawn timer |
| GoldManager | Kill gold, assist gold (>10% dmg last 10s), camp gold, objective tick gold |
| ShopUI + ShopManager | 5 tabs, 5 levels each, validation, stat apply |
| ShipUpgradeHandler | Runtime stat recalculation on upgrade purchase |
| Harbor entity | Team-assigned HP, win-condition trigger on destruction |
| LaneObjective ×2 | Capturable tower, gold ticks to controlling team |
| NeutralCamp ×3 | Guard spawns, aggro radius, AoE gold on clear |
| MinimapController + MinimapUI | Ship icons, objective icons, Sonar reveal layer |
| HUD | HP bar, gold counter, cooldown icons, kill feed |
| BotBrain | States: patrol, engage, retreat, shop, upgrade; uses BotTuningData |
| BotSteering | Navmesh-based or waypoint-based movement on water plane |
| GameplayMVP scene | Full map blockout (2 lanes, flanks, harbors, camps) |
| MatchStateMachine | WaitingForPlayers → Countdown (5s) → Playing → GameOver |
| Audio pass | Cannon fire, torpedo, explosion, horn, ambient ocean |

**Acceptance Criteria:**
- Full 3v3 vs bots match completable start to finish without crash
- Economy loop: earn gold → shop at Harbor → visible stat improvement
- All 4 archetypes feel distinct in movement speed and weapon profile
- Harbor destruction correctly triggers win for attacking team
- Bots: navigate, attack, retreat when low HP, return to shop, buy upgrades

**Risks:**
| Risk | Mitigation |
|------|-----------|
| Bot pathfinding on irregular water map | Use Unity NavMesh with carved obstacles; fake 2D top-down |
| Balance: matches decided too fast/slow | Instrument gold earn rates; tuning pass week 9 |
| Scope creep on art | Block out only; no final art until Phase 3 |

---

### PHASE 3 — PvP Alpha (Weeks 11–16 | ~25 dev-days)
**Goal:** Unranked quickplay, hosted listen-server, server-authoritative, Mirror + KCP.

| Task | Detail |
|------|--------|
| NetworkManagerGame | Mirror NetworkManager subclass, lobby flow (host/join) |
| NetworkShip | SyncVar HP, SyncVar upgrades; [Command] fire, [Command] ability |
| NetworkProjectile | Server-spawned, server-simulated, ClientRpc visual spawn |
| NetworkGoldSync | SyncVar gold per player, server-only modification |
| Projectile hit validation | Server rewinds position ±RTT/2 (cap 150ms) before registering hit |
| Movement replication | NetworkTransform 20Hz + client-side smooth interpolation for remotes |
| Server-side bots | ServerBotManager spawns/runs BotBrain on server for empty slots |
| Lobby flow UI | Host/Join, player list, ready-up, start match |
| Disconnect handling | Graceful bot-replace on drop, match continues |
| Ping display | HUD corner, color-coded (green/yellow/red) |
| Anti-cheat basics | Server rejects gold overspend, speed violations, client-reported damage |
| Mobile-PC cross-session | Verified in playtests (crossplay full support Phase 4+) |
| Profiling pass | Target: <16.7ms frame on mobile; strip physics if needed |

**Acceptance Criteria:**
- 3v3 online match plays server-authoritative with no desync on projectile hits
- LAN playtest: <50ms felt latency; regional internet: <150ms acceptable
- Server-side bots fill empty slots without exploitable behavior
- Disconnect: remaining players notified, bot replaces within 3s

**Risks:**
| Risk | Mitigation |
|------|-----------|
| Mobile NAT traversal | KCP punches through most NAT; add simple relay fallback (free tier) |
| Projectile rewind accuracy at high ping | Cap rewind at 150ms; beyond that, miss is miss |
| Relay cost spike | Use Edgegap free tier or self-hosted turn server for alpha |

---

### PHASE 4 — Dedicated Server Migration (Weeks 17–20 | ~15 dev-days)
**Goal:** Headless Linux server builds + matchmaker outline.

| Task | Detail |
|------|--------|
| Headless build target | Unity batch-mode build, stripped rendering (no GPU), Linux IL2CPP |
| Server/client code separation | `#if UNITY_SERVER` guards; no camera/input on server |
| Matchmaker stub | Lightweight REST API (C# ASP.NET or Node.js) + Docker container |
| Match flow | Client → matchmaker → assigned server IP:port → connect |
| Server selection logic | Dedicated first; listen-server fallback if no dedicated available |
| Load test | 2 concurrent matches on $5/mo VPS (DigitalOcean/Hetzner) |
| Transport evaluation | Benchmark KCP vs Telepathy vs SimpleWebTransport for server |
| Monitoring | Server stdout logging, match result POST to matchmaker |

**Acceptance Criteria:**
- Headless Unity server boots and runs full match on Ubuntu 22.04 VPS
- Matchmaker creates room, clients connect without manual IP entry
- 2 concurrent matches stable on minimum-spec VPS

**Risks:**
| Risk | Mitigation |
|------|-----------|
| Unity headless licensing (paid seats) | Unity 6 Personal/Plus headless is royalty-free below revenue threshold |
| Docker image size | Strip assets not needed server-side; target <500MB image |

---

### PHASE 5 — Ranked / Ladder Beta (Weeks 21–26 | ~20 dev-days)
**Goal:** ELO/MMR with seasons, cosmetics-only progression.

| Task | Detail |
|------|--------|
| ELO/MMR system | Server-side Elo calculation post-match; stored in player profile DB |
| Ranked queue UI | Ranked vs Casual toggle, estimated wait time |
| Season framework | Timed reset (90-day seasons), snapshot leaderboard, reward distribution |
| Cosmetic system | CosmeticData SOs, loadout screen, preview, equip |
| Cosmetic shop / earnable | Earn-through-play currency (no gameplay advantage) |
| Battle pass stub | 50-tier track, free + premium track [A4] |
| Analytics | Unity Analytics or custom event pipeline (match outcome, gold flow) |
| Platform submissions | Google Play closed beta + Apple TestFlight |

**Acceptance Criteria:**
- Post-match MMR updates visible in player profile
- Cosmetics: zero stat modifiers confirmed (inspector flag + automated test)
- Season 1 rewards distributed to qualifying players on reset
- App submitted and passing store review

---

## D) SCRIPTABLEOBJECT SCHEMAS

```csharp
// ── ShipClassData ──────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "IronTide/Ship Class")]
public class ShipClassData : ScriptableObject {
    public string    shipId;
    public string    displayName;
    public Sprite    icon;
    public GameObject shipPrefab;

    [Header("Base Stats")]
    public float baseMaxHP;
    public float baseArmor;          // flat damage reduction %
    public float baseSpeed;          // units/sec
    public float baseTurnRate;       // deg/sec
    public float baseAcceleration;

    [Header("Weapons")]
    public WeaponData primaryWeapon;
    public WeaponData secondaryWeapon;
    public AbilitySlot[] abilitySlots;  // max 3

    [Header("Economy")]
    public int  purchaseCost;        // harbor gold cost; 0 = free starter
    public int  tier;                // 1 Corvette → 4 Ironclad
    public ShipArchetype archetype;  // enum

    [Header("Upgrade Caps")]
    public int maxHullLevel;
    public int maxEngineLevel;
    public int maxCannonLevel;
    public int maxTorpedoLevel;
    public int maxUtilityLevel;
}

// ── WeaponData ─────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "IronTide/Weapon")]
public class WeaponData : ScriptableObject {
    public string     weaponId;
    public string     displayName;
    public WeaponType weaponType;    // enum: Cannon, Torpedo, Utility

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float projectileLifetime;
    public float arcHeight;          // parabolic peak height (cannons)
    public float blastRadius;        // splash AoE radius
    public float steeringFactor;     // 0=straight, >0=torpedo drift

    [Header("Damage")]
    public float      baseDamage;
    public float      damageVariance;   // ±%
    public DamageType damageType;       // enum: Kinetic, Explosive, Fire

    [Header("Firing")]
    public float cooldown;
    public int   salvoCount;
    public float salvoInterval;
    public float spread;             // degrees arc spread per salvo

    [Header("Range")]
    public float minRange;
    public float maxRange;

    [Header("Audio / VFX")]
    public AudioClip  fireSound;
    public GameObject muzzleFlashPrefab;
}

// ── ModuleData ─────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "IronTide/Module")]
public class ModuleData : ScriptableObject {
    public string         moduleId;
    public string         displayName;
    public string         description;
    public Sprite         icon;
    public ModuleCategory category;  // enum: Hull,Engine,Cannon,Torpedo,Utility,Support

    public StatModifier[]   statModifiers;
    public bool             hasActiveAbility;
    public AbilityData      abilityData;

    public ShipArchetype[]  allowedArchetypes; // empty = all
    public int              slotCost;
}

// ── UpgradeData ────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "IronTide/Upgrade")]
public class UpgradeData : ScriptableObject {
    public string          upgradeId;
    public UpgradeCategory category;   // enum: Hull,Engine,Cannon,Torpedo,Utility
    public int             level;      // 1–5
    public int             goldCost;
    public string          displayName;
    public Sprite          icon;
    public StatModifier[]  bonuses;
    public UpgradeData     prerequisite; // null = no prereq
}

// ── MapObjectiveData ───────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "IronTide/Map Objective")]
public class MapObjectiveData : ScriptableObject {
    public string        objectiveId;
    public ObjectiveType type;         // enum: Harbor, LaneObjective, NeutralCamp

    [Header("HP")]
    public float maxHP;

    [Header("Rewards")]
    public int   killGoldReward;
    public int   assistGoldReward;
    public int   tickGoldReward;
    public float tickInterval;         // seconds between ticks

    [Header("Capture (LaneObjective)")]
    public float captureTime;

    [Header("Camp")]
    public GameObject guardPrefab;
    public int        guardCount;
    public float      guardAggroRadius;
    public bool       respawns;
    public float      respawnDelay;
}

// ── BotTuningData ──────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "IronTide/Bot Tuning")]
public class BotTuningData : ScriptableObject {
    public string          botId;
    public BotDifficulty   difficulty;  // enum: Easy, Medium, Hard
    public ShipClassData   shipClass;

    [Header("Decision Weights")]
    [Range(0,1)] public float aggressionBias;
    [Range(0,1)] public float retreatHPThreshold;
    [Range(0,1)] public float shopPriorityBias;
    [Range(0,1)] public float campPriorityBias;

    [Header("Aim Accuracy")]
    public float aimErrorDegrees;
    public float reactionTime;
    [Range(0,1)] public float leadShotFactor;

    [Header("Upgrade Priority")]
    public UpgradeCategory[] upgradePriority;

    [Header("Navigation")]
    public float waypointRadius;
    public float avoidanceRadius;
}

// ── CosmeticData ───────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "IronTide/Cosmetic")]
public class CosmeticData : ScriptableObject {
    public string       cosmeticId;
    public string       displayName;
    public CosmeticType type;      // enum: ShipSkin, Trail, Decal, Horn, Nameplate
    public Rarity       rarity;    // enum: Common, Rare, Epic, Legendary
    public Sprite       previewImage;

    [Header("Applied Assets")]
    public Material    skinMaterial;
    public GameObject  trailPrefab;
    public Texture2D   decalTexture;
    public AudioClip   hornClip;

    [Header("Unlock")]
    public CosmeticSource source;         // enum: Earnable, BattlePass, DirectPurchase
    public int            earnCurrencyCost;

    // Enforced: cosmetics must never touch gameplay stats
    [SerializeField, HideInInspector]
    private bool _hasGameplayImpact = false; // tested in automated suite
}
```

---

## E) NETWORKING SPECS

### Authority Model

| System              | Authority  | Notes                                            |
|--------------------|------------|--------------------------------------------------|
| Player movement     | Server validates (speed cap) | Client sends input; server applies |
| Projectile spawn    | Server only | Client [Command] triggers; server spawns NetworkProjectile |
| Hit detection       | Server only | Server simulates projectile tick + overlap check |
| Damage application  | Server only | TakeDamage() called server-side only            |
| Gold changes        | Server only | GoldManager server-only; SyncVar to owner       |
| Objectives/capture  | Server only | LaneObjective capture timer on server           |
| Death / respawn     | Server only | Server sets state; ClientRpc plays death VFX    |
| UI / VFX / camera   | Client only | No network cost                                 |

### Movement Replication
```
Tick rate: 20Hz (configurable via NetworkManager.sendRate)

Client (owner):
  Input → ShipController → local position update (responsive, no interp)
  [Command] CmdSendInput(Vector2 move, float heading)

Server:
  Receive input → validate (speed cap = baseSpeed * 1.1)
  Apply movement authoritatively
  NetworkTransform broadcasts position + rotation + velocity to others

Remote clients:
  Receive NetworkTransform snapshot
  Smooth interpolation (Mirror's built-in NetworkTransform, interpolation factor tunable)

Lag compensation (Phase 3+):
  Store position history ring buffer (150ms depth) on server
  On hit validation: rewind to (serverTime - RTT/2), capped at 150ms
```

### Projectile / Hit Validation
```
1. Client presses fire:
   [Command] CmdFireWeapon(weaponId, Vector3 origin, Vector3 direction, double clientTimestamp)

2. Server receives:
   a. Validate cooldown (reject if fired too early)
   b. Validate origin within tolerance of server-known ship position
   c. Spawn NetworkProjectile (server-owned object)
   d. [ClientRpc] RpcSpawnProjectileVisual → all clients instantiate visual copy

3. Server simulates projectile each FixedUpdate:
   Kinematic move → OverlapSphere check → if hit NetworkShip:
     a. Rewind target position by RTT/2 for lag compensation
     b. Register hit → call target.TakeDamage(damage, attackerNetId)
     c. [ClientRpc] RpcPlayHitEffect(position)
     d. Despawn NetworkProjectile

4. NO client-reported hits are ever accepted.
```

### Damage + Gold Attribution
```
Server-side DamageTracker per target:
  Dictionary<netId, DamageRecord> where DamageRecord = {attackerNetId, cumulativeDamage, timestamp}

On kill:
  - Killer receives KillGold (ShipClassData.tier × 50g base) [A5]
  - Assists: any attacker with >10% total damage in last 10s receives AssistGold (KillGold × 0.4)
  - Damage records cleared on kill

On camp clear:
  - Team members within 20u radius share goldReward equally

On objective tick:
  - GoldManager.AddGold(amount, allTeamMembers) each tickInterval

Shop validation (server-side ShopManager):
  - [Command] CmdRequestPurchase(upgradeId)
  - Server checks: playerGold >= cost, ship class allows upgrade, level prereq met
  - On success: deduct gold (SyncVar), apply StatModifier via ShipUpgradeHandler
  - On fail: [TargetRpc] RpcPurchaseFailed(reason) → client shows error toast
```

### Transport Configuration
```csharp
// NetworkManagerGame.cs (Mirror)
// Phase 3: KCP (UDP, mobile-friendly)
GetComponent<kcp2k.KcpTransport>().Port = 7777;

// Phase 4: Switch to headless:
//   Build target: Server (Dedicated Server in Unity 6)
//   UNITY_SERVER define → strip all rendering, camera, client-input code
//   Matchmaker: REST POST /match → returns {serverIP, port, matchId}
//   Client: ConnectToServer(ip, port) after matchmaker response
```

---

## F) FIRST 7 DAYS — EXACT CHECKLIST

> One engineer, full days. Adjust if part-time or team. [A6]

---

### DAY 1 — Project Foundation
**Scenes created:** `Bootstrap.unity`, `DevSandbox.unity`

**Scripts:**
- `Core/ServiceLocator.cs` — static generic registry; `Register<T>`, `Get<T>`
- `Core/EventBus.cs` — typed event pub/sub; `Publish<T>`, `Subscribe<T>`, `Unsubscribe<T>`
- `Utils/LayerMaskConstants.cs` — all layer names as `public const string`

**Tasks:**
- [ ] Create Unity 6 LTS project at `C:\game\battleships` via Unity Hub
- [ ] Install packages: Mirror (Package Manager git URL), URP, Input System, Cinemachine, TextMeshPro
- [ ] Configure URP pipeline asset (PC tier: shadows on, MSAA 4x | Mobile tier: shadows off, no MSAA)
- [ ] Set up Bootstrap scene: GameManager stub + ServiceLocator Init on DontDestroyOnLoad object
- [ ] Init git repo, add Unity .gitignore, first commit, create `develop` branch

**Tests:**
- EditMode: `ServiceLocator` registers and resolves type correctly
- EditMode: `EventBus` delivers event to subscriber, not to unsubscribed handler
- PlayMode: Bootstrap loads without errors or null refs

---

### DAY 2 — Camera System
**Scripts:**
- `Camera/IsometricCameraController.cs` — follow target, zoom [min/max], rotation, smooth damping
- `Input/InputRouter.cs` — detect platform, instantiate correct handler, expose `IInputHandler`
- `Input/PCInputHandler.cs` — WASD, mouse look, scroll wheel zoom, middle-mouse drag rotate
- `Input/MobileInputHandler.cs` — twin-stick (left joystick move, right joystick aim), pinch zoom, 2-finger rotate

**Prefabs:**
- `Prefabs/Map/CameraRig.prefab` — Cinemachine Virtual Camera + IsometricCameraController

**Tasks:**
- [ ] Implement IsometricCameraController with Cinemachine follow target
- [ ] PC: scroll wheel zoom, middle-drag rotation
- [ ] Mobile: pinch gesture zoom, 2-finger swipe rotation
- [ ] Clamp zoom: 3m–40m; clamp rotation: full 360°
- [ ] Place placeholder ship cube in DevSandbox; camera follows it

**Tests:**
- PlayMode: Zoom clamped at [3, 40], no overshoot
- PlayMode: Rotation wraps cleanly (no gimbal flip)
- PlayMode: InputRouter returns PCInputHandler in Editor, MobileInputHandler when forced

---

### DAY 3 — Ship Movement
**Scripts:**
- `Ships/ShipController.cs` — reads `IInputHandler`, applies velocity + turn, uses `ShipStats`
- `Ships/ShipStats.cs` — runtime stat container; recalculated when upgrades applied
- `Ships/ShipHealth.cs` — maxHP, currentHP, armor, `TakeDamage(float, netId)`, `OnDeath` UnityEvent

**Prefabs:**
- `Prefabs/Ships/ShipBase.prefab` — Rigidbody (kinematic), CapsuleCollider, ShipController, ShipStats, ShipHealth

**Data:**
- `Data/Ships/SC_Frigate.asset` — first populated ShipClassData

**Tasks:**
- [ ] ShipController: WASD → forward force, A/D → turn torque (or direct angle); mobile: left stick vector
- [ ] Speed and turn rate sourced from ShipStats (loaded from ShipClassData)
- [ ] Kinematic Rigidbody: no gravity, constrained Y position
- [ ] ShipHealth: armor reduces incoming damage by flat %

**Tests:**
- PlayMode: Ship reaches max speed from ShipClassData; cannot exceed it
- PlayMode: Turn rate matches ShipClassData.baseTurnRate within 5%
- PlayMode: TakeDamage reduces currentHP; armor applied correctly
- PlayMode: OnDeath fires exactly once at 0 HP

---

### DAY 4 — Primary Weapon (Cannon)
**Scripts:**
- `Weapons/WeaponBase.cs` — abstract: cooldown tracking, `TryFire()`, salvo coroutine
- `Weapons/CannonWeapon.cs` — computes arc trajectory, spawns CannonShell from pool
- `Weapons/ProjectileBase.cs` — abstract: lifetime countdown, `OnHit()` abstract, return-to-pool
- `Weapons/CannonShell.cs` — parabolic arc motion (`Mathf.Lerp` height), splash OverlapSphere on land
- `Weapons/ProjectileHitValidator.cs` — offline: immediate hit confirm; online: server-side stub
- `Utils/ObjectPool.cs` — generic pool `Get<T>()`, `Return(T)`

**Prefabs:**
- `Prefabs/Projectiles/CannonShell.prefab` — mesh + SphereCollider (trigger) + CannonShell
- `Prefabs/Effects/CannonMuzzleFlash.prefab` — particle system stub

**Data:**
- `Data/Weapons/WD_Cannon_Frigate.asset` — populated WeaponData

**Tasks:**
- [ ] Arc motion: lerp from origin to target over flightTime, sin curve for height
- [ ] On land: OverlapSphere(blastRadius) → call TakeDamage on all hits in radius
- [ ] Cooldown: WeaponBase blocks re-fire until cooldown elapsed
- [ ] Salvo: fire salvoCount shells at salvoInterval, spread applied per shell
- [ ] Pool: pre-warm 20 CannonShell instances on scene load

**Tests:**
- PlayMode: Shell arc peaks at arcHeight, lands at aim target (±2u tolerance)
- PlayMode: Splash applies to objects within blastRadius; not outside
- PlayMode: Cooldown blocks fire; fires again exactly after cooldown
- PlayMode: Pool: 20 shells cycled, no GC alloc after warm-up (Profiler confirm)

---

### DAY 5 — Water + Environment Blockout
**Assets:**
- `Art/Environment/WaterMaterial.mat` — ShaderGraph: scrolling normal maps ×2, depth fog, foam
- `Art/Environment/WaterMaterial_Mobile.mat` — simplified: single normal, no foam, lower res

**Prefabs:**
- `Prefabs/Map/WaterPlane.prefab` — tessellated quad, WaterMaterial
- `Prefabs/Map/HarborZone_Alpha.prefab` — box + BoxCollider trigger stub + Harbor.cs stub
- `Prefabs/Map/LaneDivider_Rock.prefab` — grey box mesh as island placeholder

**Tasks:**
- [ ] ShaderGraph water: 2 scrolling normal layers at different speeds/angles
- [ ] Depth fog: sample scene depth, lerp to fog color (URP DepthTexture)
- [ ] Mobile LOD: material keyword to strip foam + second normal
- [ ] DevSandbox: rough map blockout (2 lane corridors, open flank, 2 harbor zones, 3 camp markers)
- [ ] Profile: open Unity Profiler in mobile simulation, confirm frame <16.7ms

**Tests:**
- Manual visual: water renders without shader errors on URP
- PlayMode: Frame time <16.7ms on mobile quality tier (Profiler CPU frame budget)
- Manual: mobile LOD variant visually acceptable (no major artifact)

---

### DAY 6 — Economy + Shop Stub
**Scripts:**
- `Economy/GoldManager.cs` — singleton; `AddGold(int, playerId)`, `SpendGold(int, playerId)`, `OnGoldChanged` event
- `Economy/KillTracker.cs` — tracks damage per attacker per target; awards gold on death
- `Economy/ShopManager.cs` — validates purchase against GoldManager + ShipUpgradeHandler
- `Ships/ShipUpgradeHandler.cs` — list of purchased UpgradeData; calls `ShipStats.Recalculate()`

**Prefabs:**
- `Prefabs/UI/Shop/ShopUI.prefab` — 5 category tabs, 5 level buttons each, cost labels
- `Prefabs/UI/HUD/GoldDisplay.prefab` — TMP gold counter with coin icon

**Data (full Hull category):**
- `Data/Upgrades/Hull/UP_Hull_L1.asset` through `UP_Hull_L5.asset`

**Tasks:**
- [ ] GoldManager: stores per-player gold dict; events propagate to HUD
- [ ] KillTracker: damage record per target; assist window 10s; kill/assist gold formula
- [ ] ShopUI: each button shows current level, cost, locked state (no prereq met)
- [ ] On purchase: GoldManager.SpendGold → ShipUpgradeHandler.AddUpgrade → ShipStats.Recalculate
- [ ] ShipStats.Recalculate: iterate all UpgradeData.bonuses, sum StatModifiers by type

**Tests:**
- PlayMode: Player earns kill gold when target HP reaches 0
- PlayMode: Purchase deducts correct gold amount from GoldManager
- PlayMode: Purchasing beyond available gold rejected; gold unchanged
- PlayMode: StatModifier applied: ShipStats.maxHP increases after Hull L1 purchase
- EditMode: UpgradeData cost curve assets load and deserialize without error

---

### DAY 7 — Integration + First Playable Loop
**Scripts:**
- `Core/GameManager.cs` — owns MatchStateMachine, ship spawner, team assignment
- `Core/MatchStateMachine.cs` — states: WaitingForPlayers → Countdown → Playing → GameOver
- `Map/Harbor.cs` — team tag, HP pool, `TakeDamage()`, `OnDestroyed` → GameManager.DeclareWinner

**Prefabs:**
- `Prefabs/Map/Harbor.prefab` — box mesh + BoxCollider + Harbor.cs + HealthBar UI stub

**Scenes:**
- `Scenes/GameplayMVP.unity` — Day 5 blockout + two Harbor prefabs + ship spawn points

**Tasks:**
- [ ] GameManager: spawns player ship at spawn point, assigns team, wires OnDeath
- [ ] MatchStateMachine: Countdown shows 5s UI timer, transitions to Playing
- [ ] Harbor.TakeDamage: only projectiles of opposing team deal damage
- [ ] Harbor.OnDestroyed → GameManager.DeclareWinner(winningTeam) → GameOver state
- [ ] GameOver: display winner text, "Return to Menu" button
- [ ] Bot stub: one BotBrain that moves toward enemy Harbor and fires cannon

**Tests:**
- PlayMode: Full loop — spawn → move → shoot → earn gold → buy upgrade → shoot Harbor → GameOver fires
- PlayMode: MatchStateMachine transitions: WaitingForPlayers → Countdown → Playing → GameOver in correct order
- PlayMode: Harbor destroyed by correct team triggers correct winner
- PlayMode: Harbor not damaged by own team (friendly fire check)
- PlayMode: Return to Menu button loads MainMenu scene cleanly

---

## G) FOLLOW-UP QUESTIONS (max 6, only if needed)

1. **Art pipeline** — Do you have a 3D artist, or should the plan budget for
   asset store / AI-generated art during prototype and MVP phases?

2. **Team size & time** — How many engineers are on this project?
   (Milestone day estimates assume 1 senior full-time; revise if team/part-time.)

3. **Minimum mobile spec** — Lowest Android device you must support?
   (Affects water shader, polygon budget, draw call limits, shadow quality.)

4. **Networking region** — Primary server region for PvP Alpha?
   (Affects relay/transport choice and latency targets for non-LAN players.)

5. **wc3bs.com ship data** — That site was unreachable. Do you have a local
   copy, screenshot, or exported data from it? The armory ship data would help
   tune archetype stat ratios more accurately.

6. **Monetization model** — Free-to-play with cosmetic shop, or premium
   (paid upfront)? Affects battle pass priority and store integration scope.

---

### Key Assumptions Made
| ID  | Assumption |
|-----|-----------|
| A1  | Ship class upgrade is one-way per match (no downgrade) |
| A2  | Lane objectives provide soft blocking (deny Harbor direct assault) not hard gate |
| A3  | UI Toolkit for shop/menus; legacy Canvas for in-world HUD elements (perf) |
| A4  | Battle pass is stub only in Phase 5; full implementation post-launch |
| A5  | Kill gold base = tier × 50g (Corvette 50g → Ironclad 200g); tunable via SO |
| A6  | Dev-day estimates assume 1 senior engineer, 8h/day; no QA bottleneck |
