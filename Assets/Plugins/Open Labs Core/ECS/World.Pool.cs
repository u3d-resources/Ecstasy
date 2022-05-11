

using DP = AV.ECS.DataPool;

namespace AV.ECS 
{
    public partial class World 
    {
        public bool HasPool(int typeId)
        {
            return pools.Length > typeId;
        }
        public DataPool GetPool(int typeId)
        {
            return typeId > 0 && HasPool(typeId) ? pools[typeId] : null;
        }

        DataPool<T> P<T>()
        {
            var idx = DataPool<T>.TypeId;

            if (pools.Length <= idx)
                System.Array.Resize(ref pools, (int)idx + 1);

            ref var pool = ref pools[idx];
            if (pool == null)
                pool = new DataPool<T>(this);
            return pool as DataPool<T>;
        }

        public DataPool<T> Pool<T>() => P<T>();
        public (DataPool<T1>, DataPool<T2>) Pool<T1, T2>() => (P<T1>(), P<T2>());
        public (DataPool<T1>, DataPool<T2>, DataPool<T3>) Pool<T1, T2, T3>() => (P<T1>(), P<T2>(), P<T3>());
        public (DataPool<T1>, DataPool<T2>, DataPool<T3>, DataPool<T4>) Pool<T1, T2, T3, T4>() => (P<T1>(), P<T2>(), P<T3>(), P<T4>());
        public (DataPool<T1>, DataPool<T2>, DataPool<T3>, DataPool<T4>, DataPool<T5>) Pool<T1, T2, T3, T4, T5>() => (P<T1>(), P<T2>(), P<T3>(), P<T4>(), P<T5>());


        public (DataPool<T1>, DataPool<T2>) Pool<T1, T2>(out DP smallestPool)
        {
            var p1 = P<T1>(); var p2 = P<T2>();
            smallestPool = SmallestPool(p1, p2); return (p1, p2);
        }
        public (DataPool<T1>, DataPool<T2>, DataPool<T3>) Pool<T1, T2, T3>(out DP smallestPool)
        {
            var p1 = P<T1>(); var p2 = P<T2>(); var p3 = P<T3>();
            smallestPool = SmallestPool(p1, p2, p3); return (p1, p2, p3);
        }
        public (DataPool<T1>, DataPool<T2>, DataPool<T3>, DataPool<T4>) Pool<T1, T2, T3, T4>(out DP smallestPool)
        {
            var p1 = P<T1>(); var p2 = P<T2>(); var p3 = P<T3>(); var p4 = P<T4>();
            smallestPool = SmallestPool(p1, p2, p3, p4); return (p1, p2, p3, p4);
        }
        public (DataPool<T1>, DataPool<T2>, DataPool<T3>, DataPool<T4>, DataPool<T5>) Pool<T1, T2, T3, T4, T5>(out DP smallestPool)
        {
            var p1 = P<T1>(); var p2 = P<T2>(); var p3 = P<T3>(); var p4 = P<T4>(); var p5 = P<T5>();
            smallestPool = SmallestPool(p1, p2, p3, p4, p5); return (p1, p2, p3, p4, p5);
        }

        public DP SmallestPool(DP p1, DP p2)
        {
            return p1.count < p2.count ? p1 : p2;
        }
        public DP SmallestPool(DP p1, DP p2, DP p3)
        {
            var p = p1; if (p2.count < p.count) p = p2;
            if (p3.count < p.count) p = p3; return p;
        }
        public DP SmallestPool(DP p1, DP p2, DP p3, DP p4)
        {
            var p = p1; if (p2.count < p.count) p = p2;
            if (p3.count < p.count) p = p3;
            if (p4.count < p.count) p = p4; return p;
        }
        public DP SmallestPool(DP p1, DP p2, DP p3, DP p4, DP p5)
        {
            var p = p1; if (p2.count < p.count) p = p2;
            if (p3.count < p.count) p = p3;
            if (p4.count < p.count) p = p4;
            if (p5.count < p.count) p = p5; return p;
        }
        public DP SmallestPool(DP p1, DP p2, DP p3, DP p4, DP p5, DP p6)
        {
            var p = p1; if (p2.count < p.count) p = p2;
            if (p3.count < p.count) p = p3;
            if (p4.count < p.count) p = p4;
            if (p5.count < p.count) p = p5;
            if (p6.count < p.count) p = p6; return p;
        }
        public DP SmallestPool(DP p1, DP p2, DP p3, DP p4, DP p5, DP p6, DP p7)
        {
            var p = p1; if (p2.count < p.count) p = p2;
            if (p3.count < p.count) p = p3;
            if (p4.count < p.count) p = p4;
            if (p5.count < p.count) p = p5;
            if (p6.count < p.count) p = p6;
            if (p7.count < p.count) p = p7; return p;
        }
    }
}