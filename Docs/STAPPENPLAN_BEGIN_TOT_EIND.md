# Iron Tide - Stappenplan Van Begin Tot Eind

**Datum:** 2026-02-27  
**Status:** Actief werkplan (globaal, aanpasbaar per sprint)

## 1. Doel van dit plan

Een praktische volgorde van concept naar speelbare release, zonder scope-chaos.
We werken in fases, leveren per fase een duidelijke milestone op, en sturen bij waar nodig.

## 2. Werkprincipes

1. Eerst werkend, dan mooier.
2. Eerst offline stabiel, daarna online.
3. Eerst kernloop, daarna uitbreidingen.
4. Elke fase eindigt met een testbare Definition of Done.
5. Wijzigingen onderweg zijn normaal: scope mag bijgesteld worden per sprint.

## 3. Fase-overzicht (begin tot eind)

1. Preflight en alignment
2. Phase 1 - Vertical slice prototype
3. Phase 2 - Volledige offline bot match (MVP)
4. Phase 3 - Online PvP alpha
5. Phase 4 - Dedicated server + matchmaker
6. Phase 5 - Ranked beta + cosmetica + release prep

---

## 4. Fase 0 - Preflight en alignment (1-2 dagen)

**Doel:** realistische startpositie en duidelijke prioriteiten.

### Taken
1. Documentatie syncen met huidige code-status.
2. Vaststellen wat al af is, deels af is, of ontbreekt.
3. Eén korte backlog maken met prioriteit `Must / Should / Later`.
4. Branch- en commitafspraken bevestigen.

### Output
1. Geüpdatete status in docs.
2. Geprioriteerde backlog voor Phase 1.

### DoD
1. Team weet exact wat als eerste gebouwd/afgemaakt wordt.

---

## 5. Phase 1 - Vertical Slice Prototype (week 1-3)

**Doel:** 1 speelbare loop: varen, schieten, damage, dood/respawn, camera, basis water.

### Taken
1. Core valideren: ServiceLocator, EventBus, Layer constants, tests.
2. Ship loop afronden: movement, aim, weapon fire, projectiles, hit/damage.
3. Scene opzetten: `DevSandbox` met spawnpoints, target/dummy, respawn manager.
4. Camera en input afronden (PC eerst, mobile basis daarna).
5. Water basis shader toepassen.
6. Eerste prefab/asset set koppelen in Unity.

### Output
1. Werkende `DevSandbox` vertical slice.
2. Basis geautomatiseerde tests groen.

### DoD
1. Je kunt 5-10 minuten zonder crash spelen in DevSandbox.

---

## 6. Phase 2 - Offline MVP vs bots (week 4-10)

**Doel:** complete 3v3 bot-match offline.

### Taken
1. Alle 4 scheepsarchetypes speelbaar maken.
2. Abilities per archetype implementeren.
3. Economy loop afronden: gold, assists, upgrades, shop.
4. Bot AI state machine afronden (patrol/engage/retreat/shop).
5. Map objectives afronden (harbor, lanes, camps, win condition).
6. HUD basis compleet maken (hp, gold, cooldowns, minimap, score/timer).
7. Blockout art + Blender pipeline structureel uitvoeren.

### Output
1. Volledige offline matchloop van start tot einde.

### DoD
1. 3v3 bot match draait stabiel 10+ minuten zonder blocker errors.

---

## 7. Phase 3 - Online PvP alpha (week 11-16)

**Doel:** echte online matches met basis netcode.

### Taken
1. Mirror netwerklaag opzetten (host/join + network ship/projectile).
2. Server-authoritative damage en gold afdwingen.
3. Lobby flow maken (join, ready, start countdown).
4. Ping display + basis prediction/interpolation.
5. Disconnect/reconnect basislogica.

### Output
1. Online speelbare alpha flow: lobby -> match -> resultaat.

### DoD
1. Twee echte clients kunnen een volledige match spelen via internet.

---

## 8. Phase 4 - Dedicated server + matchmaker (week 17-20)

**Doel:** geen handmatige host meer nodig.

### Taken
1. Headless Linux server build opzetten.
2. Client-only code afschermen met `#if !UNITY_SERVER`.
3. Matchmaker API bouwen (`/queue`, status pollen, server assignment).
4. Unity client koppelen aan matchmaker.
5. Containerisatie en basis load test.

### Output
1. Match toewijzing via matchmaker en connectie naar dedicated server.

### DoD
1. Minimaal 1 stabiele servermatch zonder handmatig IP invoeren.

---

## 9. Phase 5 - Ranked beta + release prep (week 21-26)

**Doel:** competitieve beta met progression en store-ready builds.

### Taken
1. ELO/MMR implementeren in backend.
2. Ranked queue en season reset regels toevoegen.
3. Cosmetica inventory/equip flow afronden.
4. Platform build checks doen (Android, iOS, PC).
5. QA, balancing en performance tuning per target.

### Output
1. Ranked beta kandidaat met basis live-ops systemen.

### DoD
1. Release candidate build per doelplatform met bekende blockerslijst.

---

## 10. Roadmap ritme (hoe we werken)

1. Werken in sprints van 1 week.
2. Elke sprint start met 3-7 concrete doelen.
3. Elke sprint eindigt met demo + test + docs update.
4. Scopewijzigingen alleen aan sprintgrens, behalve blockers.

## 11. Aanpassen onderweg (expliciet afgesproken)

We passen dit plan bewust aan wanneer:
1. Technische risico's hoger blijken dan verwacht.
2. Gameplay niet leuk genoeg is in playtests.
3. Tijd/budget verschuift.
4. Nieuwe prioriteiten ontstaan.

Regel: we schrappen liever slim dan half afbouwen.

## 12. Eerste concrete startvolgorde (nu)

1. Fase 0 afronden: docs + status + backlog opschonen.
2. Phase 1 vertical slice volledig speelbaar en stabiel krijgen.
3. Daarna pas uitbreiden naar volledige Phase 2 bot-MVP.

---

Dit document is de globale route.  
Per fase maken we aparte sprintplannen met detailtaken en prompts.
