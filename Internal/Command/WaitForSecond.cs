using System;

using BLK10.Singleton;
using UnityEngine;


namespace BLK10.Iterator.Command
{    
	public class WaitForSecond : AYieldCommand
    {
        private float _strtSecond;
        private float _waitSecond;


        public WaitForSecond(float _second)
        {            
            this._strtSecond = float.MinValue;
            this._waitSecond = Math.Max(_second, 0.0f);
            this._processed  = false;
        }
        
        internal override bool OnProcess()
        {
            float second = Scheduler.Instance.Second;

            if (this._strtSecond < -0.0001f)
            {
                this._strtSecond = second;
            }
            else
            {
                if (second >= (this._strtSecond + this._waitSecond))
                {   
                    this._processed = true;                    
                }
            }

            return (this._processed);
        }
        
	}
}
