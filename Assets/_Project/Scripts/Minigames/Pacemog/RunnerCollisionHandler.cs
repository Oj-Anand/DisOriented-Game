using UnityEngine;

namespace DisOriented.Minigames.Pacemog
{
    /// <summary>Detects contact with obstacles and fires events</summary>
    [RequireComponent(typeof(Collider2D))]
    public class RunnerCollisionHandler : MonoBehaviour
    {
        public event System.Action OnHitPedestrian;
        public event System.Action<int> OnCollectCoin;
        public event System.Action OnHitIce;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Pedestrian>(out _))
            {
                OnHitPedestrian?.Invoke();
            }
            else if (other.TryGetComponent<Coin>(out var coin))
            {
                OnCollectCoin?.Invoke(coin.value);
                coin.Deactivate();
            }
            else if (other.TryGetComponent<IcePatch>(out _))
            {
                OnHitIce?.Invoke();
            }
        }

    }
}


