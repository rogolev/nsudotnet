using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rogolev.Nsudotnet.TaskSheduler
{
    public abstract class JobStarter
    {
        protected void StartJob(IJob job)
        {
            Thread jobThread = new Thread(job.Execute);
            jobThread.Start();
        }
    }
}
