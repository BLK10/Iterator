namespace BLK10.Iterator
{
    public enum ECoroutStatus
    {
        /// <summary>The coroutine has been initialized but has not yet been scheduled.</summary>
        Created,
        /// <summary>The coroutine is running but has not yet completed.</summary>
        Running,
        /// <summary>The coroutine completed execution successfully.</summary>
        RanToCompletion,        
        /// <summary>The coroutine acknowledged cancellation.</summary>
        Canceled,
        /// <summary>The coroutine completed due to an unhandled exception.</summary>
        Faulted
    }
}
