# AGENTS.md — Project Roguelike

## Project

Unity **2022.3.62f3** 2D roguelike (URP). Portuguese codebase — all names, comments, logs in pt-BR.

## Architecture

- **All scripts** in global namespace, flat under `Assets/Scripts/`. No subdirectories, no namespaces.
- **Entrypoint**: `MenuPrincipal.unity` → `SelecaoArma.unity` → `SampleScene.unity` (build order).
- **Singleton**: `GameManager.Instance` (`GameManager.cs`) tracks room progression state.
- **Input**: Old system (`Input.GetAxisRaw`, `Input.GetMouseButton`). New Input System package present but unused.
- **Weapon system**: `ArmaChao` (floor pickup) ↔ `PlayerAttack` (equips `Arma` struct). Melee and ranged via `TipoArma` enum.
- **Enemy flow**: `InimigosSpawner` spawns enemies → each death calls `InimigoDerrotado()` → when all dead, `GameManager.SalaLimpa()` → mini-boss spawn → `GameManager.MiniChefeDerrotado()` → next room.

## Key commands (Unity Editor)

No custom CLI tooling. All interaction through the Unity Editor:
- Open any scene under `Assets/Scenes/` to edit/test.
- Ctrl+P to play from current scene.
- Test Framework (`com.unity.test-framework`) is available but no tests exist.

## Code conventions

- All `MonoBehaviour` scripts in global namespace (no `using namespace` declarations).
- Serialized fields with `[Header(...)]` annotations.public fields for Unity inspector.
- `Debug.Log` / `Debug.LogError` for runtime diagnostics.
- Tag strings used directly: `"Player"`, `"Inimigo"`, `"Parede"`.
- Prefabs in `Assets/Prefabs/`, sprites in `Assets/Sprites/`.

## Git workflow (see CONTRIBUTING.md)

- `main` — production-ready only (no direct commits).
- `develop` — integration branch (no direct commits).
- `feature/*` — branched from `develop` for new features.
- All changes via PR with at least one peer review.
- Commit messages in Portuguese imperative.

## Licensing

- Code (`.cs`, scenes, prefabs) → **MIT License**
- Art (`Assets/Sprites/`, etc.) → **CC BY-NC-ND 4.0** (see `Assets/LICENSE-ASSETS.md`)
