# The Final Key

A 3D first-person maze and escape adventure made in Unity for the mid-term group project.

You wake up trapped in a mysterious furnished cabin. Explore the maze-like rooms, find **3 keys**, and reach the **exit**.

- **Engine:** Unity 6.3 LTS (6000.3.8f1), macOS (Apple Silicon)
- **Language:** C#
- **Environment asset:** [Furnished Cabin - 3D Urban](https://assetstore.unity.com/packages/3d/environments/urban/furnished-cabin-71426) by Johnny Kasapi

## Team

| Member | Role |
|---|---|
| Kimtech | Scene management and game flow |
| Socheat | Player controller |
| Bonsak | Environment, map, Win/Lose screens, README |
| Sarim | UI and player interaction |

## Controls

| Input | Action |
|---|---|
| W A S D | Move |
| Mouse | Look around |
| Space | Jump |
| Left Shift | Run |
| E | Interact / collect key |

## How to play

1. Press **Start Game** in the Main Menu.
2. Explore the cabin and find the 3 keys. Look at a key and press **E**.
3. The HUD shows your progress (0/3 to 3/3).
4. Reach the exit door. With fewer than 3 keys it stays locked and tells you how many are missing.
5. With all 3 keys the **Win** screen appears.
6. Touching the red hazard zone (jump over it!) sends you to the **Lose** screen.

## Setup (first time, every member)

1. Install **Unity 6.3 LTS (6000.3.8f1)** from Unity Hub.
2. Clone this repository and open the folder with Unity Hub.
3. If you are creating the project fresh: create a new **3D** project named `TheFinalKey`, then copy this repository's `Assets/` folder, `README.md` and `.gitignore` into it.
4. In Unity, run the menu **The Final Key > Build All Scenes (greybox)** (only ONE person does this, then commits the scenes).
5. Open `Assets/Scenes/MainMenu.unity` and press Play.

> The builder creates the 4 scenes, the Key and Player prefabs, and fills Build Settings automatically. The Game scene is a greybox (cube walls) maze that Bonsak decorates with the Furnished Cabin asset.

## Project structure

```
TheFinalKey/
├── Assets/
│   ├── Scenes/            MainMenu, Game, Win, Lose
│   ├── Scripts/
│   │   ├── Player/        PlayerController, CameraController, GameInput
│   │   ├── Game/          GameManager, KeyCollectible, ExitDoor, Hazard
│   │   ├── UI/            UIManager
│   │   ├── Interaction/   PlayerInteraction
│   │   ├── Management/    SceneController
│   │   └── Editor/        FinalKeySceneBuilder (setup tool, editor only)
│   ├── Prefabs/           Player/, Keys/, Environment/
│   ├── Materials/  Models/  UI/  Audio/
│   └── FurnishedCabin/    (imported asset - do not edit)
├── Packages/
├── ProjectSettings/
└── README.md
```

## Scripts

| Script | Owner | Attached to | Purpose |
|---|---|---|---|
| `GameInput` | Socheat | (static helper) | Reads keyboard and mouse for both Unity input systems |
| `PlayerController` | Socheat | Player | Walk, run, jump (CharacterController) |
| `CameraController` | Socheat | Main Camera | Mouse look |
| `GameManager` | Kimtech | GameManager object | Key count, win/lose/restart |
| `SceneController` | Kimtech | SceneController object (menu, Win, Lose) | The only scene loader; button methods |
| `KeyCollectible` | Sarim | Key prefab | One-time key pickup |
| `ExitDoor` | Sarim | Exit trigger | Win check or "keys missing" message |
| `Hazard` | Socheat / Bonsak | Hazard trigger | Loads the Lose scene |
| `UIManager` | Sarim | GameUI canvas | HUD: key count, prompt, messages |
| `PlayerInteraction` | Sarim | Main Camera | Raycast + E to collect |

### How scripts communicate

```
PlayerInteraction --(E)--> KeyCollectible.Collect() --> GameManager.CollectKey() --> UIManager.UpdateKeyCount()
ExitDoor (trigger) --> GameManager.HasAllKeys()
      true  --> GameManager.WinGame()  --> SceneController.Load("Win")
      false --> UIManager.ShowMessage("... need N more keys")
Hazard (trigger) --> GameManager.LoseGame() --> SceneController.Load("Lose")
Win / Lose / Menu buttons --> SceneController.LoadGame() / LoadMainMenu() / QuitGame()
```

## Git workflow

Branches: `main`, `feature/scene-management-kimtech`, `feature/player-controller-socheat`, `feature/environment-bonsak`, `feature/ui-interaction-sarim`.

- Bonsak owns `Game.unity`. Nobody else edits it at the same time.
- Commit often with clear messages; review before merging into `main`.
- Agree on method names before changing shared scripts.
- In Unity: Edit > Project Settings > Editor > Asset Serialization = **Force Text**.

## Using the Furnished Cabin asset (Bonsak)

1. Import the asset from Package Manager > My Assets into `Assets/FurnishedCabin/` and do not modify it.
2. Open its demo scene and note which rooms, walls and furniture exist.
3. Drop furniture prefabs into `Game.unity` under the `Environment` object, replacing or covering the greybox walls where it makes sense.
4. Make sure every wall and furniture piece has a Collider. Keep passages at least 1.2 m wide.
5. If the asset's materials look pink, convert them: Edit > Rendering > Materials > Convert Selected Built-in Materials to URP.
6. Keep the Key, Exit and Hazard objects and the Player; move them to match the new layout.

## Testing checklist

- [ ] Start Game loads the Game scene
- [ ] Walk, run, jump; cannot walk through walls
- [ ] "Press E to Collect" appears when looking at a key
- [ ] Each key counts once; HUD shows 1/3, 2/3, 3/3
- [ ] Exit with fewer than 3 keys shows the missing-keys message
- [ ] Exit with 3 keys loads Win
- [ ] Hazard loads Lose
- [ ] Restart/Retry and Main Menu buttons work
- [ ] Standalone macOS build runs

## Needs confirmation

- Submission deadline: **16 November 2026** .
- Lose condition (hazard zone) must be approved by the team.
- Required submission format and links (GitHub, MP4, build ZIP, Slido).

## Credits

- Furnished Cabin - 3D Urban by Johnny Kasapi (Unity Asset Store)
# TheFinalKey_GameDevProject
