# Iron Tide: Naval Supremacy — Roadmap + Prompts

**Van niets naar live game — stap voor stap.**
Globaal plan. Onderweg passen we dingen aan.

> **Gebruik:** Elke stap heeft een `PROMPT →` blok.
> Kopieer dat blok, plak het in Claude Code, laat uitvoeren.
> Vink de stap af als hij klaar is. Ga door naar de volgende.

---

## HOE DIT WERKT

```
Jij opent Unity / Blender / terminal
    ↓
Jij leest de stap
    ↓
Jij kopieert de PROMPT en plakt die hier in Claude Code
    ↓
Claude Code bouwt / schrijft / configureert
    ↓
Jij test in Unity ("werkt het?")
    ↓
Volgende stap
```

Als iets niet werkt: plak de foutmelding hier in dit gesprek.
We lossen het op en gaan verder.

---

# FASE 1 — PROTOTYPE
## Doel: iets beweegt, iets schiet, iets ontploft. Speelbaar in 3 weken.

---

## STAP 1 — Unity project opzetten
**Tijd:** 2 uur | **Eenmalig**

### Wat je handmatig doet:
1. Open **Unity Hub**
2. Klik **New Project**
3. Kies template: **Universal 3D (URP)**
4. Naam: `IronTide`
5. Locatie: `C:\game\battleships`
6. Klik **Create Project**
7. Wacht tot Unity opent (eerste keer ~5 min)

### Daarna plak je dit in Claude Code:

```
PROMPT 1 — Project basisstructuur aanmaken

Het Unity 6 project staat in C:\game\battleships.
URP is al geïnstalleerd via de Universal 3D template.

Doe het volgende:

1. Installeer deze packages via Package Manager (Window → Package Manager):
   - Mirror Networking: git URL https://github.com/MirrorNetworking/Mirror.git#latest
   - Input System: com.unity.inputsystem
   - Cinemachine: com.unity.cinemachine
   - TextMeshPro: al aanwezig, zo niet installeer het
   - ProBuilder: com.unity.probuilder
   - VFX Graph: com.unity.visualeffectgraph

2. Maak deze mappenstructuur aan in Assets/:
   _Game/
     Scripts/
       Core/
       Ship/
       Weapons/
       Abilities/
       Economy/
       AI/
       Networking/
       UI/
       Objectives/
       Match/
       Camera/
       Data/
       Utils/
     Tests/
       EditMode/
       PlayMode/
     Data/
       Ships/
       Weapons/
       Upgrades/
     Prefabs/
       Ships/
       Projectiles/
       Environment/
     Art/
       Ships/
       Environment/
       UI/
         Fonts/
       VFX/
     Audio/
       SFX/
       Music/
     Scenes/

3. Maak twee scenes aan in Assets/_Game/Scenes/:
   - Bootstrap.unity
   - DevSandbox.unity

4. Voeg beide toe aan Build Settings (Bootstrap index 0, DevSandbox index 1)

5. Maak aan in Project Settings → Tags & Layers:
   Layer 6: Ships
   Layer 7: Projectiles
   Layer 8: MapObjects
   Layer 9: Water
   Layer 10: HarborZone

6. Maak twee URP Pipeline Assets aan:
   - Assets/_Game/Art/URP_PC.asset
     (schaduwen AAN, MSAA 4x)
   - Assets/_Game/Art/URP_Mobile.asset
     (schaduwen UIT, MSAA UIT)
   Koppel in Project Settings → Quality

Rapporteer welke stappen gelukt zijn en welke niet.
```

**Check:** Unity opent zonder errors. Alle mappen zichtbaar. Beide scenes in Build Settings.

---

## STAP 2 — Kern scripts schrijven
**Tijd:** 1 dag

```
PROMPT 2 — Core systemen

Schrijf de volgende scripts in C:\game\battleships\Assets\_Game\Scripts\Core\:

1. ServiceLocator.cs
   - Statische klasse met Register<T>(T service) en Get<T>()
   - Geeft foutmelding als service niet gevonden
   - Thread-safe

2. EventBus.cs
   - Generiek event systeem: Subscribe<T>(Action<T>), Publish<T>(T evt), Unsubscribe<T>
   - Geen MonoBehaviour afhankelijkheden

3. In Assets\_Game\Scripts\Utils\:
   LayerMaskConstants.cs
   - Public const int Ships = 6, Projectiles = 7, MapObjects = 8, Water = 9, HarborZone = 10

Schrijf daarna een EditMode test in Assets\_Game\Tests\EditMode\CoreSystemTests.cs:
- Test 1: ServiceLocator registreert en geeft service terug
- Test 2: EventBus publiceert event, subscriber ontvangt het
- Test 3: EventBus unsubscribe werkt (subscriber ontvangt niets meer)

Alle tests moeten passen.
```

**Check:** Window → Test Runner → EditMode → Run All → alles groen.

---

## STAP 3 — ScriptableObjects aanmaken
**Tijd:** halve dag

```
PROMPT 3 — Data ScriptableObjects

Schrijf in Assets\_Game\Scripts\Data\:

1. ShipClassData.cs (ScriptableObject)
   Velden:
   - string shipId
   - string displayName
   - ShipArchetype archetype (enum: Corvette, Frigate, Destroyer, Ironclad)
   - float baseMaxHP
   - float baseArmor        (0-1, percentage damage reduction)
   - float baseSpeed
   - float baseTurnRate
   - float baseAcceleration
   - int tier               (1-4)
   - int purchaseCost
   - WeaponData primaryWeapon

2. WeaponData.cs (ScriptableObject)
   Velden:
   - string weaponId
   - WeaponType weaponType  (enum: Cannon, Torpedo, Smokescreen, SonarPing, Repair)
   - float damage
   - float range
   - float cooldown
   - float projectileSpeed
   - int salvoCount         (1 = enkelvoudig, 2+ = salvo)
   - GameObject projectilePrefab

3. UpgradeData.cs (ScriptableObject)
   Velden:
   - UpgradeCategory category (enum: Hull, Weapon, Engine)
   - int level               (1-5)
   - int cost
   - float hpBonus
   - float armorBonus
   - float damageBonus
   - float speedBonus

Maak daarna de asset-bestanden aan in Assets\_Game\Data\:

Ships/SC_Corvette.asset:
  shipId=corvette, displayName="Wraith", archetype=Corvette
  baseMaxHP=600, baseArmor=0.02, baseSpeed=16, baseTurnRate=110
  baseAcceleration=20, tier=1, purchaseCost=300

Ships/SC_Frigate.asset:
  shipId=frigate, displayName="Bulwark", archetype=Frigate
  baseMaxHP=1000, baseArmor=0.05, baseSpeed=10, baseTurnRate=80
  baseAcceleration=14, tier=2, purchaseCost=500

Ships/SC_Destroyer.asset:
  shipId=destroyer, displayName="Specter", archetype=Destroyer
  baseMaxHP=750, baseArmor=0.02, baseSpeed=14, baseTurnRate=95
  baseAcceleration=18, tier=2, purchaseCost=450

Ships/SC_Ironclad.asset:
  shipId=ironclad, displayName="Leviathan", archetype=Ironclad
  baseMaxHP=1800, baseArmor=0.15, baseSpeed=6, baseTurnRate=45
  baseAcceleration=8, tier=3, purchaseCost=700

Wapens:
Weapons/WD_Cannon_Frigate.asset:
  weaponId=cannon_frigate, type=Cannon
  damage=120, range=18, cooldown=2.5, projectileSpeed=25, salvoCount=2

Weapons/WD_Torpedo_Destroyer.asset:
  weaponId=torpedo_specter, type=Torpedo
  damage=350, range=30, cooldown=8, projectileSpeed=12, salvoCount=1
```

**Check:** Alle assets zichtbaar in Inspector. Velden kloppen met bovenstaande waarden.

---

## STAP 4 — Schip beweegt
**Tijd:** 1 dag

```
PROMPT 4 — Ship movement

Schrijf in Assets\_Game\Scripts\Ship\:

1. ShipStats.cs
   - Leest waarden uit ShipClassData SO
   - Properties: MaxHP, CurrentHP, Armor, Speed, TurnRate, Team
   - Methode: TakeDamage(float raw) → berekent armor, past HP aan, gooit OnDamaged event
   - Methode: Die() → gooit OnDeath event via EventBus
   - Initialiseert via InitFromData(ShipClassData data)

2. ShipController.cs (MonoBehaviour)
   - Kinematic Rigidbody beweging (geen physics forces)
   - Leest ShipClassData.baseSpeed en baseTurnRate
   - Input via Unity Input System: WASD of joystick
   - Methode: SetMovementInput(Vector2 input)
   - Schip beweegt altijd vooruit als W ingedrukt (tank controls)
   - A/D draait het schip
   - S = achteruit (halve snelheid)
   - Smoothe acceleratie via baseAcceleration

3. Maak prefab aan: Assets\_Game\Prefabs\Ships\PFB_Ship_Frigate.prefab
   Structuur:
   Root "PFB_Ship_Frigate"
   ├── Rigidbody (isKinematic=true, constraints: FreezeY + FreezeRotX + FreezeRotZ)
   ├── BoxCollider (size 1x0.5x3, layer=Ships=6)
   ├── ShipStats (koppel SC_Frigate.asset)
   ├── ShipController
   └── MuzzlePoint (leeg child Transform, z+2 van root)

4. Maak een simpele PlayerInputHandler.cs
   - Leest WASD van keyboard
   - Stuurt naar ShipController.SetMovementInput()

Voeg het prefab toe aan DevSandbox.unity en test.
```

**Check:** Schip beweegt soepel met WASD. Roteert. Stopt bij loslaten. Geen errors.

---

## STAP 5 — Schip schiet
**Tijd:** 1 dag

```
PROMPT 5 — Weapon systeem

Schrijf in Assets\_Game\Scripts\Weapons\:

1. ProjectileBase.cs (MonoBehaviour)
   - Beweegt forward op vaste snelheid (uit WeaponData.projectileSpeed)
   - OnTriggerEnter: roep TakeDamage aan op geraakt ShipStats
   - Vernietigt zichzelf na range bereikt of na treffer
   - Negeert eigen team (check ShipTeam component)

2. CannonShell.cs (erft van ProjectileBase)
   - Geen extra logica in Fase 1
   - Prefab: kleine bol (Sphere primitive), layer=Projectiles=7

3. WeaponMount.cs (MonoBehaviour)
   - Referentie naar WeaponData SO
   - Methode: TryFire() → checkt cooldown → spawnt projectiel op MuzzlePoint
   - Cooldown telt af in Update()
   - Spacebar of rechter muisknop triggert TryFire()

Maak aan:
Assets\_Game\Prefabs\Projectiles\PFB_Projectile_CannonShell.prefab
  - Sphere (schaal 0.3), geen renderer schaduw, layer=7
  - CannonShell component erop

Voeg WeaponMount toe aan PFB_Ship_Frigate.prefab
Koppel WD_Cannon_Frigate.asset aan WeaponMount
Koppel PFB_Projectile_CannonShell aan WeaponMount.projectilePrefab
```

**Check:** Spatiebalk = schip schiet. Kogel vliegt vooruit. Raakt dummy object = damage.

---

## STAP 6 — Water shader
**Tijd:** halve dag

```
PROMPT 6 — Water ShaderGraph

Maak een water ShaderGraph aan in Assets\_Game\Art\Environment\Water\Water.shadergraph

Eigenschappen (Blackboard):
  _WaterColor       = Color  (#0A1628 Void Navy)
  _FoamColor        = Color  (#06B6D4 Biolum Cyan)
  _NormalMap        = Texture2D (default bump)
  _NormalMap2       = Texture2D (default bump)
  _NormalStrength   = Float (0.5)
  _ScrollSpeed1     = Float (0.05)
  _ScrollSpeed2     = Float (0.03)
  _DepthFogStrength = Float (0.8)

Shader logica:
1. Twee scrollende normal maps op verschillende snelheid en 60° rotatie
2. Sample SceneDepth → lerp WaterColor → FoamColor op basis van diepte
3. Fresnel aan randen (foam effect)
4. Geen transparantie (opaque) — performance op mobiel

Maak een material aan: Assets\_Game\Art\Environment\Water\M_Water.mat
Maak een plane GameObject aan in DevSandbox.unity (schaal 100x1x100, y=0)
Wijs M_Water.mat toe
Zet layer op Water (9)
```

**Check:** Water is zichtbaar. Beweegt subtiel. Schip "vaart" eroverheen. Geen z-fighting.

---

## STAP 7 — Camera volgt schip
**Tijd:** 2 uur

```
PROMPT 7 — Cinemachine top-down camera

In DevSandbox.unity:

1. Voeg Cinemachine Virtual Camera toe
   - Binding Mode: World Space
   - Follow: PFB_Ship_Frigate root transform
   - Body: Transposer
     - Offset: (0, 25, -8)   ← top-down met lichte tilt
     - Damping: (1, 1, 1)
   - Aim: Composer
     - Dead Zone: 0.1
   - Lens: FOV 60

2. Schrijf GameCamera.cs (MonoBehaviour)
   - Referentie naar Cinemachine Virtual Camera
   - Methode: SetTarget(Transform t) voor later als we meerdere schepen hebben
   - Edge scroll op PC: als muis <50px van rand → verschuif camera licht

3. Voeg GameCamera toe aan Camera GameObject in DevSandbox
```

**Check:** Camera volgt schip. Schip gaat nooit "off-screen". Edge scroll werkt.

---

## STAP 8 — Dood + respawn loop
**Tijd:** halve dag

```
PROMPT 8 — Dood en respawn

Schrijf Assets\_Game\Scripts\Match\DevSandboxManager.cs

Functionaliteit:
- Luistert naar EventBus ShipDiedEvent
- Bij dood: disable het schip (renderer + collider uit), start 3 seconden timer
- Na 3 seconden: schip terug naar spawn positie, HP volledig hersteld, ingeschakeld
- Spawn positie is een leeg GameObject "SpawnPoint_Player" in de scene

Voeg toe aan DevSandbox.unity:
- DevSandboxManager GameObject
- SpawnPoint_Player leeg GameObject (positie -20, 0, 0)
- Een tweede kubus als "vijand" target (statisch, met ShipStats zodat het kapot kan)

Schrijf ook ShipDiedEvent.cs in Scripts/Match/:
  public class ShipDiedEvent { public ShipStats victim; public ShipStats killer; }

Gooi deze event vanuit ShipStats.Die() via EventBus.Publish<ShipDiedEvent>()
```

**Check:** Schiet op het dummy target → het "sterft". Eigen schip sterft → 3s wachten → terug. Loop herhaalbaar.

---

## STAP 9 — FASE 1 AFRONDEN + eerste commit
**Tijd:** halve dag

```
PROMPT 9 — CI en eerste commit

1. Schrijf .gitignore in C:\game\battleships\ voor Unity:
   Library/
   Temp/
   Obj/
   Build/
   Builds/
   .vs/
   *.csproj
   *.sln
   *.suo
   *.user
   .DS_Store
   *.pidb
   *.booproj
   /[Ll]ogs/
   /[Mm]emory[Cc]aptures/
   userSettings/

2. Schrijf .gitattributes voor Git LFS:
   *.fbx filter=lfs diff=lfs merge=lfs -text
   *.png filter=lfs diff=lfs merge=lfs -text
   *.psd filter=lfs diff=lfs merge=lfs -text
   *.wav filter=lfs diff=lfs merge=lfs -text
   *.ogg filter=lfs diff=lfs merge=lfs -text
   *.mp3 filter=lfs diff=lfs merge=lfs -text
   *.tga filter=lfs diff=lfs merge=lfs -text
   *.tif filter=lfs diff=lfs merge=lfs -text

3. Schrijf .github/workflows/test.yml:
   - Trigger: push naar develop en elke PR
   - Stap: checkout
   - Stap: cache Library/
   - Stap: Unity Test Runner (EditMode)
   - Rapporteer resultaten als PR check

4. Initialiseer git repository:
   git init
   git lfs install
   git add .
   git commit -m "feat: Phase 1 prototype foundation"
```

**Check:** Git repo aangemaakt. GitHub Actions workflow bestand aanwezig. Eerste commit gedaan.

---

# FASE 2 — MVP vs AI BOTS
## Doel: echte wedstrijd vs bots. Alle 4 schepen, alle abilities, winconditie.
## Tijd: 5–7 weken

---

## STAP 10 — Alle 4 schepen speelbaar
**Tijd:** 3 dagen

```
PROMPT 10 — Alle 4 schip prefabs

Maak 3 extra ship prefabs gebaseerd op PFB_Ship_Frigate als template:

PFB_Ship_Corvette.prefab:
  - ShipStats → SC_Corvette.asset
  - BoxCollider size: 0.6x0.4x2.5
  - ShipController speed override: 16

PFB_Ship_Destroyer.prefab:
  - ShipStats → SC_Destroyer.asset
  - BoxCollider size: 0.7x0.4x4
  - 4 MuzzlePoints (torpedo tubes): TorpedoMount_FL, FR, RL, RR
    posities: (-0.8, 0, 1), (0.8, 0, 1), (-0.8, 0, -1), (0.8, 0, -1)

PFB_Ship_Ironclad.prefab:
  - ShipStats → SC_Ironclad.asset
  - BoxCollider size: 2x0.6x3.5
  - 3 MuzzlePoints: Turret_Fore (z+2), Turret_Mid (z0), Turret_Aft (z-2)

Voeg aan elk prefab toe:
  - ShipTeam component (property: TeamID int, 0=Allied 1=Enemy)
  - AbilityHandler component (leeg voor nu, we vullen dit later)

Test: spawn alle 4 in DevSandbox. Ze bewegen allemaal correct.
```

---

## STAP 11 — Abilities implementeren
**Tijd:** 4 dagen

```
PROMPT 11A — Corvette abilities

Schrijf in Assets\_Game\Scripts\Abilities\:

SmokescreenAbility.cs
- Spawnt een smoke zone (cylinder collider, radius 5m) op schip-positie
- Schepen in de zone zijn "verborgen" (OnSmokescreenEntered event)
- Duurt 6 seconden, daarna verdwijnt het
- Cooldown: 20 seconden
- VFX: zwarte particle sphere (placeholder: zwarte semi-transparante bol)

SonarPingAbility.cs
- Spawnt een expanderende ring (lineRenderer cirkel) vanuit schip
- Ring groeit van radius 0 naar 40m in 2 seconden
- Alle vijandelijke schepen in de ring: OnDetected event, zichtbaar voor 5s
- Cooldown: 15 seconden
- VFX: cyan cirkel (LineRenderer, kleur #06B6D4)
```

```
PROMPT 11B — Frigate ability

TwinSalvoAbility.cs
- Vuurt 2 kanonschoten in 0.2s na elkaar
- Gebruikt WeaponMount.TryFire() twee keer
- Aparte cooldown van 3 seconden
- (De normale WeaponMount fire blijft ook werken)
```

```
PROMPT 11C — Destroyer ability

TorpedoAbility.cs
- Spawnt een Torpedo projectiel vanuit dichtstbijzijnde TorpedoMount
- Torpedo: hoge damage (350), langzaam (projectileSpeed=12), groot collider
- Gaat in een rechte lijn, mist-baar
- Ontploft ook als hij muur raakt
- Cooldown: 10 seconden per tube (4 tubes = 4 onafhankelijke cooldowns)
```

```
PROMPT 11D — Ironclad abilities

HeavySalvoAbility.cs
- 3 kanonschoten van verschillende turrets in 1.5 seconden (0.5s tussenpauze)
- Elke turret schiet naar muis-richting

AreaDenialAbility.cs
- Spawnt een onzichtbare "mine zone" op aangewezen positie (muisklik)
- Schepen die erdoorheen varen: 50 damage per seconde
- Verdwijnt na 12 seconden of na 200 totale damage gedaan
- Cooldown: 25 seconden
```

---

## STAP 12 — Economy systeem
**Tijd:** 2 dagen

```
PROMPT 12 — Gold en upgrades

Schrijf Assets\_Game\Scripts\Economy\GoldManager.cs:
- Per speler/bot: houdt goud bij
- Passief inkomen: 5 gold per seconde (configureerbaar)
- Kill gold: SlachtofferTier × 50 gold
- Assist gold: als speler >10% damage deed in laatste 10s → KillGold × 0.4
- Events: GoldEarnedEvent(int amount, GoldSource source)
- Methode: SpendGold(int amount) → false als niet genoeg

Schrijf Assets\_Game\Scripts\Economy\ShipUpgradeSystem.cs:
- Heeft lijst van gekochte UpgradeData assets per categorie
- Methode: TryBuyUpgrade(UpgradeCategory cat) → volgende level kopen
- Past ShipStats aan na aankoop (HP, armor, speed, damage multiplier)
- Gooit UpgradePurchasedEvent

Schrijf UpgradePurchasedEvent.cs en GoldEarnedEvent.cs in Scripts/Match/

Test: kill een bot → gold stijgt. Koop upgrade → stat verandert.
```

---

## STAP 13 — Bot AI
**Tijd:** 5 dagen

```
PROMPT 13 — Bot AI controller

Schrijf Assets\_Game\Scripts\AI\BotController.cs:

De bot heeft een simpele state machine:
  PATROL  → rijdt langs vaste waypoints in zijn lane
  ATTACK  → vijand in range → rij naar hem toe en schiet
  RETREAT → HP < 30% → terug naar eigen haven
  BUY     → bij eigen haven → koop upgrades als genoeg gold

Implementeer:
- WaypointFollower: lijst van Transform waypoints in de scene
- EnemyDetection: overlap sphere radius=20m, zoekt laag Ships, filtert op team
- AttackBehavior: rij naar vijand, vuurt WeaponMount.TryFire() als in range
- RetreatBehavior: rijdt terug naar SpawnPoint
- BuyBehavior: roept ShipUpgradeSystem.TryBuyUpgrade() aan in volgorde Hull→Engine→Weapon

Maak BotTuningData.cs ScriptableObject:
  - float aggressionLevel (0-1, hoe snel PATROL→ATTACK)
  - float goldSpendThreshold (minimum gold voor aankoop)
  - float aimAccuracy (0-1, 1=perfect aim, 0=wild)
  - int preferredLane (0=top, 1=mid, 2=bot)

Maak 3 bot tuning presets:
  BT_Easy.asset   (aggression=0.3, accuracy=0.4)
  BT_Normal.asset (aggression=0.6, accuracy=0.65)
  BT_Hard.asset   (aggression=0.9, accuracy=0.85)
```

---

## STAP 14 — Map + objectives
**Tijd:** 3 dagen

```
PROMPT 14 — Map en winconditie

In DevSandbox.unity, bouw met ProBuilder:

Kaart layout (top-down, 100x100 units):
  - Twee haven zones: links (Team 0) en rechts (Team 1), elk 15x20u
  - Drie lanes: boven, midden, onder — verbinden de havens
  - Eilanden/obstakels in elke lane (ProBuilder boxen als blokkades)
  - Neutrale camp zones: 2x op de kaart (midden links/rechts)

Schrijf HarborObjective.cs:
  - BoxTrigger zone voor elke haven
  - Als vijand in zone: capture timer telt op (10s voor volledig capture)
  - Gecaptured haven geeft 2HP/s regen aan schepen van het veroverende team
  - Kan terugveroverd worden

Schrijf NeutralCamp.cs:
  - Bot guard spawnt (simpele statische ShipStats met 200HP)
  - Versla de guard → 150 gold reward voor het team
  - Respawnt na 60 seconden

Schrijf MatchManager.cs:
  - Houdt bij: kills (0 tot 30), tijd (20 minuten max)
  - Win door: 30 kills FIRST, OF meeste kills na 20 min, OF vijand hoofdhaven vernietigen
  - Gooit MatchEndedEvent(TeamID winner)
```

---

## STAP 15 — HUD bouwen
**Tijd:** 3 dagen

```
PROMPT 15 — HUD systeem

Maak een Canvas (Screen Space - Camera overlay) in DevSandbox.unity.

Schrijf en bouw de volgende UI componenten:

1. HpBarUI.cs
   - Horizontale slider stijl (Image fill)
   - Kleur: #DC2626 (rood fill), #1A2332 (donkere achtergrond)
   - Knippert bij <30% HP
   - Font: Rajdhani (als geïnstalleerd), anders standaard
   - Positie: linksboven

2. GoldDisplayUI.cs
   - TextMeshPro tekst: "💰 340"
   - Animatie: scale pulse bij gold earn (1.0→1.15→1.0 in 0.3s)
   - Positie: links naast HP balk

3. AbilityCooldownUI.cs
   - 3 ability slots (circulaire Image fill per slot)
   - Fill kleur: #06B6D4 (cyan) bij ready, grijs bij cooldown
   - Clockwipe (fillAmount 0→1 per cooldown)
   - Positie: linksonder

4. MinimapUI.cs
   - RawImage met RenderTexture (aparte camera kijkt van boven)
   - Cirkelmasker (porthole stijl)
   - Eigen schip: gouden stip. Vijanden: rode stip.
   - Update elke 0.2 seconden
   - Positie: rechtsonder

5. KillFeedUI.cs
   - Rechts midden, verticale stack
   - Per kill: "Bulwark → Specter" tekst
   - Fade na 5 seconden
   - Max 5 tegelijk zichtbaar
```

---

## STAP 16 — Shop UI
**Tijd:** 2 dagen

```
PROMPT 16 — Shop paneel

Schrijf Assets\_Game\Scripts\UI\ShopPanel.cs en bouw het UI paneel:

Layout:
  ┌─────────────────────────┐
  │ UPGRADE SHOP            │
  ├──────────┬──────────┬───┤
  │ HULL     │ ENGINE   │ WPN│
  │ Lvl II   │ Lvl I    │ I  │
  │ 300g     │ 150g     │150g│
  │ [KOOP]   │ [KOOP]   │[KP]│
  └──────────┴──────────┴───┘

Functionaliteit:
- Opent met Tab of B knop
- Pauzeert NIET (je kunt bestuurd worden terwijl shop open is)
- Toont huidig level per categorie
- Toont kosten van volgende level
- [KOOP] knop → roept ShipUpgradeSystem.TryBuyUpgrade() aan
- Grijst uit als onvoldoende gold
- Sluit bij zelfde knop of Escape

Stijl:
  Achtergrond: rgba(6,182,212,0.15) = semi-transparant donker met cyan tint
  Border: #92400E (Brass Gold)
  Tekst: wit, Rajdhani Bold voor headers
```

---

## STAP 17 — 3v3 bot match speelbaar
**Tijd:** 2 dagen

```
PROMPT 17 — Volledige bot match configureren

In DevSandbox.unity:

1. Spawn configuratie:
   Team 0 (speler + 2 bots): 3x SpawnPoint links
   Team 1 (3 bots): 3x SpawnPoint rechts

2. ShipSpawner.cs:
   - Leest team configuratie (welk schip, welk tuning preset)
   - Spawnt alle schepen bij match start
   - Geeft bots hun BotController + BotTuningData

3. Voeg ScoreUI toe: "3 — 1" boven in het midden
   - Team 0 score links, Team 1 score rechts
   - Timer eronder: "14:32"

4. Eindigscherm bij MatchEndedEvent:
   - "VICTORY" of "DEFEAT" groot in beeld
   - Scoretabel: kills, damage, gold earned per schip
   - Knop: "Play Again" → herlaadt DevSandbox scene

5. Test volledig: start match → bots bewegen → gevecht → winconditie → eindigscherm
   Zonder crashes over 10 minuten.
```

**Check voor einde Fase 2:**
- 3v3 bot match speelt zonder errors
- Economy, shop, upgrades werken
- HUD volledig zichtbaar
- Winconditie triggert
- Eindigscherm toont score

---

# FASE 3 — PvP ONLINE
## Doel: twee mensen spelen via internet. Lobby → match → resultaat.
## Tijd: 5–6 weken

---

## STAP 18 — Mirror networking opzetten
**Tijd:** 3 dagen

```
PROMPT 18 — Mirror NetworkManager

In Bootstrap.unity:

1. Voeg NetworkManager toe (Mirror)
   - Transport: KcpTransport
   - Port: 7777
   - Max Connections: 6
   - Player Prefab: PFB_Ship_Frigate (tijdelijk)

2. Schrijf NetworkShip.cs:
   - Erft van NetworkBehaviour
   - [SyncVar] Vector3 position, Quaternion rotation, float currentHP
   - [Command] CmdSetMovementInput(Vector2 input)
   - Server: valideer snelheid (niet sneller dan MaxSpeed × 1.2)
   - [ClientRpc] RpcTakeDamage(float amount) → update SyncVar + UI

3. Schrijf NetworkProjectile.cs:
   - Server spawnt projectiel via NetworkServer.Spawn()
   - Alle clients zien het projectiel
   - Hit detectie: alleen op server, [ClientRpc] voor visuele feedback

4. Maak een simpele host/join UI in Bootstrap scene:
   - Knop "Host Game" → StartHost()
   - Knop "Join Game" → tekstveldje voor IP → StartClient()
```

---

## STAP 19 — Lobby systeem
**Tijd:** 2 dagen

```
PROMPT 19 — Lobby scene

Maak Assets\_Game\Scenes\Lobby.unity

Schrijf LobbyManager.cs met Mirror NetworkRoomManager:
- Speler treedt in → ziet lijst van andere spelers
- Elke speler kiest zijn schip (dropdown: Corvette/Frigate/Destroyer/Ironclad)
- Ready knop → kleur verandert
- Als ALLE spelers ready: countdown 5 seconden → laad GameScene voor iedereen
- Minimaal 2 spelers vereist, maximaal 6

UI:
  ┌─────────────────────────────────┐
  │ IRON TIDE — LOBBY               │
  ├─────────────────────────────────┤
  │ Speler 1: [Frigate]    ✓ READY  │
  │ Speler 2: [Destroyer]  ○ wacht  │
  │ Speler 3: (leeg)                │
  ├─────────────────────────────────┤
  │ [KLAAR]           [VERLAAT]     │
  └─────────────────────────────────┘
```

---

## STAP 20 — Lag compensatie + ping
**Tijd:** 3 dagen

```
PROMPT 20 — Client prediction en ping display

1. Client-side prediction voor schip beweging:
   Pas NetworkShip.cs aan:
   - Client past beweging DIRECT toe (geen wachten op server)
   - Server stuurt correcties via [ClientRpc] elke 100ms
   - Client interpoleert naar server-positie (niet hard snap)
   - Resultaat: soepel gevoel tot ~200ms latentie

2. PingDisplayUI.cs:
   - Toont RTT in ms (via NetworkTime.rtt × 1000)
   - Update elke 2 seconden
   - Kleur:
     < 80ms  → groen (#22C55E)
     < 150ms → geel (#F59E0B)
     > 150ms → rood (#DC2626)
   - Positie: rechtsboven klein

3. Disconnect handling in ConnectionManager.cs:
   - Speler disconnect → schip bevriest 30 seconden
   - Na 30s geen reconnect → schip verwijderd, team speelt verder
   - Host disconnect → Mirror host migration
```

---

# FASE 4 — DEDICATED SERVER
## Doel: echte server op Linux, automatische matchmaking.
## Tijd: 4 weken

---

## STAP 21 — Headless server build

```
PROMPT 21 — Server build prepareren

1. Zoek ALLE Unity-code die client-only is (Camera, Audio, UI, Input):
   Wrap elke client-only sectie:
   #if !UNITY_SERVER
   // client code hier
   #endif

   Server-only code:
   #if UNITY_SERVER
   // server logic hier
   #endif

2. Controleer deze scripts specifiek:
   - GameCamera.cs → alles in #if !UNITY_SERVER
   - AudioManager.cs → alles in #if !UNITY_SERVER
   - alle UI scripts → alles in #if !UNITY_SERVER
   - PlayerInputHandler.cs → alles in #if !UNITY_SERVER

3. Build test: File → Build Settings → Platform: Linux 64-bit
   Zet "Server Build" vinkje aan
   Build en rapporteer of er compile errors zijn
```

---

## STAP 22 — Matchmaker

```
PROMPT 22 — Simpele matchmaker

Schrijf een Node.js matchmaker server in Server/Matchmaker/:

package.json + index.js

REST API:
  POST /queue          body: {playerId, region, shipChoice}
                       response: {queueId}

  GET  /queue/:id      response: {status: "waiting"|"matched", serverIp, serverPort}

  DELETE /queue/:id    response: {ok: true}

Matchmaker logica:
  - Queue van wachtende spelers
  - Als 6 spelers in queue (of 2 voor dev-mode): maak een match
  - Wijs toe aan beschikbare game server
  - Antwoord met server IP+port

Game servers registreren zichzelf:
  POST /servers/register  body: {ip, port, region, maxPlayers}
  POST /servers/heartbeat body: {ip, port, currentPlayers}

Schrijf ook MatchmakerClient.cs in Unity:
  - POST naar /queue bij zoeken
  - Poll /queue/:id elke 2 seconden
  - Verbindt automatisch met server IP:port via Mirror.StartClient()
```

---

## STAP 23 — Docker

```
PROMPT 23 — Dockerfile voor server

Maak Server/Dockerfile:

FROM ubuntu:22.04
RUN apt-get update && apt-get install -y \
    libglib2.0-0 \
    libssl3 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY Build/LinuxServer/ .
RUN chmod +x IronTide.x86_64

EXPOSE 7777/udp
EXPOSE 7778/tcp

CMD ["./IronTide.x86_64", "-batchmode", "-nographics", "-logFile", "/dev/stdout"]

Maak Server/docker-compose.yml:
version: '3'
services:
  gameserver:
    build: .
    ports:
      - "7777:7777/udp"
      - "7778:7778/tcp"
    restart: unless-stopped
    environment:
      - SERVER_PORT=7777
      - MAX_PLAYERS=6

  matchmaker:
    build: ./Matchmaker
    ports:
      - "3000:3000"
    restart: unless-stopped
```

---

# FASE 5 — RANKED BETA
## Doel: ELO rating, seizoenen, cosmetica, store submissions.
## Tijd: 5–6 weken

---

## STAP 24 — MMR systeem

```
PROMPT 24 — ELO/MMR

Schrijf Server/Matchmaker/Elo.js:

Elo berekening:
  K-factor: 32
  Verwachte score: 1 / (1 + 10^((ratingB - ratingA) / 400))
  Nieuwe rating: ratingA + K × (uitkomst - verwacht)
  uitkomst: 1 = win, 0.5 = gelijkspel, 0 = verlies

Extra regels:
  - Eerste 10 matches: "provisional" (grotere K-factor van 64)
  - Minimum rating: 500 (nooit lager)
  - Maximum winst per match: +50 MMR
  - Seizoensreset: newRating = 0.75 × currentRating (soft reset)

Matchmaking range:
  - Start zoeken: MMR ±150
  - Na 30s: verruim naar ±250
  - Na 60s: verruim naar ±400
  - Na 120s: accepteer iedereen

Schrijf ook RankTier.js:
  Bronze:   500–999
  Silver:   1000–1499
  Gold:     1500–1999
  Platinum: 2000–2499
  Admiral:  2500+
```

---

## STAP 25 — Cosmetica systeem

```
PROMPT 25 — Cosmetica

Schrijf Assets\_Game\Scripts\Data\CosmeticData.cs (ScriptableObject):
  - string cosmeticId
  - string displayName
  - ShipArchetype forShip
  - Material[] hullMaterials    (overschrijft default materials)
  - GameObject vfxOverride      (optioneel custom VFX prefab)
  - Sprite previewImage
  - int storePrice              (0 = gratis/verdiend)
  - bool isSeasonReward

Schrijf CosmeticInventory.cs:
  - Houdt per speler bij: List<string> ownedCosmeticIds
  - string equippedCosmeticId per schiptype
  - Persisteer via PlayerPrefs (lokaal voor nu)
  - Methode: ApplyCosmetic(ShipClassData ship, CosmeticData cosmetic)
    → vervangt materials op het schip prefab instantie

Maak 1 test cosmetic:
  Assets\_Game\Data\Cosmetics\CS_Frigate_Season1.asset
  displayName = "Iron Season I — Bulwark"
  forShip = Frigate
  isSeasonReward = true
  storePrice = 0
```

---

## STAP 26 — Store submissions (laatste stap)

```
PROMPT 26 — Build checklist voor stores

Controleer en rapporteer voor elke platform:

ANDROID (Google Play):
  □ Target API Level: 33 of hoger
  □ 64-bit: ARM64 only (geen 32-bit)
  □ App Bundle (.aab) aangemaakt
  □ Keystore aangemaakt en veilig opgeslagen
  □ Versienummer ingesteld (versionCode + versionName)
  □ Permissions in AndroidManifest: alleen INTERNET + ACCESS_NETWORK_STATE
  □ Geen embedded placeholder assets (jsfxr sounds etc.)
  □ Privacy policy URL aanwezig

IOS (App Store):
  □ Bundle Identifier: com.irontide.navalsupremacy
  □ Target iOS: 14.0 minimum
  □ Privacy manifest (PrivacyInfo.xcprivacy) aanwezig
  □ Icons: alle vereiste groottes (via Unity Icon configuratie)
  □ TestFlight build uploaded

PC (Steam):
  □ Windows IL2CPP x64 build
  □ Steam AppID geregistreerd
  □ Steam depot geconfigureerd
  □ Achievements gedefinieerd (minimaal 5)
  □ Steam store pagina met screenshots

Rapporteer per platform: OK, MIST, of BLOCKER
```

---

# GLOBAAL OVERZICHT

```
FASE 1 — PROTOTYPE (Week 1-3)
  Stap 1:  Unity project opzetten
  Stap 2:  Kern scripts (ServiceLocator, EventBus)
  Stap 3:  ScriptableObjects (schepen, wapens, upgrades)
  Stap 4:  Schip beweegt
  Stap 5:  Schip schiet
  Stap 6:  Water shader
  Stap 7:  Camera
  Stap 8:  Dood + respawn
  Stap 9:  Git + CI
           ↓ MILESTONE: iets beweegt, schiet, ontploft

FASE 2 — MVP vs BOTS (Week 4-10)
  Stap 10: Alle 4 schepen
  Stap 11: Abilities (4 schepen × 2 abilities)
  Stap 12: Economy (gold, upgrades)
  Stap 13: Bot AI
  Stap 14: Map + objectives + winconditie
  Stap 15: HUD (HP, gold, cooldowns, minimap, killfeed)
  Stap 16: Shop UI
  Stap 17: 3v3 bot match speelbaar
           ↓ MILESTONE: volledige match vs bots

FASE 3 — PvP ONLINE (Week 11-16)
  Stap 18: Mirror networking
  Stap 19: Lobby systeem
  Stap 20: Lag compensatie + ping
           ↓ MILESTONE: twee mensen spelen via internet

FASE 4 — DEDICATED SERVER (Week 17-20)
  Stap 21: Headless server build
  Stap 22: Matchmaker API
  Stap 23: Docker container
           ↓ MILESTONE: server draait in cloud

FASE 5 — RANKED BETA (Week 21-26)
  Stap 24: MMR / ELO systeem
  Stap 25: Cosmetica systeem
  Stap 26: Store submissions
           ↓ MILESTONE: live op stores
```

---

## HOE DIT TE GEBRUIKEN

**Elke dag:**
1. Kijk welke stap je op zit
2. Kopieer de PROMPT van die stap
3. Plak het hier in dit gesprek
4. Test het resultaat in Unity
5. Als het werkt → volgende stap
6. Als er een fout is → plak de foutmelding hier

**Dingen aanpassen onderweg:**
- Mechanic werkt niet? Zeg het hier — we passen de prompt aan
- Wil je iets anders proberen? Altijd goed
- Tijd tekort? We schrappen of vereenvoudigen een stap
- Nieuwe idee? We voegen het in op het juiste moment

**Prioriteit als tijd schaars is:**
Stap 1 t/m 8 zijn VERPLICHT (zonder dit geen game)
Stap 9 t/m 17 zijn het HART (de game zelf)
Stap 18 t/m 26 zijn UITBREIDINGEN (mooi maar niet dag-1 noodzakelijk)

---

*Bijgewerkt: 2026-02-27 | Huidige fase: PRE-START*
