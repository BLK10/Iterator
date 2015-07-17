using System;

using UnityEngine;


namespace BLK10.Iterator.Command
{
    public class WaitForMouseDown : AYieldCommand
    {
        private int _mouseButton;


        public WaitForMouseDown(int button)
        {
            this._mouseButton = Math.Min(Math.Max(button, 0), 2);            
        }
        
        internal override bool OnProcess()
        {
            return (Input.GetMouseButtonDown(this._mouseButton));
        }
    }
}
