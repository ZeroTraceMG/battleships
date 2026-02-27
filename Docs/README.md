# Iron Tide: Naval Supremacy — Project Documentation

## Documenten in deze map

| Bestand | Inhoud |
|---------|--------|
| `BUILDPLAN.md` | Volledig build plan: game design, architectuur, milestones, SO-schemas, networking specs, 7-dagenplan |
| `TECHNICAL_PLAN.md` | Technisch plan: klasse-relaties, data-architectuur, scene-structuur, netwerk-specs, performance-budget, testplan, build-pipeline, risico-register |
| `PROMPT_PLAN.md` | Kant-en-klare Claude Code prompts per feature — kopieer en plak om elke feature te bouwen |

## Snelle navigatie

| Vraag | Ga naar |
|-------|---------|
| Wat is het spel? Wat is de core loop? | BUILDPLAN.md § A |
| Welke technologie gebruiken we? | BUILDPLAN.md § B / TECHNICAL_PLAN.md § 1-2 |
| Wanneer is wat klaar? | BUILDPLAN.md § C |
| Hoe zijn de ScriptableObjects opgebouwd? | BUILDPLAN.md § D / TECHNICAL_PLAN.md § 3 |
| Hoe werkt het netwerk (Mirror/KCP)? | BUILDPLAN.md § E / TECHNICAL_PLAN.md § 5 |
| Wat doe ik vandaag? (7-dagenplan) | BUILDPLAN.md § F |
| Hoe bouw ik feature X stap voor stap? | **PROMPT_PLAN.md** — kopieer de juiste prompt |
| Scene- en prefab-structuur? | TECHNICAL_PLAN.md § 4 |
| Performance-doelstellingen? | TECHNICAL_PLAN.md § 6 |
| Testplan? | TECHNICAL_PLAN.md § 7 |
| Git-strategie en build-targets? | TECHNICAL_PLAN.md § 8 |
| Risico's en maatregelen? | TECHNICAL_PLAN.md § 10 |

## Waar staat de code?

```
Assets/_Game/Scripts/   ← alle C# scripts (44 bestanden)
Assets/_Game/Tests/     ← EditMode + PlayMode tests
Assets/_Game/Data/      ← ScriptableObject .asset bestanden (aan te maken in Unity)
Assets/_Game/Prefabs/   ← prefabs (aan te maken in Unity Editor)
Assets/_Game/Scenes/    ← scenes (aan te maken in Unity Editor)
```

## Volgende stap

Open Unity Hub → **Add project from disk** → selecteer `C:\game\battleships`
Kies Unity 6 LTS. Packages worden automatisch geïnstalleerd via `Packages/manifest.json`.
