using System;
using System.Collections.Generic;

using BLK10.Collections;
using UnityEngine;


namespace BLK10.Iterator
{

    internal class CoroutInterlace : Corout
    {
        private Queue<Corout> _queues;


        internal CoroutInterlace(Corout[] coroutines)
        {
            this._queues = new Queue<Corout>();

            for (int i = 0; i < coroutines.Length; i++)
                this._queues.Enqueue(coroutines[i]);
        }


        public override object Current
        {
            get
            {
                if (this._queues == null)
                    return (null);

                if (this._queues.Count == 0)
                    return (null);

                var co = this._queues.Peek();

                if (co == null)
                    return (null);

                return (co.Current);
            }
        }

        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();
                        
            if (!hasNext)
                this.Callback();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._queues == null)
                return (false);

            if (this._queues.Count == 0)
                return (false);

            var co = this._queues.Dequeue();

            if (co == null)
                return (false);

            if (co.MoveNext())
            {
                this._queues.Enqueue(co);
                return (true);
            }

            if (co.Exception != null)
                this.AppendError(co.Exception);
            
            var d = co as IDisposable;
            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            return (this._queues.Count != 0);
        }

    }

    internal class CoroutInterlace<U, V> : Corout<Tuple<U, V>>
    {
        private Queue<Tuple<Corout, int>> _queues;
        private U _result01;
        private V _result02;


        internal CoroutInterlace(Corout<U> corout01, Corout<V> corout02)
        {
            this._queues = new Queue<Tuple<Corout, int>>(2);            
            this._queues.Enqueue(new Tuple<Corout, int>(corout01, 0));            
            this._queues.Enqueue(new Tuple<Corout, int>(corout02, 1));
        }


        public override object Current
        {
            get
            {
                if (this._queues == null)
                    return (null);

                if (this._queues.Count == 0)
                    return (null);

                var tuple = this._queues.Peek();

                if (tuple == null)
                    return (null);

                if (tuple.Item1 == null)
                    return (null);

                return (tuple.Item1.Current);
            }
        }

        public override Tuple<U, V> Result
        {
            get { return (new Tuple<U, V>(this._result01, this._result02)); }
        }

        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();
                        
            if (!hasNext)
                this.Callback();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._queues == null)
                return (false);

            if (this._queues.Count == 0)
                return (false);

            Tuple<Corout, int> tuple;
            do
            {
                tuple = this._queues.Dequeue();
            } while ((tuple == null) && (this._queues.Count != 0));

            if (tuple == null)
                return (false);

            if (tuple.Item1 == null)
                return (false);

            if (tuple.Item1.MoveNext())
            {
                this._queues.Enqueue(tuple);
                return (true);
            }

            if (tuple.Item1.Exception != null)
                this.AppendError(tuple.Item1.Exception);

            this.AssignResult(tuple);

            var d = tuple.Item1 as IDisposable;
            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            return (this._queues.Count != 0);
        }

        private void AssignResult(Tuple<Corout, int> tuple)
        {
            switch (tuple.Item2)
            {
                case 1:
                    {
                        var corout = tuple.Item1 as Corout<V>;
                        this._result02 = (corout != null) ? corout.Result : default(V);
                    }
                    break;

                case 0:
                    {
                        var corout = tuple.Item1 as Corout<U>;
                        this._result01 = (corout != null) ? corout.Result : default(U);
                    }
                    break;
            }
        }

    }

    internal class CoroutInterlace<U, V, W> : Corout<Tuple<U, V, W>>
    {
        private Queue<Tuple<Corout, int>> _queues;
        private U _result01;
        private V _result02;
        private W _result03;


        internal CoroutInterlace(Corout<U> corout01, Corout<V> corout02, Corout<W> corout03)
        {            
            this._queues = new Queue<Tuple<Corout, int>>(3);
            this._queues.Enqueue(new Tuple<Corout, int>(corout01, 0));
            this._queues.Enqueue(new Tuple<Corout, int>(corout02, 1));
            this._queues.Enqueue(new Tuple<Corout, int>(corout03, 2));            
        }


        public override object Current
        {
            get
            {
                if (this._queues == null)
                    return (null);

                if (this._queues.Count == 0)
                    return (null);

                var tuple = this._queues.Peek();

                if (tuple == null)
                    return (null);

                if (tuple.Item1 == null)
                    return (null);

                return (tuple.Item1.Current);
            }
        }

        public override Tuple<U, V, W> Result
        {
            get { return (new Tuple<U, V, W>(this._result01, this._result02, this._result03)); }
        }

        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();
                        
            if (!hasNext)
                this.Callback();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._queues == null)
                return (false);

            if (this._queues.Count == 0)
                return (false);

            Tuple<Corout, int> tuple;
            do
            {
                tuple = this._queues.Dequeue();
            } while ((tuple == null) && (this._queues.Count != 0));

            if (tuple == null)
                return (false);

            if (tuple.Item1 == null)
                return (false);

            if (tuple.Item1.MoveNext())
            {
                this._queues.Enqueue(tuple);
                return (true);
            }

            if (tuple.Item1.Exception != null)
                this.AppendError(tuple.Item1.Exception);

            this.AssignResult(tuple);

            var d = tuple.Item1 as IDisposable;
            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            return (this._queues.Count != 0);
        }

        private void AssignResult(Tuple<Corout, int> tuple)
        {
            switch (tuple.Item2)
            {
                case 2:
                    {
                        var corout = tuple.Item1 as Corout<W>;
                        this._result03 = (corout != null) ? corout.Result : default(W);
                    }
                    break;

                case 1:
                    {
                        var corout = tuple.Item1 as Corout<V>;
                        this._result02 = (corout != null) ? corout.Result : default(V);
                    }
                    break;

                case 0:
                    {
                        var corout = tuple.Item1 as Corout<U>;
                        this._result01 = (corout != null) ? corout.Result : default(U);
                    }
                    break;
            }
        }

    }

    internal class CoroutInterlace<U, V, W, X> : Corout<Tuple<U, V, W, X>>
    {
        private Queue<Tuple<Corout, int>> _queues;
        private U _result01;
        private V _result02;
        private W _result03;
        private X _result04;

        internal CoroutInterlace(Corout<U> corout01, Corout<V> corout02, Corout<W> corout03, Corout<X> corout04)
        {
            this._queues = new Queue<Tuple<Corout, int>>(4);            
            this._queues.Enqueue(new Tuple<Corout, int>(corout01, 0));            
            this._queues.Enqueue(new Tuple<Corout, int>(corout02, 1));            
            this._queues.Enqueue(new Tuple<Corout, int>(corout03, 2));            
            this._queues.Enqueue(new Tuple<Corout, int>(corout04, 3));
        }


        public override object Current
        {
            get
            {
                if (this._queues == null)
                    return (null);

                if (this._queues.Count == 0)
                    return (null);

                var tuple = this._queues.Peek();

                if (tuple == null)
                    return (null);

                if (tuple.Item1 == null)
                    return (null);

                return (tuple.Item1.Current);
            }
        }

        public override Tuple<U, V, W, X> Result
        {
            get { return (new Tuple<U, V, W, X>(this._result01, this._result02, this._result03, this._result04)); }
        }

        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();
                        
            if (!hasNext)
                this.Callback();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._queues == null)
                return (false);

            if (this._queues.Count == 0)
                return (false);

            Tuple<Corout, int> tuple;
            do
            {
                tuple = this._queues.Dequeue();
            } while ((tuple == null) && (this._queues.Count != 0));

            if (tuple == null)
                return (false);

            if (tuple.Item1 == null)
                return (false);

            if (tuple.Item1.MoveNext())
            {
                this._queues.Enqueue(tuple);
                return (true);
            }

            if (tuple.Item1.Exception != null)
                this.AppendError(tuple.Item1.Exception);
                        
            this.AssignResult(tuple);

            var d = tuple.Item1 as IDisposable;
            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            return (this._queues.Count != 0);
        }

        private void AssignResult(Tuple<Corout, int> tuple)
        {
            switch (tuple.Item2)
            {
                case 3:
                    {
                        var corout = tuple.Item1 as Corout<X>;
                        this._result04 = (corout != null) ? corout.Result : default(X);
                    }
                    break;

                case 2:
                    {
                        var corout = tuple.Item1 as Corout<W>;
                        this._result03 = (corout != null) ? corout.Result : default(W);
                    }
                    break;

                case 1:
                    {
                        var corout = tuple.Item1 as Corout<V>;
                        this._result02 = (corout != null) ? corout.Result : default(V);
                    }
                    break;

                case 0:
                    {
                        var corout = tuple.Item1 as Corout<U>;
                        this._result01 = (corout != null) ? corout.Result : default(U);
                    }
                    break;
            }
        }

    }

    internal class CoroutInterlace<U, V, W, X, Y> : Corout<Tuple<U, V, W, X, Y>>
    {
        private Queue<Tuple<Corout, int>> _queues;
        private U _result01;
        private V _result02;
        private W _result03;
        private X _result04;
        private Y _result05;

        internal CoroutInterlace(Corout<U> corout01, Corout<V> corout02, Corout<W> corout03, Corout<X> corout04, Corout<Y> corout05)
        {
            this._queues = new Queue<Tuple<Corout, int>>(5);            
            this._queues.Enqueue(new Tuple<Corout, int>(corout01, 0));            
            this._queues.Enqueue(new Tuple<Corout, int>(corout02, 1));            
            this._queues.Enqueue(new Tuple<Corout, int>(corout03, 2));            
            this._queues.Enqueue(new Tuple<Corout, int>(corout04, 3));            
            this._queues.Enqueue(new Tuple<Corout, int>(corout05, 4));
        }


        public override object Current
        {
            get
            {
                if (this._queues == null)
                    return (null);

                if (this._queues.Count == 0)
                    return (null);

                var tuple = this._queues.Peek();

                if (tuple == null)
                    return (null);

                if (tuple.Item1 == null)
                    return (null);

                return (tuple.Item1.Current);
            }
        }

        public override Tuple<U, V, W, X, Y> Result
        {
            get { return (new Tuple<U, V, W, X, Y>(this._result01, this._result02, this._result03, this._result04, this._result05)); }
        }


        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();
                        
            if (!hasNext)
                this.Callback();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._queues == null)
                return (false);

            if (this._queues.Count == 0)
                return (false);

            Tuple<Corout, int> tuple;
            do
            {
                tuple = this._queues.Dequeue();
            } while ((tuple == null) && (this._queues.Count != 0));

            if (tuple == null)
                return (false);

            if (tuple.Item1 == null)
                return (false);

            if (tuple.Item1.MoveNext())
            {
                this._queues.Enqueue(tuple);
                return (true);
            }

            if (tuple.Item1.Exception != null)
                this.AppendError(tuple.Item1.Exception);

            this.AssignResult(tuple);

            var d = tuple.Item1 as IDisposable;
            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            return (this._queues.Count != 0);
        }

        private void AssignResult(Tuple<Corout, int> tuple)
        {
            switch (tuple.Item2)
            {
                case 4:
                    {
                        var corout = tuple.Item1 as Corout<Y>;
                        this._result05 = (corout != null) ? corout.Result : default(Y);
                    }
                    break;

                case 3:
                    {
                        var corout = tuple.Item1 as Corout<X>;
                        this._result04 = (corout != null) ? corout.Result : default(X);
                    }
                    break;

                case 2:
                    {
                        var corout = tuple.Item1 as Corout<W>;
                        this._result03 = (corout != null) ? corout.Result : default(W);
                    }
                    break;

                case 1:
                    {
                        var corout = tuple.Item1 as Corout<V>;
                        this._result02 = (corout != null) ? corout.Result : default(V);
                    }
                    break;

                case 0:
                    {
                        var corout = tuple.Item1 as Corout<U>;
                        this._result01 = (corout != null) ? corout.Result : default(U);
                    }
                    break;
            }
        }

    }

}
