namespace BLK10.Iterator
{
    public enum ECoroutType
    {
        /// <summary>Normal.</summary>
        Asynchrone = 0,
        /// <summary>Fifo, coroutine is queued.</summary>
        LazyPriority,
        /// <summary>Lifo. coroutine is stacked.</summary>
        Priority,
        /// <summary>Blocking.</summary>
        Synchrone
    };
}