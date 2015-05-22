using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogolev.Nsudotnet.TaskSheduler
{
    public interface IJob
    {
        void Execute(object argument);
    }
}
