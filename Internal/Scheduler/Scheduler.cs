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
    internal sealed class Scheduler : MonoBehaviour, IScheduler
    {        
        private List<Corout>     _runningCorouts;
        private Queue<Corout>    _coroutsToRemove;
        
        private int              _firstIndex;
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
            {
                this._step++;
            }
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
                
        public int Count
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
        public void SubmitCorout(Corout coroutine)
        {
            if (coroutine == null)
                throw new ArgumentNullException("coroutine");

            if (this._runningCorouts.Contains(coroutine))
                throw new Exception("coroutine already scheduled.");
                        
            switch (coroutine.Type)
            {
                case ECoroutType.Synchrone:                    
                    this._runningCorouts.Insert(0, coroutine);
                    this._firstIndex++;
                    this.Play(); 
                    this.Run();
                    break;

                case ECoroutType.Priority:
                    this._runningCorouts.Insert(0, coroutine);
                    this._firstIndex++;
                    this.Play();                    
                    break;

                case ECoroutType.LazyPriority:
                    this._runningCorouts.Insert(this._firstIndex, coroutine);
                    this._firstIndex++;
                    this.Play();                     
                    break;

                case ECoroutType.Asynchrone:
                default:
                    this._runningCorouts.Add(coroutine);
                    this.Play();                    
                    break;
            }
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
                this._firstIndex = 0;
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
                    {
                        break;
                    }
                }                
            }

            while (this._coroutsToRemove.Count > 0)
            {
                Corout corout = this._coroutsToRemove.Dequeue();

                if (corout != null)
                {                    
                    if ((corout.Type == ECoroutType.Synchrone) ||
                        (corout.Type == ECoroutType.Priority) ||
                        (corout.Type == ECoroutType.LazyPriority))
                    {
                        this._firstIndex--;
                    }
                    
                    this._runningCorouts.Remove(corout);

                    if (corout.ThrowError && (corout.Error != null))
                    {
                        foreach (var ex in corout.Error.Exceptions)
                        {
                            throw ex;
                        }
                    }

                    corout.ThrowIfCanceledWithReason();
                    
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
                    if (index != 0)
                        return (false);

                    corout.MoveNext();
                }

                bool nxt = false;

                do
                {                    
                    nxt = corout.MoveNext();

                    if (!nxt)
                        this._coroutsToRemove.Enqueue(corout);

                } while (nxt && (corout.Type == ECoroutType.Synchrone));

                if ((corout.Type == ECoroutType.LazyPriority) ||
                    (corout.Type == ECoroutType.Priority))
                {
                    return (false);
                }
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

            this._firstIndex = 0;
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
