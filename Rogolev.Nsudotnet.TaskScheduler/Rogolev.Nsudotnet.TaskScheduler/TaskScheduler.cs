using System;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    public class TaskScheduler : IDisposable
    {
        private BlockingCollection<JobInfo<TimeSpan>> _delayedJobsCollection;
        private BlockingCollection<JobInfo<TimeSpan>> _periodicJobsCollection;
        private BlockingCollection<JobInfo<string>> _cronJobsCollection;
        private DelayedJobsManager _delayedJobsManager;
        private PeriodicJobsManager _periodicJobsManager;
        private CronJobsManager _cronJobsManager;
        private Thread _delayedJobManagerThread;
        private Thread _periodicJobManagerThread;
        private Thread _cronJobManagerThread;
        public TaskScheduler()
        {
            _delayedJobsCollection = new BlockingCollection<JobInfo<TimeSpan>>();
            _periodicJobsCollection = new BlockingCollection<JobInfo<TimeSpan>>();
            _cronJobsCollection = new BlockingCollection<JobInfo<string>>();
            _delayedJobsManager = new DelayedJobsManager(_delayedJobsCollection);
            _periodicJobsManager = new PeriodicJobsManager(_periodicJobsCollection);
            _cronJobsManager = new CronJobsManager(_cronJobsCollection);
            _delayedJobManagerThread = new Thread(_delayedJobsManager.ManageJobs);
            _periodicJobManagerThread = new Thread(_periodicJobsManager.ManageJobs);
            _cronJobManagerThread = new Thread(_cronJobsManager.ManageJobs);
            _delayedJobManagerThread.Start();
            _periodicJobManagerThread.Start();
            _cronJobManagerThread.Start();
        }
        public void ScheduleDelayedJob(IJob job, TimeSpan delay, object argument)
        {
            _delayedJobsCollection.Add(new JobInfo<TimeSpan>(job, delay, argument));
        }

        public void SchedulePeriodicJob(IJob job, TimeSpan period, object argument)
        {
            _periodicJobsCollection.Add(new JobInfo<TimeSpan>(job, period, argument));
        }

        public void SchedulePeriodicJob(IJob job, string cronExpression, object argument)
        {
            _cronJobsCollection.Add(new JobInfo<string>(job, cronExpression, argument));
        }

        public void Dispose()
        {
            if (_delayedJobsManager != null)
                _delayedJobsManager.Dispose();
            _delayedJobsManager = null;
            if (_periodicJobsManager != null)
                _periodicJobsManager.Dispose();
            _periodicJobsManager = null;
            if (_cronJobsManager != null)
                _cronJobsManager.Dispose();
            _cronJobsManager = null;
            if (_delayedJobsCollection != null)
                _delayedJobsCollection.Dispose();
            _delayedJobsCollection = null;
            if (_periodicJobsCollection != null)
                _periodicJobsCollection.Dispose();
            _periodicJobsCollection = null;
            if (_cronJobsCollection != null)
                Dispose();
            _cronJobsCollection = null;
        }
    }
}
