using System;


namespace BLK10.Iterator
{    
    public interface IScheduler
    {
        ESchedulerStatus Status { get; }        
        int   Count  { get; }
        float Second { get; }
        int   Step   { get; }        
        
        void SubmitCorout(Corout coroutine);
        
        void Play();
        void Stop();
        void Pause();
    }
}
