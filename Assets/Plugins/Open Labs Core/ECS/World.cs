using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif
#if ENTITY_SHORT_ID
using IntID = System.UInt16;
#else
using IntID = System.Int32;
#endif

namespace AV.ECS
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-1000000)]
    public sealed partial class World
    {
        public static World Default;

        internal const int MaxEntities = IntID.MaxValue;
        internal const int ArrayMinGrow = 100;
        internal const int ArrayMaxGrow = 1000;

        internal Dictionary<GameObject, Entity> goToEntity = new Dictionary<GameObject, Entity>();
        internal Dictionary<IntID, GameObject> idToGo = new Dictionary<IntID, GameObject>();

        public IntID entityCount;
        public Entity[] entities;
        IntID freeEntityCount;
        IntID[] freeEntities;

        public DataPool[] pools = new DataPool[0];

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void OnEditorLoad()
        {
            Init();
            SceneManager.activeSceneChanged += (_, __) => Init();
            EditorApplication.playModeStateChanged += state => {
                if (state == PlayModeStateChange.EnteredEditMode)
                    Init();
            };
        }
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void Init() {
            Default = new World();
        }


        public bool IsEntityNull(int id) {
            return id < entities.Length ? entities[id].id == 0 : true;
        }

        public ref Entity GetEntity(int index) => ref entities[index];

        public Entity GetEntity(GameObject go)
        {
            if (!goToEntity.TryGetValue(go, out var e))
            {
                goToEntity.Add(go, e = CreateEntity());
                AddOrAssign(idToGo, e.id, go);
            }

            if (IsEntityNull(e.id))
                e = CreateEntity();
            return e;
        }

        public Entity CreateEntity()
        {
            var id = ++entityCount;
            if (freeEntityCount > 0) 
            {
                World.GrowArray(ref freeEntities, freeEntityCount);
                id = freeEntities[--freeEntityCount];
            }
            World.GrowArray(ref entities, id);
            return entities[id] = new Entity { id = id };
        }
        public void RemoveEntity(IntID index)
        {
            World.GrowArray(ref entities, index);

            entityCount--;
            ref var entity = ref entities[index];
            
            for (int i = 0; i < pools.Length; i++)
            {
                var pool = pools[i];
                if (pool.Has(entity.id))
                    pool.RemoveData(entity.id);
            }
            
            entity.id = 0;
            //entity.version++;
            //entity.componentsMask = default;

            World.GrowArray(ref freeEntities, freeEntityCount);
            freeEntities[freeEntityCount] = index;
            freeEntityCount++;
        }
        public void ClearEntityComponents(IntID id)
        {
            World.GrowArray(ref entities, id);
            ref var entity = ref entities[id];

            for (int i = 0; i < pools.Length; i++)
            {
                var pool = pools[i];
                if (pool.Has(entity.id))
                    pool.RemoveData(entity.id);
            }
        }

        public ref T GetData<T>(IntID entityId)
        {
            var pool = P<T>();
            return ref pool.Get(entityId);
        }
        public ref T GetData<T>(IntID entityId, T defaultValue)
        {
            var pool = P<T>();
            if (!pool.Has(entityId))
                pool.AddData(entityId, defaultValue);
            return ref pool.Get(entityId);
        }

        public void AddData<T>(IntID entityId, T componentData)
        {
            ref var e = ref entities[entityId];
            P<T>().AddData(e.id, componentData);
            //entity.componentsMask[ComponentPool<T>.MaskId] = true;
        }
        public void AddData<T1, T2>(IntID entityId, T1 c1, T2 c2)
        {
            var id = entities[entityId].id;
            P<T1>().AddData(id, c1); P<T2>().AddData(id, c2); 
        }
        public void AddData<T1, T2, T3>(IntID entityId, T1 c1, T2 c2, T3 c3)
        {
            var id = entities[entityId].id;
            P<T1>().AddData(id, c1); P<T2>().AddData(id, c2); P<T3>().AddData(id, c3);
        }
        public void AddData<T1, T2, T3, T4>(IntID entityId, T1 c1, T2 c2, T3 c3, T4 c4)
        {
            var id = entities[entityId].id;
            P<T1>().AddData(id, c1); P<T2>().AddData(id, c2); P<T3>().AddData(id, c3); P<T4>().AddData(id, c4);
        }
        public void AddData<T1, T2, T3, T4, T5>(IntID entityId, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5)
        {
            var id = entities[entityId].id;
            P<T1>().AddData(id, c1); P<T2>().AddData(id, c2); P<T3>().AddData(id, c3); P<T4>().AddData(id, c4); P<T5>().AddData(id, c5);
        }

        public void RemoveData<T>(IntID entityId)
        {
            var id = entities[entityId].id;
            P<T>().RemoveData(id);
        }
        public void RemoveData<T1, T2>(IntID entityId)
        {
            var id = entities[entityId].id;
            P<T1>().RemoveData(id); P<T2>().RemoveData(id);
        }
        public void RemoveData<T1, T2, T3>(IntID entityId)
        {
            var id = entities[entityId].id;
            P<T1>().RemoveData(id); P<T2>().RemoveData(id); P<T3>().RemoveData(id);
        }
        public void RemoveData<T1, T2, T3, T4>(IntID entityId)
        {
            var id = entities[entityId].id;
            P<T1>().RemoveData(id); P<T2>().RemoveData(id); P<T3>().RemoveData(id); P<T4>().RemoveData(id);
        }
        public void RemoveData<T1, T2, T3, T4, T5>(IntID entityId)
        {
            var id = entities[entityId].id;
            P<T1>().RemoveData(id); P<T2>().RemoveData(id); P<T3>().RemoveData(id); P<T4>().RemoveData(id); P<T5>().RemoveData(id);
        }
        

        /* Utils */
        internal static void AddOrAssign<TK, TV>(Dictionary<TK, TV> lookup, TK k, TV v)
        {
            if (lookup.ContainsKey(k)) lookup[k] = v; else lookup.Add(k, v);
        }
        internal static void GrowArray<T>(ref T[] array, int index, int maxGrow = ArrayMaxGrow)
        {
            if (array == null)
                array = new T[index + ArrayMinGrow];

            var count = array.Length;
            if (count <= index)
            {
                var newSize = index + ArrayMinGrow + (int)Mathf.Min(count / 2, maxGrow);
                Array.Resize(ref array, newSize);
            }
        }


        /* Reflection */
        void AddData_reflection<T>(IntID id, T data) => AddData<T>(id, data);
        void RemoveData_reflection<T>(IntID id) => RemoveData<T>(id);

        static Dictionary<Type, MethodInfo> AddDataInfo = new Dictionary<Type, MethodInfo>();
        static Dictionary<Type, MethodInfo> RemoveDataInfo = new Dictionary<Type, MethodInfo>();
        static object[] objParam1 = new object[1], objParam2 = new object[2];
        const BindingFlags AnyBind = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public void AddDataTypeless(IntID entityId, object component)
        {
            var type = component.GetType();
            if (!AddDataInfo.TryGetValue(type, out var method))
                AddDataInfo.Add(type, method = GetType().GetMethod(nameof(AddData_reflection), AnyBind)
                .MakeGenericMethod(type));

            objParam2[0] = entityId; objParam2[1] = component;
            method.Invoke(this, objParam2);
        }
        public void RemoveDataTypeless(IntID entityId, Type type)
        {
            if (!RemoveDataInfo.TryGetValue(type, out var method))
                RemoveDataInfo.Add(type, method = GetType().GetMethod(nameof(RemoveData_reflection), AnyBind)
                .MakeGenericMethod(type));

            objParam1[0] = entityId;
            method.Invoke(this, objParam1);
        }
    }
}