using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGameConsole;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;

namespace OrthoCite
{
    public class RuntimeData
    {
        GameConsole _console;
        Camera2D _camera;

        public RuntimeData(GameConsole Console, Camera2D camera)
        {
            _console = Console;
            _camera = camera;
        }

        public GameConsole Console
        {
            get { return _console; }
        }

        public Camera2D Camera
        {
            get { return _camera; }
        }
    }
}
