using System;

using UnityEngine;


namespace BLK10.Iterator.Command
{
    public class WaitForKeyDown : AYieldCommand
    {       
        private KeyCode _keyCode;
        
        public WaitForKeyDown(KeyCode key)
        {
            if (key == KeyCode.None)
                throw new ArgumentException("keyCode could not be equal to None.");
            
            this._keyCode = key;            
        }
                
        internal override bool OnProcess()
        {
            return (Input.GetKeyDown(this._keyCode));
        }        
    }
}
