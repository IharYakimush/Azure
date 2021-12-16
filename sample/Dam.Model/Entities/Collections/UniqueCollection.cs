using System.Collections;

namespace Dam.Model.Entities.Collections
{
    public class UniqueCollection<T> : ICollection<T>
    {
        private readonly int maxCount = 8;

        private readonly HashSet<T> items;

        public UniqueCollection(int maxCount, IEnumerable<T>? items)
        {
            if (maxCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount), maxCount, "<1");
            }

            this.items = new HashSet<T>(maxCount);

            if (items != null)
            {
                foreach (var item in items)
                {
                    this.items.Add(item);
                }
            }

            this.maxCount = maxCount;
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public virtual void Add(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (items.Count >= maxCount)
            {
                throw new InvalidOperationException($"Max items count of {maxCount} exceeded");
            }

            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
