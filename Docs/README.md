# Iron Tide: Naval Supremacy — Project Documentation

## Documenten in deze map

| Bestand | Inhoud |
|---------|--------|
| `BUILDPLAN.md` | Volledig build plan: game design, architectuur, milestones, SO-schemas, networking specs, 7-dagenplan |
| `TECHNICAL_PLAN.md` | Technisch plan: klasse-relaties, data-architectuur, scene-structuur, netwerk-specs, performance-budget, testplan, build-pipeline, risico-register |
| `PROMPT_PLAN.md` | Kant-en-klare Claude Code prompts per feature — kopieer en plak om elke feature te bouwen |
| `ART_DIRECTION.md` | Dieselpunk/sci-fi visual identity bible — color palette (exact hex), ship silhouettes, UI style, VFX palette, audio mood |
| `ASSET_PIPELINE.md` | End-to-end asset workflow: Blender → texturing → Unity. Naming conventions, texture specs, import settings, LOD rules |
| `TOOLS_PLAN.md` | All tools: Unity packages, AI tools, 3D/audio/CI tools — versions, license costs, setup notes |
| `IMPLEMENTATION_CHECKLIST.md` | Master Phase 1–5 step-by-step checklist — every task with tool, output, and acceptance criterion |

Extra document toegevoegd: `BLENDER_INSTALL_EN_PROMPTS.md` (installatie + prompt pack).
Extra document toegevoegd: `STAPPENPLAN_BEGIN_TOT_EIND.md` (globale route van start tot release).
Extra document toegevoegd: `SPRINT_1_DAGPLAN_EN_PROMPTS.md` (dag-tot-dag sprintplan met prompts).

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
| Kleurenpalet (exact hex), visuele stijl? | **ART_DIRECTION.md** |
| Hoe exporteer ik assets vanuit Blender? | **ASSET_PIPELINE.md** § 4 |
| Welke tools gebruik ik en wat kost het? | **TOOLS_PLAN.md** |
| Wat is de volgende stap in het project? | **IMPLEMENTATION_CHECKLIST.md** |

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
