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

        private List<Action<Corout>> _callback;        
        private AggregateException   _error;
        private ECoroutStatus        _status;
        private ECoroutType          _type;

        private bool                 _fence;
        private bool                 _throwError;
                

        #region "CTOR"
       
        /// <summary>.</summary>
        internal protected Corout()
        {
            this._callback = new List<Action<Corout>>();
            this._error    = null;
            this._status   = ECoroutStatus.Created;
            this._type     = ECoroutType.Asynchrone;            
            this._fence    = false;

            this._token    = new CoroutToken(this.GetHashCode());
            this._routine  = this.IENull();         
        }

        /// <summary>.</summary> 
        internal protected Corout(IEnumerator routine)
        {            
            this._callback = new List<Action<Corout>>();
            this._error    = null;
            this._status   = ECoroutStatus.Created;
            this._type     = ECoroutType.Asynchrone;            
            this._fence    = false;

            this._token    = new CoroutToken(this.GetHashCode());
            this._routine  = routine;            
        }

        /// <summary>.</summary>
        internal protected Corout(Func<IEnumerator> routine)
        {            
            this._callback = new List<Action<Corout>>();
            this._error    = null;
            this._status   = ECoroutStatus.Created;
            this._type     = ECoroutType.Asynchrone;
            this._fence    = false;
            this._token    = new CoroutToken(this.GetHashCode());

            try
            {               
                this._routine = routine();                
            }
            catch (Exception ex)
            {
                this.AddError(ex);
                this.Complete();
            }            
        }
        
        /// <summary>.</summary>
        internal protected Corout(Func<CoroutToken, IEnumerator> routine)
        {            
            this._callback = new List<Action<Corout>>();
            this._error    = null;
            this._status   = ECoroutStatus.Created;
            this._type     = ECoroutType.Asynchrone;
            this._fence    = false;
            this._token    = new CoroutToken(this.GetHashCode());

            try
            {
                this._routine = routine(this._token);                
            }
            catch (Exception ex)
            {
                this.AddError(ex);
                this.Complete();
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

        internal protected List<Action<Corout>> Callback
        {
            get { return (this._callback); }
            protected set { this._callback = value; }
        }

        internal protected AggregateException Error
        {
            get { return (this._error); }
            protected set { this._error = value; }
        }

        internal protected ECoroutStatus Status
        {
            get { return (this._status); }
            protected set { this._status = value; }
        }

        internal protected ECoroutType Type
        {
            get { return (this._type); }
            protected set { this._type = value; }
        }

        internal protected bool ThrowError
        {
            get { return (this._throwError); }
            protected set { this._throwError = value; }
        }

        internal protected bool IsFence
        {
            get { return (this._fence); }
        }
        

        public bool IsDone
        {
            get { return (this.IsCompleted || this.IsCanceled || this.IsFaulted); }
        }
        
        public bool IsCompleted
        {
            get { return (this._status == ECoroutStatus.RanToCompletion); }
        }

        public bool IsCanceled
        {
            get { return (this._status == ECoroutStatus.Canceled); }
        }

        public bool IsFaulted
        {
            get { return (this._status == ECoroutStatus.Faulted); }
        }
                
        #endregion


        #region "IENUMERATOR | IDISPOSABLE"

        public virtual object Current
        {
            get
            {
                // ? if !(Created || Running)
                if (this.IsCanceled)
                    return (null);

                return ((this._routine == null) ? null : this._routine.Current);
            }
        }

        public virtual bool MoveNext()
        {
            if (this._status == ECoroutStatus.Created)
                this._status = ECoroutStatus.Running;


            if (this._status != ECoroutStatus.Running)
                return (false);

            if (this._routine == null)
                return (false);

            bool hasNext = false;

            try
            {
                AYieldCommand command = this._routine.Current as AYieldCommand;

                if (command != null)
                {
                    if (!command.OnProcess())
                    {
                        return (true);
                    }
                }

                this.Token.CheckCancellation();
                hasNext = (this._routine.MoveNext() && !this._token.IsCanceled);
            }
            catch (Exception ex)
            {
                this.AddError(ex);
                hasNext = false;
            }

            if (!hasNext)
                this.Complete();

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

        #endregion

        
        #region "METHODS"

        public virtual void Start()
        {
            this.Start(this._type, this._throwError);
        }

        public virtual void Start(ECoroutType type)
        {
            this.Start(type, this._throwError);            
        }
                
        public virtual void Start(ECoroutType type, bool forceThrowError)
        {
            var scheduler = Scheduler.Instance;

            if (scheduler == null)
                throw new NullReferenceException("coroutine could not be started.");

            if (this._status != ECoroutStatus.Created)
                throw new Exception("coroutine already scheduled.");

            this._type       = type;
            this._throwError = forceThrowError;
            scheduler.SubmitCorout(this);
        }


        public Corout OnComplete(Action<Corout> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (this.IsDone)
            {
                this.SetStatus();

                if (this._status == ECoroutStatus.Canceled)
                    return (this);
                
                this.Invoke(callback);                               
            }
            else
            {
                this._callback.Add(callback);
            }

            return (this);
        }
        
        public Corout OnError<U>(Action<U, CoroutToken> errorHandler) where U : Exception
        {
            if (errorHandler == null)
                throw new ArgumentNullException("errorHandler");

            return (this.OnComplete(co =>
            {
                if (co._error != null)
                {
                    if (typeof(U) == typeof(AggregateException))
                    {
                        errorHandler(co._error as U, co._token);

                        co._error  = null;
                        co._status = ECoroutStatus.RanToCompletion;                                                
                    }
                    else
                    {
                        foreach (var except in co._error.Exceptions)
                        {
                            var ex = except as U;
                            if (ex != null)
                            {
                                errorHandler(ex, co._token);                                
                                co.RemoveError(ex);
                            }
                        }

                        if (co._error.ExceptionsCount == 0)
                        {
                            co._error  = null;
                            co._status = ECoroutStatus.RanToCompletion;
                        }
                    }
                }
            }));
        }

        public Corout ClearError<U>() where U : Exception
        {
            return (this.OnComplete(co =>
            {
                if (co._error != null)
                {
                    if (typeof(U) == typeof(AggregateException))
                    {                        
                        co._error  = null;
                        co._status = ECoroutStatus.RanToCompletion;
                    }
                    else
                    {
                        foreach (var except in co._error.Exceptions)
                        {
                            var ex = except as U;
                            if (ex != null)
                            {                                
                                co.RemoveError(ex);
                            }
                        }

                        if (co._error.ExceptionsCount == 0)
                        {
                            co._error  = null;
                            co._status = ECoroutStatus.RanToCompletion;
                        }
                    }
                }
            }));
        }
        

        public Corout ContinueWithCorout(Func<Corout> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (new CoroutContinuation(this, continuation));
        }

        public Corout<U> ContinueWithCorout<U>(Func<Corout<U>> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (new CoroutContinuation<U>(this, continuation));
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
        
                
        public void ThrowIfCanceledWithReason()
        {
            if (this._token.IsCanceled && this._token.HasCancelReason)           
            {
                throw (this._token.CancelReason);
            }
        }
                
        public void ForceThrowIfCanceled()
        {
            if (this._token.IsCanceled)
            {
                throw ((this._token.CancelReason != null) ? this._token.CancelReason : new CoroutCanceledException());
            }
        }


        protected void AddError<U>(U ex) where U : Exception
        {            
            var agg = ex as AggregateException;

            if (agg != null)
            {
                if (this._error == null)
                    this._error = agg;
                else
                    this._error.Merge(agg);
            }
            else
            {
                if (this._error == null)
                    this._error = new AggregateException(ex);
                else
                    this._error.Merge(ex);
            }
        }
                
        protected void RemoveError<U>(U ex) where U : Exception
        {
            if (this._error == null)
                return;

            if (ex != null)
            {
                AggregateException agg = ex as AggregateException;

                if (agg != null)
                    this._error.Remove(agg);
                else
                    this._error.Remove(ex);
            }
        }
        
        protected void Complete()
        {
            this.SetStatus();

            if (this._status == ECoroutStatus.Canceled)
                return;

            //

            if (this._callback.Count != 0)
            {
                for (int i = 0; i < this._callback.Count; i++)
                {                    
                    this.Invoke(this._callback[i]);
                    
                    if (this._token.IsCanceled)
                        break;
                }
                
                this._callback.Clear();
            }

            this.SetStatus();           
        }


        private void SetStatus()
        {
            if (this._token.IsCanceled)
                this._status = ECoroutStatus.Canceled;
            else if (this._error != null)
                this._status = ECoroutStatus.Faulted;
            else
                this._status = ECoroutStatus.RanToCompletion;
        }

        private void Invoke(Action<Corout> callback)
        {
            try
            {
                callback(this);
            }
            catch (Exception exception)
            {                
                this.AddError(exception);                
            }
        }

        private IEnumerator IENull()
        {
            yield return null;
        }
        
        #endregion
               
    }
}
