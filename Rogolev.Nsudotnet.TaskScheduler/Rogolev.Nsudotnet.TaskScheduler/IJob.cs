namespace Rogolev.Nsudotnet.TaskScheduler
{
    public interface IJob
    {
        void Execute(object argument);
    }
}
