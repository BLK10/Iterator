using System;

using BLK10.Singleton;
using UnityEngine;


namespace BLK10.Iterator.Command
{
    public class WaitForStep : AYieldCommand
    {
        private int _strtStep;
        private int _waitStep;


        public WaitForStep(int step)
        {
            this._strtStep  = Scheduler.Instance.Step;
            this._waitStep  = Math.Max(step, 0);
        }        
        
        internal override bool OnProcess()
        {
            return (Scheduler.Instance.Step >= (this._strtStep + this._waitStep));
        } 
    }
}