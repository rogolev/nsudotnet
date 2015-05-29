using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal abstract class JobsManager<T> : IDisposable
    {
        protected readonly BlockingCollection<JobInfo<T>> UnmanagedJobs;
        protected bool StayActive;
        protected Mutex _mutex;
        protected HashSet<Timer> Timers;
        private bool disposed;

        protected JobsManager(BlockingCollection<JobInfo<T>> jobsCollection)
        {
            UnmanagedJobs = jobsCollection;
            StayActive = true;
            Timers = new HashSet<Timer>();
            _mutex = new Mutex(false);
            disposed = false;
        }

        public void ManageJobs()
        {
            while (StayActive)
            {
                try
                {
                    if (_mutex != null)
                        _mutex.WaitOne();
                    if (StayActive)
                    {
                        JobInfo<T> jobInfo = UnmanagedJobs.Take();
                        if (jobInfo == null)
                            break;
                        DoJobManagement(jobInfo);
                    }
                }
                finally
                {
                    if (_mutex != null)
                        _mutex.ReleaseMutex();
                }
            }
        }

        protected abstract void DoJobManagement(JobInfo<T> info);

        public void Dispose()
        {
            if (disposed)
                return;
            try
            {
                UnmanagedJobs.Add(null);
                _mutex.WaitOne();
                StayActive = false;
                UnmanagedJobs.Add(null);
                foreach (Timer timer in Timers)
                {
                    timer.Dispose();
                }
                UnmanagedJobs.Dispose();
            }
            finally
            {
                if (_mutex != null)
                {
                    _mutex.ReleaseMutex();
                    _mutex.Dispose();
                    _mutex = null;
                }
            }
            Dispose(true);
            disposed = true;
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}
