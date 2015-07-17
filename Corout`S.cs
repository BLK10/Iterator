using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using BLK10.Collections;
using BLK10.Iterator.Command;


namespace BLK10.Iterator
{
    public partial class Corout
    {
        public static Corout Create(Action<Corout> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            var co = new Corout().OnSucceed(callback);
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

            var co = new Corout<T>().OnSucceed(callback);
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

        

        public static Corout WhenAll(params Func<IEnumerator>[] routines)
        {
            if (routines == null)
                throw new ArgumentNullException("routines");
            if (routines.Length == 0)
                throw new ArgumentException("routines array is empty.");

            List<Corout> lCorouts = new List<Corout>(routines.Length);

            for (int i = 0; i < routines.Length; i++)
            {
                var rout = routines[i];

                if (rout == null)
                    throw new ArgumentException("routine could not be null.");

                if ((i + 1) < routines.Length)
                {
                    if (Array.FindIndex(routines, i + 1, (r) => { return (r.Equals(rout)); }) > -1)
                        throw new ArgumentException("routines could not be equal.");
                }

                lCorouts.Add(new Corout(rout));                
            }

            return (new CoroutInterlace(lCorouts.ToArray()));
        }

        public static Corout WhenAll(Func<CoroutToken, IEnumerator>[] routines)
        {
            if (routines == null)
                throw new ArgumentNullException("routines");
            if (routines.Length == 0)
                throw new ArgumentException("routines array is empty.");

            List<Corout> lCorouts = new List<Corout>(routines.Length);

            for (int i = 0; i < routines.Length; i++)
            {
                var rout = routines[i];

                if (rout == null)
                    throw new ArgumentException("routine could not be null.");

                if ((i + 1) < routines.Length)
                {
                    if (Array.FindIndex(routines, i + 1, (r) => { return (r.Equals(rout)); }) > -1)
                        throw new ArgumentException("routines could not be equal.");
                }

                lCorouts.Add(new Corout(rout));
            }

            return (new CoroutInterlace(lCorouts.ToArray()));
        }

        public static Corout<Tuple<U, V>>
            WhenAll<U, V>(Func<Action<U>, IEnumerator> rout01, Func<Action<V>, IEnumerator> rout02)
        {
            if (rout01 == null)
                throw new ArgumentNullException("rout01");
            if (rout02 == null)
                throw new ArgumentNullException("rout02");

            if (rout01.Equals(rout02))
                throw new ArgumentException("routines could not be equal.");

            var corout01 = new Corout<U>(rout01);
            var corout02 = new Corout<V>(rout02);

            return (new CoroutInterlace<U, V>(corout01, corout02));
        }

        public static Corout<Tuple<U, V>>
            WhenAll<U, V>(Func<Action<U>, CoroutToken, IEnumerator> rout01, Func<Action<V>, CoroutToken, IEnumerator> rout02)
        {
            if (rout01 == null)
                throw new ArgumentNullException("rout01");
            if (rout02 == null)
                throw new ArgumentNullException("rout02");

            if (rout01.Equals(rout02))
                throw new ArgumentException("routines could not be equal.");

            var corout01 = new Corout<U>(rout01);
            var corout02 = new Corout<V>(rout02);

            return (new CoroutInterlace<U, V>(corout01, corout02));
        }

        public static Corout<Tuple<U, V, W>>
            WhenAll<U, V, W>(Func<Action<U>, IEnumerator> rout01, Func<Action<V>, IEnumerator> rout02, Func<Action<W>, IEnumerator> rout03)
        {
            if (rout01 == null)
                throw new ArgumentNullException("rout01");
            if (rout02 == null)
                throw new ArgumentNullException("rout02");
            if (rout03 == null)
                throw new ArgumentNullException("rout03");

            if (rout01.Equals(rout02) || rout01.Equals(rout03) || rout02.Equals(rout03))
                throw new ArgumentException("routines could not be equal.");

            var corout01 = new Corout<U>(rout01);
            var corout02 = new Corout<V>(rout02);
            var corout03 = new Corout<W>(rout03);

            return (new CoroutInterlace<U, V, W>(corout01, corout02, corout03));
        }

        public static Corout<Tuple<U, V, W>>
            WhenAll<U, V, W>(Func<Action<U>, CoroutToken, IEnumerator> rout01, Func<Action<V>, CoroutToken, IEnumerator> rout02, Func<Action<W>, CoroutToken, IEnumerator> rout03)
        {
            if (rout01 == null)
                throw new ArgumentNullException("rout01");
            if (rout02 == null)
                throw new ArgumentNullException("rout02");
            if (rout03 == null)
                throw new ArgumentNullException("rout03");

            if (rout01.Equals(rout02) || rout01.Equals(rout03) || rout02.Equals(rout03))
                throw new ArgumentException("routines could not be equal.");

            var corout01 = new Corout<U>(rout01);
            var corout02 = new Corout<V>(rout02);
            var corout03 = new Corout<W>(rout03);

            return (new CoroutInterlace<U, V, W>(corout01, corout02, corout03));
        }
        
        public static Corout<Tuple<U, V, W, X>>
            WhenAll<U, V, W, X>(Func<Action<U>, IEnumerator> rout01, Func<Action<V>, IEnumerator> rout02,
                                Func<Action<W>, IEnumerator> rout03, Func<Action<X>, IEnumerator> rout04)
        {
            if (rout01 == null)
                throw new ArgumentNullException("rout01");
            if (rout02 == null)
                throw new ArgumentNullException("rout02");
            if (rout03 == null)
                throw new ArgumentNullException("rout03");
            if (rout04 == null)
                throw new ArgumentNullException("rout04");

            if (rout01.Equals(rout02) || rout01.Equals(rout03) ||
                rout01.Equals(rout04) || rout02.Equals(rout03) ||
                rout02.Equals(rout04) || rout03.Equals(rout04))
            {
                throw new ArgumentException("routines could not be equal.");
            }

            var corout01 = new Corout<U>(rout01);
            var corout02 = new Corout<V>(rout02);
            var corout03 = new Corout<W>(rout03);
            var corout04 = new Corout<X>(rout04);

            return (new CoroutInterlace<U, V, W, X>(corout01, corout02, corout03, corout04));
        }

        public static Corout<Tuple<U, V, W, X>>
            WhenAll<U, V, W, X>(Func<Action<U>, CoroutToken, IEnumerator> rout01, Func<Action<V>, CoroutToken, IEnumerator> rout02,
                                Func<Action<W>, CoroutToken, IEnumerator> rout03, Func<Action<X>, CoroutToken, IEnumerator> rout04)
        {
            if (rout01 == null)
                throw new ArgumentNullException("rout01");
            if (rout02 == null)
                throw new ArgumentNullException("rout02");
            if (rout03 == null)
                throw new ArgumentNullException("rout03");
            if (rout04 == null)
                throw new ArgumentNullException("rout04");

            if (rout01.Equals(rout02) || rout01.Equals(rout03) ||
                rout01.Equals(rout04) || rout02.Equals(rout03) ||
                rout02.Equals(rout04) || rout03.Equals(rout04))
            {
                throw new ArgumentException("routines could not be equal.");
            }

            var corout01 = new Corout<U>(rout01);
            var corout02 = new Corout<V>(rout02);
            var corout03 = new Corout<W>(rout03);
            var corout04 = new Corout<X>(rout04);

            return (new CoroutInterlace<U, V, W, X>(corout01, corout02, corout03, corout04));
        }

        public static Corout<Tuple<U, V, W, X, Y>>
            WhenAll<U, V, W, X, Y>(Func<Action<U>, IEnumerator> rout01, Func<Action<V>, IEnumerator> rout02,
                                   Func<Action<W>, IEnumerator> rout03, Func<Action<X>, IEnumerator> rout04, Func<Action<Y>, IEnumerator> rout05)
        {
            if (rout01 == null)
                throw new ArgumentNullException("rout01");
            if (rout02 == null)
                throw new ArgumentNullException("rout02");
            if (rout03 == null)
                throw new ArgumentNullException("rout03");
            if (rout04 == null)
                throw new ArgumentNullException("rout04");
            if (rout05 == null)
                throw new ArgumentNullException("rout05");

            if (rout01.Equals(rout02) || rout01.Equals(rout03) ||
                rout01.Equals(rout04) || rout01.Equals(rout05) ||
                rout02.Equals(rout03) || rout02.Equals(rout04) ||
                rout02.Equals(rout05) || rout03.Equals(rout04) ||
                rout03.Equals(rout05) || rout04.Equals(rout05))
            {
                throw new ArgumentException("routines could not be equal.");
            }

            var corout01 = new Corout<U>(rout01);
            var corout02 = new Corout<V>(rout02);
            var corout03 = new Corout<W>(rout03);
            var corout04 = new Corout<X>(rout04);
            var corout05 = new Corout<Y>(rout05);

            return (new CoroutInterlace<U, V, W, X, Y>(corout01, corout02, corout03, corout04, corout05));
        }

        public static Corout<Tuple<U, V, W, X, Y>>
            WhenAll<U, V, W, X, Y>(Func<Action<U>, CoroutToken, IEnumerator> rout01, Func<Action<V>, CoroutToken, IEnumerator> rout02,
                                   Func<Action<W>, CoroutToken, IEnumerator> rout03, Func<Action<X>, CoroutToken, IEnumerator> rout04,
                                   Func<Action<Y>, CoroutToken, IEnumerator> rout05)
        {
            if (rout01 == null)
                throw new ArgumentNullException("rout01");
            if (rout02 == null)
                throw new ArgumentNullException("rout02");
            if (rout03 == null)
                throw new ArgumentNullException("rout03");
            if (rout04 == null)
                throw new ArgumentNullException("rout04");
            if (rout05 == null)
                throw new ArgumentNullException("rout05");

            if (rout01.Equals(rout02) || rout01.Equals(rout03) ||
                rout01.Equals(rout04) || rout01.Equals(rout05) ||
                rout02.Equals(rout03) || rout02.Equals(rout04) ||
                rout02.Equals(rout05) || rout03.Equals(rout04) ||
                rout03.Equals(rout05) || rout04.Equals(rout05))
            {
                throw new ArgumentException("routines could not be equal.");
            }

            var corout01 = new Corout<U>(rout01);
            var corout02 = new Corout<V>(rout02);
            var corout03 = new Corout<W>(rout03);
            var corout04 = new Corout<X>(rout04);
            var corout05 = new Corout<Y>(rout05);

            return (new CoroutInterlace<U, V, W, X, Y>(corout01, corout02, corout03, corout04, corout05));
        }


        public static Fence OpenFence()
        {
            return (new Fence(Scheduler.Instance.CoroutCount, null));
        }

        public static Fence OpenFence(Action onClose)
        {
            return (new Fence(Scheduler.Instance.CoroutCount, onClose));
        }
        
        internal static Corout CreateFence(int startIndex)
        {            
            return (new Corout() { _fenceIdx = startIndex });            
        }

                
        public static void WaitThen(int waitStep, Action then)
        {
            if (waitStep <= 0)
                then();
            else                
                using (Corout.OpenFence())
                    Corout.Create(Corout.IEWait(waitStep - 1)).OnFinally(co => then()).Start();
        }

        public static void WaitThen(float waitSecond, Action then)
        {
            if (waitSecond < 0.015f)
                then();
            else                
                using (Corout.OpenFence())
                    Corout.Create(Corout.IEWait(Scheduler.Instance.Second + waitSecond)).OnFinally(co => then()).Start();
        }


        private static IEnumerator IEWait(int step)
        {            
            for (int i = 0; i < step; i++)
                yield return null;
        }

        private static IEnumerator IEWait(float second)
        {
            while ((second - Scheduler.Instance.Second) > -float.Epsilon)
                yield return null;

        }

    }
}
