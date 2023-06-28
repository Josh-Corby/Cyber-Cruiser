using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    [Serializable]
    public abstract class PickupEffectBase : MonoBehaviour
    {
        public abstract void OnPickup();
    }
}
