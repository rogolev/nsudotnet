using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal abstract class JobsManager<T> : IDisposable
    {
        protected readonly BlockingCollection<JobInfo<T>> _unmanagedJobs;
        protected bool _stayActive;
        protected readonly Mutex _mutex;
        protected HashSet<Timer> _timers;

        protected JobsManager(BlockingCollection<JobInfo<T>> periodicJobsCollection)
        {
            _unmanagedJobs = periodicJobsCollection;
            _stayActive = true;
            _timers = new HashSet<Timer>();
            _mutex = new Mutex(false);
        }

        public void ManageJobs()
        {
            while (_stayActive)
            {
                JobInfo<T> jobInfo = _unmanagedJobs.Take();
                try
                {
                    _mutex.WaitOne();
                    if (_stayActive)
                    {
                        _mutex.GetAccessControl();
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
            try
            {
                _mutex.GetAccessControl();
                _stayActive = false;
                foreach (Timer timer in _timers)
                {
                    _mutex.WaitOne();
                    timer.Dispose();
                }

            }
            finally
            {
                if (_mutex != null)
                {
                    _mutex.ReleaseMutex();
                    _mutex.Dispose();
                }
            }
        }
    }
}
