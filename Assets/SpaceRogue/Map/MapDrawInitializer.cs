using UnityEngine;

namespace SpaceRogue.Map
{
    public class MapDrawInitializer : MonoBehaviour
    {
        private void Start()
        {
            MapManager.Instance.RegenerateMap();
        }
    }
}