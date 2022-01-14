using System;
using System.Collections.Generic;

namespace m3md2
{
    public class UpgList<T> : List<T>
    {
        public event EventHandler OnAdd;
        public event EventHandler OnRemove;

        public new void Add(T item) // "new" to avoid compiler-warnings, because we're hiding a method from base-class
        {
            base.Add(item);
            OnAdd?.Invoke(this, null);
        }

        public new void Remove(T item)
        {
            _ = base.Remove(item);
            OnRemove?.Invoke(this, null);
        }
    }
}
