using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
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
            _mutex.GetAccessControl();
            IJob job = jobInfo.Job;
            TimeSpan delay = jobInfo.Info;
            object argument = jobInfo.Argument;
            Timer timer = new Timer(delay.TotalMilliseconds);
            _timers.Add(timer);
            timer.Elapsed += (source, e) =>
            {
                if (source.Equals(timer))
                {
                    try
                    {
                        timer.Stop();
                        _timers.Remove(timer);
                        ThreadPool.QueueUserWorkItem(job.Execute, argument);
                    }
                    finally
                    {
                        timer.Dispose();
                    }
                }
            };
            timer.Start();
        }
    }
}
