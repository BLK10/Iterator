namespace BLK10.Iterator
{
    public enum ECoroutStatus
    {
        /// <summary>The coroutine has been created but has not yet been scheduled.</summary>
        Created = 0,
        /// <summary>The coroutine has been scheduled.</summary>
        Scheduled,
        /// <summary>The coroutine succeeded.</summary>
        Succeeded,
        /// <summary>The coroutine was canceled.</summary>
        Canceled,
        /// <summary>The coroutine faulted.</summary>
        Faulted
    }
}
