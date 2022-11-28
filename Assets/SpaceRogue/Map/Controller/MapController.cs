using UnityEngine;

namespace SpaceRogue.Map.Controller
{
    public class MapController : MonoBehaviour
    {
        private MapGenerator mapGenerator;

        void Start()
        {
            this.mapGenerator = new MapGenerator();
        }

        void Update() { }
    }
}