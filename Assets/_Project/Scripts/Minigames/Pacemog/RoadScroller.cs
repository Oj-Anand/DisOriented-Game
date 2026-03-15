using UnityEngine;

namespace DisOriented.Minigames.Pacemog
{
    /// <summary>Infinite scrolling road using two leapfrogging sprites.</summary>
    public class RoadScroller : MonoBehaviour
    {
        [SerializeField] private Transform roadTileA;
        [SerializeField] private Transform roadTileB;
        [SerializeField] private float tileHeight = 20f;

        private float _scrollSpeed;

        public void SetScrollSpeed(float speed) => _scrollSpeed = speed;

        private void Update()
        {
            float delta = _scrollSpeed * Time.deltaTime;
            roadTileA.position += Vector3.down * delta;
            roadTileB.position += Vector3.down * delta;

            //when a tile goes below screen, move it above the other
            if (roadTileA.position.y <= -tileHeight)
                roadTileA.position = roadTileB.position + Vector3.up * tileHeight;
            if (roadTileB.position.y <= -tileHeight)
                roadTileB.position = roadTileA.position + Vector3.up * tileHeight;
        }
    }
}

