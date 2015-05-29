using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal class CronJobsManager : JobsManager<string>
    {
        private readonly BlockingCollection<JobInfo<string>> _unmanagedJobs;
        private readonly BlockingCollection<JobInfo<TimeSpan>> _jobsForDelayedJobsManager;
        //private HashSet<Timer> timers;
        private DelayedJobsManager _delayedJobsManager;
        /*private bool _isActive;
        private Mutex _mutex;*/
        private Thread _delayedJobsManagerThread;

        public CronJobsManager(BlockingCollection<JobInfo<string>> cronJobsCollection) : base(cronJobsCollection)
        {
            _jobsForDelayedJobsManager = new BlockingCollection<JobInfo<TimeSpan>>();
            _delayedJobsManager = new DelayedJobsManager(_jobsForDelayedJobsManager);
            _delayedJobsManagerThread = new Thread(_delayedJobsManager.ManageJobs);
            _delayedJobsManagerThread.Start();
        }

        protected override void DoJobManagement(JobInfo<string> jobInfo)
        {
            CronExpression cronExpression = new CronExpression(jobInfo.Info);
            var currentDateTime = DateTime.Now;
            TimeSpan delayBeforeNextJob = cronExpression.GetNexDateTime(currentDateTime.AddMinutes(1)) - currentDateTime;
            _jobsForDelayedJobsManager.Add(new JobInfo<TimeSpan>(jobInfo.Job, delayBeforeNextJob, jobInfo.Argument));
            Timer timer = new Timer(delayBeforeNextJob.TotalMilliseconds);
            Timers.Add(timer);
            timer.Elapsed += (sender, args) =>
            {
                if (sender.Equals(timer))
                {
                    ((Timer)sender).Stop();
                    Timers.Remove(((Timer)sender));
                    DoJobManagement(jobInfo);
                }
            };
            timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_delayedJobsManager != null)
                    _delayedJobsManager.Dispose();
                _delayedJobsManager = null;
            }
        }
    }
}
