using System.Collections;
using System.Collections.Generic;
using AV.ECS;
using UnityEngine;


namespace AV.ECS.Examples
{
    // TODO: Totally unoptimized! There is a ton of room for improvements :)
    public class SimplePhysicsSystem : SystemBehaviour
    {
        const float DepenetrationError = 0.0001f;

        public bool enableSelfCollision;
        [Range(0, 1)]
        public float depenetrationFactor;
        public float velocityFactor;
        public int gridCapacity;
        public float cellSize;

        Collider[] overlaps;
        //FixedHashTable<SmallBuffer7<int>> quadrantMap;
        Dictionary<int, List<int>> quadrantMap; // used for depenetrating bodies from each other

        void Awake(){
            overlaps = new Collider[10];
            quadrantMap = new Dictionary<int, List<int>>(gridCapacity);
            //quadrantMap = new FixedHashTable<SmallBuffer7<int>>(gridCapacity);
        }

        int GetPositionMapKey(Vector3 pos)
        {
            return (int)(Mathf.Floor(pos.x / cellSize) + (1000000 + Mathf.Floor(pos.y / cellSize)) + (1000000 + Mathf.Floor(pos.z / cellSize)));
        }
        
        public override void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
            var physicsScene = Physics.defaultPhysicsScene;

            var gravityPool = world.Pool<CustomGravity>();
            // Required components
            var (trsPool, colliderPool, bodyPool) = world.Pool<TRS, BodyCollider, SimpleRigidbody>(out var smallestPool);

            quadrantMap.Clear();
            // Collect bodies into quadrant map
            foreach (var id in smallestPool) 
            {
                ref var trs = ref trsPool[id];

                var posKey = GetPositionMapKey(trs.position);
                if (!quadrantMap.TryGetValue(posKey, out var buff))
                    quadrantMap.Add(posKey, buff = new List<int>());

                buff.Add(id);
                //Debug.Log($"{posKey} {id} {buff.Count}");
            }

            //for (int i = 0; i < smallestPool.count; i++)
            foreach (var id in smallestPool)
            {
                //var id = trsPool.GetEntityID(i);
                ref var trs = ref trsPool[id];
                ref var body = ref bodyPool[id];
                ref var collider = ref colliderPool[id];

                var (pos, rot, scale) = trs.Deconstruct();

                body.velocity = Vector3.Lerp(body.velocity, default, body.velocityDamper * deltaTime);

                var overlapCount = physicsScene.OverlapSphere(pos, collider.radius, overlaps, body.collisionMask, QueryTriggerInteraction.UseGlobal);
                
                // Depenetrate world colliders
                bool didPenetrate = false;
                for (int o = 0; o < overlapCount; o++)
                {
                    var other = overlaps[o]; var otherTrs = other.transform;

                    var penetrated = Physics.ComputePenetration(collider.collider, pos, rot, other, otherTrs.position, otherTrs.localRotation, out var dir, out var dist);
                    if (!penetrated)
                        continue;
                    
                    didPenetrate = true;
                    var depen = dir * (dist + DepenetrationError);
                    trs.position += depen * depenetrationFactor;
                    body.velocity += depen * body.depenetrationVelocity;

                    var hitPoint = pos - dir * (collider.radius - dist);

                    var otherBody = other.attachedRigidbody;
                    if (otherBody)
                        otherBody.AddForceAtPosition(dir * -body.mass, hitPoint, ForceMode.Force);
                        //otherBody.AddForce(dir * -body.mass, ForceMode.Force);
                }

                // Depenetrate other bodies in a cell
                var posKey = GetPositionMapKey(trs.position);

                if (enableSelfCollision && quadrantMap.TryGetValue(posKey, out var buff))
                {
                    //Debug.Log($"{posKey} {buff.Length}");
                    for(int o = 0; o < buff.Count; o++)
                    {
                        var otherId = buff[o];
                        var other = colliderPool[otherId];
                        ref var otherTrs = ref trsPool[otherId];

                        //Debug.Log($"{o} {other.collider} {otherTrs.position}");

                        var penetrated = Physics.ComputePenetration(other.collider, otherTrs.position, otherTrs.rotation, collider.collider, pos, rot, out var dir, out var dist);
                        if (penetrated)
                        {
                            didPenetrate = true;

                            ref var otherBody = ref bodyPool[otherId];
                            var depen = dir * (dist + DepenetrationError) * (depenetrationFactor / 2);
                            //trs.position -= depen;
                            //otherTrs.position += depen;
                            body.velocity -= depen * body.depenetrationVelocity;
                            otherBody.velocity += depen * otherBody.depenetrationVelocity;
                        }
                    }
                }

                //if (!didPenetrate)
                {
                    if (gravityPool.Has(id)) // optional gravity
                    {
                        var gravity = gravityPool[id].gravity;
                        body.velocity.y += gravity * deltaTime;
                    }
                }

                trs.position += body.velocity * (velocityFactor * deltaTime);
            }
        }
    }
}
