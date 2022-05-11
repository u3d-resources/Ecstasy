

using System.Collections;
using System.Collections.Generic;
using AV.ECS;
using UnityEngine;

namespace AV.ECS.Examples
{
    // TODO: Implement using Graphics.DrawMeshInstanced
    public class SimpleRenderSystem : SystemBehaviour
    {
        public override void OnEnabled(){
            Camera.onPreCull += OnRender;
        }
        void OnDisable(){
            Camera.onPreCull -= OnRender;
        }

        public override void OnUpdate() {}
        void OnRender(Camera camera)
        {
            var (trsPool, meshPool) = world.Pool<TRS, BodyMesh>(out var smallestPool);

            for (int i = 0; i < smallestPool.count; i++)
            {
                var id = trsPool.GetEntityID(i);
                ref var trs = ref trsPool[id];
                ref var mesh = ref meshPool[id];

                var (pos, rot, scale) = trs.Deconstruct();

                Graphics.DrawMesh(mesh.mesh, pos, rot, mesh.material, mesh.layer, camera, 0);
            }
        }
    }
}
