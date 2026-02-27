# Battleships -> Dieselpunk / Sci-Fi Ocean Transformatieplan

## 0. Doel en aanpak

Doel: het bestaande project ombouwen naar een duidelijke dieselpunk/sci-fi-ocean ervaring met een smooth game-feel, zonder de huidige technische basis te slopen.

Aanpak:
- Eerst fundament en stabiliteit.
- Dan game-feel (besturing, camera, combat flow).
- Daarna art/audio/UI richting.
- Dan content, bots en economy polish.
- Als laatste netwerk/online en release-hardening.

Werkprincipe:
- Elke fase sluit af met harde checks (DoD).
- Geen fase "klaar" zonder testresultaat + korte playtest-notes.

---

## 1. Huidige status (gelezen uit code + docs)

Sterk:
- Core architectuur staat: `ServiceLocator`, `EventBus`, `MatchStateMachine`.
- Basiscombat staat: cannon/torpedo, projectiles, upgrades, gold.
- Input + camera basis staat (PC en mobile handlers).
- Bot- en objective-systeem is gestart.
- Unit/PlayMode tests bestaan (5 testbestanden).

Belangrijkste gaten die eerst dicht moeten:
- TODO's in bots/objectives/minimap zijn nog open.
- `ObjectPool` heeft statische return-map maar registert pools nu niet automatisch.
- `TorpedoProjectile` detecteert Harbor, maar past daar nu geen damage toe.
- `BotSteering` roteert vooral, maar stuurt geen echte input-flow naar `ShipController`.
- Economy attributie is deels stub (`KillTracker.GetTier` altijd 1).

Conclusie:
- Basis is goed genoeg voor een gecontroleerde transformatie.
- We starten met stabilisatie + smoothness, daarna stijl/inhoud.

---

## 2. Faseplan (uitvoerbaar + controleerbaar)

## Fase 1 - Stabiliseren en technische schuld wegwerken (Week 1)

Doel:
- Kernsystemen betrouwbaar maken zodat latere style/gameplay werk niet breekt.

Taken:
- Fix `ObjectPool` auto-register in `Awake`.
- Fix Harbor hit-flow voor torpedo's.
- Bot basisgedrag afmaken: retreat/shop route placeholders vervangen.
- LaneObjective gold tick koppelen aan echte team-speler registry.
- KillTracker tier-attributie koppelen aan actieve ship data.
- Minimap sonar reveal daadwerkelijk visualiseren (tijdelijke marker voldoende).

Checks (DoD):
- Alle bestaande tests slagen.
- Nieuwe tests toegevoegd voor:
  - Torpedo beschadigt Harbor.
  - ObjectPool return werkt zonder destroy fallback.
  - Kill gold gebruikt echte tier.
- 10 minuten sandbox-run zonder errors/warnings spam.

---

## Fase 2 - Smooth game-feel pass (Week 2)

Doel:
- "Smooth" besturing/camera/combat, prioriteit boven nieuwe features.

Taken:
- `ShipController` fine-tuning:
  - Acceleration/deceleration curves.
  - Turn blending bij lage snelheid.
  - Aim-to-heading smoothing verbeteren.
- `IsometricCameraController` tuning:
  - Minder jitter op follow.
  - Betere pinch/rotate damping op mobile.
  - Zoom steps consistenter maken.
- Weapon feel:
  - Cannon impact readability (timing + VFX trigger timing).
  - Torpedo drift en snelheid per archetype balanceren.
- Input feel:
  - Mobile twin-stick deadzones.
  - Primary/secondary input buffering (kleine tolerance).

Checks (DoD):
- Playtestscript "5 rondes x 5 min" op PC doorlopen.
- Mobile smoke-test met stabiele controls zonder input spikes.
- Gemeten:
  - Geen onverwachte velocity spikes.
  - Camera overshoot binnen acceptabele marge.

---

## Fase 3 - Dieselpunk / sci-fi ocean stijlrichting implementeren (Week 3-4)

Doel:
- Visuele en auditieve identiteit voelbaar maken in gameplay.

Taken:
- Art direction bible als data:
  - Kleurpalet, materiaalregels, silhouette-regels per ship class.
- Environment upgrade:
  - Water shader varianten (PC/Mobile) met bioluminescente accenten.
  - Harbor/lane props: riveted steel, pipes, fog vents, signal lights.
- Ship visual pass:
  - Placeholder meshes vervangen door duidelijke dieselpunk silhouettes.
  - Team-identiteit via licht/trim, niet alleen vlakke kleur.
- VFX pass:
  - Muzzle flash, shell splash, torpedo wake, smoke volume.
- Audio pass:
  - Engine loops, industrial cannon SFX, sonar ping, harbor ambience.

Checks (DoD):
- 1 volledige match met nieuwe art/audio zonder functionele regressie.
- Mobile profiel:
  - FPS target gehaald met mobile water/material variant.
- Stijlcheck:
  - Elk schip in 1 seconde visueel herkenbaar op rol/archetype.

---

## Fase 4 - UX/UI ombouw naar dieselpunk control-room look (Week 5)

Doel:
- UI moet niet generiek zijn; duidelijke dieselpunk marine-console feel.

Taken:
- HUD restyle:
  - Brass/steel panel language, duidelijke contrasten.
  - Cooldown/status readability verbeteren.
- Shop UX:
  - Upgrade impact-preview (voor/na stats).
  - Heldere lock/prereq feedback.
- Minimap:
  - Betere iconhiërarchie en sonar ping leesbaarheid.
- Main menu/lobby theming consistent met in-game UI.

Checks (DoD):
- Usability-run: 3 kernflows zonder uitleg:
  - Upgrade kopen.
  - Cooldowns lezen.
  - Objective status snappen.
- Geen critical UI overlap bugs op 16:9 + mobile aspect ratios.

---

## Fase 5 - Gameplay diepte: objectives, bots, economy balans (Week 6-7)

Doel:
- Matchflow moet spannend en stabiel zijn (niet snowball-only).

Taken:
- Lane/camp rewards balanceren op matchduur-doel.
- Bot verbetering:
  - Doelkeuze (harass/objective/shop) volgens state.
  - Positionering en retreat gedrag.
- Ship class distinctiveness pass:
  - Corvette/Frigate/Destroyer/Ironclad per rol duidelijker maken.
- Upgrade curve fine-tuning (geen mandatory single-path build).

Checks (DoD):
- Minimaal 20 bot-matches gelogd met metrics:
  - Gemiddelde matchduur.
  - Winrate per archetype.
  - Gold source verdeling (kills/objectives/camps).
- Balanscriteria gehaald:
  - Geen archetype >55% winrate in testset.

---

## Fase 6 - Network/PvP readiness en performance hardening (Week 8-9)

Doel:
- Solide alpha-klaar gedrag voor online tests.

Taken:
- Mirror flow afmaken volgens technisch plan:
  - Ship/projectile auth flow, disconnect handling, bot replacement.
- Server-validaties uitbreiden:
  - Fire rate, speed, purchase sanity checks.
- Netcode smoothing voor remote ships/projectiles.
- Profiler-optimalisaties:
  - GC allocs verlagen.
  - Physics queries en draw calls reduceren.

Checks (DoD):
- 3v3 sessies zonder kritieke desync of crash.
- Acceptabele latency-ervaring in testrange.
- Budget checks gehaald (frame/network).

---

## Fase 7 - Vertical slice acceptatie en releasevoorbereiding (Week 10)

Doel:
- Een complete "dieselpunk sci-fi ocean" verticale slice die toonbaar is.

Taken:
- End-to-end regressietest op alle kernloops.
- Bugfix sprint op P0/P1 issues.
- Build pipeline check (PC + mobile testbuild).
- Korte design lock: wat gaat wel/niet mee naar volgende milestone.

Checks (DoD):
- "Start -> match -> shop -> objectives -> game over" volledig stabiel.
- Bekende blockers = 0.
- Testrapport + backlog prioriteit voor volgende iteratie staat klaar.

---

## 3. Uitvoervolgende sprintblokken (concreet per week)

Week 1:
- Fase 1 volledig afronden.

Week 2:
- Fase 2 volledig afronden.

Week 3-4:
- Fase 3 uitvoeren + performance checks.

Week 5:
- Fase 4.

Week 6-7:
- Fase 5.

Week 8-9:
- Fase 6.

Week 10:
- Fase 7.

Regel:
- Geen nieuwe features starten als vorige fase-checks rood zijn.

---

## 4. Quality gates (elke fase verplicht)

- Build gate: project compileert clean.
- Test gate: EditMode + PlayMode tests groen.
- Playtest gate: minimaal 1 volledige matchrun.
- Perf gate: vooraf afgesproken budget niet overschreden.
- Design gate: stijlconsistentie check (dieselpunk/sci-fi-ocean signatuur zichtbaar).

---

## 5. Directe eerste uitvoering (volgende 3 concrete acties)

1. Fase 1 fixes uitvoeren in code (pool, torpedo-harbor, kill tier, objective payout stubs).
2. Ontbrekende tests toevoegen en laten slagen.
3. Korte baseline playtest + profiler capture maken als referentie voor Fase 2 smoothness-pass.

