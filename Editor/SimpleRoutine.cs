using System;

using UnityEngine;


namespace BLK10.Test.Iterator
{
    [ExecuteInEditMode]
    public static class SimpleRoutine
    {

        public static float GetSQRF(float x)
        {
            return (x * x);
        }

        public static int GetSQRI(int x)
        {
            return (x * x);
        }

        public static string GetString(float x)
        {
            return (x.ToString());
        }

        public static int GetLength(string s)
        {
            return (s.Length);
        }

        public static int GetException()
        {
            throw new NotSupportedException();
        }

    }
}
