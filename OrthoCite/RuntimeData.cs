using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace OrthoCite
{
    public class RuntimeData
    {
        Camera2D _camera;
        Rectangle _window;

        public RuntimeData()
        {
        }

        public Rectangle Window
        {
            get { return _window; }
            set { _window = value; }
        }
    }
}
