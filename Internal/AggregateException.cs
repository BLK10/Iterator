using System;
using System.Collections.Generic;
using System.Linq;


namespace BLK10.Iterator
{
    public class AggregateException : Exception
    {

        private Exception[] _exceptions;


        public AggregateException(params Exception[] exceptions)
            : this((IEnumerable<Exception>)exceptions)
        {            
        }

        public AggregateException(IEnumerable<Exception> exceptions)
        {
            if (exceptions == null)
                throw new ArgumentNullException("exceptions");

            this._exceptions = exceptions.ToArray();
        }


        #region "PORPERTIES"
        
        public IEnumerable<Exception> Exceptions
        {
            get { return (this._exceptions.ToArray()); }
        }
                
        public Exception this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this._exceptions.Length))
                    throw new IndexOutOfRangeException("index");

                return (this._exceptions[index]);
            }
        }

        public int Count
        {
            get { return (this._exceptions.Length); }
        }

        public override string Message
        {
            get
            {
                var sb = new System.Text.StringBuilder()
                                        .AppendLine(string.Format("{0} error(s): ", this._exceptions.Length));

                for (int i = 0; i < this._exceptions.Length; i++)
                {
                    sb.AppendLine(this._exceptions[i].Message);
                }

                return (sb.ToString());                
            }
        }

        #endregion


        internal void Merge(AggregateException ex)
        {
            this.Merge(ex._exceptions);
        }

        internal void Merge(params Exception[] exceptions)
        {
            if (exceptions == null)
                throw new ArgumentNullException("exceptions");

            for (int i = 0; i < exceptions.Length; i++)
            {
                var except = exceptions[i];

                if (except != null)
                {
                    CArray.Append(ref this._exceptions, except);
                }
            }
        }

        internal void Remove(AggregateException ex)
        {
            this.Remove(ex._exceptions);
        }

        internal void Remove(params Exception[] exceptions)
        {
            if (exceptions == null)
                throw new ArgumentNullException("exceptions");

            for (int i = 0; i < exceptions.Length; i++)
            {
                var ex = exceptions[i];

                if (ex != null)
                {
                    int idx = -1;
                    do
                    {
                        idx = Array.FindIndex(this._exceptions, (e) => { return (e == ex); });

                        if (idx > -1)
                            CArray.RemoveAt(ref this._exceptions, idx);

                    } while (idx > -1);
                }
            }
        }


        public override string ToString()
        {
            var sb = new System.Text.StringBuilder()
                .AppendLine(base.ToString())
                .AppendLine()
                .AppendLine("inner exceptions:");

            for (int i = 0; i < this._exceptions.Length; i++)
            {
                sb.AppendLine(this._exceptions[i].ToString());
            }

            return (sb.ToString());
        }
                
    }
}
