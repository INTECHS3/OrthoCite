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
        Rectangle _window;
        DataSave _dataSave;

        public RuntimeData()
        {
        }

        public Rectangle Window
        {
            get { return _window; }
            set { _window = value; }
        }

        public DataSave DataSave => _dataSave;
    }
}
