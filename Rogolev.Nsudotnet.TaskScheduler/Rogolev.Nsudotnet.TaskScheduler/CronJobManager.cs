using System;
using System.Threading;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal class CronJobManager : JobStarter, IJob
    {
        private CronExpression _cronExpression;
        private IJob _job;
        private TimeSpan _oneMinute = new TimeSpan(0, 0, 1, 0);
        public CronJobManager(IJob job, CronExpression cronExpression)
        {
            _job = job;
            _cronExpression = cronExpression;
        }

        public void Execute(object argument)
        {
            while (true)
            {
                DateTime currentDateTime = DateTime.Now;
                DateTime nextDateTime = _cronExpression.GetNexDateTime(currentDateTime);
                if (!nextDateTime.Equals(nextDateTime))
                    Thread.Sleep(nextDateTime.Subtract(currentDateTime));
                StartJob(_job, argument);
                Thread.Sleep(_oneMinute);
            }
        }
    }
}
