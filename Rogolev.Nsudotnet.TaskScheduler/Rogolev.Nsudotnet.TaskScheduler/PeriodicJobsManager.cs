using System;
using System.Collections.Concurrent;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal class PeriodicJobsManager : JobsManager<TimeSpan>
    {
        public PeriodicJobsManager(BlockingCollection<JobInfo<TimeSpan>> periodicJobsCollection) : base(periodicJobsCollection)
        {
        }
        override protected void DoJobManagement(JobInfo<TimeSpan> jobInfo)
        {
            IJob job = jobInfo.Job;
            TimeSpan delay = jobInfo.Info;
            object argument = jobInfo.Argument;
            Timer timer = new Timer(delay.TotalMilliseconds);
            timer.Elapsed += (source, elapsedEventArgs) => job.Execute(argument);
            Timers.Add(timer);
            timer.AutoReset = true;
            timer.Start();
        }
    }
}
