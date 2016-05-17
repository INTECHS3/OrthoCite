using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;

namespace OrthoCite
{
    public class RuntimeData
    {
        Rectangle _scene;
        DataSave _dataSave;
        ViewportAdapter _viewAdapter;

        public RuntimeData()
        {
            _dataSave = new DataSave();
        }

        public Rectangle Scene
        {
            get { return _scene; }
            set { _scene = value; }
        }
        
        public ViewportAdapter ViewAdapter
        {
            get { return _viewAdapter; }
            set { _viewAdapter = value; }
        }

        public DataSave DataSave => _dataSave;
    }
}
