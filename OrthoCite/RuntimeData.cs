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
        readonly OrthoCite _orthoCite;

        public GameTime GameTime { set; get; }
        public int gidLast { set; get; }


        public RuntimeData(OrthoCite orthoCite)
        {
            _orthoCite = orthoCite;
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

        public OrthoCite OrthoCite
        {
            get { return _orthoCite; }
        }
    }
}
