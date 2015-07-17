using System;
using System.Collections;
using System.Collections.Generic;

using BLK10.Collections;
using BLK10.Iterator.Command;
using UnityEngine;

namespace BLK10.Iterator
{
    public partial class Corout : IEnumerator, IDisposable
    {
        private IEnumerator          _routine;
        private CoroutToken          _token;

        private List<Action<Corout>> _succeed;
        private List<Action<Corout>> _cancel;
        private List<Action<Corout>> _catch;
        private List<Action<Corout>> _finally;

        private AggregateException   _exception;
        private ECoroutStatus        _status;
        private bool                 _synchron;
        private EAsyncMode           _asyncMode;        
        private int                  _fenceIdx;
               

        #region "CTOR"
       
        /// <summary>.</summary>
        internal protected Corout()
        {
            this._succeed = new List<Action<Corout>>();
            this._cancel  = new List<Action<Corout>>();
            this._catch   = new List<Action<Corout>>();
            this._finally = new List<Action<Corout>>();

            this._exception = null;
            this._status    = ECoroutStatus.Created;
            this._synchron  = false;
            this._asyncMode = EAsyncMode.Normal;
            this._fenceIdx  = -1;
            this._token     = new CoroutToken();

            this._routine = this.IENull();         
        }

        /// <summary>.</summary> 
        internal protected Corout(IEnumerator routine)
        {
            this._succeed = new List<Action<Corout>>();
            this._cancel  = new List<Action<Corout>>();
            this._catch   = new List<Action<Corout>>();
            this._finally = new List<Action<Corout>>();

            this._exception = null;
            this._status    = ECoroutStatus.Created;
            this._synchron  = false;
            this._asyncMode = EAsyncMode.Normal;           
            this._fenceIdx  = -1;
            this._token     = new CoroutToken();

            this._routine = routine;            
        }

        /// <summary>.</summary>
        internal protected Corout(Func<IEnumerator> routine)
        {
            this._succeed = new List<Action<Corout>>();
            this._cancel  = new List<Action<Corout>>();
            this._catch   = new List<Action<Corout>>();
            this._finally = new List<Action<Corout>>();

            this._exception = null;
            this._status    = ECoroutStatus.Created;
            this._synchron  = false;
            this._asyncMode = EAsyncMode.Normal;
            this._fenceIdx  = -1;
            this._token     = new CoroutToken();

            try
            {               
                this._routine = routine();                
            }
            catch (Exception ex)
            {
                this.AppendError(ex);
                this.Callback();
            }            
        }
        
        /// <summary>.</summary>
        internal protected Corout(Func<CoroutToken, IEnumerator> routine)
        {
            this._succeed = new List<Action<Corout>>();
            this._cancel  = new List<Action<Corout>>();
            this._catch   = new List<Action<Corout>>();
            this._finally = new List<Action<Corout>>();

            this._exception = null;
            this._status    = ECoroutStatus.Created;
            this._synchron  = false;
            this._asyncMode = EAsyncMode.Normal;
            this._fenceIdx  = -1;
            this._token     = new CoroutToken();

            try
            {
                this._routine = routine(this._token);                
            }
            catch (Exception ex)
            {
                this.AppendError(ex);
                this.Callback();
            } 
        }

        #endregion
                

        #region "PROPERTIES"

        internal protected IEnumerator Routine
        {
            get { return (this._routine); }
            protected set { this._routine = value; }
        }

        internal protected CoroutToken Token
        {
            get { return (this._token); }
            protected set { this._token = value; }
        }
        
        internal protected AggregateException Exception
        {
            get { return (this._exception); }
            protected set { this._exception = value; }
        }

        internal protected ECoroutStatus Status
        {
            get { return (this._status); }
            protected set { this._status = value; }
        }
        
        internal protected bool IsSynchronous
        {
            get { return (this._synchron); }
            protected set { this._synchron = value; }
        }
                
        internal protected EAsyncMode AsyncMode
        {
            get { return (this._asyncMode); }
            protected set { this._asyncMode = value; }
        }
        
        internal protected bool IsFence
        {
            get { return (this._fenceIdx >= 0); }
        }

        internal protected int FenceIndex
        {
            get { return (this._fenceIdx); }
        }
        

        public bool IsDone
        {
            get { return (this.Succeeded || this.Canceled || this.Faulted); }
        }
        
        public bool Succeeded
        {
            get { return (this._status == ECoroutStatus.Succeeded); }
        }

        public bool Canceled
        {
            get { return (this._status == ECoroutStatus.Canceled); }
        }

        public bool Faulted
        {
            get { return (this._status == ECoroutStatus.Faulted); }
        }
                
        #endregion


        #region "IENUMERATOR | IDISPOSABLE"

        public virtual object Current
        {
            get
            {
                if ((this._status != ECoroutStatus.Created) || (this._status != ECoroutStatus.Scheduled))
                    return (null);

                return ((this._routine == null) ? null : this._routine.Current);
            }
        }

        public virtual bool MoveNext()
        {
            if (this._status == ECoroutStatus.Created)
                this._status = ECoroutStatus.Scheduled;

            if (this._status != ECoroutStatus.Scheduled)
                return (false);
                        
            bool hasNext = false;

            try
            {
                AYieldCommand command = this._routine.Current as AYieldCommand;

                if (command != null)
                {
                    if (!command.OnProcess())
                        return (true);
                }

                hasNext  = this._routine.MoveNext();
                hasNext &= !this.IsTokenCanceled;            
            }
            catch (Exception ex)
            {
                this.AppendError(ex);
                hasNext = false;
            }

            if (!hasNext)
                this.Callback();

            return (hasNext);
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            var d = this._routine as IDisposable;

            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            this._routine = null;
        }

        private bool IsTokenCanceled
        {
            get
            {                
                if ((!this._token.IsCanceled) && (!this._token.IsCanceledError))                
                {
                    if (this._token.IsPendingCancelTimeout)                    
                    {
                        if ((Scheduler.Instance.Second - this._token.EndTimeout) > -float.Epsilon)
                            this._token.Cancel();
                    }
                    else if (this._token.IsPendingCancelStep)
                    {
                        if (Scheduler.Instance.Step >= this._token.EndStep)
                            this._token.Cancel();
                    }
                }

                if (this._token.IsCanceledError)
                    this.AppendError(this._token.CancelException);

                return ((this._token.IsCanceled) || (this._token.IsCanceledError));
            }
        }

        #endregion

        
        #region "METHODS"
        
        public void Start()
        {
            this.Start(false);
        }

        public void Start(bool synchronous)
        {
            var scheduler = Scheduler.Instance;

            if (scheduler == null)
                throw new NullReferenceException("Scheduler.");

            if (this._status != ECoroutStatus.Created)
                throw new Exception("coroutine already started.");
                        
            this._synchron = synchronous;            
            scheduler.Append(this);
        }

        internal void Start(EAsyncMode asynchronousMode)
        {
            var scheduler = Scheduler.Instance;

            if (scheduler == null)
                throw new NullReferenceException("Scheduler.");

            if (this._status != ECoroutStatus.Created)
                throw new Exception("coroutine already started.");
            
            this._synchron  = false;              
            this._asyncMode = asynchronousMode;
            scheduler.Append(this);
        }

        // Abort corout without calling cancel callback, but execute final statement
        public void Abort()
        {
            if ((this._status == ECoroutStatus.Created) || (this._status == ECoroutStatus.Scheduled))
            {
                this._status = ECoroutStatus.Canceled;

                for (int i = 0; i < this._finally.Count; i++)
                    this._finally[i](this);

                this._finally.Clear();
            }
        }


        public Corout OnSucceed(Action<Corout> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (this._status == ECoroutStatus.Succeeded)
                callback(this); 
            else
                this._succeed.Add(callback);

            return (this);
        }

        public Corout OnCancel(Action<Corout> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (this._status == ECoroutStatus.Canceled)
                callback(this);
            else
                this._cancel.Add(callback);

            return (this);
        }
        
        public Corout OnCatch<U>(Action<U, Corout> errorHandler) where U : Exception
        {
            if (errorHandler == null)
                throw new ArgumentNullException("errorHandler");

            Action<Corout> callback =
                co =>
                {
                    if (co._exception != null)
                    {
                        if (typeof(U) == typeof(AggregateException))
                        {                            
                            co._exception = null;                            
                            errorHandler(co._exception as U, co);                            
                        }
                        else
                        {
                            for (int i = co.Exception.Count - 1; i >= 0 ; i--)
                            {
                                var ex = co.Exception[i] as U;
                                if (ex != null)
                                {
                                    co.RemoveError(ex);

                                    if (co._exception.Count == 0)
                                        co._exception = null;

                                    errorHandler(ex, co);
                                }
                            }                            
                        }
                    }
                };
            
            
            if (this._status == ECoroutStatus.Faulted)
                this.Invoke(callback);
            else
                this._catch.Add(callback);
            

            return (this);
        }

        public Corout OnFinally(Action<Corout> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (this.IsDone)
                callback(this);
            else
                this._finally.Add(callback);

            return (this);
        }


        public Corout ContinueWithCorout(Func<Corout> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (new CoroutRelay(this, continuation));
        }

        public Corout<U> ContinueWithCorout<U>(Func<Corout<U>> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (new CoroutRelay<U>(this, continuation));
        }
        
        public Corout ContinueWith(Func<IEnumerator> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (this.ContinueWithCorout(() => new Corout(continuation())));
        }
                
        public Corout<U> ContinueWith<U>(Func<Action<U>, IEnumerator> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (this.ContinueWithCorout(() => new Corout<U>(continuation)));
        }
                

        protected void AppendError<U>(U ex) where U : Exception
        {            
            var agg = ex as AggregateException;

            if (agg != null)
            {
                if (this._exception == null)
                    this._exception = agg;
                else
                    this._exception.Merge(agg);
            }
            else
            {
                if (this._exception == null)
                    this._exception = new AggregateException(ex);
                else
                    this._exception.Merge(ex);
            }
        }
                
        protected void RemoveError<U>(U ex) where U : Exception
        {
            if (this._exception == null)
                return;

            if (ex != null)
            {
                AggregateException agg = ex as AggregateException;

                if (agg != null)
                    this._exception.Remove(agg);
                else
                    this._exception.Remove(ex);
            }
        }
        
        protected void Callback()
        {
            this.SetStatus();

            switch (this._status)
            {
                case ECoroutStatus.Faulted:
                    for (int i = 0; i < this._catch.Count; i++)
                        this.Invoke(this._catch[i]);                        

                    this._catch.Clear();
                    break;

                case ECoroutStatus.Canceled:
                    for (int i = 0; i < this._cancel.Count; i++)
                        this._cancel[i](this);

                    this._cancel.Clear();
                    break;

                case ECoroutStatus.Succeeded:
                default:
                    for (int i = 0; i < this._succeed.Count; i++)
                        this._succeed[i](this);

                    this._succeed.Clear();
                    break;
            }


            {
                for (int i = 0; i < this._finally.Count; i++)
                    this._finally[i](this);

                this._finally.Clear();
            }
        }


        private void Invoke(Action<Corout> callback)
        {
            try
            {
                callback(this);
            }
            catch (Exception ex)
            {
                this.AppendError(ex);
            }
        }
        
        private void SetStatus()
        {
            if (this._exception != null)
                this._status = ECoroutStatus.Faulted;
            else if (this._token.IsCanceled)
                this._status = ECoroutStatus.Canceled;
            else
                this._status = ECoroutStatus.Succeeded;           
        }
        
        private IEnumerator IENull()
        {
            yield return null;
        }
                
        #endregion

    }
}
