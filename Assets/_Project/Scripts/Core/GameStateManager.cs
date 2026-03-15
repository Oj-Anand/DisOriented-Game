using UnityEngine;
using System;
using System.Collections.Generic;
using DisOriented.Data;
using DisOriented.Core.Events;
using System.Runtime.InteropServices.WindowsRuntime;

namespace DisOriented.Core
{
    ///<summary>
    ///Master game state machine
    ///Validates state transitions, fires events, manages pause state 
    ///</summary>
    public class GameStateManager : Singleton<GameStateManager>
    {
        //========= EVENTS =============
        ///<summary>Fires on every valid state transition</summary>
        public event Action<GameStateChangedEvent> OnStateChanged;

        ///<summary>Fires when pause state is entered</summary>
        public event Action OnPaused;

        ///<summary>Fires when pause state is exited</summary>
        public event Action OnResumed;

        //========== STATE ==============
        private GameState _currentState = GameState.Boot;
        private GameState _stateBeforePause;
        private HashSet<(GameState from, GameState to)> _validTransitions;

        //========= PUBLIC READ ============
        public GameState CurrentState => _currentState; 
        public bool IsPaused => _currentState == GameState.Paused;

        ///<summary>Pre pause state used by resume method</summary>
        public GameState StateBeforePause => _stateBeforePause;
        //################################
        //         INITIALIZATION
        //################################
        protected override void OnInitialize()
        {
            _currentState = GameState.Boot;
            BuildTransitionTable(); 
        }

        private void BuildTransitionTable()
        {
            _validTransitions = new HashSet<(GameState, GameState)>
            {
                //Boot flow
                (GameState.Boot, GameState.MainMenu),

                //Main menu flow
                (GameState.MainMenu,  GameState.Room),

                //Room -> Minigame
                (GameState.Room, GameState.Minigame),

                //Minigame -> Minigame results
                (GameState.Minigame, GameState.MinigameResults),

                //Room -> main menu
                (GameState.Room, GameState.MainMenu),

                //minigame -> main menu (save quit from pause)
                (GameState.Minigame, GameState.MainMenu),

                //Minigame results to room 
                (GameState.Minigame, GameState.Room),

                // Pause(can pause from Room or Minigame)
                (GameState.Room,      GameState.Paused),
                (GameState.Minigame,  GameState.Paused),
                (GameState.Paused, GameState.Room),
                (GameState.Paused, GameState.Minigame),
                (GameState.Paused, GameState.MainMenu),
            };
        }

        //#################################
        //          PUBLIC API 
        //#################################
        ///<summary>
        ///Requests a state transition, returns true if valid 
        ///fires onstatechanged event if successful transition
        ///</summary>
        public bool TransitionTo(GameState newState)
        {
            if (newState == _currentState)
            {
                Debug.LogWarning($"[GAMESTATEMANAGER] Already in {_currentState}");
                return false; 
            }

            if (!_validTransitions.Contains((_currentState, newState)))
            {
                Debug.LogError($"[GAMESTATEMANAGER] Invalid transition from {_currentState} to {newState}");
                return false; 
            }

            GameState previous = _currentState;
            _currentState = newState;

            Debug.Log($"[GAMESTATEMANAGER] {previous} -> {newState}");
            OnStateChanged?.Invoke(new GameStateChangedEvent(previous, newState)); 

            return true;
        }

        ///<summary>
        ///Toggle pause on/off from minigame or room 
        ///remembers pre pause state
        ///</summary>
        public bool TogglePause()
        {
            if (_currentState == GameState.Paused)
            {
                //Resume to pre pause state
                _currentState = _stateBeforePause; 
                Time.timeScale = 1.0f;
                OnResumed ?.Invoke();
                OnStateChanged?.Invoke( new GameStateChangedEvent(GameState.Paused, _currentState));

                Debug.Log($"[GAMESTATEMANAGER] Resumed to {_currentState}");
                return true; 
            }

            if (_currentState == GameState.Room || _currentState == GameState.Minigame)
            {
                _stateBeforePause = _currentState;
                _currentState = GameState.Paused;
                Time.timeScale = 0.0f;
                OnPaused?.Invoke();
                OnStateChanged?.Invoke(new GameStateChangedEvent(_stateBeforePause,GameState.Paused ));

                Debug.Log($"[GAMESTATEMANAGER] Paused from {_stateBeforePause}");
                return true;

            }

            Debug.LogWarning($"[GAMESTATEMANAGER] Game cant be paused from {_currentState}");
            return false; 
        }

        ///<summary>Check if a transition is valid without actually transitioning</summary>
        public bool CanTransitionTo(GameState target)
        {
            return _validTransitions.Contains((_currentState, target));  
        }
    }
}
