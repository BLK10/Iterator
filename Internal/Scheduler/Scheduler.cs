using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using BLK10.Singleton;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace BLK10.Iterator
{   
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RuntimeToolbox))]
    public sealed class Scheduler : MonoBehaviour, IScheduler
    {        
        private List<Corout>     _runningCorouts;
        private Queue<Corout>    _coroutsToRemove;
                
        private ESchedulerStatus _status;
        private bool             _initialized;
        
        private int       _step;
        private Stopwatch _timer;
        

        #region "MONOBEHAVIOUR METHODS"
        
        private void Awake()
        {
            this._initialized = false;
            this.Init();
        }
                
        private void Update()
        {            
            this.Run();
        }

        private void LateUpdate()
        {            
            if (this._status == ESchedulerStatus.Play)
                this._step++;
        }

        private void OnApplicationQuit()
        {
            this.Stop();
            this._initialized = false;
        }

        #endregion


        #region "PROPERTIES"

        public ESchedulerStatus Status
        {
            get { return (this._status); }
        }
                
        public int CoroutCount
        {
            get { return (this._runningCorouts.Count); }
        }

        public float Second
        {
            get { return ((float)this._timer.Elapsed.TotalSeconds); }
        }

        public int Step
        {
            get { return (this._step); }
        }
        
        #endregion


        #region "PUBLIC METHODS"
           
        /// <summary>.</summary>
        public void Stop()
        {
            if (this._status != ESchedulerStatus.Stop)
            {
                this._status = ESchedulerStatus.Stop;
                this._timer.Reset();
                this._step   = 0;
                this.Clear();
            }
        }
        
        /// <summary>.</summary>
        public void Play()
        {
            if (this._status != ESchedulerStatus.Play)
            {                
                this._timer.Start();
                this._status = ESchedulerStatus.Play;                
            }
        }
        
        /// <summary>.</summary>
        public void Pause()
        {
            if (this._status == ESchedulerStatus.Play)
            {
                this._status = ESchedulerStatus.Pause;
                this._timer.Stop();
            }           
        }


        /// <summary></summary>
        /// <param name="coroutine"></param>               
        public void Append(Corout coroutine)
        {
            if (coroutine == null)
                throw new ArgumentNullException("coroutine");

            if (this._runningCorouts.Contains(coroutine))
                throw new Exception("coroutine already scheduled.");
                        
            if (coroutine.IsSynchronous)
            {
                this._runningCorouts.Insert(0, coroutine);                
                this.Play();
                this.Run();
            }
            else
            {                
                switch (coroutine.AsyncMode)
                {
                    case EAsyncMode.Priorize:
                        this._runningCorouts.Insert(0, coroutine);                        
                        this.Play();
                        break;

                    case EAsyncMode.Deferize:                        
                        this._runningCorouts.Add(coroutine);
                        this.Play();
                        break;

                    case EAsyncMode.Normal:
                    default:
                         this._runningCorouts.Add(coroutine);
                         this.Play();
                        break;
                }
            }
        }
        
        /// <summary></summary>
        /// <param name="coroutine"></param>
        public bool Contains(Corout coroutine)
        {
            if (coroutine == null)
                throw new ArgumentNullException("coroutine");

            if (this._runningCorouts != null)
                return (this._runningCorouts.Contains(coroutine));

            return (false);
        }

        #endregion


        #region "PRIVATE METHODS"

        private void Init()
        {
            if (!this._initialized)
            {
                this._runningCorouts  = this._runningCorouts  ?? new List<Corout>();
                this._coroutsToRemove = this._coroutsToRemove ?? new Queue<Corout>();

                this._timer      = this._timer ?? new Stopwatch();
                this._timer.Reset();
                this._step       = 0;                
                this._status     = ESchedulerStatus.Stop;                

                this._initialized = true;
            }
        }

        private void Run()
        {
            if (this._status == ESchedulerStatus.Play)
            {
                for (int i = 0; i < this._runningCorouts.Count; ++i)
                {
                    if (!this.RunAtIndex(i))
                        break;
                }                
            }

            while (this._coroutsToRemove.Count > 0)
            {
                Corout corout = this._coroutsToRemove.Dequeue();

                if (corout != null)
                {                    
                    this._runningCorouts.Remove(corout);

                    if (corout.Exception != null)
                        throw corout.Exception;

                    var d = corout as IDisposable;
                    if (d != null)
                    {
                        d.Dispose();
                        d = null;
                    }
                    corout = null;
                }                    
            }                
        }

        private bool RunAtIndex(int index)
        {
            Corout corout = this._runningCorouts[index];

            if (corout != null)
            {
                if (corout.IsFence)
                {                    
                    if (index > corout.FenceIndex)
                        return (false);
                    
                    corout.MoveNext();                    
                    //while (corout.MoveNext()) { };
                    //return (true);
                }

                bool nxt = false;

                do
                {
                    nxt = corout.MoveNext();

                    if (!nxt)
                        this._coroutsToRemove.Enqueue(corout);

                } while (nxt && corout.IsSynchronous);

                if ((corout.AsyncMode == EAsyncMode.Priorize) || (corout.AsyncMode == EAsyncMode.Deferize))
                    return (false);
            }
            else
                this._runningCorouts.RemoveAt(index);
            

            return (true);
        }

        private void Clear()
        {
            for (int i = 0; i < this._runningCorouts.Count; ++i)
            {
                if (this._runningCorouts[i] != null)
                    this._coroutsToRemove.Enqueue(this._runningCorouts[i]);
            }

            while (this._coroutsToRemove.Count > 0)
            {
                Corout corout = this._coroutsToRemove.Dequeue();

                if (corout != null)
                {
                    this._runningCorouts.Remove(corout);
                    var d = corout as IDisposable;
                    if (d != null)
                    {
                        d.Dispose();
                        d = null;
                    }
                    corout = null;
                }
            }
        }

        #endregion
                
        
        public static IScheduler Instance
        {
            get
            {
                IToolbox rtool = RuntimeToolbox.Instance;

                if (rtool != null)
                    return (rtool.GetOrAddComponent<Scheduler>() as IScheduler);
                
                throw new NullReferenceException("Singleton \"RuntimeToolbox\" could not be null.");                
            }
        }
        
    }
}
