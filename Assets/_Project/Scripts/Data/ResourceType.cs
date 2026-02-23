using UnityEngine;

namespace DisOriented.Data
{
    /// <summary>
    /// the 4 core resrouces the player manages throughout the game
    /// </summary>
    public enum ResourceType
    {
        Mood,    // Emotional state. Drained by stressful activities 
        Swag,    // Social status. Affected by outfits, social choices
        Energy,  // Physical and Mental stamina. Drained by activities and restored by eating and rest
        Tummy    // Hunger. Decreases over time, or by doing activities, restored by eating  
    }
}
