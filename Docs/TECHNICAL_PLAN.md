# Iron Tide: Naval Supremacy — Technisch Plan

> Versie 1.0 | Engine: Unity 6 LTS | Render: URP | Netwerk: Mirror + KCP

---

## 1. SYSTEEMOVERZICHT

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENT                                   │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌───────────────┐  │
│  │InputRouter│→│ShipCtrl  │→│WeaponBase │→│ProjectileBase │  │
│  └──────────┘  └────┬─────┘  └────┬─────┘  └───────┬───────┘  │
│                     │              │                 │          │
│  ┌──────────┐  ┌────▼─────┐  ┌────▼─────┐           │          │
│  │ShipStats │←│ShipUpgrade│  │AbilityBase│           │          │
│  └──────────┘  └──────────┘  └──────────┘           │          │
│                                                      │          │
│  ┌──────────┐  ┌──────────┐  ┌──────────────────────▼───────┐  │
│  │GoldMgr   │←│KillTracker│←│ShipHealth (TakeDamage events) │  │
│  └────┬─────┘  └──────────┘  └──────────────────────────────┘  │
│       │                                                         │
│  ┌────▼─────┐  ┌──────────┐  ┌──────────┐  ┌───────────────┐  │
│  │ShopMgr   │  │Harbor    │  │LaneObj.  │  │NeutralCamp    │  │
│  └──────────┘  └──────────┘  └──────────┘  └───────────────┘  │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ EventBus (typed pub/sub — decouples alle systemen)      │   │
│  └─────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ ServiceLocator (statische registry voor singletons)     │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
          ▲ Mirror [Command] / [ClientRpc] / SyncVar ▼
┌─────────────────────────────────────────────────────────────────┐
│                        SERVER (headless)                        │
│  NetworkShip | NetworkProjectile | NetworkGoldSync              │
│  ServerBotManager → BotBrain (per lege slot)                   │
│  GoldManager (authoritative) | ShopManager (validatie)         │
│  ProjectileHitValidator (lag-compensated OverlapSphere)        │
└─────────────────────────────────────────────────────────────────┘
```

---

## 2. KLASSE-RELATIES

### 2.1 Ship-hiërarchie

```
GameObject "Ship_Frigate"
├── ShipController          ← beweegt het schip via IInputHandler
├── ShipStats               ← runtime stats (base + upgrades)
├── ShipHealth              ← HP, armor, TakeDamage, OnDeath
├── ShipTeam                ← teamId (0 of 1)
├── ShipUpgradeHandler      ← lijst van UpgradeData, roept Stats.Recalculate()
├── ShipVisuals             ← animatie, wake-effect, damage-states (Phase 2)
├── CannonWeapon            ← erft WeaponBase, gebruikt ObjectPool
├── TorpedoWeapon           ← erft WeaponBase (optioneel slot)
├── SmokeScreenAbility      ← erft AbilityBase
├── SonarAbility            ← erft AbilityBase
├── RepairAbility           ← erft AbilityBase
├── BotBrain                ← alleen actief als dit een bot is
├── BotSteering             ← alleen actief als bot
├── BotCombatModule         ← alleen actief als bot
├── MinimapIcon             ← toont schip op minimap
└── AudioSource             ← engine-geluid (Phase 2)
```

### 2.2 Projectiel-hiërarchie

```
ProjectileBase  (abstract)
├── CannonShell             ← parabool, splash OverlapSphere bij landing
└── TorpedoProjectile       ← lineair + steeringFactor drift, trigger op contact
```

### 2.3 Wapen-hiërarchie

```
WeaponBase  (abstract MonoBehaviour)
├── CannonWeapon            ← leest CannonShell pool, berekent arcHeight
└── TorpedoWeapon           ← leest TorpedoProjectile pool
```

### 2.4 Economy-flow

```
ShipHealth.TakeDamage(dmg, attackerId)
  → KillTracker.RecordDamage(attackerId, victimId, dmg)
  → [bij dood] EventBus.Publish(ShipDiedEvent)
       → KillTracker.OnShipDied()
            → GoldManager.AddGold(killerId, killGold)
            → GoldManager.AddGold(assisterId, assistGold)  [≥10% dmg, ≤10s]
  → EventBus.Publish(GoldChangedEvent)
       → GoldDisplay.OnGoldChanged()  [HUD update]
```

### 2.5 Shop-flow

```
Speler rijdt Harbor in (trigger) → ShopUI.Show()
Speler klikt upgrade-knop → ShopUI.RequestPurchase(upgradeData)
  → ShopManager.TryPurchase(playerId, upgrade, handler, classData)
       → GoldManager.TrySpendGold(playerId, cost)   [server-side in online]
       → ShipUpgradeHandler.AddUpgrade(upgrade)
            → ShipStats.Recalculate()
       → return PurchaseResult (Success / InsufficientGold / CapReached / ...)
  → ShopUI.ShowResult(result)
```

---

## 3. DATA-ARCHITECTUUR (ScriptableObjects)

```
Assets/_Game/Data/
│
├── Ships/
│   ├── SC_Corvette.asset         tier=1, cost=0,    speed=14, HP=600
│   ├── SC_Frigate.asset          tier=2, cost=500,  speed=10, HP=1000
│   ├── SC_Destroyer.asset        tier=3, cost=900,  speed=11, HP=800
│   └── SC_Ironclad.asset         tier=4, cost=1500, speed=6,  HP=1600
│
├── Weapons/
│   ├── WD_Cannon_Light.asset     baseDmg=60,  cooldown=2.5s, arcH=2
│   ├── WD_Cannon_Heavy.asset     baseDmg=120, cooldown=5s,   arcH=4
│   ├── WD_Torpedo_Std.asset      baseDmg=180, cooldown=8s,   steering=0.5
│   └── WD_Torpedo_Fast.asset     baseDmg=120, cooldown=5s,   speed=25
│
├── Upgrades/
│   ├── Hull/
│   │   ├── UP_Hull_L1.asset      +8% MaxHP,  cost=150
│   │   ├── UP_Hull_L2.asset      +8% MaxHP,  cost=300
│   │   ├── UP_Hull_L3.asset      +10% MaxHP, cost=500
│   │   ├── UP_Hull_L4.asset      +10% MaxHP, cost=800
│   │   └── UP_Hull_L5.asset      +12% MaxHP, cost=1200
│   ├── Engine/   (zelfde curve, stat=Speed)
│   ├── Cannon/   (stat=CannonDamage + CannonRange)
│   ├── Torpedo/  (stat=TorpedoDamage + TorpedoSpeed)
│   └── Utility/  (stat=AbilityCooldown)
│
├── Objectives/
│   ├── OBJ_Harbor.asset          HP=8000, killGold=0 (win condition)
│   ├── OBJ_LaneObjective.asset   HP=3000, captureTime=8s, tick=10g/5s
│   └── OBJ_NeutralCamp.asset     HP=0 (guards), killGold=150, respawn=60s
│
├── Bots/
│   ├── BOT_Easy.asset            aimError=12°, reaction=1.0s, lead=0.2
│   ├── BOT_Medium.asset          aimError=6°,  reaction=0.5s, lead=0.5
│   └── BOT_Hard.asset            aimError=2°,  reaction=0.2s, lead=0.9
│
└── Cosmetics/
    └── COS_DefaultHorn.asset     (placeholder)
```

---

## 4. SCENE-STRUCTUUR

### Bootstrap.unity
```
[DontDestroyOnLoad]
└── GameBootstrap
    ├── GameManager          + MatchStateMachine
    ├── AudioManager
    ├── InputRouter
    └── KillTracker
```

### DevSandbox.unity (prototype iteratie)
```
Environment
├── WaterPlane               (WaterMaterial URP ShaderGraph)
└── GroundCollider           (invisible Y=0 plane voor physics)

Ships
└── Ship_Frigate             (alle componenten, player-controlled)

Targets
└── DummyTarget              (ShipHealth, geen controller)

Camera
└── CameraRig
    ├── IsometricCameraController
    └── CinemachineVirtualCamera

Managers
├── ObjectPool_CannonShells  (20x pre-warmed)
└── ObjectPool_Torpedoes
```

### GameplayMVP.unity
```
Environment
├── WaterPlane
├── Island_LaneA_01..03      (grey boxes, blokkades)
├── Island_LaneB_01..03
└── OpenFlank_Rocks

Teams
├── Team0_HarborZone         (Harbor.cs, teamId=0, spawn points ×3)
└── Team1_HarborZone         (Harbor.cs, teamId=1, spawn points ×3)

Objectives
├── LaneObjective_A          (LaneObjective.cs, OBJ_LaneObjective.asset)
├── LaneObjective_B
├── NeutralCamp_Center
├── NeutralCamp_Left
└── NeutralCamp_Right

Minimap
└── MinimapCamera            (orthographic overhead, Minimap layer only)

Managers
├── GoldManager
├── ShopManager
├── KillTracker
├── ObjectPool_CannonShells
├── ObjectPool_Torpedoes
├── ObjectPool_SmokeCloud
└── MinimapController
```

---

## 5. NETWERK-ARCHITECTUUR (Mirror)

### 5.1 Authoriteitsmodel

| Actie | Wie beslist | Hoe |
|-------|------------|-----|
| Beweging | Server (speed-cap) | Client → `[Command]CmdSendInput` → server past toe, broadcast |
| Projectiel spawnen | Server | Client → `[Command]CmdFireWeapon` → server spawnt `NetworkProjectile` |
| Hit-detectie | Server | Server simuleert projectiel, doet `OverlapSphere` |
| Schade toepassen | Server | `TakeDamage()` alleen server-side, HP via `SyncVar` |
| Goud | Server | `GoldManager` server-only, per-player `SyncVar` |
| Objectives | Server | `LaneObjective` capture-timer op server |
| Dood / respawn | Server | Server zet `IsDead` SyncVar, `[ClientRpc]` speelt animatie |

### 5.2 Netwerk-klassen

```csharp
// NetworkShip : NetworkBehaviour
[SyncVar] float networkHP;
[SyncVar] int   networkGold;
[SyncVar] int   networkUpgradeMask;   // bitfield per upgrade level
[Command]  void CmdSendInput(Vector2 move, float heading);
[Command]  void CmdFireWeapon(byte weaponSlot, Vector3 dir, double ts);
[Command]  void CmdActivateAbility(byte slot);
[Command]  void CmdRequestPurchase(string upgradeId);
[ClientRpc] void RpcPlayDeathEffect(Vector3 pos);
[ClientRpc] void RpcPurchaseFailed(byte reason);

// NetworkProjectile : NetworkBehaviour
// Spawned server-side; visuele kopie via ClientRpc bij alle clients
[SyncVar] Vector3 serverPosition;
// Server simuleert in FixedUpdate; clients interpoleren naar serverPosition

// NetworkGoldSync : NetworkBehaviour
// Aparte component zodat gold-sync de ship-sync niet bloat
```

### 5.3 Tick-rates en bandbreedte-budget

| Systeem | Rate | Payload/tick | ~kbps (6 spelers) |
|---------|------|-------------|-------------------|
| Beweging (NetworkTransform) | 20 Hz | 12 bytes (pos+rot) | 14 kbps |
| Projectielen (max 12 actief) | 20 Hz | 8 bytes/projectiel | 19 kbps |
| HP SyncVar | bij wijziging | 4 bytes | <1 kbps |
| Goud SyncVar | bij wijziging | 4 bytes | <1 kbps |
| **Totaal** | | | **~35 kbps** |

Doelstelling: <100 kbps per match (comfortabel op 4G mobiel).

### 5.4 Lag-compensatie (Phase 3)

```
Server houdt per NetworkShip een ring-buffer bij:
  PositionHistory[64] = { Vector3 pos, double serverTime }
  Schrijft elke FixedUpdate (50 Hz)

Bij hit-validatie:
  rewindTime = serverTime - (clientRTT / 2)      // capped op 150ms
  pos        = PositionHistory.Sample(rewindTime) // lineaire interpolatie
  hit        = OverlapSphere(pos, shipRadius)
```

---

## 6. PERFORMANCE-BUDGET

### 6.1 Mobiel (mid-range Android, doel: 60 fps)

| Categorie | Budget | Strategie |
|-----------|--------|-----------|
| CPU frame | 16.7 ms | Kinematic Rigidbodies, geen vaste physics-colliders op water |
| Draw calls | ≤ 100 | GPU instancing op schepen, static batching op omgeving |
| Driehoeken | ≤ 150k | LOD op schepen (2 levels: 1500 / 500 tris) |
| Texturen | ≤ 256 MB VRAM | ASTC compressie, atlassen per categorie |
| Schaduwen | Uit op mobiel | URP Mobile quality tier: shadows disabled |
| Water shader | ≤ 3 ms | Enkelvoudige normal-map, geen tessellatie, geen schuim |

### 6.2 PC

| Categorie | Budget |
|-----------|--------|
| CPU frame | 6 ms (144 fps target) |
| Draw calls | ≤ 300 |
| Driehoeken | ≤ 500k |
| Water shader | Twee normals, diepte-fog, schuim aan randen |

### 6.3 Netwerk (mobiel)

| Meting | Doel |
|--------|------|
| Bandbreedte | < 100 kbps upload + 100 kbps download |
| Latency (LAN) | < 30 ms gevoel |
| Latency (regionaal) | < 150 ms gevoel |
| Pakketverlies tolerantie | KCP herzendt automatisch; max 3% verlies acceptabel |

---

## 7. TESTPLAN

### 7.1 EditMode (geen scene vereist)

| Test | Klasse | Wat wordt getest |
|------|--------|-----------------|
| `ServiceLocator_RegisterAndGet` | ServiceLocatorTests | Register + Get geeft juiste instantie terug |
| `ServiceLocator_Unregister` | ServiceLocatorTests | Na Unregister geeft Get null terug |
| `EventBus_DeliverToSubscriber` | EventBusTests | Event bereikt subscriber |
| `EventBus_Unsubscribe_NoCall` | EventBusTests | Afgemelde handler ontvangt niets |
| `EventBus_NoCrossContamination` | EventBusTests | GoldEvent bereikt geen ShipDied handler |
| `UpgradeCategory_EnumValues` | UpgradeDataTests | Alle 5 categorieën aanwezig |
| `StatModifier_Additive` | UpgradeDataTests | +100 HP correct opgeteld |
| `StatModifier_Multiplicative` | UpgradeDataTests | ×1.08 correct vermenigvuldigd |
| `CosmeticData_NoGameplayImpact` | UpgradeDataTests | `_hasGameplayImpact` veld bestaat en is bool |

### 7.2 PlayMode (scene vereist)

| Test | Klasse | Wat wordt getest |
|------|--------|-----------------|
| `TakeDamage_ReducesHP` | ShipHealthTests | 200 schade → HP 1000→800 |
| `Armor_ReducesDamage` | ShipHealthTests | 50% armor → 200 schade = 100 |
| `Death_FiresExactlyOnce` | ShipHealthTests | OnDeath event exact 1× |
| `Heal_RestoresHP` | ShipHealthTests | Heal na schade herstelt correct |
| `Heal_DoesNotExceedMaxHP` | ShipHealthTests | Heal boven max = max |
| `AddGold_IncreasesBalance` | GoldManagerTests | Saldo stijgt correct |
| `TrySpendGold_Success` | GoldManagerTests | Goud afgetrokken bij voldoende saldo |
| `TrySpendGold_Fail` | GoldManagerTests | Saldo ongewijzigd bij tekort |
| `AddGold_PublishesEvent` | GoldManagerTests | GoldChangedEvent gepubliceerd |
| `Purchase_StatModApplied` | GoldManagerTests | Koop → ShipStats.MaxHP verhoogd |

### 7.3 Handmatige tests (per milestone)

| Milestone | Test |
|-----------|------|
| Phase 1 | Camera zoom [3–40m], rotatie 360°, schip bereikt maxSpeed |
| Phase 1 | Kanon vuurt boog, splash raakt doelen binnen straal |
| Phase 1 | 60 fps op Android mid-range (Unity Profiler mobiel-simulatie) |
| Phase 2 | Volledige 3v3 match vs bots van start tot einde |
| Phase 2 | Harbor vernietigd door verkeerd team → geen win |
| Phase 2 | Bot koopt upgrades na goud verdienen |
| Phase 3 | Projectiel-hit desynct niet bij 150ms gesimuleerde latency |
| Phase 3 | Speler disconnect → bot neemt slot over binnen 3s |

---

## 8. BUILD-PIPELINE

### 8.1 Git-strategie

```
main          ← alleen stabiele releases (tag v0.1, v0.2 ...)
develop       ← integratiebranch, altijd compileerbaar
feature/xxx   ← feature-branches, PR naar develop
hotfix/xxx    ← bugfix direct op main indien kritiek
```

### 8.2 Build-targets

| Target | Platform | Render | Scripting Backend |
|--------|----------|--------|------------------|
| PC Development | Windows x64 | URP PC tier | Mono (snelle iteratie) |
| PC Release | Windows x64 | URP PC tier | IL2CPP |
| Android | ARM64 | URP Mobile tier | IL2CPP |
| iOS | ARM64 | URP Mobile tier | IL2CPP |
| Server (Phase 4) | Linux x64 headless | Geen render | IL2CPP |

### 8.3 Headless server (Phase 4)

```csharp
// Compile-guards voor server-only code:
#if UNITY_SERVER
    // Geen camera, geen input, geen VFX
    // Alleen: physics, Mirror, BotBrain, GoldManager, ShopManager
#endif

// Unity 6: ingebouwd via Build Settings → Server Build
// Docker-image target: < 500 MB
// Start-commando: ./IronTide.x86_64 -batchmode -nographics -port 7777
```

---

## 9. MAPPENSTRUCTUUR (volledig)

```
C:\game\battleships\
├── .gitignore
├── Packages/
│   └── manifest.json          ← Mirror, URP, InputSystem, Cinemachine, TMP
├── Docs/
│   ├── README.md
│   ├── BUILDPLAN.md           ← volledig gameplan (A-G)
│   ├── TECHNICAL_PLAN.md      ← dit document
│   └── PROMPT_PLAN.md         ← AI-prompts per feature
└── Assets/
    └── _Game/
        ├── Art/
        │   ├── Ships/
        │   ├── Environment/
        │   ├── Effects/
        │   └── UI/
        ├── Audio/
        │   ├── SFX/
        │   └── Music/
        ├── Data/
        │   ├── Ships/          ← SC_Corvette/Frigate/Destroyer/Ironclad.asset
        │   ├── Weapons/        ← WD_Cannon_Light/Heavy/Torpedo_Std/Fast.asset
        │   ├── Modules/
        │   ├── Upgrades/
        │   │   ├── Hull/       ← UP_Hull_L1..L5.asset
        │   │   ├── Engine/
        │   │   ├── Cannon/
        │   │   ├── Torpedo/
        │   │   └── Utility/
        │   ├── Objectives/     ← OBJ_Harbor/LaneObjective/NeutralCamp.asset
        │   ├── Bots/           ← BOT_Easy/Medium/Hard.asset
        │   └── Cosmetics/
        ├── Prefabs/
        │   ├── Ships/
        │   ├── Projectiles/
        │   ├── Effects/
        │   ├── Map/
        │   └── UI/
        ├── Scenes/
        │   ├── Bootstrap.unity
        │   ├── MainMenu.unity
        │   ├── Lobby.unity
        │   ├── GameplayMVP.unity
        │   └── DevSandbox.unity
        ├── Scripts/
        │   ├── IronTide.asmdef
        │   ├── Core/
        │   ├── Ships/
        │   ├── Weapons/
        │   ├── Abilities/
        │   ├── Economy/
        │   ├── Map/
        │   ├── Networking/
        │   ├── UI/
        │   ├── Camera/
        │   ├── Input/
        │   ├── Bots/
        │   ├── Audio/
        │   └── Utils/
        └── Tests/
            ├── EditMode/
            └── PlayMode/
```

---

## 10. RISICO-REGISTER

| # | Risico | Kans | Impact | Maatregel |
|---|--------|------|--------|-----------|
| R1 | Watershader te zwaar op mobiel | Hoog | Hoog | Profiler dag 5; flat-color fallback klaar |
| R2 | Bot-navigatie slecht op onregelmatige kaart | Middel | Middel | NavMesh + carved obstacles; waypoint-fallback |
| R3 | Mirror KCP NAT traversal probleem op mobiel | Middel | Hoog | Edgegap relay free-tier als backup |
| R4 | Projectiel-desync bij hoge latency | Laag | Hoog | Lag-comp capped op 150ms; boven dat: miss |
| R5 | Unity headless licentie-kosten (Phase 4) | Laag | Middel | Unity 6 Personal/Plus gratis onder revenue-drempel |
| R6 | Scope creep art in Phase 2 | Hoog | Middel | Strict: alleen grey-box art t/m Phase 2 einde |
| R7 | Balans (matches te snel beslist) | Middel | Middel | Goud-instrumentatie week 9; tweak via SO's |
