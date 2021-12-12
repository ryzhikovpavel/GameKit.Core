using System.Collections;
using System.Collections.Generic;

namespace GameKit.Implementation
{
    internal class IssuedEnumerator<T>: IEnumerator<T>
    {
        private IEnumerator<PoolItem<T>> itemsEnumerator;
        
        public IssuedEnumerator(IEnumerator<PoolItem<T>> itemsEnumerator)
        {
            this.itemsEnumerator = itemsEnumerator;
        }
        
        public bool MoveNext()
        {
            if (itemsEnumerator.MoveNext() == false) return false;
            Current = default;
            if (itemsEnumerator.Current == null) return false;
            if (itemsEnumerator.Current.State != PoolItemState.Issued) return MoveNext();
            if (itemsEnumerator.Current.Instance == null) return MoveNext();
            Current = itemsEnumerator.Current.Instance;
            return true;
        }

        public void Reset()
        {
            itemsEnumerator.Reset();
        }

        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            itemsEnumerator.Dispose();
            itemsEnumerator = null;
        }
    }
}