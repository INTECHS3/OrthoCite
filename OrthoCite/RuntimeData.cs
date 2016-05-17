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
        ViewportAdapter _viewAdapter;

        public RuntimeData()
        {
        }

        public Rectangle Scene
        {
            get { return _scene; }
            set { _scene = value; }
        }
        
        public ViewportAdapter viewAdapter
        {
            get { return _viewAdapter; }
            set { _viewAdapter = value; }
        }

        
    }
}
