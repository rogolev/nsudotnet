using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rogolev.Nsudotnet.TaskSheduler
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
