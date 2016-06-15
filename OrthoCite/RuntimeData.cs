using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;
using OrthoCite.Entities;
using OrthoCite.Entities.MiniGames;
using OrthoCite.Helpers;
using MonoGame.Extended.Maps.Tiled;
using System.IO;
using System.Reflection;

namespace OrthoCite
{
    public class RuntimeData
    {
        Rectangle _scene;
        DataSave _dataSave;
        ViewportAdapter _viewAdapter;
        DialogBox _dialogBox;
        OrthoCite _orthoCite;

        public GameTime GameTime { set; get; }
        public int gidLast { set; get; }
        Map map;

        public Dictionary<ListPnj, PNJ> pnj = new Dictionary<ListPnj, Helpers.PNJ>();

        int _credits;

        public RuntimeData()
        {
        }

        public DataSave DataSave
        {
            get { return _dataSave; }
            set { _dataSave = value; }
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

        public OrthoCite OrthoCite
        {
            get { return _orthoCite; }
            set { _orthoCite = value; }
        }

        public Map Map
        {
            get { return map; }
            set { map = value; }
        }
       
        public DialogBox DialogBox
        {
            get { return _dialogBox; }
            set { _dialogBox = value; }
        }

        public Player Player { get; set; }

        public Dictionary<ListPnj, PNJ> PNJ
        {
            get { return pnj; }
            set { pnj = value; }
        }
        
        public DoorGame DoorGame { get; set; }

        public int Lives => _dataSave.NumberOfLives;
        public int Credits => _dataSave.NumberOfCredits;

        /// <summary>
        /// Loose the given number of lives, saving it in the datasave, and return whether the player is alive or not.
        /// </summary>
        /// <param name="count">Number of lives to loose.</param>
        /// <returns>Is the player alive?</returns>
        public bool LooseLive(int count = 1)
        {
            _dataSave.NumberOfLives -= (byte)count;
            if (_dataSave.NumberOfLives < 0) _dataSave.NumberOfLives = 0;

            _dataSave.Save();

            return _dataSave.NumberOfLives > 0;
        }

        /// <summary>
        /// Gain the given number of lives, saving it in the datasave.
        /// </summary>
        /// <param name="count">Number of lives to gain.</param>
        public void GainLive(int count = 1)
        {
            _dataSave.NumberOfLives += (byte)count;

            _dataSave.Save();
        }

        /// <summary>
        /// Gain the given number of credits, saving it in the datasave.
        /// </summary>
        /// <param name="count">Number of credits to gain.</param>
        public void GainCredit(int count = 1)
        {
            _dataSave.NumberOfCredits += (byte)count;

            _dataSave.Save();
        }
    }
}
