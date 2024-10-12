# Pass-It (working title)

## Overview

3d platformer, super smash bros style game where u throw bombs at each other

## Contents

- [Pass-It (working title)](#pass-it-working-title)
  - [Overview](#overview)
  - [Contents](#contents)
  - [.gitignore](#gitignore)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
  - [Branching strategy](#branching-strategy)


## .gitignore

Ensure you have a `.gitignore` file in your project to avoid tracking unnecessary files

```
[Ll]ibrary/
[Tt]emp/
[Bb]uild/
[Uu]serSettings/
[Ll]ogs/
```

# Prerequisites
- Unity 2022.3.47f1

# Project Structure
Here's a typical project structure tailored for multiple types of games

```
Game Folder/
├── Assets/
│   ├── Art/
│       ├── Textures/
│       ├── Materials/
│       └── Models/
│   ├── Audio/
│       ├── Dialog/
│       ├── SFX/
│       └── Music/
│   ├── Scripts/
│       ├── Gameplay/
│       ├── Tools/
│       └── Editor/
│   └── Scenes/
├── ProjectSettings/
├── Documentation/ <Contains design documents, and other project-related documentation>
│   └── Design Docs/
└── README.md
```

# Contributing

Team contributions are very encouragted! Please follow these steps to contribute

1. Create a new branch for your feature or bug fix:
```bash
git checkout -b feature/inventory
```

2. Make your changes and commit them:
```bash
git commit -m "feat: added inventory system"
```

3. Push your changes
```bash
git push origin feature/inventory
```

## Branching strategy
- **main**: The stable version of the game
- **dev**: All ongoing features and fixes go to this.
  - **feature/**: A new feature
  - **fix/**: A fix for a bug/issue (ex. `fix/issue-69`)
  - **if u have more ideas for branch categories**, just do it lol

