using System;

namespace Rogolev.Nsudotnet.TaskSheduler
{
    public class TaskManager : JobStarter
    {
        public void SheduleDelayedJob(IJob job, TimeSpan delay, object argument)
        {
            IJob delayedJobManager = new DelayedJobManager(job, delay);
            StartJob(delayedJobManager, argument);
        }

        public void ShedulePeriodicJob(IJob job, TimeSpan period, object argument)
        {
            IJob periodicJobManager = new PeriodicJobManager(job, period);
            StartJob(periodicJobManager, argument);
        }

        public void ShedulePeriodicJob(IJob job, string cronExpression, object argument)
        {
            try
            {
                CronExpression cronExpressionInstance = new CronExpression(cronExpression);
                IJob cronJobManager = new CronJobManager(job, cronExpressionInstance);
                StartJob(cronJobManager, argument);
            }
            catch (InvalidCronExpressionException e)
            {
                throw;
            }
        }
    }
}
