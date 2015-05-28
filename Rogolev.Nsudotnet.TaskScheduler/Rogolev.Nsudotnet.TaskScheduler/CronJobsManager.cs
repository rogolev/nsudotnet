using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal class CronJobsManager : IDisposable
    {
        private readonly BlockingCollection<JobInfo<string>> _unmanagedJobs;
        private readonly BlockingCollection<JobInfo<TimeSpan>> _jobsForDelayedJobsManager;
        private HashSet<Timer> timers;
        private DelayedJobsManager _delayedJobsManager;
        private bool _isActive;
        private Mutex _mutex;
        private Thread _delayedJobsManagerThread;
        public CronJobsManager(BlockingCollection<JobInfo<string>> cronJobsCollection)
        {
            _unmanagedJobs = cronJobsCollection;
            _jobsForDelayedJobsManager = new BlockingCollection<JobInfo<TimeSpan>>();
            _delayedJobsManager = new DelayedJobsManager(_jobsForDelayedJobsManager);
            _isActive = true;
            timers = new HashSet<Timer>();
            _mutex = new Mutex(false);
            _delayedJobsManagerThread = new Thread(_delayedJobsManager.ManageJobs);
            _delayedJobsManagerThread.Start();
        }

        public void ManageJobs()
        {
            while (_isActive)
            {
                JobInfo<string> jobInfo = _unmanagedJobs.Take();
                try
                {
                    _mutex.WaitOne();
                    if (_isActive)
                        DoJobManagement(jobInfo);
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
                
            }
        }

        void DoJobManagement(JobInfo<string> jobInfo)
        {
            CronExpression cronExpression = new CronExpression(jobInfo.Info);
            var currentDateTime = DateTime.Now;
            TimeSpan delayBeforeNextJob = cronExpression.GetNexDateTime(currentDateTime.AddMinutes(1)) - currentDateTime;
            Console.WriteLine(delayBeforeNextJob.TotalMilliseconds);
            _jobsForDelayedJobsManager.Add(new JobInfo<TimeSpan>(jobInfo.Job, delayBeforeNextJob, jobInfo.Argument));
            Timer timer = new Timer(delayBeforeNextJob.TotalMilliseconds);
            timers.Add(timer);
            timer.Elapsed += (sender, args) =>
            {
                if (sender.Equals(timer))
                {
                    ((Timer)sender).Stop();
                    timers.Remove(((Timer)sender));
                    DoJobManagement(jobInfo);
                }
            };
            timer.Start();
        }

        public void Dispose()
        {
            try
            {
                _mutex.WaitOne();
                _delayedJobsManager.Dispose();
                _jobsForDelayedJobsManager.Dispose();
            }
            finally
            {
                if (_mutex != null)
                    _mutex.ReleaseMutex();
                _mutex = null;
            }
        }
    }
}
