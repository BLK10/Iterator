using System;
using System.Collections;

using BLK10.Iterator;
using UnityEngine;


namespace BLK10.Test.Iterator
{
    [ExecuteInEditMode]
    public class SimpleCoroutine
    {
        public static IEnumerator NStepAsync(int n)
        {
            for (int i = 0; i < n; i++)
            {
                yield return null;
            }
        }

        public static IEnumerator NStepVerboseAsync(int n)
        {
            for (int i = 1; i <= n; i++)
            {
                Debug.Log("- Step: " + i.ToString("00"));
                yield return null;
            }
        }
        
        public static IEnumerator GetSQRFAsync(float x, Action<float> completed)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            var result = SimpleRoutine.GetSQRF(x);
            completed(result);
        }

        public static IEnumerator GetSQRIAsync(int x, Action<int> completed)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            var result = SimpleRoutine.GetSQRI(x);
            completed(result);
        }

        public static IEnumerator GetSQRIVerboseAsync(int x, Action<int> completed)
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.Log("- Step: " + i.ToString("00"));
                yield return null;
            }

            var result = SimpleRoutine.GetSQRI(x);
            completed(result);
        }
        
        public static IEnumerator GetSQRIFaultedAsync(int x, Action<int> completed)
        {
            int i = 0;
            for (; i < 5; ++i)
            {
                yield return null;
            }

            if (i == 5)
            {
                throw new NullReferenceException();
            }

            var result = SimpleRoutine.GetSQRI(x);
            completed(result);
        }
        
        public static IEnumerator GetStringAsync(float x, Action<string> completed)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            var result = SimpleRoutine.GetString(x);
            completed(result);
        }

        public static IEnumerator GetLengthAsync(string s, Action<int> completed)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            var result = SimpleRoutine.GetLength(s);
            completed(result);
        }

        public static IEnumerator FailAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            SimpleRoutine.GetException();
        }

        public static IEnumerator FailAsync(Action<int> completed)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            var result = SimpleRoutine.GetException();
            completed(result);
        }
         
       
        public static IEnumerator CancelableAsync(float x, int n, Action<float> completed, CoroutToken token)
        {
            for (int i = 0; i < n; i++)
            {                                
                yield return null;
            }

            token.Cancel();

            completed(SimpleRoutine.GetSQRF(x));
        }

        public static IEnumerator CancelableAfterAsync(float x, int n, Action<float> completed, CoroutToken token)
        {
            token.CancelAfter(1.15f);

            for (int i = 0; i < n; i++)
            {
                yield return null;
            }

            completed(SimpleRoutine.GetSQRF(x));
        }
        
        /*
        public static IEnumerator GetSQRFTokenAsync(float x, Action<float> completed, CancelToken token)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            var result = SimpleRoutine.GetSQRF(x);
            completed(result);
        }

        public static IEnumerator GetStringTokenAsync(float x, Action<string> completed, CancelToken token)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            var result = SimpleRoutine.GetString(x);
            completed(result);
        }

        public static IEnumerator GetLengthTokenAsync(string s, Action<int> completed, CancelToken token)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            var result = SimpleRoutine.GetLength(s);
            completed(result);
        }
        */

    }
}
