using System;


namespace BLK10.Iterator
{

    internal class CoroutRelay : Corout
    {
        private Corout       _coroutine;        
        private Func<Corout> _continuation;
        

        internal CoroutRelay(Corout coroutine, Func<Corout> continuation)
        {            
            this._coroutine    = coroutine;            
            this._continuation = continuation;
        }


        public override object Current
        {
            get
            {
                if (this._coroutine != null)
                    return (this._coroutine.Current);

                return (null);
            }
        }

        public override bool MoveNext()
        {
            if (this._coroutine == null)
                return (false);
            
            if (this._coroutine.MoveNext())
                return (true);

            if (this._coroutine.Exception != null)
            {
                this.AppendError(this._coroutine.Exception);
                this.Resume();
                return (false);
            }

            if (this._coroutine.Token.IsCanceled)
            {
                this.Token.CancelError(this._coroutine.Token.CancelException);                
                this.Resume();
                return (false);                
            }

            if (this._continuation != null)
            {
                this.DisposeCurrent();                
                this._coroutine    = this._continuation();                
                this._continuation = null;
                return (true);
            }
            
            this.Resume();
            return (false);
        }


        private void DisposeCurrent()
        {
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

        private void Resume()
        {            
            this.Callback();
            this.DisposeCurrent();

            if (this._continuation != null)
            {
                var d = this._continuation() as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                    d = null;
                }
                this._continuation = null;
            }
        }

    }

    internal class CoroutRelay<U> : Corout<U>
    {
        private Corout          _coroutine;        
        private Func<Corout<U>> _continuation;


        internal CoroutRelay(Corout coroutine, Func<Corout<U>> continuation)
        {
            this._coroutine    = coroutine;            
            this._continuation = continuation;
        }


        public override object Current
        {
            get
            {
                if (this._coroutine != null)
                    return (this._coroutine.Current);

                return (null);
            }
        }

        public override U Result
        {
            get { return (base.Result); }
        }

        public override bool MoveNext()
        {
            if (this._coroutine == null)
                return (false);

            if (this._coroutine.MoveNext())
                return (true);

            if (this._coroutine.Token.IsCanceled)
            {
                this.Token.Cancel();
                this.Resume();
                return (false);
            }

            if (this._coroutine.Exception != null)
            {
                this.AppendError(this._coroutine.Exception);
                this.AssignResult();                
                this.Resume();
                return (false);
            }

            if (this._continuation != null)
            {
                this.DisposeCurrent();                
                this._coroutine    = this._continuation();                
                this._continuation = null;
                return (true);
            }

            this.AssignResult();            
            this.Resume();
            return (false);
        }


        private void AssignResult()
        {            
            var co = (this._coroutine as Corout<U>);
            if (co != null)
                base.Result = (co.Result);
            else
                base.Result = (default(U));            
        }

        private void DisposeCurrent()
        {
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
        
        private void Resume()
        {
            this.Callback();
            this.DisposeCurrent();

            if (this._continuation != null)
            {
                var d = this._continuation() as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                    d = null;
                }
                this._continuation = null;
            }
        }

    }    

}
