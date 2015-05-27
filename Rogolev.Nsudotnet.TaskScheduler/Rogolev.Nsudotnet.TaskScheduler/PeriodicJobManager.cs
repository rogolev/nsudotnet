using System;
using System.Threading;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal class PeriodicJobManager : JobStarter, IJob
    {
        private readonly IJob _job;
        private readonly TimeSpan _period;
        public PeriodicJobManager(IJob job, TimeSpan period)
        {
            _job = job;
            _period = period;
        }
        public void Execute(object argument)
        {
            while (true)
            {
                Thread.Sleep(_period);
                StartJob(_job, argument);
            }
        }

        
    }
}
