using System;
using System.Threading;

namespace Rogolev.Nsudotnet.TaskSheduler
{
    internal class CronJobManager : JobStarter, IJob
    {
        private CronExpression _cronExpression;
        private IJob _job;
        private int _oneMinute = 6000;
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
                Thread.Sleep(nextDateTime.Subtract(currentDateTime));
                StartJob(_job, argument);
                Thread.Sleep(_oneMinute);
            }
        }
    }
}
