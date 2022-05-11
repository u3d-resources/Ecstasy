

using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;

#if ENTITY_SHORT_ID
using IntID = System.UInt16;
#else
using IntID = System.Int32;
#endif

namespace AV.ECS
{
    public struct NativePool<T> where T : unmanaged
    {
        [NativeDisableParallelForRestriction] public NativeArray<T> data;
        [ReadOnly] public NativeArray<IntID> packedIds;
        [ReadOnly] public NativeArray<IntID> sparseIds;

        public ref T Get(IntID entityId) => ref DataPoolExtensions.Ref(data, sparseIds[entityId]);
        public IntID GetIndex(IntID entityId) => sparseIds[entityId];
        public IntID GetEntityID(int index) => packedIds[++index];
        public bool Has(IntID entityId) => sparseIds[entityId] != 0;
        
        public void Dispose() 
        {
            DataPoolExtensions.Dispose(data);
            DataPoolExtensions.Dispose(packedIds);
            DataPoolExtensions.Dispose(sparseIds);
        }

        public static implicit operator NativePool(NativePool<T> pool) => new NativePool { packedIds = pool.packedIds, sparseIds = pool.sparseIds };
    } 
    
    public struct NativePool
    {
        [ReadOnly] public NativeArray<IntID> packedIds;
        [ReadOnly] public NativeArray<IntID> sparseIds;

        public IntID GetIndex(IntID entityId) => sparseIds[entityId];
        public IntID GetEntityID(int index) => packedIds[++index];
        public bool Has(IntID entityId) => sparseIds[entityId] != 0;

        public void Dispose()
        {
            DataPoolExtensions.Dispose(packedIds);
            DataPoolExtensions.Dispose(sparseIds);
        }
    }

    public static unsafe class DataPoolExtensions 
    {
        // https://github.com/stella3d/SharedArray/blob/master/Runtime/SharedArray.cs
        public static NativePool<T> AllocateNativePool<T>(this DataPool<T> pool, Allocator allocator = Allocator.TempJob) 
            where T : unmanaged
        {
            return new NativePool<T>() 
            {
                data = ConvertExistingDataToNativeArray<T>(pool.data, pool.packedIds.Length, allocator),
                packedIds = ConvertExistingDataToNativeArray<IntID>(pool.packedIds, pool.packedIds.Length, allocator),
                sparseIds = ConvertExistingDataToNativeArray<IntID>(pool.sparseIds, pool.sparseIds.Length, allocator),
            };
        }
        
        public static NativePool AllocateNativePool(this DataPool pool, Allocator allocator = Allocator.TempJob) 
        {
            return new NativePool()
            {
                packedIds = ConvertExistingDataToNativeArray<IntID>(pool.packedIds, pool.packedIds.Length, allocator),
                sparseIds = ConvertExistingDataToNativeArray<IntID>(pool.sparseIds, pool.sparseIds.Length, allocator),
            };
        }
        public static NativePool AllocateIDsArray(this DataPool pool, Allocator allocator = Allocator.TempJob)
        {
            return new NativePool()
            {
                packedIds = new NativeArray<IntID>(pool.packedIds, allocator),
                sparseIds = new NativeArray<IntID>(pool.sparseIds, allocator),
            };
        }

        public static TransformAccessArray AllocateTransformAccess(this DataPool pool, int desiredJobCount = -1)
        {
            var lookup = pool.world.idToGo;
            var count = pool.count;
            var array = new TransformAccessArray(count, desiredJobCount);

            for (int i = 1; i <= count; i++)
            {
                var id = pool.packedIds[i];
                if (lookup.TryGetValue(id, out var go))
                    array.Add(go.transform);
            }
            return array;
        }

        static NativeArray<T> ConvertExistingDataToNativeArray<T>(T[] managed, int length, Allocator allocator) 
            where T : unmanaged
        {
            NativeArray<T> native;
            // this is the trick to making a NativeArray view of a managed array (or any pointer)
            fixed (void* ptr = managed)
            {
                native = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, length, Allocator.None);
            }
            #if UNITY_EDITOR
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref native, AtomicSafetyHandle.Create());
            #endif
            return native;
        }

        internal static void Dispose<T>(NativeArray<T> native) where T : struct
        {
#if UNITY_EDITOR
            var handle = NativeArrayUnsafeUtility.GetAtomicSafetyHandle(native);
            AtomicSafetyHandle.CheckDeallocateAndThrow(handle);
            AtomicSafetyHandle.Release(handle);
#endif
        }

        // https://forum.unity.com/threads/ref-returns-in-native-collections.856117/
        public static ref T Ref<T>(this NativeArray<T> native, int i) where T : struct
        {
            if (i >= native.Length) throw new ArgumentOutOfRangeException(nameof(i));
            unsafe { return ref UnsafeUtility.ArrayElementAsRef<T>(native.GetUnsafePtr(), i); }
        }
    }
}