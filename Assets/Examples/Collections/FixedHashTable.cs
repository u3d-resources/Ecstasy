
using System.Collections;
using System.Collections.Generic;

namespace AV.ECS.Examples 
{
    /// https://thomaslevesque.com/2020/05/15/things-every-csharp-developer-should-know-1-hash-codes/
    public class FixedHashTable<V> : IEnumerable<FixedHashTable<V>.Bucket>
    {
        // Could be further improved with SmallBuffer instead of managed arrays?
        public struct Bucket
        {
            public readonly int[] keys; public readonly V[] values; public int count;

            public Bucket(int size) { keys = new int[size]; values = new V[size]; count = 0; }

            public ref V Get(int k)
            {
                for (int i = 0; i < count; i++)
                    if (keys[i] == k) return ref values[i];

                keys[count] = k; return ref values[count++];
            }
        }
        readonly int size, perKeySize;
        // Need some way to iterate on all assigned values + how to clear em all
        readonly Bucket[] buckets;
        readonly int[] sparse;
        
        public int count;

        public FixedHashTable(int size, int perKeySize = 5)
        {
            this.size = size;
            this.perKeySize = perKeySize;
            buckets = new Bucket[size];
        }

        public ref V Get(int hash)
        {
            var i = hash % size; ref var b = ref buckets[i < 0 ? -i : i];
            if (b.keys == null)
            {
                b = new Bucket(perKeySize);
                sparse[count++] = i;
            }
            return ref b.Get(hash);
        }

        public void Clear() {
            for (int i = 0; i < count; i++)
                buckets[sparse[i]].count = 0;
            count = 0;
        }

        // Not sure if this actually has any use..
        public IEnumerator<FixedHashTable<V>.Bucket> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return buckets[sparse[i]];
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
