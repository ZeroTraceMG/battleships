# Iron Tide: Naval Supremacy — Project Documentation

## Documenten in deze map

| Bestand | Inhoud |
|---------|--------|
| `BUILDPLAN.md` | Volledig build plan: game design, architectuur, milestones, SO-schemas, networking specs, 7-dagenplan |

## Snelle navigatie

- **Game design + concept** → BUILDPLAN.md § A
- **Technische architectuur + mappenstructuur** → BUILDPLAN.md § B
- **Milestone plan (5 fasen, ~26 weken)** → BUILDPLAN.md § C
- **ScriptableObject schemas (alle 7 typen)** → BUILDPLAN.md § D
- **Networking specs (Mirror/KCP, authority model)** → BUILDPLAN.md § E
- **Eerste 7 dagen exacte checklist** → BUILDPLAN.md § F

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
