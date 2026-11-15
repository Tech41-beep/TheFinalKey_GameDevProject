# The Final Key

A 3D first-person maze and escape game made in Unity for the Mid-Term Group Project (Playable Obstacle Course).

## Goal

You wake up trapped in a furnished cabin. Find **3 keys** hidden in the maze, then reach the **exit door** to escape.

## Rules

1. Look at a key and press **E** to collect it. The HUD shows your progress (0/3 to 3/3).
2. The exit door stays locked until you have all 3 keys. With fewer keys, it tells you how many are missing.
3. Reaching the exit with all 3 keys shows the **Win** screen.
4. Touching the red hazard zone shows the **Lose** screen. Jump over it.
5. On the Win and Lose screens you can **restart** or go back to the **Main Menu**.

## Controls

| Input | Action |
|---|---|
| W A S D | Move |
| Mouse | Look around |
| Space | Jump |
| Left Shift | Run |
| E | Interact / collect key |

## Unity version and setup

- **Unity:** 6.3 LTS (6000.3.8f1)
- **Platform:** macOS (Apple Silicon)
- **Language:** C#

To run from source:

1. Install Unity 6.3 LTS (6000.3.8f1) from Unity Hub.
2. Clone this repository and open the folder with Unity Hub.
3. Open `Assets/Scenes/MainMenu.unity` and press Play.

To run the build: download the ZIP from the link below, unzip it and open the game. Unity is not needed.

## Submission links

| Item | Link |
|---|---|
| Git repository | _add link_ |
| Gameplay video (MP4) | _add link_ |
| Standalone build (ZIP) | _add link_ |

## Team and contributions

| Member | Role | Contribution |
|---|---|---|
| Kimtech | Scene management and game flow | `GameManager`, `SceneController` (win, lose, restart, scene loading) |
| Socheat | Player controller | `PlayerController`, `CameraController`, `GameInput` (move, run, jump, mouse look) |
| Bonsak | Environment, map, Win/Lose screens, README | Cabin environment and maze layout, Win and Lose scenes, `Hazard` (loads the Lose scene), README |
| Sarim | UI and player interaction | `UIManager`, `PlayerInteraction`, `KeyCollectible`, `ExitDoor` (HUD, key pickup, exit check) |

## Project structure

```
TheFinalKey/
├── Assets/
│   ├── Scenes/              MainMenu, Game, Win, Lose
│   ├── Scripts/
│   │   ├── Player/          PlayerController, CameraController, GameInput
│   │   ├── Game/            GameManager, KeyCollectible, ExitDoor, Hazard
│   │   ├── UI/              UIManager
│   │   ├── Interaction/     PlayerInteraction
│   │   ├── Management/      SceneController
│   │   └── Editor/          FinalKeySceneBuilder (editor-only setup tool)
│   ├── Prefabs/
│   │   ├── Player/          Player prefab (tag "Player")
│   │   ├── Keys/            Key prefab
│   │   └── Environment/     Environment prefabs
│   ├── Materials/           Hazard_Red, Key_Gold, Exit_Green, Floor_Wood, Wall_Beige
│   ├── Models/  UI/  Audio/ Extra models, UI assets and sounds
│   └── FurnishedCabin/      Imported Asset Store package (not edited)
├── Packages/                Unity package list
├── ProjectSettings/         Unity project settings (Build Settings, Input, Tags)
└── README.md
```

| Folder | What it holds |
|---|---|
| `Scenes` | The 4 scenes, in Build Settings order: `MainMenu`, `Game`, `Win`, `Lose`. `Game` is the only gameplay scene. |
| `Scripts/Player` | Movement, running, jumping and mouse look (Socheat). |
| `Scripts/Game` | Game rules and objects: key count, key pickup, exit door and hazard zone. |
| `Scripts/UI` | The HUD: key count, interact prompt and messages (Sarim). |
| `Scripts/Interaction` | Looking at a key and pressing E to collect it (Sarim). |
| `Scripts/Management` | `SceneController`, the only script that loads scenes (Kimtech). |
| `Scripts/Editor` | A tool that builds the scenes and prefabs. It only runs in the Unity Editor and is not part of the build. |
| `Prefabs` | Reusable objects: the Player and the Key. |
| `Materials` | Colors used in the maze: red hazard, gold key, green exit, wood floor, beige walls. |
| `FurnishedCabin` | Furniture and environment from the Asset Store. Credited below. |

## Scripts

| Script | Purpose |
|---|---|
| `GameInput` | Reads keyboard and mouse input |
| `PlayerController` | Walk, run and jump using a CharacterController |
| `CameraController` | Mouse look |
| `ThirdPersonCamera` | It places the camera behind and above the player, lets the mouse orbit around the player, and turns the player to face the same direction as the camera. W always moves the player away from the camera |
| PlayerWalkAnimation | Swings the arms and legs while the player walks, so the character looks like it’s walking rather than gliding. The limbs stop when the player stands still or is in the air. |
| `GameManager` | Key count, win and lose |
| `SceneController` | Loads scenes; used by the menu buttons |
| `KeyCollectible` | One-time key pickup |
| `ExitDoor` | Trigger: win if all keys are collected, otherwise shows a message |
| `Hazard` | Trigger: loads the Lose scene when the player touches it |
| `UIManager` | HUD: key count, interact prompt, messages |
| `PlayerInteraction` | Raycast from the camera and E to collect |

## Credits

- Environment: [Furnished Cabin - 3D Urban](https://assetstore.unity.com/packages/3d/environments/urban/furnished-cabin-71426) by Johnny Kasapi (Unity Asset Store)
- AI assistance: _state which parts (for example code help or README wording) were AI-assisted, as the course rules require_
- Tutorials: _list any you used, or remove this line_
