


using System;
using UnityEngine;

namespace AV.ECS.Examples 
{
    public struct BodyMesh
    {
        public Mesh mesh;
        public Material material;
        public int layer;
    }
    
    public struct BodyCollider
    {
        public Collider collider;
        public float radius;
    }
    
    [Serializable] public struct SimpleRigidbody 
    {
        public float mass;
        public float velocityDamper;
        public float depenetrationVelocity;
        public LayerMask collisionMask;
        [NonSerialized] public Vector3 velocity;
    }
    
    public struct CustomGravity 
    {
        public float gravity;
    }
}