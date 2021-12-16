namespace Dam.Model.Entities.Collections
{
    public class StringUniqueCollection : UniqueCollection<string>
    {
        private readonly int maxLength = 64;
        public StringUniqueCollection(int maxCount, int maxLength, IEnumerable<string>? items) : base(maxCount, items)
        {
            if (maxLength < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "<1");
            }

            this.maxLength = maxLength;
        }

        public override void Add(string item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Length > this.maxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(item), item.Length, $">{this.maxLength}");
            }

            base.Add(item);
        }
    }
}
