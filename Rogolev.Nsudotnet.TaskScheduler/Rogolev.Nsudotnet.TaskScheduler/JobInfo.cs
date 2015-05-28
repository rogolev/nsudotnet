using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogolev.Nsudotnet.TaskScheduler
{
    internal class JobInfo <T>
    {
        public JobInfo(IJob job, T info, object argument = null)
        {
            _job = job;
            _info = info;
            _argument = argument;
        }

        private IJob _job;
        public IJob Job
        {
            get { return _job; }
        }

        private T _info;
        public T Info { get { return _info; } }

        private object _argument;
        public object Argument
        {
            get { return _argument; }
        }
    }
}
