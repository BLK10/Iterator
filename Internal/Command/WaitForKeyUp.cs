using System;

using UnityEngine;


namespace BLK10.Iterator.Command
{
    public class WaitForKeyUp : AYieldCommand
    {
        private KeyCode _keyCode;
        private string  _keyString;


        public WaitForKeyUp(KeyCode key)
        {            
            if (key == KeyCode.None)
            {
                throw new ArgumentException("keyCode could not be equal to None.");
            }
            
            this._keyCode   = key;
            this._keyString = null;
            this._processed = false;
        }

        public WaitForKeyUp(string key)
        {            
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key could not be null or empty.", "key");
            }
            
            this._keyCode   = KeyCode.None;
            this._keyString = key;
            this._processed = false;
        }


        internal override bool OnProcess()
        {            
            if (this._keyCode == KeyCode.None)
            {
                this._processed = Input.GetKeyUp(this._keyString);
            }
            else
            {
                this._processed = Input.GetKeyUp(this._keyCode);
            }

            return (this._processed);
        }

    }
}
