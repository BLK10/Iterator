using System;
using System.Diagnostics;


namespace BLK10.Iterator
{
    public class CoroutToken
    {        
        private bool      _canceled;
        private float     _endTimeout;
        private int       _endStep;
        private Exception _cancelException;
        
               
        public CoroutToken()
        {            
            this._canceled   = false;
            this._endTimeout = float.MinValue;
            this._endStep    = int.MinValue;
            this._cancelException = null;            
        }


        public bool IsCanceled
        {
            get { return ((this._canceled) && (this._cancelException == null)); }
        }
        
        public bool IsCanceledError
        {
            get { return (this._canceled && (this._cancelException != null)); }
        }

        public Exception CancelException
        {
            get { return (this._cancelException); }
        }
               
        
        internal float EndTimeout
        {
            get { return (this._endTimeout); }            
        }

        internal float EndStep
        {
            get { return (this._endStep); }            
        }

        internal bool IsPendingCancelTimeout
        {
            get { return (this._endTimeout > float.Epsilon); }           
        }

        internal bool IsPendingCancelStep
        {
            get { return (this._endStep > 0); }            
        }
        
                
        public void Cancel()
        {            
            this._canceled    = true;                       
            this._endTimeout  = float.MinValue;
            this._endStep     = int.MinValue;
        }
                
        public void CancelError(Exception exception)
        {
            this._canceled    = true;
            this._cancelException = exception;
            this._endTimeout  = float.MinValue;
            this._endStep     = int.MinValue;            
        }
                        
        public void CancelAfter(float second)
        {
            if ((!this._canceled) && (!this.IsPendingCancelStep))
            {
                if (second < 0.001f)
                    this._canceled   = true;
                else
                    this._endTimeout = Scheduler.Instance.Second + second;
            }
        }
                
        public void CancelAfter(int step)
        {
            if ((!this._canceled) && (!this.IsPendingCancelTimeout))
            {
                if (step <= 0)
                    this._canceled = true;
                else
                    this._endStep  = Scheduler.Instance.Step + step;
            }
        }

        public void CancelErrorAfter(float second, Exception exception)
        {
            if ((!this._canceled) && (!this.IsPendingCancelStep))
            {
                if (second < 0.001f)
                {
                    this._canceled    = true;
                    this._cancelException = exception;
                }
                else
                {
                    this._endTimeout      = Scheduler.Instance.Second + second;
                    this._cancelException = exception;
                }
            }
        }
        
        public void CancelErrorAfter(int step, Exception exception)
        {
            if ((!this._canceled) && (!this.IsPendingCancelTimeout))
            {
                if (step <= 0)
                {
                    this._canceled        = true;
                    this._cancelException = exception;
                }
                else
                {
                    this._endStep         = Scheduler.Instance.Step + step;
                    this._cancelException = exception;
                }
            }
        }
        
    }
}
