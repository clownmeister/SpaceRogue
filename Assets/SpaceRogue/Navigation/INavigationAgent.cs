using System;
using UnityEngine;

namespace SpaceRogue.Navigation
{
    public interface INavigationAgent
    {
        void SetTarget(Vector2 position, Action<bool> onFinish);
        void Stop();
    }
}