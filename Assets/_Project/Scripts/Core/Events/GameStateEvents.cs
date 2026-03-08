using DisOriented.Data;

namespace DisOriented.Core.Events
{
    /// <summary>
    /// Fired once every valid state transition
    /// </summary>
    public readonly struct GameStateChangedEvent
    {
        public readonly GameState PreviousState;
        public readonly GameState NewState;

        public GameStateChangedEvent(GameState previousState, GameState newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }

    }
}


