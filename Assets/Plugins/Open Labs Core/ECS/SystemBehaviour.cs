using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace AV.ECS
{
    public abstract class SystemBehaviour : MonoBehaviour
    {
        public World world => World.Default;
        
        public enum UpdateMode
        {
            EndOfFrame,
            FixedUpdate,
            FixedTimestep,
            FixedTimestepScaled,
        }

        [SerializeField] UpdateMode updateMode;
        [SerializeField] float timestep = 0.02f;

        internal double elapsedMS;
        object yieldInstruction;
        readonly Stopwatch stopwatch = new Stopwatch();

        
        public virtual void OnEnabled() {}
        public abstract void OnUpdate();
        
        void OnEnable()
        {
            OnValidate();
            OnEnabled();

            var coroutine = UpdateLoop();
            StartCoroutine(coroutine);
        }
        void OnValidate() => yieldInstruction = GetYieldMode();

        object GetYieldMode()
        {
            switch (updateMode)
            {
                case UpdateMode.FixedTimestep: return new WaitForSeconds(timestep);
                case UpdateMode.FixedTimestepScaled: return new WaitForSecondsRealtime(timestep);
                case UpdateMode.EndOfFrame: return new WaitForEndOfFrame();
                case UpdateMode.FixedUpdate: return new WaitForFixedUpdate();
                default: return null;
            }
        }

        IEnumerator UpdateLoop()
        {
            while (true) 
            {
                //Profiler.BeginSample(GetType().Name);
                stopwatch.Restart();
                try 
                {
                    OnUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                stopwatch.Stop();
                //Profiler.EndSample();

                elapsedMS = stopwatch.Elapsed.TotalMilliseconds;
                
                yield return yieldInstruction;
            }
        }
    }
}