


using UnityEngine;

namespace AV.ECS.Examples {
    public class FluidSpawner : MonoBehaviour {

        public GameObject prefab;
        public float gravity;
        public SimpleRigidbody rigidbodyData;

        void FixedUpdate(){
            if (Input.GetMouseButton(0))
            {
                var entity = World.Default.CreateEntity();
                var collider = prefab.GetComponent<Collider>();

                World.Default.AddData(entity,
                    new TRS(transform),
                    new BodyMesh
                    {
                        mesh = prefab.GetComponent<MeshFilter>().sharedMesh,
                        material = prefab.GetComponent<MeshRenderer>().sharedMaterial,
                        layer = prefab.layer,
                    },
                    new BodyCollider
                    {
                        collider = collider,
                        radius = (collider as SphereCollider).radius,
                    },
                    rigidbodyData);

                if (gravity != 0)
                    World.Default.AddData(entity, new CustomGravity 
                    { 
                        gravity = gravity,
                    });
            }
        }
    }
}
