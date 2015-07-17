using System;
using System.Collections;

using BLK10.Collections;
using BLK10.Iterator;
using BLK10.Iterator.Command;

using UnityEngine;


namespace BLK10.Iterator.Test
{
    public class Routines
    {        
        public static IEnumerator NStep(int step)
        {
            //for (int i = 0; i < stepCnt; i++)
                //yield return null;
            yield return new WaitForStep(step);
        }

        public static IEnumerator NSecond(float second)
        {
            yield return new WaitForSecond(second);
        }
        
        public static IEnumerator NStepVerbose(int stepCnt)
        {
            for (int i = 0; i < stepCnt; i++)
            {
                Debug.Log("- Step: " + i.ToString("00"));
                yield return null;
            }
        }
                
        public static IEnumerator GetIntSquareRoot(int x, Action<int> retrun)
        {
            for (int i = 0; i < 5; i++)
                yield return null;

            retrun(x * x);
        }

        public static IEnumerator GetFloatSquareRoot(float x, Action<float> retrun)
        {
            for (int i = 0; i < 5; i++)
                yield return null;

            retrun(x * x);
        }

        public static IEnumerator GetIntString(int x, Action<string> retrun)
        {
            for (int i = 0; i < 5; i++)
                yield return null;

            retrun(x.ToString());
        }
        
        public static IEnumerator GetFloatString(float x, Action<string> retrun)
        {
            for (int i = 0; i < 5; i++)
                yield return null;
            
            retrun(x.ToString());
        }

        public static IEnumerator GetLength(string s, Action<int> retrun)
        {
            for (int i = 0; i < 5; i++)
                yield return null;
                       
            retrun(s.Length);
        }

        public static IEnumerator JoinString(Tuple<int, float, string> tuple, Action<string> retrun)
        {
            for (int i = 0; i < 5; i++)
                yield return null;

            retrun(string.Join(" ", new string[] { tuple.Item1.ToString(), tuple.Item2.ToString(), tuple.Item3 }));
        }

        public static IEnumerator Failing(int x, Action<int> retrun)
        {
            for (int i = 0; i < 5; i++)
                yield return null;
            
            throw new NotSupportedException();

            // Unreachable code detected
#pragma warning disable 0162
            retrun(x * x);
#pragma warning restore 0162
        }

        public static IEnumerator Cancelable(float x, Action<float> retrun, CoroutToken token)
        {
            for (int i = 0; i < 5; i++)
                yield return null;

            token.Cancel();
            retrun(x * x);
        }

        public static IEnumerator CancelableAfter(float second, float x, Action<float> retrun, CoroutToken token)
        {
            token.CancelAfter(second);

            for (int i = 0; i < 1000; i++)
                yield return null;

            retrun(x * x);
        }

        public static IEnumerator CancelableAfter(int step, float x, Action<float> retrun, CoroutToken token)
        {
            token.CancelAfter(step);

            for (int i = 0; i < 1000; i++)
                yield return null;

            retrun(x * x);
        }

        

    }
}
