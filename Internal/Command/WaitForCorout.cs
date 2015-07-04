using System;
using System.Collections;

using UnityEngine;


namespace BLK10.Iterator.Command
{
    /// <summary>Synchronous / Blocking coroutine</summary>
    public class WaitForCorout : AYieldCommand
    {
        private Corout _coroutine;
        
        public WaitForCorout(IEnumerator routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");

            this._coroutine = new Corout(routine).OnComplete(co => this.Complete());            
            this._processed = false;
        }

        public WaitForCorout(Func<IEnumerator> routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");

            this._coroutine = new Corout(routine).OnComplete(co => this.Complete());            
            this._processed = false;
        }
        

        private void Complete()
        {
            this._processed = true;
            
            if (this._coroutine != null)
            {
                var d = this._coroutine as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                    d = null;
                }                
            }
        }

        internal override bool OnProcess()
        {
            if (this._coroutine != null)
            {
                this._coroutine.MoveNext();
            }

            return (this._processed);            
        }

    }
    
    /// <summary>Synchronous / Blocking coroutine</summary>
    public class WaitForCorout<T> : AYieldCommand
    {
        private Corout<T> _coroutine;

        public WaitForCorout(Func<Action<T>, IEnumerator> routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");

            this._coroutine = new Corout<T>(routine).OnComplete(co => this.Complete());
            this._processed = false;
        }
        
        
        private void Complete()
        {
            this._processed = true;

            if (this._coroutine != null)
            {
                var d = this._coroutine as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                    d = null;
                }
            }
        }

        internal override bool OnProcess()
        {
            if (this._coroutine != null)
            {
                this._coroutine.MoveNext();
            }

            return (this._processed);
        }

    }
}
