using UnityEngine;

namespace DisOriented.Data
{
    public enum GameState
    {
        Boot,             // Initial state after managers are done
        MainMenu,         // TitleScreen and mian menu 
        Room,             // Exploring rahul's room ass the main gameplay hub 
        Minigame,         // Inside an active minigame
        MinigameResults,  // Viewing results after a minigame is over 
        Paused,           // Pause overlay actrive 
    }
}
