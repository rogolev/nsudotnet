using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal abstract class JobsManager<T> : IDisposable
    {
        protected BlockingCollection<JobInfo<T>> UnmanagedJobs;
        protected bool StayActive;
        private Mutex _mutex;
        protected HashSet<Timer> Timers;
        private bool _disposed;

        protected JobsManager(BlockingCollection<JobInfo<T>> jobsCollection)
        {
            UnmanagedJobs = jobsCollection;
            StayActive = true;
            Timers = new HashSet<Timer>();
            _mutex = new Mutex(false);
            _disposed = false;
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
            if (_disposed)
                return;
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
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
            _mutex = null;
            UnmanagedJobs = null;
            Timers = null;
            _disposed = true;
        }

        ~JobsManager()
        {
            Dispose(false);
        }
    }
}
