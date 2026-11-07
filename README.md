# Endless Night Car Game

A night-time, three-lane endless runner built in Unity 6 URP as coursework (GD_HW2):
procedurally generated road, static and moving traffic, coin and fuel pickups, lives and a
draining fuel tank, neon underglow that scales with speed, fireworks, and a fully
procedurally synthesised soundtrack.

## Running it

1. Open this folder in Unity Hub (**Add project from disk**) with Unity **6000.6.3f1**.
   The packages in `Packages/manifest.json` restore on their own: URP 17.6.0,
   `com.unity.cloud.gltfast` 6.20.0, Input System, TextMeshPro, XR.
2. Open `Assets/Scenes/Game.unity` and press Play.
3. Controls: `A` / `←` and `D` / `→` change lane, `P` or `Esc` pauses.
   Menu, HUD, pause and game-over screens are driven by `UIManager` on one canvas.

## The skybox is deliberately not in this repo

`Assets/Skybox/NightSkyHDRI002_8K_HDR..hdr` is a single 93 MB file, so it is git-ignored.
Its **`.meta` is committed**, which pins the asset GUID
(`a24648d948c9e43f8a6b1cf50a3cf33b`) — drop the file back and `Assets/Materials/NightSky.mat`
re-links itself, no manual re-wiring.

- Download **Night Sky HDRI 002**, 8K HDR variant, from Poly Haven:
  <https://polyhaven.com/a/night_sky_hdri_002> (CC0).
- Save it as `Assets/Skybox/NightSkyHDRI002_8K_HDR..hdr` — note the **two dots** before the
  extension; that is the exact name the committed `.meta` belongs to.
- Refocus Unity so it reimports. The game runs fine without it; the sky is flat black and
  the scene is noticeably dimmer, so screenshots will not match.

## Layout

| Path | What lives there |
| --- | --- |
| `Assets/Scenes/Game.unity` | the only scene: player car, road spawner, volume, canvas |
| `Assets/Scripts/` | `GameManager` (state machine), `PlayerController`, `RoadGenerator` + `RoadSegment`, `PoolManager` + `AutoRelease`, `Interactable` → `ObstacleCar` → `MovingObstacleCar` / `Coin` / `FuelCan`, `NeonController`, `FireworksManager`, `AudioManager`, `CameraFollow`, `UIManager` |
| `Assets/Prefabs/` | road segment, obstacle cars, pickups, firework variants |
| `Assets/Materials/Extracted/` | copies of the glTF material sub-assets — glTFast sub-assets revert on reimport, so the dielectric (metallic 0) paint budget lives here and the prefabs reference these |
| `Assets/VFX/NightProfile.asset` | URP volume: Bloom, ACES tonemapping, colour adjustments, vignette |
| `Assets/Audio/` | WAVs synthesised for this project (no third-party audio) |
| `Assets/Models/` | the imported `.glb` cars, coin and jerrycan |

## Third-party assets and licences

| Asset | Source | Licence |
| --- | --- | --- |
| `NightSkyHDRI002_8K_HDR` | Poly Haven (formerly HDRI Haven) | CC0 |
| `1985_delorean_dmc-12_time_machine_bttf.glb` | Sketchfab — Diaz Design | CC-BY-NC-SA 4.0 |
| `1996_mercedes-benz_e50_amg_w210.glb` | Sketchfab — Diaz Design | CC-BY-NC-SA 4.0 |
| `italian_euro_coin.glb` | Sketchfab — therapy | CC-BY-ND 4.0 |
| `fuel_tank.glb` | Sketchfab — ROY (Gearloft) | CC-BY 4.0 |
| `Static.glb`, `Moving.glb`, `ObstacleCar_Moving.glb` | exported from Blender in this project | own work |

The car models are non-commercial licences — acceptable here because this is a graded,
non-commercial submission. Attribution is above; please keep it if you reuse the repo.
