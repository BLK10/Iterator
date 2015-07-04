using System;
using System.Collections.Generic;

using BLK10.Collections;
using UnityEngine;


namespace BLK10.Iterator
{

    internal class CoroutParallelization : Corout
    {
        private Queue<KeyValuePair<Corout, Action>> _coroutines;
        

        internal CoroutParallelization(Corout[] coroutines, Action[] completes)
        {            
            this._coroutines = new Queue<KeyValuePair<Corout, Action>>();

            for (int i = 0; i < coroutines.Length; i++)
            {
                coroutines[i].Token.Id = this.Token.Id;
                this._coroutines.Enqueue(new KeyValuePair<Corout, Action>(coroutines[i], completes[i]));
            }
        }

                
        public override object Current
        {
            get
            {
                if (this._coroutines == null)
                    return (null);

                if (this._coroutines.Count == 0)
                    return (null);

                var routine = this._coroutines.Peek();

                if (routine.Key == null)
                    return (null);

                return (routine.Key.Current);
            }
        }

        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();

            // call 'WhenAll' corout complete
            if (!hasNext)
                this.Complete();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._coroutines == null)
                return (false);

            if (this._coroutines.Count == 0)
                return (false);

            var routine = this._coroutines.Dequeue();
            
            if (routine.Key == null)
                return (false);

            if (routine.Key.MoveNext())
            {
                this._coroutines.Enqueue(routine);
                return (true);
            }

            if (routine.Key.Error != null)
                this.AddError(routine.Key.Error);

            if (routine.Value != null)
                routine.Value();

            var r = routine.Key as IDisposable;
            if (r != null)
            {
                r.Dispose();
                r = null;
            }

            return (this._coroutines.Count != 0);
        }

    }
   
    internal class CoroutParallelization<U, V>
        : Corout<Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>>>
    {
        private Queue<Tuple<Corout, Action, int>> _coroutines;
        private KeyValuePair<U, bool> _result01;
        private KeyValuePair<V, bool> _result02;        


        internal CoroutParallelization(Corout<U> corout01, Corout<V> corout02, Action[] completes)
        {
            this._coroutines = new Queue<Tuple<Corout, Action, int>>(3);
            corout01.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout01, completes[0], 0));
            corout02.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout02, completes[1], 1));
        }


        public override object Current
        {
            get
            {
                if (this._coroutines == null)
                    return (null);

                if (this._coroutines.Count == 0)
                    return (null);

                var tuple = this._coroutines.Peek();

                if (tuple == null)
                    return (null);

                if (tuple.Item1 == null)
                    return (null);

                return (tuple.Item1.Current);
            }
        }

        public override Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>> Result
        {
            get { return (new Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>>(this._result01, this._result02)); }
        }

        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();

            // call 'WhenAll' corout complete
            if (!hasNext)
                this.Complete();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._coroutines == null)
                return (false);

            if (this._coroutines.Count == 0)
                return (false);

            Tuple<Corout, Action, int> tuple;
            do
            {
                tuple = this._coroutines.Dequeue();
            } while ((tuple == null) && (this._coroutines.Count != 0));

            if (tuple == null)
                return (false);

            if (tuple.Item1 == null)
                return (false);

            if (tuple.Item1.MoveNext())
            {
                this._coroutines.Enqueue(tuple);
                return (true);
            }

            if (tuple.Item1.Error != null)
                this.AddError(tuple.Item1.Error);

            if (tuple.Item2 != null)
                tuple.Item2();

            this.AssignResult(tuple);

            var d = tuple.Item1 as IDisposable;
            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            return (this._coroutines.Count != 0);
        }

        private void AssignResult(Tuple<Corout, Action, int> tuple)
        {
            switch (tuple.Item3)
            {                
                case 1:
                    {
                        var corout = tuple.Item1 as Corout<V>;
                        if (corout != null)
                        {
                            this._result02 = new KeyValuePair<V, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result02 = new KeyValuePair<V, bool>(default(V), false);
                        }
                    }
                    break;

                case 0:
                    {
                        var corout = tuple.Item1 as Corout<U>;
                        if (corout != null)
                        {
                            this._result01 = new KeyValuePair<U, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result01 = new KeyValuePair<U, bool>(default(U), false);
                        }
                    }
                    break;
            }
        }

    }
    
    internal class CoroutParallelization<U, V, W>
        : Corout<Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>, KeyValuePair<W, bool>>>
    {
        private Queue<Tuple<Corout, Action, int>> _coroutines;
        private KeyValuePair<U, bool> _result01;
        private KeyValuePair<V, bool> _result02;
        private KeyValuePair<W, bool> _result03;
        

        internal CoroutParallelization(Corout<U> corout01, Corout<V> corout02, Corout<W> corout03, Action[] completes)
        {
            this._coroutines = new Queue<Tuple<Corout, Action, int>>(3);
            corout01.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout01, completes[0], 0));
            corout02.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout02, completes[1], 1));
            corout03.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout03, completes[2], 2));            
        }


        public override object Current
        {
            get
            {
                if (this._coroutines == null)
                    return (null);

                if (this._coroutines.Count == 0)
                    return (null);

                var tuple = this._coroutines.Peek();

                if (tuple == null)
                    return (null);

                if (tuple.Item1 == null)
                    return (null);

                return (tuple.Item1.Current);
            }
        }

        public override Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>, KeyValuePair<W, bool>> Result
        {
            get
            {
                return (new Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>,
                                  KeyValuePair<W, bool>>(this._result01, this._result02, this._result03));
            }
        }

        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();

            // call 'WhenAll' corout complete
            if (!hasNext)
                this.Complete();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._coroutines == null)
                return (false);

            if (this._coroutines.Count == 0)
                return (false);

            Tuple<Corout, Action, int> tuple;
            do
            {
                tuple = this._coroutines.Dequeue();
            } while ((tuple == null) && (this._coroutines.Count != 0));

            if (tuple == null)
                return (false);

            if (tuple.Item1 == null)
                return (false);

            if (tuple.Item1.MoveNext())
            {
                this._coroutines.Enqueue(tuple);
                return (true);
            }

            if (tuple.Item1.Error != null)
                this.AddError(tuple.Item1.Error);

            if (tuple.Item2 != null)
                tuple.Item2();

            this.AssignResult(tuple);

            var d = tuple.Item1 as IDisposable;
            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            return (this._coroutines.Count != 0);
        }

        private void AssignResult(Tuple<Corout, Action, int> tuple)
        {
            switch (tuple.Item3)
            {                
                case 2:
                    {
                        var corout = tuple.Item1 as Corout<W>;
                        if (corout != null)
                        {
                            this._result03 = new KeyValuePair<W, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result03 = new KeyValuePair<W, bool>(default(W), false);
                        }
                    }
                    break;

                case 1:
                    {
                        var corout = tuple.Item1 as Corout<V>;
                        if (corout != null)
                        {
                            this._result02 = new KeyValuePair<V, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result02 = new KeyValuePair<V, bool>(default(V), false);
                        }
                    }
                    break;

                case 0:
                    {
                        var corout = tuple.Item1 as Corout<U>;
                        if (corout != null)
                        {
                            this._result01 = new KeyValuePair<U, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result01 = new KeyValuePair<U, bool>(default(U), false);
                        }
                    }
                    break;
            }
        }

    }
    
    internal class CoroutParallelization<U, V, W, X>
        : Corout<Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>, KeyValuePair<W, bool>, KeyValuePair<X, bool>>>
    {
        private Queue<Tuple<Corout, Action, int>> _coroutines;
        private KeyValuePair<U, bool> _result01;
        private KeyValuePair<V, bool> _result02;
        private KeyValuePair<W, bool> _result03;
        private KeyValuePair<X, bool> _result04;

        internal CoroutParallelization(Corout<U> corout01, Corout<V> corout02, Corout<W> corout03, Corout<X> corout04, Action[] completes)
        {
            this._coroutines = new Queue<Tuple<Corout, Action, int>>(3);
            corout01.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout01, completes[0], 0));
            corout02.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout02, completes[1], 1));
            corout03.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout03, completes[2], 2));
            corout04.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout04, completes[3], 3));
        }


        public override object Current
        {
            get
            {
                if (this._coroutines == null)
                    return (null);

                if (this._coroutines.Count == 0)
                    return (null);

                var tuple = this._coroutines.Peek();

                if (tuple == null)
                    return (null);

                if (tuple.Item1 == null)
                    return (null);

                return (tuple.Item1.Current);
            }
        }

        public override Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>,
                              KeyValuePair<W, bool>, KeyValuePair<X, bool>> Result
        {
            get
            {
                return (new Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>,
                                  KeyValuePair<W, bool>, KeyValuePair<X, bool>>
                                  (this._result01, this._result02, this._result03, this._result04));
            }
        }

        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();

            // call 'WhenAll' corout complete
            if (!hasNext)
                this.Complete();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._coroutines == null)
                return (false);

            if (this._coroutines.Count == 0)
                return (false);

            Tuple<Corout, Action, int> tuple;
            do
            {
                tuple = this._coroutines.Dequeue();
            } while ((tuple == null) && (this._coroutines.Count != 0));

            if (tuple == null)
                return (false);

            if (tuple.Item1 == null)
                return (false);

            if (tuple.Item1.MoveNext())
            {
                this._coroutines.Enqueue(tuple);
                return (true);
            }

            if (tuple.Item1.Error != null)
                this.AddError(tuple.Item1.Error);

            if (tuple.Item2 != null)
                tuple.Item2();

            this.AssignResult(tuple);

            var d = tuple.Item1 as IDisposable;
            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            return (this._coroutines.Count != 0);
        }

        private void AssignResult(Tuple<Corout, Action, int> tuple)
        {
            switch (tuple.Item3)
            {
                case 3:
                    {
                        var corout = tuple.Item1 as Corout<X>;
                        if (corout != null)
                        {
                            this._result04 = new KeyValuePair<X, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result04 = new KeyValuePair<X, bool>(default(X), false);
                        }
                    }
                    break;

                case 2:
                    {
                        var corout = tuple.Item1 as Corout<W>;
                        if (corout != null)
                        {
                            this._result03 = new KeyValuePair<W, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result03 = new KeyValuePair<W, bool>(default(W), false);
                        }
                    }
                    break;

                case 1:
                    {
                        var corout = tuple.Item1 as Corout<V>;
                        if (corout != null)
                        {
                            this._result02 = new KeyValuePair<V, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result02 = new KeyValuePair<V, bool>(default(V), false);
                        }
                    }
                    break;

                case 0:
                    {
                        var corout = tuple.Item1 as Corout<U>;
                        if (corout != null)
                        {
                            this._result01 = new KeyValuePair<U, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result01 = new KeyValuePair<U, bool>(default(U), false);
                        }
                    }
                    break;
            }
        }

    }
    
    internal class CoroutParallelization<U, V, W, X, Y>
        : Corout<Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>, KeyValuePair<W, bool>, KeyValuePair<X, bool>, KeyValuePair<Y, bool>>>
    {
        private Queue<Tuple<Corout, Action, int>> _coroutines;
        private KeyValuePair<U, bool> _result01;
        private KeyValuePair<V, bool> _result02;
        private KeyValuePair<W, bool> _result03;
        private KeyValuePair<X, bool> _result04;
        private KeyValuePair<Y, bool> _result05;

        internal CoroutParallelization(Corout<U> corout01, Corout<V> corout02, Corout<W> corout03, Corout<X> corout04, Corout<Y> corout05, Action[] completes)
        {
            this._coroutines = new Queue<Tuple<Corout, Action, int>>(3);
            corout01.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout01, completes[0], 0));
            corout02.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout02, completes[1], 1));
            corout03.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout03, completes[2], 2));
            corout04.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout04, completes[3], 3));
            corout05.Token.Id = this.Token.Id;
            this._coroutines.Enqueue(new Tuple<Corout, Action, int>(corout05, completes[4], 4));
        }


        public override object Current
        {
            get
            {
                if (this._coroutines == null)
                    return (null);

                if (this._coroutines.Count == 0)
                    return (null);

                var tuple = this._coroutines.Peek();

                if (tuple == null)
                    return (null);

                if (tuple.Item1 == null)
                    return (null);

                return (tuple.Item1.Current);
            }
        }

        public override Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>,
                              KeyValuePair<W, bool>, KeyValuePair<X, bool>,
                              KeyValuePair<Y, bool>> Result
        {
            get
            {
                return (new Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>,
                                  KeyValuePair<W, bool>, KeyValuePair<X, bool>, KeyValuePair<Y, bool>>
                                  (this._result01, this._result02, this._result03, this._result04, this._result05));
            }
        }


        public override bool MoveNext()
        {
            bool hasNext = this.InternalMoveNext();

            // call 'WhenAll' corout complete
            if (!hasNext)
                this.Complete();

            return (hasNext);
        }


        private bool InternalMoveNext()
        {
            if (this._coroutines == null)
                return (false);

            if (this._coroutines.Count == 0)
                return (false);

            Tuple<Corout, Action, int> tuple;
            do
            {
                tuple = this._coroutines.Dequeue();
            } while ((tuple == null) && (this._coroutines.Count != 0));

            if (tuple == null)
                return (false);

            if (tuple.Item1 == null)
                return (false);

            if (tuple.Item1.MoveNext())
            {
                this._coroutines.Enqueue(tuple);
                return (true);
            }

            if (tuple.Item1.Error != null)
                this.AddError(tuple.Item1.Error);

            if (tuple.Item2 != null)
                tuple.Item2();

            this.AssignResult(tuple);

            var d = tuple.Item1 as IDisposable;
            if (d != null)
            {
                d.Dispose();
                d = null;
            }

            return (this._coroutines.Count != 0);
        }

        private void AssignResult(Tuple<Corout, Action, int> tuple)
        {
            switch (tuple.Item3)
            {
                case 4:
                    {
                        var corout = tuple.Item1 as Corout<Y>;
                        if (corout != null)
                        {
                            this._result05 = new KeyValuePair<Y, bool>(corout.Result, corout.IsCompleted);                           
                            corout = null;
                        }
                        else
                        {
                            this._result05 = new KeyValuePair<Y, bool>(default(Y), false);                            
                        }
                    }
                    break;

                case 3:
                    {
                        var corout = tuple.Item1 as Corout<X>;
                        if (corout != null)
                        {
                            this._result04 = new KeyValuePair<X, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result04 = new KeyValuePair<X, bool>(default(X), false);
                        }
                    }
                    break;

                case 2:
                    {
                        var corout = tuple.Item1 as Corout<W>;
                        if (corout != null)
                        {
                            this._result03 = new KeyValuePair<W, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result03 = new KeyValuePair<W, bool>(default(W), false);
                        }
                    }
                    break;

                case 1:
                    {
                        var corout = tuple.Item1 as Corout<V>;
                        if (corout != null)
                        {
                            this._result02 = new KeyValuePair<V, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result02 = new KeyValuePair<V, bool>(default(V), false);
                        }
                    }
                    break;

                case 0:
                    {
                        var corout = tuple.Item1 as Corout<U>;
                        if (corout != null)
                        {
                            this._result01 = new KeyValuePair<U, bool>(corout.Result, corout.IsCompleted);
                            corout = null;
                        }
                        else
                        {
                            this._result01 = new KeyValuePair<U, bool>(default(U), false);
                        }
                    }
                    break;
            }
        }

    }

}
