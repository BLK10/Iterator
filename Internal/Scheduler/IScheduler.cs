using System;


namespace BLK10.Iterator
{    
    public interface IScheduler
    {
        ESchedulerStatus Status { get; }        
        int   CoroutCount  { get; }
        float Second { get; }
        int   Step   { get; }        
        
        void Append(Corout coroutine);
        bool Contains(Corout coroutine);

        void Play();
        void Stop();
        void Pause();
    }
}
