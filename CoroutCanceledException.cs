using System;
using System.Runtime.Serialization;


namespace BLK10.Iterator
{
    [Serializable]
    public class CoroutCanceledException : OperationCanceledException
    {        
        public CoroutCanceledException() { } 
       
        public CoroutCanceledException(string message)
            : base(message) { }

        public CoroutCanceledException(string message, Exception innerException)
            : base(message, innerException) { }

        protected CoroutCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
