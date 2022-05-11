using System;
using System.Collections.Generic;
using System.Collections;

#if ENTITY_SHORT_ID
using IntID = System.UInt16;
#else
using IntID = System.Int32;
#endif

namespace AV.ECS
{
    public abstract class DataPool : IEnumerable<IntID>
    {
        internal const int DataArrayMaxGrow = 100;
        internal static uint LastComponentTypeId;

        public abstract uint typeId { get; }
        public abstract Type type { get; }
        public abstract Array array { get; }

        public IntID count, maxSparseId;
        public IntID[] packedIds = new IntID[0];
        public IntID[] sparseIds = new IntID[0];

        public readonly World world;
        public DataPool(World world)
        {
            this.world = world;
        }

        public IntID GetIndex(IntID entityId) => sparseIds[entityId];
        public IntID GetEntityID(int index) => packedIds[++index];
        public bool Has(IntID entityId) => entityId <= maxSparseId && sparseIds[entityId] != 0;

        public abstract object GetRaw(IntID index);
        public abstract void AddData(IntID index, object obj);
        public abstract void RemoveData(IntID index);

        public IEnumerator<IntID> GetEnumerator()
        {
            for (int i = 1; i <= count; i++)
                yield return packedIds[i];
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class DataPool<T> : DataPool
    {
        public static readonly uint TypeId = DataPool.LastComponentTypeId++;
        
        public override uint typeId => TypeId;
        public override Type type { get; } = typeof(T);
        public override Array array => data;

        // Array instead of List allows for direct access by ref, without struct copy (ref var item = ref data[idx])
        // Also makes it possible to use with NativeArray directly
        public T[] data = new T[0];

        // We use Sparse Sets for iterating over entities.
        // This gives us the following advantages:
        // - Data is tightly packed, contiguously per-type, no memory waste
        // - Loop over smallest pool when querying for multiple types
        // - No component bitmasks, simply check if entity ID is assigned in sparse array
        // - Super easy to support Jobs with NativeArrays
        // https://www.david-colson.com/2020/02/09/making-a-simple-ecs.html
        // https://github.com/SanderMertens/ecs-faq
        // https://www.geeksforgeeks.org/sparse-set/

        // 0 item is reserved for null entity. Yeah, it's a waste of one data slot..
        // The other way, which I cound't get to work — start counter at 1, 
        // then, at each packedIds[] - decrement index + check array bounds?
        // also, be wary of cast to int, as c# doesn't do arifmetics on shorts:
        // https://stackoverflow.com/questions/941584/byte-byte-int-why
        // https://stackoverflow.com/questions/43510056/c-casting-to-byte-unit8-t-during-subtraction-wont-force-underflow-like-i-ex/43578929#43578929

        public ref T this[IntID id] => ref Get(id);
        public ref T this[Entity e] => ref Get(e.id);

        public DataPool(World world) : base(world) 
        {
        }

        public ref T Get(IntID entityId) => ref data[sparseIds[entityId]];

        public override object GetRaw(IntID index) => Get(index);
        public override void AddData(IntID index, object obj) => AddData(index, (T)obj);

        public void AddData(IntID entityId, T component)
        {
            count++;
            World.GrowArray(ref sparseIds, entityId, DataPool.DataArrayMaxGrow);
            World.GrowArray(ref packedIds, count, DataPool.DataArrayMaxGrow);
            World.GrowArray(ref data, count, DataPool.DataArrayMaxGrow);

            if (maxSparseId < entityId)
                maxSparseId = entityId;

            sparseIds[entityId] = count;
            packedIds[count] = entityId;
            data[count] = component;
        }
        public override void RemoveData(IntID entityId)
        {
            if (entityId > maxSparseId)
                return;
            var idx = GetIndex(entityId);
            var last = packedIds[count];

            sparseIds[last] = sparseIds[entityId];
            packedIds[idx] = last;
            data[idx] = data[count];

            sparseIds[entityId] = 0;
            data[count] = default;
            count--;
        }
    }
}