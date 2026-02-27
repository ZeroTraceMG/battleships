# Iron Tide - Blender Installatie en Prompt Pack

**Doel:** snelle, herhaalbare workflow voor modeling/export richting Unity.
**Projectpad:** `C:\Game\battleships`
**Bronrichtlijn:** `Docs/ASSET_PIPELINE.md`

## 1. Installatie stap voor stap

1. Installeer `Blender 4.x` via de officiele website.
2. Open Blender en zet basisinstellingen:
   - `Edit > Preferences > Save & Load > Auto Save Temporary Files` aan.
   - Interface schaal instellen op leesbaar niveau.
3. Maak (indien nog niet aanwezig) deze mappen:
   - `Assets/_Game/Art/Ships/Frigate/Meshes`
   - `Assets/_Game/Art/Ships/Frigate/Textures`
   - `Assets/_Game/Art/Ships/Frigate/Materials`
4. Optionele tools:
   - `Armor Paint` of `Substance Painter` voor texturing.
   - `RizomUV` voor geavanceerde UV workflows.
5. Maak in Blender een FBX export preset: `IronTide_FBX` met:
   - Scale `0.01`
   - Forward `-Z Forward`
   - Up `Y Up`
   - Apply Transform `ON`
   - Smoothing `Face`
   - Apply Modifiers `ON`
   - Tangent Space `ON`
6. Doe 1 test-export met een simpele mesh naar:
   - `Assets/_Game/Art/Ships/Frigate/Meshes/SHIP_Frigate_LOD0.fbx`
7. Open Unity en controleer:
   - correcte schaal
   - correcte orientatie
   - geen vreemde normals

## 2. Prompt Pack (kopieer/plak per sessie)

### Prompt A - Blender Setup Check

```text
Context:
- Project: C:\Game\battleships
- Pipeline regels uit Docs/ASSET_PIPELINE.md volgen
- Doel: correcte Blender scene + export preset

Doe:
1. Geef een checklist om Blender-scene klaar te zetten voor Unity export.
2. Controleer naming: SHIP_[Archetype]_[LOD].fbx.
3. Geef exacte FBX export waarden die ik in Blender moet invullen.
4. Geef een korte self-check (scale, pivot, normals, transforms) voor export.

Output:
- Alleen praktische stappen, geen theorie.
```

### Prompt B - Frigate Blockout (LOD0)

```text
Doel:
Maak een duidelijke Frigate-silhouette (blockout) voor Phase 2.

Eisen:
- Silhouette moet op 64x64 herkenbaar zijn.
- Max ~8000 tris voor LOD0.
- Geen kleine details; focus op vormtaal dieselpunk/scifi.
- Objectnaam: SHIP_Frigate_LOD0

Lever:
1. Stap-voor-stap modeling workflow in Blender.
2. Welke modifiers wel/niet.
3. Tri-count controle stappen.
4. Klaar-voor-export checklist.
```

### Prompt C - LOD1 Afleiding

```text
Doel:
Maak LOD1 van bestaande SHIP_Frigate_LOD0.

Eisen:
- Target ~2500 tris.
- Zelfde silhouette, minder detail.
- Bestandsnaam: SHIP_Frigate_LOD1.fbx

Lever:
1. Concrete reductie-aanpak in Blender (welke delen eerst vereenvoudigen).
2. Kwaliteitscontrole zodat silhouette niet breekt.
3. Exportstappen voor beide LOD's.
```

### Prompt D - UV en Texture Voorbereiding

```text
Doel:
UV unwrap voor Frigate assets volgens pipeline.

Eisen:
- LOD0: 2048x2048 textures.
- Maps: Albedo, Normal, MetallicSmooth, Emission (optioneel).
- Correcte naamconventie T_SHIP_Frigate_*.png.

Lever:
1. UV workflow stap-voor-stap.
2. Texel density richtlijn.
3. Export vanuit Blender naar texture tool.
4. Unity import checks (sRGB vs Linear, normal map settings).
```

### Prompt E - Unity Import en Prefab Assembly

```text
Doel:
FBX importeren en speelklare prefab opzetten.

Eisen:
- LOD Group: 0-20m LOD0, 20-60m LOD1, daarna culled.
- Layer: Ships (6)
- Prefabnaam: PFB_Ship_Frigate

Lever:
1. Exacte Unity import settings.
2. Prefab hierarchie.
3. Material assignment (Default + Damaged variant).
4. Final QA checklist voor commit.
```

## 3. Globale volgorde (begin tot eind)

1. Setup valideren met Prompt A.
2. Eerst silhouettes voor alle 4 schepen (blockout).
3. Daarna LOD1 voor alle 4 schepen.
4. Dan UV + textures per schip.
5. Daarna Unity import + prefab assembly + LOD Groups.
6. Als laatste detail/polish pass.

## 4. Korte DoD checklist per schip

- Naamgeving klopt (`SHIP_*`, `T_*`, `M_*`, `PFB_*`).
- LOD0 en LOD1 aanwezig en correct budget.
- FBX import zonder schaal- of as-fouten.
- Materialen gekoppeld (Default + Damaged).
- Prefab met LOD Group opgeslagen en getest in scene.
