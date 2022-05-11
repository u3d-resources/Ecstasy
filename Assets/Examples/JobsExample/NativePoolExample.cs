using System.Collections;
using System.Collections.Generic;
using AV.ECS;
using UnityEngine;
using UnityEngine.Jobs;

namespace AV.ECS.Examples 
{
    public class NativePoolExample : MonoBehaviour
    {
        struct TestData { public float value; }
        struct IgnoreTag { }

        public GameObject prefab;
        public int spawnCount;
        public float spawnRadius;

        void Start()
        {
            for (int i = 0; i < spawnCount; i++)
            {
                var inst = Instantiate(prefab, Random.insideUnitSphere * spawnRadius, Quaternion.identity);
                var entity = World.Default.GetEntity(inst);

                World.Default.AddData(entity, new TestData());
                if (Random.value > 0.5f)
                    World.Default.AddData(entity, new IgnoreTag());
            }
        }

        void Update()
        {
            var (dataPool, ignorePool) = World.Default.Pool<TestData, IgnoreTag>();

            var job = new DataJob()
            {
                dataPool = dataPool.AllocateNativePool(),
                ignorePool = ignorePool.AllocateNativePool(),
            };
            var transformArray = dataPool.AllocateTransformAccess();
            var handle = job.Schedule(transformArray);

            handle.Complete();
            job.dataPool.Dispose();
            job.ignorePool.Dispose();
            transformArray.Dispose();
        }

        //[BurstCompile] // not in the project :P
        struct DataJob : IJobParallelForTransform
        {
            public NativePool<TestData> dataPool;
            public NativePool ignorePool;

            public void Execute(int i, TransformAccess transform)
            {
                //Debug.Log($"{i} {dataPool.packedIds.Length} {dataPool.sparseIds.Length}");
                var id = dataPool.GetEntityID(i);
                if (ignorePool.Has(id))
                    return;

                //Debug.Log(id);
                ref var data = ref dataPool.Get(id);

                data.value += id;

                transform.localPosition += new Vector3(1, 1, 1) * 0.01f;
                Debug.Log(data.value);
            }
        }
    }
}
