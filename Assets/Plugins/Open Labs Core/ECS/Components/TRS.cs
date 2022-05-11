using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AV.ECS
{
    public struct TRS
    {
        public Matrix4x4 matrix;
        // position is the fastest to retrieve from matrix, so we don't store it as a field
        public Vector3 position { get => Position(ref matrix); set => Position(ref matrix, value); }
        public Quaternion rotation;
        public Vector3 scale;

        public (Vector3 pos, Quaternion rot, Vector3 scale) Deconstruct() => (position, rotation, scale);

        public TRS(Transform transform)
        {
            this.rotation = transform.rotation;
            this.scale = transform.lossyScale;
            this.matrix = Matrix4x4.TRS(transform.position, rotation, scale);
        } 
        
        public TRS(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            this.rotation = rot;
            this.scale = scale;
            this.matrix = Matrix4x4.TRS(pos, rot, scale);
        }

        public static Vector3 Position(ref Matrix4x4 m)
        {
            var v = default(Vector3);
            v.x = m.m03;
            v.y = m.m13;
            v.z = m.m23; return v;
        }
        public static void Position(ref Matrix4x4 m, Vector3 v)
        {
            m.m03 = v.x;
            m.m13 = v.y;
            m.m23 = v.z;
        }

        public static Vector3 Scale(ref Matrix4x4 m)
        {
            var v = default(Vector3);
            v.x = m.m00;
            v.y = m.m11;
            v.z = m.m22; return v;
        }
        public static void Scale(ref Matrix4x4 m, Vector3 v)
        {
            m.m00 = v.x;
            m.m11 = v.y;
            m.m22 = v.z;
        }
        public static void Rotate(ref Matrix4x4 m, Quaternion q)
        {
            var num = q.x * 2f;
            var num2 = q.y * 2f;
            var num3 = q.z * 2f;
            var num4 = q.x * num;
            var num5 = q.y * num2;
            var num6 = q.z * num3;
            var num7 = q.x * num2;
            var num8 = q.x * num3;
            var num9 = q.y * num3;
            var num10 = q.w * num;
            var num11 = q.w * num2;
            var num12 = q.w * num3;

            m.m00 = 1f - (num5 + num6);
            m.m10 = num7 + num12;
            m.m20 = num8 - num11;
            m.m30 = 0f;
            m.m01 = num7 - num12;
            m.m11 = 1f - (num4 + num6);
            m.m21 = num9 + num10;
            m.m31 = 0f;
            m.m02 = num8 + num11;
            m.m12 = num9 - num10;
            m.m22 = 1f - (num4 + num5);
            m.m32 = 0f;
            m.m33 = 1f;
        }

        public static void Matrix(ref Matrix4x4 m, Vector3 t, Quaternion r, Vector3 s)
        {
            m.m00 = (1.0f - 2.0f * (r.y * r.y + r.z * r.z)) * s.x;
            m.m10 = (r.x * r.y + r.z * r.w) * s.x * 2.0f;
            m.m20 = (r.x * r.z - r.y * r.w) * s.x * 2.0f;
            //m.m30 = 0.0f;
            m.m01 = (r.x * r.y - r.z * r.w) * s.y * 2.0f;
            m.m11 = (1.0f - 2.0f * (r.x * r.x + r.z * r.z)) * s.y;
            m.m21 = (r.y * r.z + r.x * r.w) * s.y * 2.0f;
            //m.m31 = 0.0f;
            m.m02 = (r.x * r.z + r.y * r.w) * s.z * 2.0f;
            m.m12 = (r.y * r.z - r.x * r.w) * s.z * 2.0f;
            m.m22 = (1.0f - 2.0f * (r.x * r.x + r.y * r.y)) * s.z;
            //m.m32 = 0.0f;
            m.m03 = t.x;
            m.m13 = t.y;
            m.m23 = t.z;
            m.m33 = 1.0f;
        }
    }


    // https://github.com/Unity-Technologies/Graphics/blob/master/TestProjects/BatchRendererGroup_URP/Assets/SampleScenes/SimpleExample/SimpleBRGExample.cs#L39
    /*
    struct PackedMatrix
    {
        public float c0x, c0y, c0z;
        public float c1x, c1y, c1z;
        public float c2x, c2y, c2z;
        public float c3x, c3y, c3z;

        public PackedMatrix(Matrix4x4 m)
        {
            c0x = m.m00;
            c0y = m.m10;
            c0z = m.m20;
            c1x = m.m01;
            c1y = m.m11;
            c1z = m.m21;
            c2x = m.m02;
            c2y = m.m12;
            c2z = m.m22;
            c3x = m.m03;
            c3y = m.m13;
            c3z = m.m23;
        }
    }*/
}