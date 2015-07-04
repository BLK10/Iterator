using System;
using System.Collections;
using System.Collections.Generic;

using BLK10.Collections;


namespace BLK10.Iterator
{   
    public class Corout<T> : Corout
    {
        private T _result;


        #region "CTOR"

        internal protected Corout()
            : base()
        {
        }

        internal protected Corout(T result)
            : base()
        {
            this._result = result;
        }

        internal protected Corout(Func<Action<T>, IEnumerator> routine)
            : base()
        {            
            try
            {
                this.Routine = routine(r => { this._result = r; });
            }
            catch (Exception ex)
            {
                this.AddError(ex);
                this.Complete();
            }         
        }

        internal protected Corout(Func<Action<T>, CoroutToken, IEnumerator> routine)
            : base()
        {
            try
            {
                this.Routine = routine(r => { this._result = r; }, this.Token);
            }
            catch (Exception ex)
            {
                this.AddError(ex);
                this.Complete();
            }            
        }

        #endregion
        
                
        public virtual T Result
        {
            get { return (this._result); }
            protected set { this._result = value; }
        }


        #region "METHODS"

        public override void Start()
        {
            this.Start(this.Type, this.ThrowError);
        }

        public override void Start(ECoroutType type)
        {
            this.Start(type, this.ThrowError);
        }

        public override void Start(ECoroutType type, bool forceThrowError)
        {
            var scheduler = Scheduler.Instance;

            if (scheduler == null)
                throw new NullReferenceException("Coroutine could not be started.");

            if (this.Status != ECoroutStatus.Created)
                throw new Exception("Coroutine already scheduled.");

            this.Type       = type;
            this.ThrowError = forceThrowError;
            scheduler.SubmitCorout(this);
        }


        public Corout<T> OnComplete(Action<Corout<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            base.OnComplete(co => callback(this));
            return (this);
        }
       
        public new Corout<T> OnError<U>(Action<U, CoroutToken> errorHandler) where U : Exception
        {
            if (errorHandler == null)
                throw new ArgumentNullException("errorHandler");

            return (this.OnComplete(co =>
            {
                if (co.Error != null)
                {                    
                    if (typeof(U) == typeof(AggregateException))
                    {
                        errorHandler(co.Error as U, co.Token);

                        co.Error  = null;
                        co.Status = ECoroutStatus.RanToCompletion;                      
                    }
                    else
                    {
                        foreach (var except in co.Error.Exceptions)
                        {
                            var ex = except as U;
                            if (ex != null)
                            {
                                errorHandler(ex, co.Token);                                
                                co.RemoveError(ex);
                            }
                        }

                        if (co.Error.ExceptionsCount == 0)
                        {
                            co.Error  = null;
                            co.Status = ECoroutStatus.RanToCompletion;
                        }                      
                    }
                }
            }));
        }

        public new Corout<T> ClearError<U>() where U : Exception
        {            
            return (this.OnComplete(co =>
            {
                if (co.Error != null)
                {
                    if (typeof(U) == typeof(AggregateException))
                    {                        
                        co.Error = null;
                        co.Status = ECoroutStatus.RanToCompletion;
                    }
                    else
                    {
                        foreach (var except in co.Error.Exceptions)
                        {
                            var ex = except as U;
                            if (ex != null)
                            {                                
                                co.RemoveError(ex);
                            }
                        }

                        if (co.Error.ExceptionsCount == 0)
                        {
                            co.Error = null;
                            co.Status = ECoroutStatus.RanToCompletion;
                        }
                    }
                }
            }));
        }


        public Corout ContinueWithCorout(Func<T, Corout> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (new CoroutContinuation(this, () =>
            {
                var res = this.Result;                               
                return (continuation(res));
            }));
        }

        public Corout<U> ContinueWithCorout<U>(Func<T, Corout<U>> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (new CoroutContinuation<U>(this, () =>
            {
                var res = this.Result;
                return (continuation(res));
            }));
        }
        
        public Corout ContinueWith(Func<T, IEnumerator> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (this.ContinueWithCorout(() => new Corout(continuation(this.Result))));
        }
                
        public Corout<U> ContinueWith<U>(Func<T, Action<U>, IEnumerator> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            Func<T, Corout<U>> f = x => new Corout<U>(a => continuation(x, a));

            return (this.ContinueWithCorout<U>(f));
        }
        
        #endregion

        
    }    
}
