using System;
using System.Threading;

namespace DotOPDS.Tasks
{
    interface ITaskArgs
    {
    }

    interface ITask : IDisposable
    {
        void Run(ITaskArgs args);
    }

    class TaskExtArgs
    {
        public ITask Task { get; set; }
        public ITaskArgs Args { get; set; }
        public Action<Exception> Handler { get; set; }
    }

    static class TaskExt
    {
        public static void Start(this ITask task, ITaskArgs args, Action<Exception> handler = null)
        {
            var thread = new Thread(StartTask);
            thread.Start(new TaskExtArgs { Task = task, Args = args, Handler = handler });
        }

        private static void StartTask(object args_)
        {
            var args = (TaskExtArgs)args_;
            try
            {
                args.Task.Run(args.Args);
            }
            catch (Exception e)
            {
                args.Handler?.Invoke(e);
            }
        }
    }
}
