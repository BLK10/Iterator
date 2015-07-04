using System;

using UnityEngine;


namespace BLK10.Iterator.Command
{
    public class WaitForMouseDown : AYieldCommand
    {
        private int _mouseButton;


        public WaitForMouseDown(int button)
        {            
            if ((button < 0) || (button > 2))
            {
                throw new ArgumentOutOfRangeException("mouse button.");
            }
           
            this._mouseButton = button;
            this._processed   = false;
        }
        
        internal override bool OnProcess()
        {            
            this._processed = Input.GetMouseButtonDown(this._mouseButton);
            
            return (this._processed);
        }
    }
}
