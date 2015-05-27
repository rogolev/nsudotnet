using System.Threading;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    public abstract class JobStarter
    {
        protected void StartJob(IJob job, object parameter)
        {
            Thread jobThread = new Thread(job.Execute);
            jobThread.Start(parameter);
        }
    }
}
