# Dis-Oriented

An indie life sim game about the absurdity of being a young adult in the 2020s.

Follow Rahul, an international student navigating his first week of university through hilarious minigames and meaningful choices.

---

## 🎮 Team

| Name | Role |
|------|------|
| Ojas | Programming, Design, Integration |
| Morgan | 2D Art, UI/UX |
| Jaquie | 3D Modeling |

---

## 🛠️ Project Setup

### Prerequisites
- **Unity 6000.2.9f1 LTS**
- **Git** with **Git LFS** installed

### First Time Setup

1. **Install Git LFS** (in cmd prompt):
   ```bash
   git lfs install
   ```

2. **Clone the repository**:
   ```bash
   git clone https://github.com/Oj-Anand/DisOriented.git
   ```
   Or use GitHub Desktop: File → Clone Repository

3. **Open in Unity**:
   - Open Unity Hub
   - Click "Add" and select the cloned `DisOriented` folder
   - Open the project

---

## 📁 Folder Structure

```
Assets/
├── _Project/           # All game content lives here
│   ├── Art/
│   │   ├── 2D/
│   │   │   ├── UI/
│   │   │   ├── Characters/
│   │   │   └── Minigames/
│   │   └── 3D/
│   │       ├── Models/
│   │       ├── Materials/
│   │       └── Textures/
│   ├── Audio/
│   │   ├── Music/
│   │   └── SFX/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   ├── ScriptableObjects/
│   └── Resources/
├── Plugins/            # Third-party code plugins
└── ThirdParty/         # Asset Store packages
```

### Where to put your files

| If you're adding... | Put it in... |
|---------------------|--------------|
| Character portraits, UI sprites | `_Project/Art/2D/Characters/` or `UI/` |
| Minigame art (packing items, etc.) | `_Project/Art/2D/Minigames/` |
| 3D models (.fbx) | `_Project/Art/3D/Models/` |
| Textures for 3D models | `_Project/Art/3D/Textures/` |
| Materials | `_Project/Art/3D/Materials/` |
| Sound effects | `_Project/Audio/SFX/` |
| Music tracks | `_Project/Audio/Music/` |

---

## 🔄 Git Workflow

### Before you start working
```bash
git pull
```
Or in GitHub Desktop: Click "Fetch origin" then "Pull origin"

### Adding your work
1. Save your files in the correct folder
2. In GitHub Desktop, you'll see your changes listed
3. Write a short commit message describing what you added (e.g., "Add Rahul character portraits")
4. Click "Commit to main"
5. Click "Push origin"

### Asset Guidelines
- **Export final files only** — avoid committing WIP or source files (.psd, .blend)
- **Use lowercase with dashes** for filenames: `rahul-happy.png`, `backpack-grid.png`
- **Keep file sizes smol** — compress PNGs, export FBX without embedded textures

---

## 📅 Milestones

| Milestone | Due Date | Focus |
|-----------|----------|-------|
| M1 | Feb 22 | Core systems, UI, resource management |
| M2 | Mar 11 | Packing minigame + 3D room |
| M3 | Mar 29 | Class minigame + dialogue system |
| M4 | Apr 16 | Polish + integration |
| Final Demo | Apr 21 | Presentation ready |

---

## 🤝 Questions?

Shoot me a ping on Discord if you hit any Git issues or aren't sure where something should go!
