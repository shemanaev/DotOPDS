using System;

namespace DotOPDS.Commands
{
    interface ICommand : IDisposable
    {
        int Run(SharedOptions options);
    }
}
