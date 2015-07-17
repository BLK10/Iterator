using System;

using BLK10.Singleton;
using UnityEngine;


namespace BLK10.Iterator.Command
{    
	public class WaitForSecond : AYieldCommand
    {
        private float _strtSecond;
        private float _waitSecond;


        public WaitForSecond(float second)
        {
            this._strtSecond = Scheduler.Instance.Second;
            this._waitSecond = Math.Max(second - 0.015f, 0.0f);            
        }
        
        internal override bool OnProcess()
        {
            return (Scheduler.Instance.Second > (this._strtSecond + this._waitSecond));            
        }
        
	}
}
