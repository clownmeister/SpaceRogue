using UnityEngine;

namespace SpaceRogue.Map.Settings
{
    public abstract class AbstractMapSettings : ScriptableObject, IMapSettings
    {
        [field: SerializeField]public string MapName { get; set; }
    }
}