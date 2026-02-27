# Iron Tide - Sprint 1 (7 dagen) Dagplan + Prompts

**Sprintdoel:** Phase 1 vertical slice volledig speelbaar en stabiel krijgen.  
**Periode:** 7 werkdagen  
**Focus:** offline DevSandbox loop, geen nieuwe scope buiten prototype-kern.

## Dag 1 - Reality check en backlog opschonen

### Doel
Eenduidig beeld van wat al werkt en wat nog blokkeert.

### Taken
1. Code en docs status vergelijken.
2. Lijst maken: `Af`, `Deels`, `Ontbreekt`.
3. Top-10 blockers voor vertical slice bepalen.

### Output
1. Bijgewerkte statussectie in docs.
2. Geprioriteerde sprint backlog.

### Check
1. Iedereen kan in 2 minuten zien wat de echte projectstatus is.

### Prompt (kopieer/plak)
```text
Doel: maak een reality check van C:\Game\battleships voor Sprint 1.

Doe:
1. Analyseer code + docs + tests.
2. Maak drie lijsten: AF, DEELS, ONTBREEKT (alleen relevante items voor Phase 1).
3. Maak top-10 blockers die de vertical slice tegenhouden.
4. Schrijf een korte sprint backlog (max 15 taken) in uitvoervolgorde.

Output:
- Concreet en kort, zonder theoretische uitweidingen.
```

---

## Dag 2 - DevSandbox basis werkend maken

### Doel
Minimale scene waar ship kan spawnen en gecontroleerd worden.

### Taken
1. `DevSandbox` scene structureren (spawn, waterplane, target dummy).
2. Spelerprefab koppelen.
3. Input en camera basis controleren.

### Output
1. Playbare scene zonder directe null-ref spam.

### Check
1. Ship beweegt en camera volgt stabiel.

### Prompt
```text
Doel: maak DevSandbox minimaal speelbaar in Unity.

Doe:
1. Zet scene structuur op voor een testloop (spawnpoint, ship, dummy, water).
2. Koppel bestaande scripts/prefabs correct in de scene.
3. Los compile- en null-reference problemen op die deze scene blokkeren.
4. Rapporteer exact welke Unity handstappen nog nodig zijn.

Output:
- Werkende DevSandbox basis + lijst met uitgevoerde wijzigingen.
```

---

## Dag 3 - Combat loop afronden (vuren, hit, damage, death)

### Doel
Van schieten naar daadwerkelijk damage/death flow.

### Taken
1. Weapon fire flow valideren.
2. Projectile collisions en team filtering controleren.
3. Ship damage/death events robuust maken.

### Output
1. Schieten op target geeft consequent damage en death.

### Check
1. Geen dubbel-fire, geen oneindige projectielen, geen death-loop bugs.

### Prompt
```text
Doel: maak combat loop in DevSandbox betrouwbaar.

Doe:
1. Controleer en fix Weapon -> Projectile -> Hit -> Damage -> Death keten.
2. Voeg guards toe tegen veelvoorkomende runtime fouten (dubbele events, null refs).
3. Houd implementatie simpel en passend bij huidige architectuur.
4. Geef teststappen om de flow handmatig te verifiëren in Unity.

Output:
- Duidelijke codewijzigingen + korte validatiechecklist.
```

---

## Dag 4 - Respawn + match state basis

### Doel
Herhaalbare gameplay-loop zonder scene restart.

### Taken
1. Respawn manager toevoegen/afmaken.
2. MatchState overgang `Countdown -> Playing -> GameOver` basis stabiel maken.
3. EventBus koppelingen nalopen.

### Output
1. Dood -> korte wachttijd -> respawn werkt herhaalbaar.

### Check
1. Loop is 10x achter elkaar speelbaar zonder vastlopers.

### Prompt
```text
Doel: rond de respawn en match-state basis af voor prototype.

Doe:
1. Implementeer/fix respawn flow na death event.
2. Zorg dat MatchStateMachine logisch en voorspelbaar transities doet.
3. Houd events ontkoppeld via EventBus.
4. Noteer welke scene-objecten verplicht zijn voor deze flow.

Output:
- Werkende death/respawn loop + transitieoverzicht.
```

---

## Dag 5 - Water + Blender pipeline start

### Doel
Visuele basis leggen en art pipeline operationeel maken.

### Taken
1. Water shader/material toepassen op testscene.
2. Blender export preset valideren.
3. Eerste ship blockout (Frigate) importtest doen.

### Output
1. Water zichtbaar en eerste correcte FBX import flow.

### Check
1. FBX schaal en orientatie kloppen direct in Unity.

### Prompt
```text
Doel: activeer water + Blender naar Unity pipeline.

Doe:
1. Valideer watermaterial setup in DevSandbox.
2. Geef stap-voor-stap Blender export check voor IronTide_FBX preset.
3. Loop een eerste importtest door voor SHIP_Frigate_LOD0.fbx.
4. Rapporteer eventuele mismatch in scale/axis met concrete fix.

Output:
- Praktische pipeline-check en corrigeeracties.
```

---

## Dag 6 - Tests en stabiliteit

### Doel
Prototype betrouwbaar maken voor verdere ontwikkeling.

### Taken
1. EditMode/PlayMode tests nalopen en uitbreiden waar gaten zitten.
2. Belangrijkste regressies afvangen in tests.
3. Kleine refactor voor leesbaarheid en onderhoudbaarheid.

### Output
1. Betere safety net voor kernsystemen.

### Check
1. Kernfunctionaliteit heeft testdekking en blijft groen na fixes.

### Prompt
```text
Doel: verhoog stabiliteit van Phase 1 systemen.

Doe:
1. Analyseer bestaande tests en identificeer ontbrekende kritieke testcases.
2. Voeg tests toe voor de belangrijkste prototype flows.
3. Fix code waar tests terechte problemen tonen.
4. Houd wijzigingen klein en gericht op betrouwbaarheid.

Output:
- Toegevoegde tests + belangrijkste fixes + resterende risico's.
```

---

## Dag 7 - Integratie, demo en sprint-afsluiting

### Doel
Werkende vertical slice opleveren en klaarzetten voor Sprint 2.

### Taken
1. End-to-end speelsessie draaien.
2. Open issues prioriteren voor volgende sprint.
3. Docs updaten met echte status.

### Output
1. Sprint review: wat werkt, wat niet, wat volgt.

### Check
1. Duidelijke Go/No-Go voor start Phase 2 werk.

### Prompt
```text
Doel: sluit Sprint 1 af met een heldere opleverstatus.

Doe:
1. Maak een korte release-readiness check voor de vertical slice.
2. Vat samen: werkt nu, werkt deels, ontbreekt nog.
3. Maak een voorstel voor Sprint 2 met maximaal 7 doelen.
4. Werk relevante docs bij zodat ze overeenkomen met de echte status.

Output:
- Sprint 1 eindrapport + voorstel Sprint 2.
```

---

## Sprint 1 Definition of Done

1. DevSandbox is speelbaar zonder kritieke errors.
2. Ship movement + combat + damage + death/respawn werken samen.
3. Basis camera en water zijn geïntegreerd.
4. Kernscripts zijn getest en stabiel genoeg voor uitbreiding.
5. Documentatie weerspiegelt de echte status.
