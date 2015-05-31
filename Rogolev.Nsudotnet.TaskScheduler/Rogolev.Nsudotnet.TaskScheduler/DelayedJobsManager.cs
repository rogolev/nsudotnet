using System;
using System.Collections.Concurrent;
using Timer = System.Timers.Timer;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal class DelayedJobsManager : JobsManager<TimeSpan>
    {
        public DelayedJobsManager(BlockingCollection<JobInfo<TimeSpan>> delayedJobsCollection) : base(delayedJobsCollection)
        {
        }

        protected override void DoJobManagement(JobInfo<TimeSpan> jobInfo)
        {
            IJob job = jobInfo.Job;
            TimeSpan delay = jobInfo.Info;
            object argument = jobInfo.Argument;
            Timer timer = new Timer(delay.TotalMilliseconds);

            Timers.Add(timer);
            timer.Elapsed += (source, e) =>
            {
                if (source.Equals(timer))
                {
                    try
                    {
                        timer.Stop();
                        Timers.Remove(timer);
                    }
                    finally
                    {
                        timer.Dispose();
                    }
                    job.Execute(argument);
                }
            };
            timer.Start();
        }
    }
}
