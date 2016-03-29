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
    }

    static class TaskExt
    {
        public static void Start(this ITask task, ITaskArgs args)
        {
            var thread = new Thread(StartTask);
            thread.Start(new TaskExtArgs { Task = task, Args = args });
        }

        private static void StartTask(object args_)
        {
            var args = (TaskExtArgs)args_;
            args.Task.Run(args.Args);
        }
    }
}
