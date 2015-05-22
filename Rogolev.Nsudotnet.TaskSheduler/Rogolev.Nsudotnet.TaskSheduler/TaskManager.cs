using System;

namespace Rogolev.Nsudotnet.TaskSheduler
{
    class TaskManager : JobStarter
    {
        public void SheduleDelayedJob(IJob job, TimeSpan delay)
        {
            IJob delayedJobManager = new DelayedJobManager(job, delay);
            StartJob(delayedJobManager);
        }

        public void ShedulePeriodicJob(IJob job, TimeSpan period)
        {
            IJob periodicJobManager = new PeriodicJobManager(job, period);
            StartJob(periodicJobManager);
        }

        public void ShedulePeriodicJob(IJob job, string cronExpression)
        {
            try
            {
                CronExpression cronExpressionInstance = new CronExpression(cronExpression);
                IJob cronJobManager = new CronJobManager(job, cronExpressionInstance);
                StartJob(cronJobManager);
            }
            catch (InvalidCronExpressionException e)
            {
                throw;
            }
        }
    }
}
