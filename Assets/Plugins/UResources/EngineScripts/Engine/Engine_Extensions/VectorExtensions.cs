using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NATEngine
{
    public static class VectorExtensions
    {
        public static Vector3 ToXZ(this Vector2 vec, float y = 0)
        {
            return new Vector3(vec.x, y, vec.y);
        }
    }
}