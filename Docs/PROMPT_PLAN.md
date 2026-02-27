# Iron Tide: Naval Supremacy — Prompt Plan
## Kant-en-klare Claude Code prompts per feature

> **Gebruik:** Kopieer een prompt, plak hem in Claude Code (dit programma), en laat hem uitvoeren.
> Voer prompts in de aangegeven volgorde uit — elk bouwt voort op het vorige.
> Alle scripts staan al in `Assets/_Game/Scripts/`. Prompts richten zich op scene-setup,
> prefab-configuratie, ShaderGraph, UI en iteratie.

---

## FASE 1 — PROTOTYPE (Week 1–3)

---

### PROMPT 1A — Bootstrap Scene aanmaken
```
Open het Unity project in C:\game\battleships.
Maak de scene Bootstrap.unity aan in Assets/_Game/Scenes/.
Voeg een leeg GameObject toe genaamd "GameBootstrap".
Voeg hieraan toe: GameManager, MatchStateMachine, AudioManager, InputRouter, KillTracker.
Zet DontDestroyOnLoad aan op GameBootstrap.
Maak de scene DevSandbox.unity aan in dezelfde map.
Sla beide scenes op.
Voeg beide toe aan Build Settings (Bootstrap als index 0, DevSandbox als index 1).
```

---

### PROMPT 1B — URP Quality Tiers configureren
```
In het Unity project C:\game\battleships:
Maak twee URP Pipeline Assets aan:
  1. URP_PC.asset     — schaduwen AAN, MSAA 4x, reflection probes AAN
  2. URP_Mobile.asset — schaduwen UIT, MSAA UIT, geen tessellatie

Ga naar Project Settings → Quality:
  Voeg quality level "PC" toe → koppel URP_PC.asset
  Voeg quality level "Mobile" toe → koppel URP_Mobile.asset
  Zet "Mobile" als default voor Android en iOS

Maak de volgende layers aan in Project Settings → Tags & Layers:
  Layer 6: Ships
  Layer 7: Projectiles
  Layer 8: MapObjects
  Layer 9: Minimap
```

---

### PROMPT 1C — ShipBase prefab bouwen
```
In Assets/_Game/Prefabs/Ships/ maak het prefab ShipBase.prefab aan.

Structuur:
  Root GameObject "ShipBase"
    ├── Rigidbody (isKinematic=true, useGravity=false,
    │              constraints: FreezeY + FreezeRotX + FreezeRotZ)
    ├── CapsuleCollider (radius=1.5, height=4, layer=Ships)
    ├── ShipController (script)
    ├── ShipStats (script)
    ├── ShipHealth (script)
    ├── ShipTeam (script)
    ├── ShipUpgradeHandler (script)
    └── MuzzlePoint (leeg transform, positie forward op het schip)

Maak het ScriptableObject SC_Frigate.asset aan in Assets/_Game/Data/Ships/ met:
  shipId = "frigate"
  displayName = "Frigate"
  archetype = Frigate
  baseMaxHP = 1000
  baseArmor = 0.05
  baseSpeed = 10
  baseTurnRate = 80
  baseAcceleration = 14
  tier = 2
  purchaseCost = 500

Koppel SC_Frigate.asset aan ShipStats op ShipBase.prefab.
Roep ShipStats.InitFromData() aan vanuit ShipHealth.Awake.
```

---

### PROMPT 1D — Camera Rig prefab en DevSandbox scene
```
In Assets/_Game/Prefabs/Map/ maak CameraRig.prefab aan:
  Root "CameraRig"
    ├── IsometricCameraController (script)
    │     zoomMin=3, zoomMax=40, zoomCurrent=15, pitch=45
    └── CinemachineCamera (Cinemachine Virtual Camera)
          Follow: [koppel later aan schip transform]
          Body: Transposer (offset Y=15, Z=-10)
          Aim: Do Nothing

Open DevSandbox.unity.
Voeg toe:
  - Een Plane (schaal 50×50) als wateroppervlak (tijdelijk grijs materiaal)
  - ShipBase prefab op positie (0, 0, 0), kopie van SC_Frigate.asset ingevuld
  - CameraRig prefab, IsometricCameraController.target = het schip
  - Een Directional Light
  - GameObject "Managers" met: InputRouter, ObjectPool (voor shells, pre-warm 20)

Speel de scene af en controleer:
  - Schip beweegt met WASD
  - Camera volgt en kan zoomen/roteren
```

---

### PROMPT 1E — CannonShell prefab + ObjectPool
```
In Assets/_Game/Prefabs/Projectiles/ maak CannonShell.prefab:
  Root "CannonShell"
    ├── MeshFilter + MeshRenderer (Sphere, schaal 0.3)
    ├── SphereCollider (radius=0.3, isTrigger=true, layer=Projectiles)
    └── CannonShell (script, component)

In Assets/_Game/Prefabs/Effects/ maak CannonMuzzleFlash.prefab:
  Root "MuzzleFlash"
    └── ParticleSystem (simpele burst: 10 deeltjes, 0.15s duur, auto-destroy)

Maak WD_Cannon_Frigate.asset in Assets/_Game/Data/Weapons/:
  weaponId = "cannon_frigate"
  weaponType = Cannon
  projectilePrefab = CannonShell.prefab
  projectileSpeed = 18
  projectileLifetime = 3.5
  arcHeight = 3
  blastRadius = 2.5
  baseDamage = 80
  damageVariance = 0.1
  cooldown = 3
  salvoCount = 1
  muzzleFlashPrefab = CannonMuzzleFlash.prefab

Voeg CannonWeapon toe aan ShipBase.prefab:
  weaponData = WD_Cannon_Frigate.asset
  muzzlePoint = MuzzlePoint transform
  shellPool = ObjectPool GameObject in scene

Voeg ObjectPool GameObject toe aan DevSandbox scene:
  prefab = CannonShell.prefab
  initialSize = 20

Test: links muisklik vuurt een kanon; schelp reist in boog en landt.
```

---

### PROMPT 1F — Water ShaderGraph (URP)
```
Maak een ShaderGraph shader aan in Assets/_Game/Art/Environment/Water.shadergraph
Type: URP Lit Shader Graph, surface=Transparent

Nodes (verbind in deze volgorde):
1. Time → Multiply(0.05) → Add met UV → Sample Texture2D (Normal map 1) → Normal Strength(0.6)
2. Time → Multiply(0.03) → Add met UV (andere richting) → Sample Texture2D (Normal map 2) → Normal Strength(0.4)
3. Beide normals → Blend (Mode=Normal) → Fragment Normal In Tangent Space
4. Scene Depth node → Remap(0,2,0,1) → Lerp(albedo kleur donkerblauw, schuim wit) → Base Color
5. Smoothness = 0.85, Metallic = 0.0

Maak WaterMaterial.mat en WaterMaterial_Mobile.mat:
  WaterMaterial:        beide normalmaps actief
  WaterMaterial_Mobile: alleen normal map 1, geen schuim, lagere intensiteit

Maak WaterPlane.prefab in Assets/_Game/Prefabs/Map/:
  Plane mesh (Unity standaard), schaal (50,1,50), WaterMaterial
  Layer = Water

Voeg WaterPlane toe aan DevSandbox scene (vervangt tijdelijk grijze plane).
Profiel de GPU in Mobile quality tier: target < 3ms voor watershader.
```

---

## FASE 2 — MVP vs AI (Week 4–10)

---

### PROMPT 2A — Alle 4 schepen aanmaken
```
Maak op basis van ShipBase.prefab vier ship-prefabs aan in Assets/_Game/Prefabs/Ships/:
  Ship_Corvette.prefab
  Ship_Frigate.prefab
  Ship_Destroyer.prefab
  Ship_Ironclad.prefab

Maak de bijbehorende ShipClassData assets in Assets/_Game/Data/Ships/:

SC_Corvette.asset:
  tier=1, cost=0, speed=14, HP=600, turnRate=120, accel=20
  primaryWeapon = WD_Cannon_Light.asset (maak ook aan: dmg=60, cooldown=2.5)

SC_Destroyer.asset:
  tier=3, cost=900, speed=11, HP=800, turnRate=90, accel=16
  primaryWeapon = WD_Cannon_Light.asset
  secondaryWeapon = WD_Torpedo_Std.asset (maak ook aan: dmg=180, cooldown=8, steering=0.5)

SC_Ironclad.asset:
  tier=4, cost=1500, speed=6, HP=1600, turnRate=40, accel=8
  primaryWeapon = WD_Cannon_Heavy.asset (maak ook aan: dmg=120, cooldown=5, arcHeight=4)

Koppel elk asset aan het bijbehorende prefab via ShipStats.
Voeg visueel onderscheid toe: pas MeshRenderer kleur aan (blauw=Corvette, groen=Frigate, rood=Destroyer, grijs=Ironclad).
```

---

### PROMPT 2B — Harbor en win-conditie
```
Maak OBJ_Harbor.asset in Assets/_Game/Data/Objectives/:
  objectiveId = "harbor"
  type = Harbor
  maxHP = 8000

Maak Harbor.prefab in Assets/_Game/Prefabs/Map/:
  Cube mesh (schaal 4×3×6), MeshRenderer (kleur afhankelijk van team)
  BoxCollider (isTrigger=false, layer=MapObjects)
  BoxCollider trigger (isTrigger=true, groter: 8×4×10) voor shop-zone detectie
  Harbor script (objectiveData = OBJ_Harbor.asset)
  HealthBar UI (Canvas in world space, Slider component)

Open GameplayMVP.unity (maak scene aan als die niet bestaat).
Voeg twee Harbor prefabs toe:
  Harbor_Team0 op positie (-40, 0, 0), teamId=0 (blauw)
  Harbor_Team1 op positie (40, 0, 0),  teamId=1 (rood)

Test: schiet op vijandige harbor → bij 0 HP → GameManager.DeclareWinner() aangeroepen
     → MatchStateMachine gaat naar GameOver staat.
```

---

### PROMPT 2C — Upgrade data assets aanmaken (alle 5 categorieën)
```
Maak in Assets/_Game/Data/Upgrades/ de volgende assets aan:

Hull/ (5 levels):
  UP_Hull_L1.asset: level=1, cost=150, bonus=[MaxHP +8% multiplicatief (×1.08)]
  UP_Hull_L2.asset: level=2, cost=300, bonus=[MaxHP ×1.08], prereq=UP_Hull_L1
  UP_Hull_L3.asset: level=3, cost=500, bonus=[MaxHP ×1.10], prereq=UP_Hull_L2
  UP_Hull_L4.asset: level=4, cost=800, bonus=[MaxHP ×1.10], prereq=UP_Hull_L3
  UP_Hull_L5.asset: level=5, cost=1200, bonus=[MaxHP ×1.12], prereq=UP_Hull_L4

Engine/ (zelfde patroon, stat=Speed):
  L1 ×1.08, L2 ×1.08, L3 ×1.10, L4 ×1.10, L5 ×1.12

Cannon/ (stat=CannonDamage additief):
  L1 +10, L2 +10, L3 +15, L4 +15, L5 +20

Torpedo/ (stat=TorpedoDamage additief):
  L1 +15, L2 +15, L3 +20, L4 +20, L5 +30

Utility/ (stat=AbilityCooldown multiplicatief):
  L1 ×0.92, L2 ×0.92, L3 ×0.90, L4 ×0.90, L5 ×0.85

Koppel prerequisite-velden correct (elk level verwijst naar vorig level).
```

---

### PROMPT 2D — ShopUI bouwen
```
Maak ShopUI.prefab in Assets/_Game/Prefabs/UI/Shop/:

Canvas (Screen Space Overlay)
└── ShopPanel (Image achtergrond, 600×500)
    ├── Title (TMP tekst "HARBOUR SHOP")
    ├── CategoryTabs (Horizontal Layout Group)
    │   ├── Tab_Hull   (Button + TMP "Hull")
    │   ├── Tab_Engine (Button + TMP "Engine")
    │   ├── Tab_Cannon (Button + TMP "Cannon")
    │   ├── Tab_Torpedo(Button + TMP "Torpedo")
    │   └── Tab_Utility(Button + TMP "Utility")
    ├── UpgradeGrid (Vertical Layout Group, 5 rijen)
    │   └── UpgradeSlot (prefab: level label, cost label, Buy-knop, lock-icon)
    └── CloseButton

Maak ShopUI.cs in Assets/_Game/Scripts/UI/Shop/:
  - Toont ShopPanel wanneer speler in Harbor trigger-zone staat
  - Actieve categorie-tab bepaalt welke UpgradeData[] getoond wordt
  - Buy-knop roept ShopManager.TryPurchase() aan
  - Toont PurchaseResult als toast bericht (groen=succes, rood=fout)
  - GoldDisplay.cs update bij GoldChangedEvent

GoldDisplay.prefab in Assets/_Game/Prefabs/UI/HUD/:
  TMP tekst: "Gold: 0"
  Abonneert op GoldChangedEvent, update tekst.
```

---

### PROMPT 2E — Bot-systeem activeren
```
Maak de volgende BotTuningData assets in Assets/_Game/Data/Bots/:

BOT_Easy.asset:
  difficulty=Easy, aimErrorDegrees=12, reactionTime=1.0, leadShotFactor=0.2
  aggressionBias=0.4, retreatHPThreshold=0.35, shopPriorityBias=0.3
  upgradePriority=[Hull, Cannon, Engine, Torpedo, Utility]

BOT_Medium.asset:
  difficulty=Medium, aimErrorDegrees=6, reactionTime=0.5, leadShotFactor=0.5
  aggressionBias=0.6, retreatHPThreshold=0.25, shopPriorityBias=0.5

BOT_Hard.asset:
  difficulty=Hard, aimErrorDegrees=2, reactionTime=0.2, leadShotFactor=0.9
  aggressionBias=0.8, retreatHPThreshold=0.15, shopPriorityBias=0.7

Voeg aan Ship_Frigate.prefab toe (voor bots):
  BotBrain (script, tuningData=BOT_Medium.asset)
  BotSteering (script)
  BotCombatModule (script, primaryWeapon=CannonWeapon)

In GameManager.cs:
  SpawnPlayer(teamId, spawnPoint)  → instantieert schip zonder BotBrain
  SpawnBot(teamId, spawnPoint, botData) → instantieert schip met BotBrain actief

Test in GameplayMVP: 1 speler vs 5 bots (3v3 met 2 bots per team + speler).
```

---

### PROMPT 2F — Minimap en HUD
```
In GameplayMVP.unity:
Voeg een Camera toe genaamd "MinimapCamera":
  orthographic, size=60, rotation (90,0,0), positie (0,50,0)
  Culling mask: ALLEEN layer Minimap
  Target Texture: maak MinimapRenderTexture.asset (256×256, RFloat formaat)

Maak MinimapUI.prefab in Assets/_Game/Prefabs/UI/:
  RawImage (256×256, rechtsonder op scherm)
    texture = MinimapRenderTexture.asset
  Overlay layer: schip-icoontjes (kleine gekleurde cirkels per team)

Voeg MinimapIcon component toe aan elk ship-prefab:
  teamColor: blauw (team 0) of rood (team 1)

Maak HUD.prefab in Assets/_Game/Prefabs/UI/HUD/:
  Canvas (Screen Space Overlay)
  ├── GoldDisplay (linksboven)
  ├── HPBar (Slider, linksonder)
  │   HPBar.cs: abonneert op ShipHealth.CurrentHP, update slider
  ├── AbilityCooldown_1/2/3 (rechtsonder, circulaire fill-images)
  │   AbilityCooldownUI.cs: leest AbilityBase.CooldownRemaining / CooldownDuration
  ├── KillFeed (rechtsoven, TMP, laatste 3 kills als tekst)
  └── MinimapUI prefab instantie
```

---

### PROMPT 2G — NeutralCamp implementeren
```
Maak NeutralCamp.cs aan in Assets/_Game/Scripts/Map/:
  - Spawnt guardCount guards (simpele vijandige cubes) bij Awake
  - Guards hebben ShipHealth (HP=200) en bewegen naar dichtstbijzijnde speler in aggro-radius
  - Als alle guards dood zijn: AwardCampGold() → GoldManager.AddGold aan team-leden in 20u radius
  - Als respawns=true: herstart guards na respawnDelay seconden

Maak OBJ_NeutralCamp.asset in Assets/_Game/Data/Objectives/:
  type=NeutralCamp, guardCount=3, guardAggroRadius=12
  killGoldReward=150, respawns=true, respawnDelay=60

Maak NeutralCamp.prefab in Assets/_Game/Prefabs/Map/:
  Marker-mesh (oud vlaggestok, placeholder: cylinder)
  Sphere trigger collider (radius=12)
  NeutralCamp script

Voeg 3 NeutralCamp prefabs toe aan GameplayMVP.unity:
  NeutralCamp_Center  (0, 0, 0)
  NeutralCamp_Left    (-20, 0, 15)
  NeutralCamp_Right   (20, 0, 15)
```

---

## FASE 3 — PvP ALPHA (Week 11–16)

---

### PROMPT 3A — Mirror integratie: NetworkManagerGame
```
Maak NetworkManagerGame.cs in Assets/_Game/Scripts/Networking/:
  Erft van Mirror.NetworkManager

  Overschrijf:
    OnStartServer()  → SpawnServerBots(), initialiseer GoldManager server-side
    OnStopServer()   → cleanup
    OnServerAddPlayer(conn) → SpawnPlayerShip(conn)
    OnServerDisconnect(conn) → vervang met bot via ServerBotManager

  Stel in Inspector in:
    Transport: KcpTransport (poort 7777)
    maxConnections: 6
    playerPrefab: Ship_Frigate.prefab (NetworkIdentity vereist)

Voeg toe aan Bootstrap scene:
  NetworkManagerGame component
  KcpTransport component

Test: Host een spel, tweede machine (of Editor instance) join — verbinding werkt.
```

---

### PROMPT 3B — NetworkShip: beweging en schade synchroniseren
```
Maak NetworkShip.cs in Assets/_Game/Scripts/Networking/:
  Erft van Mirror.NetworkBehaviour

  SyncVars:
    [SyncVar] float networkHP  → bij wijziging: update ShipHealth.CurrentHP visueel
    [SyncVar] int   networkGold → bij wijziging: update GoldDisplay

  Commands (client → server):
    [Command] void CmdSendInput(Vector2 move, float heading)
      → server: valideer speed (max baseSpeed×1.1), pas ShipController toe
    [Command] void CmdFireWeapon(byte slot, Vector3 direction, double timestamp)
      → server: valideer cooldown, spawn NetworkProjectile

  ClientRpc (server → alle clients):
    [ClientRpc] void RpcPlayDeathEffect(Vector3 pos)
    [ClientRpc] void RpcPurchaseFailed(byte reason)

Voeg NetworkIdentity en NetworkTransform toe aan alle ship-prefabs.
NetworkTransform: syncPosition=true, syncRotation=true, interpolatePosition=true.
Stel sendRate in op 20 (20 Hz).
```

---

### PROMPT 3C — Server-side projectiel validatie
```
Maak NetworkProjectile.cs in Assets/_Game/Scripts/Networking/:
  Erft van Mirror.NetworkBehaviour

  Server-side:
    - Spawnt via NetworkServer.Spawn()
    - FixedUpdate: beweegt kinematisch (zelfde logica als CannonShell)
    - OverlapSphere per tick → als hit NetworkShip:
        Rewind target positie: PositionHistory.Sample(serverTime - RTT/2)
        Registreer hit: target.GetComponent<ShipHealth>().TakeDamage(dmg, attackerId)
        NetworkServer.Destroy(gameObject)

  Client-side (visueel):
    [ClientRpc] RpcSpawnVisualProjectile(Vector3 origin, Vector3 direction)
    → instantieert lokale (non-network) CannonShell voor visuele weergave
    → wordt vernietigd na lifetime, onafhankelijk van server

Voeg PositionHistory ring-buffer toe aan NetworkShip:
  struct PositionSample { Vector3 pos; double time; }
  PositionSample[64] history; schrijft elke FixedUpdate.
  Sample(double t) → lineaire interpolatie tussen dichtstbijzijnde samples.
```

---

### PROMPT 3D — Lobby UI
```
Maak LobbyUI.cs en Lobby.unity scene:

Lobby.unity bevat:
  LobbyUI Canvas
    ├── Panel_Host
    │   ├── Button "Host Game"   → NetworkManagerGame.StartHost()
    │   └── Port Input Field     → KcpTransport.port
    ├── Panel_Join
    │   ├── IP Input Field
    │   ├── Button "Join"        → NetworkManagerGame.StartClient()
    │   └── Button "Quick Join"  → verbindt met localhost:7777
    ├── PlayerList               → toont verbonden spelers (NetworkRoomPlayer stijl)
    ├── ReadyButton              → speler markeert zichzelf als klaar
    └── StartButton (host only)  → zichtbaar als alle spelers ready zijn

LobbyUI.cs:
  Abonneert op Mirror callbacks: OnClientConnect, OnClientDisconnect
  Update PlayerList bij elke wijziging
  StartButton → laadt GameplayMVP scene via NetworkManager.ServerChangeScene()

Voeg Ping Display toe aan HUD.prefab:
  TMP tekst rechtsboven: "Ping: --ms"
  PingDisplay.cs: update elke seconde via NetworkTime.rtt
  Kleuring: groen <80ms, geel <150ms, rood ≥150ms
```

---

## FASE 4 — DEDICATED SERVER (Week 17–20)

---

### PROMPT 4A — Headless server build
```
In Assets/_Game/Scripts/Core/GameManager.cs:
Voeg compile-guards toe voor alle client-only code:

#if !UNITY_SERVER
    // Camera initialisatie
    // VFX spawning
    // UI updates
    // InputRouter
#endif

Doe hetzelfde in:
  IsometricCameraController.cs  → hele klasse in #if !UNITY_SERVER
  InputRouter.cs                → hele klasse in #if !UNITY_SERVER
  ShipVisuals.cs                → #if !UNITY_SERVER

Maak een Build Script aan (Editor-only) in Assets/_Game/Scripts/Editor/ServerBuildScript.cs:
  [MenuItem("IronTide/Build Dedicated Server (Linux)")]
  static void BuildServer()
  BuildPipeline.BuildPlayer(scenes, "Builds/Server/IronTide.x86_64",
    BuildTarget.StandaloneLinux64,
    BuildOptions.EnableHeadlessMode);

Voeg Dockerfile toe in projectroot:
  FROM ubuntu:22.04
  COPY Builds/Server/ /app/
  EXPOSE 7777/udp
  CMD ["/app/IronTide.x86_64", "-batchmode", "-nographics", "-port", "7777"]

Test: bouw server → run in Docker → client verbindt → volledige match speelbaar.
```

---

### PROMPT 4B — Matchmaker stub (C# ASP.NET)
```
Maak een aparte map Matchmaker/ naast het Unity project (NIET in Assets/).

Maak Matchmaker/Program.cs (ASP.NET minimal API):

  POST /match
    Body: { "region": "eu", "mode": "pvp3v3" }
    Response: { "serverIp": "1.2.3.4", "port": 7777, "matchId": "uuid" }
    Logica: kies beschikbare server uit pool, markeer als bezet

  POST /server/register
    Body: { "ip": "...", "port": 7777, "maxSlots": 6 }
    Registreert dedicated server in geheugen-pool

  GET /health → 200 OK

In Unity: maak MatchmakerClient.cs (MonoBehaviour):
  async Task<MatchInfo> FindMatch()
    → UnityWebRequest POST naar matchmaker URL
    → parse JSON response
    → NetworkManagerGame.networkAddress = serverIp
    → NetworkManagerGame.StartClient()

Voeg "Find Match" knop toe aan LobbyUI naast de handmatige IP-join.
```

---

## FASE 5 — RANKED BETA (Week 21–26)

---

### PROMPT 5A — ELO/MMR systeem (server-side)
```
Maak EloCalculator.cs in Assets/_Game/Scripts/Networking/ (#if UNITY_SERVER):

  static int CalculateNewElo(int winnerElo, int loserElo, bool isWin)
    K = 32 (K-factor)
    expectedScore = 1 / (1 + pow(10, (loserElo - winnerElo) / 400))
    actualScore   = isWin ? 1 : 0
    return (int)(K * (actualScore - expectedScore))

  static void UpdateTeamElo(List<PlayerProfile> winners, List<PlayerProfile> losers)
    → berekent gemiddeld team-ELO
    → past ELO aan per speler
    → POST naar backend API of schrijft naar PlayerPrefs (offline test)

Roep aan in NetworkManagerGame.OnMatchEnd(winningTeam):
  UpdateTeamElo(winners, losers)

Maak een eenvoudige PlayerProfile klasse:
  string playerId, int elo, int wins, int losses, int assists, string[] unlockedCosmetics
```

---

### PROMPT 5B — Cosmetica systeem
```
Maak CosmeticLoadout.cs (MonoBehaviour, op het schip):
  [SyncVar] string activeSkinId   → server→client, visueel applied
  [SyncVar] string activeTrailId

  void ApplySkin(CosmeticData skin)
    → MeshRenderer.material = skin.skinMaterial
  void ApplyTrail(CosmeticData trail)
    → Instantiate(trail.trailPrefab, trailAttachPoint)

Maak LoadoutUI.prefab:
  Grid van cosmetica-items (preview-image, naam, rarity badge)
  Categorietabs: Skins / Trails / Decals / Horns
  Equip-knop → sla op in PlayerPrefs → stuur naar server bij lobby-join

Voeg CosmeticData assets toe:
  COS_DefaultSkin.asset   (source=Earnable, cost=0)
  COS_VeteranHorn.asset   (source=Earnable, cost=500 earn-currency)

Test: selecteer skin → zichtbaar voor andere spelers in online match.
Automatische test: CosmeticData._hasGameplayImpact == false voor alle assets.
```

---

## ITERATIE-PROMPTS (gebruik op elk moment)

---

### PROMPT IT-1 — Balans tunen via ScriptableObjects
```
Analyseer de huidige waarden in alle UpgradeData assets in Assets/_Game/Data/Upgrades/.
Vergelijk met de doelstellingen uit BUILDPLAN.md §A (economie-curve).
Stel aanpassingen voor als matches gemiddeld:
  - < 10 minuten eindigen → verlaag goud per kill of verhoog Harbor HP
  - > 30 minuten duren   → verhoog goud per kill of verlaag Harbor HP
  - Één schip domineert  → identificeer welke stat te hoog is en verlaag die 10%

Verander ALLEEN de .asset-waarden, geen C#-code.
Geef een tabel met voor/na-waarden.
```

---

### PROMPT IT-2 — Profiel-sessie analyseren
```
Ik heb net een Unity Profiler-sessie opgeslagen op [pad naar .data bestand].
Analyseer de top CPU-tijdverbruikers.
Geef concrete optimalisaties per systeem, specifiek voor mobile (Android, URP Mobile tier).
Prioriteer: draw calls verlagen, GC-allocaties elimineren, physics-queries reduceren.
Schrijf de optimalisaties direct in de relevante scripts.
```

---

### PROMPT IT-3 — Nieuwe ability toevoegen
```
Voeg een nieuwe ability toe: "Depth Charge"
  - Type: offensief AoE
  - Functie: gooit een explosief dat zinkt en na 2 seconden ontploft
  - Explosie-radius: 6 eenheden, schade: 200
  - Cooldown: 20 seconden
  - Beschikbaar voor: Destroyer en Ironclad

Maak:
  1. DepthChargeAbility.cs (erft AbilityBase) in Scripts/Abilities/
  2. DepthCharge.prefab in Prefabs/Projectiles/ (Sphere met SphereCollider trigger)
  3. Voeg toe aan SC_Destroyer.asset en SC_Ironclad.asset

Schrijf een PlayMode test die verifieert dat de explosie schade doet binnen straal
maar niet buiten straal.
```

---

### PROMPT IT-4 — Nieuwe kaart toevoegen
```
Maak een tweede kaart "Iron Straits" aan als nieuwe Unity scene in Assets/_Game/Scenes/.

Lay-out:
  - Centraal nauw kanaal (één lane, 12 eenheden breed)
  - Twee flanken via buitenwateren (open, 30 eenheden breed)
  - 3 eilanden in het kanaal als dekking
  - 1 NeutralCamp in het midden van het kanaal
  - Harbors aan de noord- en zuidkant (80 eenheden uit elkaar)

Gebruik bestaande prefabs: Harbor, LaneObjective, NeutralCamp, WaterPlane.
Voeg de scene toe aan Build Settings na GameplayMVP.
Voeg kaart-selectie toe aan LobbyUI (dropdown: "Twin Lanes" / "Iron Straits").
```

---

## SNEL-REFERENTIE: VOLGORDE VAN UITVOERING

```
Week 1:  1A → 1B → 1C → 1D → 1E → 1F
Week 2:  2A → 2B → 2C
Week 3:  2D → 2E → 2F → 2G  [+ handmatige test: volledige 3v3 vs bots]
Week 4+: 3A → 3B → 3C → 3D  [+ playtests met echte verbindingen]
Week 5+: 4A → 4B             [+ dedicated server deployment test]
Week 6+: 5A → 5B             [+ store submission]

Iteratie-prompts: gebruik op elk moment tussen de bovenstaande stappen.
```
