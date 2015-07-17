using System;

using UnityEngine;


namespace BLK10.Iterator.Command
{
    public class WaitForKeyUp : AYieldCommand
    {
        private KeyCode _keyCode;
        

        public WaitForKeyUp(KeyCode key)
        {            
            if (key == KeyCode.None)
                throw new ArgumentException("keyCode could not be equal to None.");
            
            this._keyCode = key;
        }
        
        internal override bool OnProcess()
        {
            return (Input.GetKeyUp(this._keyCode));
        }

    }
}
