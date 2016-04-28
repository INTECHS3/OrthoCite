using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGameConsole;
using Microsoft.Xna.Framework.Input;

namespace OrthoCite
{
    public class RuntimeData
    {
        public GameConsole _console;

        public RuntimeData(GameConsole Console)
        {
            _console = Console;    
        }

        public GameConsole Console
        {
            get { return _console; }
        }
    }
}
