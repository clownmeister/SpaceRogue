using System;
using UnityEngine;

namespace SpaceRogue.Navigation
{
    public interface INavigationAgent
    {
        void SetTarget(Vector3 position, Action<bool> onFinish);
        void Stop();
    }
}