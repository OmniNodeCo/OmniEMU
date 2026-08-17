using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OmniEMU.Common
{
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        public bool Equals(T x, T y)
        {
            return x == y;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.GetHashCode();
        }
    }
}
