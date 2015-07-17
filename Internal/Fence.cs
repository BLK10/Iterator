using System;
using System.Collections;
using System.Runtime.InteropServices;

using BLK10.Iterator.Command;
using UnityEngine;


namespace BLK10.Iterator
{
    public sealed class Fence : IDisposable
    {        
        private int    _startIdx;
        private Action _fnDispose;        
        

        public Fence(int startIndex, Action callback)
        {
            this._startIdx  = startIndex;
            this._fnDispose = callback;            
        }
        
        void IDisposable.Dispose()
        {
            var scheduler = Scheduler.Instance;

            if (scheduler == null)
                throw new NullReferenceException("scheduler could not be null.");
                        
            var fence = Corout.CreateFence(this._startIdx);

            if (this._fnDispose != null)
            {
                fence.OnFinally(co =>
                {
                    if (this._fnDispose != null)
                    {
                        this._fnDispose();
                        this._fnDispose = null;
                    }
                });
            }

            scheduler.Append(fence);
        }

        public void Close()
        {
            (this as IDisposable).Dispose();
        }
        
    }
}
