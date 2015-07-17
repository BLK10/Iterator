using System;

using UnityEngine;


namespace BLK10.Iterator.Command
{
    public class WaitForMouseUp : AYieldCommand
    {
        private int _mouseButton;


        public WaitForMouseUp(int button)
        {
            this._mouseButton = Math.Min(Math.Max(button, 0), 2);            
        }

        internal override bool OnProcess()
        {            
            return (Input.GetMouseButtonUp(this._mouseButton));
        }

    }
}
