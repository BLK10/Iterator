using System;
using System.Diagnostics;


namespace BLK10.Iterator
{
    public class CoroutToken
    {        
        private int       _id;
        private bool      _canceled;
        private float     _endTimeout;
        private int       _endStep;
        private Exception _cancelReason;
        
               
        public CoroutToken(int hashCode)
        {
            this._id           = hashCode;
            this._canceled     = false;
            this._endTimeout   = float.MinValue;
            this._endStep      = int.MinValue;
            this._cancelReason = null;            
        }


        public bool IsCanceled
        {
            get { return (this._canceled); }
        }
        
        public bool HasCancelReason
        {
            get { return (this._cancelReason != null); }
        }
                        

        internal int Id
        {
            get { return (this._id); }
            set { this._id = value; }
        }
        
        internal Exception CancelReason
        {
            get { return (this._cancelReason); }
        }
        

        private bool CancelTimeout
        {
            get { return (this._endTimeout > float.Epsilon); }
        }

        private bool CancelStep
        {
            get { return (this._endStep > 0); }
        }


        internal void CheckCancellation()
        {
            if (!this._canceled)
            {
                if (this.CancelTimeout)
                {
                    if ((Scheduler.Instance.Second - this._endTimeout) > -float.Epsilon)
                    {
                        this._canceled   = true;
                        this._endTimeout = float.MinValue;
                    }
                }
                else if (this.CancelStep)
                {
                    if (Scheduler.Instance.Step >= this._endStep)
                    {
                        this._canceled = true;
                        this._endStep  = int.MinValue;
                    }
                }
            }
        }
       

        // cancel silently (= no reason)
        public void Cancel()
        {            
            this._canceled     = true;
            this._cancelReason = null;            
        }
        
        // cancel by throwing an error
        public void Cancel(Exception exception)
        {
            this._canceled     = true;
            this._cancelReason = exception;            
        }


        // cancel silently after an amount of time
        public void CancelAfter(float second)
        {
            if ((!this._canceled) && (!this.CancelTimeout) && (!this.CancelStep))
                this._endTimeout = Scheduler.Instance.Second + Math.Max(second, 0f);
        }
        
        // cancel by throwing an error after an amount of time
        public void CancelAfter(float second, Exception exception)
        {
            if ((!this._canceled) && (!this.CancelTimeout) && (!this.CancelStep))
            {
                this._endTimeout   = Scheduler.Instance.Second + Math.Max(second, 0f);
                this._cancelReason = exception;
            }
        }
        

        // cancel silently after a number of step
        public void CancelAfter(int step)
        {
            if ((!this._canceled) && (!this.CancelTimeout) && (!this.CancelStep))
                this._endStep = Scheduler.Instance.Step + Math.Max(step, 0);
        }

        // cancel by throwing an error after a number of step
        public void CancelAfter(int step, Exception exception)
        {
            if ((!this._canceled) && (!this.CancelTimeout) && (!this.CancelStep))
            {
                this._endStep      = Scheduler.Instance.Step + Math.Max(step, 0);
                this._cancelReason = exception;
            }
        }
        
    }
}
