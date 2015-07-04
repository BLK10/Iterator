using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using BLK10.Collections;


namespace BLK10.Iterator
{
    public partial class Corout
    {


        public static Corout Create(Action<Corout> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            var co = new Corout().OnComplete(callback);
            co.MoveNext();

            return (co);            
        }

        public static Corout Create(IEnumerator routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");

            return (new Corout(routine));
        }

        public static Corout Create(Func<IEnumerator> routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");

            return (new Corout(routine));
        }

        public static Corout Create(Func<CoroutToken, IEnumerator> routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");

            return (new Corout(routine));
        }


        public static Corout<T> Create<T>(Action<Corout<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            var co = new Corout<T>().OnComplete(callback);
            co.MoveNext();

            return (co);
        }

        public static Corout<T> Create<T>(Func<Action<T>, IEnumerator> routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");

            return (new Corout<T>(routine));
        }

        public static Corout<T> Create<T>(Func<Action<T>, CoroutToken, IEnumerator> routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");

            return (new Corout<T>(routine));
        }

        
        public static Corout<T> FromResult<T>(T value)
        {
            return (new Corout<T>(value));
        }


        public static Corout WhenAll(params Corout[] coroutines)
        {
            if (coroutines == null)
                throw new ArgumentNullException("coroutines");

            if (coroutines.Length == 0)
                throw new ArgumentException("coroutines array is empty.");

            List<Corout> cor = new List<Corout>(coroutines.Length);
            List<Action> act = new List<Action>(coroutines.Length);

            for (int i = 0; i < coroutines.Length; i++)
            {
                var co = coroutines[i];

                if (co == null)
                    throw new ArgumentException("coroutine could not be null.");

                if ((i + 1) < coroutines.Length)
                {
                    if (Array.FindIndex(coroutines, i + 1, (c) => { return (c.Equals(co)); }) > -1)
                        throw new ArgumentException("coroutines could not be equal.");
                }

                cor.Add(co);
                act.Add(co.Complete);
            }

            return (new CoroutParallelization(cor.ToArray(), act.ToArray()));
        }
       
        public static Corout<Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>>>
            WhenAll<U, V>(Corout<U> corout01, Corout<V> corout02)
        {
            if (corout01 == null)
                throw new ArgumentNullException("corout01");
            if (corout02 == null)
                throw new ArgumentNullException("corout02");

            if (corout01.Equals(corout02))
            {
                throw new ArgumentException("coroutines could not be equal.");
            }

            Action[] act = new Action[2];
            act[0] = corout01.Complete;
            act[1] = corout02.Complete;

            return (new CoroutParallelization<U, V>(corout01, corout02, act));
        }

        public static Corout<Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>, KeyValuePair<W, bool>>>
            WhenAll<U, V, W>(Corout<U> corout01, Corout<V> corout02, Corout<W> corout03)
        {
            if (corout01 == null)
                throw new ArgumentNullException("corout01");
            if (corout02 == null)
                throw new ArgumentNullException("corout02");
            if (corout03 == null)
                throw new ArgumentNullException("corout03");

            if (corout01.Equals(corout02) || corout01.Equals(corout03) || corout02.Equals(corout03))
            {
                throw new ArgumentException("coroutines could not be equal.");
            }

            Action[] act = new Action[3];
            act[0] = corout01.Complete;
            act[1] = corout02.Complete;
            act[2] = corout03.Complete;

            return (new CoroutParallelization<U, V, W>(corout01, corout02, corout03, act));
        }

        public static Corout<Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>, KeyValuePair<W, bool>, KeyValuePair<X, bool>>>
            WhenAll<U, V, W, X>(Corout<U> corout01, Corout<V> corout02, Corout<W> corout03, Corout<X> corout04)
        {
            if (corout01 == null)
                throw new ArgumentNullException("corout01");
            if (corout02 == null)
                throw new ArgumentNullException("corout02");
            if (corout03 == null)
                throw new ArgumentNullException("corout03");
            if (corout04 == null)
                throw new ArgumentNullException("corout04");

            if (corout01.Equals(corout02) || corout01.Equals(corout03) ||
                corout01.Equals(corout04) || corout02.Equals(corout03) ||
                corout02.Equals(corout04) || corout03.Equals(corout04))
            {
                throw new ArgumentException("coroutines could not be equal.");
            }

            Action[] act = new Action[4];
            act[0] = corout01.Complete;
            act[1] = corout02.Complete;
            act[2] = corout03.Complete;
            act[3] = corout04.Complete;

            return (new CoroutParallelization<U, V, W, X>(corout01, corout02, corout03, corout04, act));
        }

        public static Corout<Tuple<KeyValuePair<U, bool>, KeyValuePair<V, bool>, KeyValuePair<W, bool>, KeyValuePair<X, bool>, KeyValuePair<Y, bool>>>
            WhenAll<U, V, W, X, Y>(Corout<U> corout01, Corout<V> corout02, Corout<W> corout03, Corout<X> corout04, Corout<Y> corout05)
        {
            if (corout01 == null)
                throw new ArgumentNullException("corout01");
            if (corout02 == null)
                throw new ArgumentNullException("corout02");
            if (corout03 == null)
                throw new ArgumentNullException("corout03");
            if (corout04 == null)
                throw new ArgumentNullException("corout04");
            if (corout05 == null)
                throw new ArgumentNullException("corout05");

            if (corout01.Equals(corout02) || corout01.Equals(corout03) ||
                corout01.Equals(corout04) || corout01.Equals(corout05) ||
                corout02.Equals(corout03) || corout02.Equals(corout04) ||
                corout02.Equals(corout05) || corout03.Equals(corout04) ||
                corout03.Equals(corout05) || corout04.Equals(corout05))
            {
                throw new ArgumentException("coroutines could not be equal.");
            }

            Action[] act = new Action[5];
            act[0] = corout01.Complete;
            act[1] = corout02.Complete;
            act[2] = corout03.Complete;
            act[3] = corout04.Complete;
            act[4] = corout05.Complete;

            return (new CoroutParallelization<U, V, W, X, Y>(corout01, corout02, corout03, corout04, corout05, act));
        }
        


        public static void AddFence()
        {
            var scheduler = Scheduler.Instance;

            if (scheduler == null)
                throw new NullReferenceException("could not add fence.");
            
            var corout = new Corout() { _fence = true };
            scheduler.SubmitCorout(corout);
        }

        public static void AddFence(Action callBack)
        {
            if (callBack == null)
                throw new ArgumentNullException("callBack");

            var scheduler = Scheduler.Instance;

            if (scheduler == null)
                throw new NullReferenceException("could not add fence.");

            var corout = (new Corout() { _fence = true }).OnComplete(co => callBack());
            scheduler.SubmitCorout(corout);
        }

    }
}
