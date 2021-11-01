using System;
using GameKit.Pooling;
using GameKit.Pooling.Core;
using UnityEngine;

namespace GameKit
{
    public static class PoolExtensions
    {
        public static void PushToPool(this GameObject obj)
        {
            var e = obj.GetComponent<IPoolEntity>();
            if (e == null)
                throw new Exception($"The {obj.name} is not pooled object");
            
            e.Owner.Push(e);
        }
        
        public static void PushToPool(this Component comp)
        {
            var e = comp.GetComponent<IPoolEntity>();
            if (e == null)
                throw new Exception($"The {comp.name} is not pooled object");
            
            e.Owner.Push(e);
        }
    }
}