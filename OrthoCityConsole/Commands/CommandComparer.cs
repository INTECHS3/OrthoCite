using System.Collections.Generic;

namespace OrthoCityConsole.Commands
{
    class CommandComparer:IComparer<IConsoleCommand>
    {
        public int Compare(IConsoleCommand x, IConsoleCommand y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
