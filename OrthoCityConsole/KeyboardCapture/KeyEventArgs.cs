using System;
using Microsoft.Xna.Framework.Input;

namespace OrthoCityConsole.KeyboardCapture
{
    class KeyEventArgs : EventArgs
    {
        public KeyEventArgs( Keys keyCode )
        {
            KeyCode = keyCode;
        }

        public Keys KeyCode { get; private set; }
    }
    
}
