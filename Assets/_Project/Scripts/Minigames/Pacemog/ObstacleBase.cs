using UnityEngine;

namespace DisOriented.Minigames.Pacemog
{
    /// <summary>
    /// Base class for all scrolling obstacles/collectables.
    /// Handles downward movement and despawn at bounds.
    /// </summary>
    public class ObstacleBase : MonoBehaviour
    {
        protected float moveSpeed;
        protected float despawnY;
        protected bool isActive;

        /// <summary>Fired when this obstacle scrolls past the player</summary>
        public event System.Action<ObstacleBase> OnPassedPlayer;
        /// <summary>Fired when despawned</summary>
        public event System.Action<ObstacleBase> OnDespawned;

        public virtual void Activate(float speed, float despawnBound, int laneIndex)
        {
            moveSpeed = speed;
            despawnY = despawnBound;
            isActive = true;
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            isActive = false;
            gameObject.SetActive(false);
            OnDespawned?.Invoke(this);
        }

        protected virtual void Update()
        {
            if (!isActive) return;
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;

            if (transform.position.y <= despawnY)
                Deactivate();
        }

        /// <summary>Call when obstacle passes the player Y position.</summary>
        protected void NotifyPassed()
        {
            OnPassedPlayer?.Invoke(this);
        }

    }
}

