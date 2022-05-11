using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AV.ECS
{
    public interface IConvertToEntity
    {
        public void OnConvert(GameObjectEntity go);
    }

    [AddComponentMenu("Entities/Game Object Entity")]
    public class GameObjectEntity : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] public Component[] components;
        [SerializeField] public bool convertAll = true;

        HashSet<Type> oldComponents = new HashSet<Type>();
        HashSet<Type> newComponents = new HashSet<Type>();
        public Entity entity;
        
        // This is messy. I don't think it quite works
        public void OnBeforeSerialize()
        {
            if (convertAll)
                components = GetComponents<Component>().Where(x => !(x is GameObjectEntity)).ToArray();

            InitEntity();

            if (entity.id == 0)
                return;

            newComponents.Clear(); foreach (var c in components) newComponents.Add(c.GetType());

            foreach (var c in components)
                if (!oldComponents.Contains(c.GetType()))
                    World.Default.AddDataTypeless(entity.id, c);

            foreach (var t in oldComponents)
                if (!newComponents.Contains(t))
                    World.Default.RemoveDataTypeless(entity.id, t);

            oldComponents.Clear(); foreach (var c in components) oldComponents.Add(c.GetType());
        }
        public void OnAfterDeserialize() {}

        void ConvertComponents()
        {
            foreach (var c in components)
                World.Default.AddDataTypeless(entity.id, c);

            foreach (var c in components)
                if (c is IConvertToEntity convert)
                    convert.OnConvert(this);
        }
        void InitEntity()
        {
            entity = World.Default.GetEntity(gameObject);
        }
        void OnEnable()
        {
            InitEntity();
            ConvertComponents();
        }
        void OnDisable()
        {
            if (entity.id != 0)
                World.Default.RemoveEntity(entity.id);
        }
    }
}
