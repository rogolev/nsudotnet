using System;
using System.Threading;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal class DelayedJobManager : JobStarter, IJob
    {
        private readonly IJob _job;
        private readonly TimeSpan _delay;
        public DelayedJobManager(IJob job, TimeSpan delay)
        {
            _job = job;
            _delay = delay;
        }
        public void Execute(object argument)
        {
            Thread.Sleep(_delay);
            StartJob(_job, argument);
        }
    }
}
