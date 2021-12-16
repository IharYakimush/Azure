using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dam.Model.Entities
{
    public static class GuidHelper
    {
        private static readonly HashSet<char> allowedIdChars = new(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' });

        public static void ValidateId(string id, byte expectedLength)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (expectedLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(expectedLength));
            }

            if (id.Length != expectedLength)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id.Length, expectedLength.ToString("D"));
            }

            if (id.Any(c => !allowedIdChars.Contains(c)))
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, $"[{string.Join(',', allowedIdChars)}]");
            }
        }

        public static string GenerateId(byte length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (length > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            return Guid.NewGuid().ToString("N").Substring(0, length);
        }
    }
}
