using System;
using UnityEngine;

namespace GameKit.Pooling.Core
{
    internal class PoolObjectEntity : MonoBehaviour, IPoolEntity
    {
        private int _id;
        private IPoolContainer _owner;

        int IPoolEntity.Id
        {
            get => _id;
            set => _id = value;
        }

        IPoolContainer IPoolEntity.Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public void Reset()
        {
            gameObject.SetActive(false);
        }
    }
}