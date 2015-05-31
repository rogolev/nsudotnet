using System;
using System.Collections.Concurrent;
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
        private bool _disposed;
        private const string DisposedWarning = "This object has been disposed!";
        public TaskScheduler()
        {
            _delayedJobsCollection = new BlockingCollection<JobInfo<TimeSpan>>();
            _periodicJobsCollection = new BlockingCollection<JobInfo<TimeSpan>>();
            _cronJobsCollection = new BlockingCollection<JobInfo<string>>();
            _delayedJobsManager = new DelayedJobsManager(_delayedJobsCollection);
            _periodicJobsManager = new PeriodicJobsManager(_periodicJobsCollection);
            _cronJobsManager = new CronJobsManager(_cronJobsCollection);
            Thread delayedJobManagerThread = new Thread(_delayedJobsManager.ManageJobs);
            Thread periodicJobManagerThread = new Thread(_periodicJobsManager.ManageJobs);
            Thread cronJobManagerThread = new Thread(_cronJobsManager.ManageJobs);
            delayedJobManagerThread.Start();
            periodicJobManagerThread.Start();
            cronJobManagerThread.Start();
            _disposed = false;
        }
        public void ScheduleDelayedJob(IJob job, TimeSpan delay, object argument)
        {
            if (_disposed)
                throw new ObjectDisposedException(DisposedWarning);
            _delayedJobsCollection.Add(new JobInfo<TimeSpan>(job, delay, argument));
        }

        public void SchedulePeriodicJob(IJob job, TimeSpan period, object argument)
        {
            if (_disposed)
                throw new ObjectDisposedException(DisposedWarning);
            _periodicJobsCollection.Add(new JobInfo<TimeSpan>(job, period, argument));
        }

        public void SchedulePeriodicJob(IJob job, string cronExpression, object argument)
        {
            if (_disposed)
                throw new ObjectDisposedException(DisposedWarning);
            _cronJobsCollection.Add(new JobInfo<string>(job, cronExpression, argument));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_delayedJobsManager != null)
                    _delayedJobsManager.Dispose();
                if (_periodicJobsManager != null)
                    _periodicJobsManager.Dispose();
                if (_cronJobsManager != null)
                    _cronJobsManager.Dispose();
                if (_delayedJobsCollection != null)
                    _delayedJobsCollection.Dispose();
                if (_periodicJobsCollection != null)
                    _periodicJobsCollection.Dispose();
                if (_cronJobsCollection != null)
                    _cronJobsCollection.Dispose();
            }
            _delayedJobsManager = null;
            _periodicJobsManager = null;
            _cronJobsManager = null;
            _delayedJobsCollection = null;
            _periodicJobsCollection = null;
            _cronJobsCollection = null;
            _disposed = true;
        }

        ~TaskScheduler()
        {
            Dispose(true);
        }
    }
}
