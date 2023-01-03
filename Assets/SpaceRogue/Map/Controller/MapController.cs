using SpaceRogue.Map.Settings;
using Unity.Mathematics;
using UnityEngine;

namespace SpaceRogue.Map.Controller
{
    public class MapController : MonoBehaviour
    {
        public int seed;
        public SystemMapSettings systemMapSettings;

        private SystemMap systemMap;
        void Start()
        {
            this.systemMap = new SystemMap(this.systemMapSettings, this.seed);
        }
    }
}