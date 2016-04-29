using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;

namespace OrthoCite
{
    public class RuntimeData
    {
        Camera2D _camera;

        public RuntimeData(Camera2D camera)
        {
            _camera = camera;
        }

        public Camera2D Camera
        {
            get { return _camera; }

        }
    }
}
