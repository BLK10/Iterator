using System;

using BLK10.Singleton;
using UnityEngine;


namespace BLK10.Iterator.Command
{
    public class WaitForStep : AYieldCommand
    {
        private int _strtStep;
        private int _waitStep;


        public WaitForStep(int _step)
        {            
            this._strtStep  = int.MinValue;
            this._waitStep  = Math.Max(_step, 0);
            this._processed = false;
        }        
        
        internal override bool OnProcess()
        {            
            int step = Scheduler.Instance.Step;

            if (this._strtStep < 0)
            {
                this._strtStep = step;
            }
            else
            {
                if (step >= (this._strtStep + this._waitStep))
                {
                    this._processed = true;                    
                }
            }

            return (this._processed);
        } 
    }
}