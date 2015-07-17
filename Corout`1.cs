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
        
        internal protected Corout(Func<Action<T>, IEnumerator> routine)
            : base()
        {            
            try
            {
                this.Routine = routine(r => { this._result = r; });
            }
            catch (Exception ex)
            {
                this.AppendError(ex);
                this.Callback();
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
                this.AppendError(ex);
                this.Callback();
            }            
        }

        #endregion
                        
        public virtual T Result
        {
            get { return (this._result); }
            protected set { this._result = value; }
        }
        
        #region "METHODS"
        
        public Corout<T> OnSucceed(Action<Corout<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            base.OnSucceed(co => callback(this));
            return (this);
        }

        public Corout<T> OnCancel(Action<Corout<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            base.OnCancel(co => callback(this));
            return (this);
        }

        public Corout<T> OnCatch<U>(Action<U, Corout<T>> errorHandler) where U : Exception
        {
            if (errorHandler == null)
                throw new ArgumentNullException("errorHandler");

            base.OnCatch<U>((ex, co) => errorHandler(ex, this));
            
            return (this);
        }

        public Corout<T> OnFinally(Action<Corout<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            base.OnFinally(co => callback(this));
            return (this);
        }


        public Corout ContinueWithCorout(Func<T, Corout> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (new CoroutRelay(this, () =>
            {
                var res = this.Result;                               
                return (continuation(res));
            }));
        }

        public Corout<U> ContinueWithCorout<U>(Func<T, Corout<U>> continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            return (new CoroutRelay<U>(this, () =>
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
