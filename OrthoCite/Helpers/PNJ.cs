using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Collections;


namespace OrthoCite.Helpers
{
    public enum TypePNJ
    {
        Static,
        Dynamique
    }

    public enum ItemList
    {

    }
    
    public class PNJ
    {

        TypePNJ _type;
        List<ItemList> _item;

        public List<string> _talk { get; set; }
        int i;
        const int timeTalk = 100;
        int a;
        int z;

        Player _pnj;
        

        RuntimeData _runtimeData;
        string _texture;

        public bool talking { get; set; }
        public bool stop { get; set; }


        //TALKABLE, TEXT, NEDD LESS PARAMS CONTRUCTOR

        public PNJ(TypePNJ type, Vector2 positionSpawn, List<ItemList> item, RuntimeData runtimeData, string texture)
        {
            _type = type;
            _item = item;
            _runtimeData = runtimeData;
            _texture = texture;
            _talk = new List<string>();
            _pnj = new Player(TypePlayer.WithSpriteSheet, positionSpawn ,_runtimeData, texture);
        }
        
        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _pnj.LoadContent(content);

            z = 0;
        }

        public void UnloadContent()
        {

        }
        
        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera, float deltaSeconds)
        {

            

            if(z == 0 && _runtimeData.Player != null) collisionWithPlayer();

            if (talking)
            {
               stop = true;
               talk();
            }
            else
            {
                stop = false;
                if (_type != TypePNJ.Static)
                {
                    
                    iaMovePnj();
                    
                    _pnj.checkMove(keyboardState, camera);
                }
            }
            
            _pnj.heroAnimations.Update(deltaSeconds);
            _pnj.heroSprite.Position = new Vector2(_pnj.position.X + _pnj.tileWidth / 2, _pnj.position.Y + _pnj.tileHeight / 2);

            if (z != 0 && z < 100) z++;
            else if (z >= 100) z = 0;
        }

        private void iaMovePnj()
        {
            Random t = new Random();
            int r = t.Next(1, 4);
            if(_pnj.actualDir == Direction.NONE && _pnj.separeFrame == 0)
            {
                if (r == 1) _pnj.actualDir = Direction.UP;
                if (r == 2) _pnj.actualDir = Direction.DOWN;
                if (r == 3) _pnj.actualDir = Direction.LEFT;
                if (r == 4) _pnj.actualDir = Direction.RIGHT;
                if (r == 0) _pnj.actualDir = Direction.NONE;
                Console.WriteLine(_pnj.positionVirt.X + " " + _pnj.positionVirt.Y + " " + _pnj.actualDir);
            }
            
            
        }

        private void collisionWithPlayer()
        {
            if (_runtimeData.Player.positionVirt.X == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y + 1 == _pnj.positionVirt.Y) talking = true;
            
            if (_runtimeData.Player.positionVirt.X == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y - 1 == _pnj.positionVirt.Y) talking = true;

            if (_runtimeData.Player.positionVirt.X + 1 == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y == _pnj.positionVirt.Y) talking = true;

            if (_runtimeData.Player.positionVirt.X - 1 == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y == _pnj.positionVirt.Y) talking = true;

            z++;
        }

        private void talk()
        {

            if (i == 0 && a < _talk.Count)
            {
                _runtimeData.DialogBox.SetText(_talk[a]).Show();
                z = 1;
                a++;
                i++;
            }
            else if (i==0 && a >= _talk.Count)
            {
                a = 0;
                _runtimeData.DialogBox.Hide();
                talking = false;
            }
            else if (i != 0 && i < timeTalk) i++;
            else if (i == timeTalk)
            {
                _runtimeData.DialogBox.Hide();
                i = 0;
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _pnj.Draw(spriteBatch);
        }  

        public void Execute(params string[] param)
        {

        }

        public void spriteFactory(Direction dir, SpriteSheetAnimationData spriteData)
        {
            _pnj.spriteFactory.Add(dir, spriteData);
        }

        public Player PNJPlayer
        {
            get { return _pnj; }
        }
        
    }
}
