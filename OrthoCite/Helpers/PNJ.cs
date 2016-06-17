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

    enum PnjDirection
    {
        None,
        Spawn,
        PositionSec
    }

    public enum ItemList
    {

    }
    
    public class PNJ
    {

        public delegate void PNJHandle(PNJ j);
        public event PNJHandle changeDirection;
        public void PnjChangeDirection(PNJ j)
        {
            if (changeDirection != null)
                changeDirection(j);
        }


        TypePNJ _type;
        List<ItemList> _item;

        Dictionary<string, Dictionary<string, bool>> _talkAndAnswer;

        const int timeTalk = 100;

        public Vector2 _positionSec { get; set; } // IF == DYNAMIQUE -> MOOVE TO THIS 
        public Vector2 _positionMain { get; set; } // SPAWN 

        Player _pnj;

        PnjDirection _currentDirection;

        RuntimeData _runtimeData;
        string _texture;


        TimeSpan _saveTime;

        //TALKABLE, TEXT, NEDD LESS PARAMS CONTRUCTOR

        public PNJ(TypePNJ type, Vector2 positionSpawn, List<ItemList> item, RuntimeData runtimeData, string texture)
        {
            changeDirection += changeDirectionOfPnj;

            _type = type;
            _item = item;
            _runtimeData = runtimeData;
            _texture = texture;
            _positionMain = positionSpawn;
            _currentDirection = PnjDirection.PositionSec;
            _talkAndAnswer = new Dictionary<string, Dictionary<string, bool>>();
            _pnj = new Player(TypePlayer.WithSpriteSheet, positionSpawn ,_runtimeData, texture);


            _runtimeData.AnswerBox.heAnswerFalse += DownLifeOfPlayer;
            _runtimeData.AnswerBox.heAnswerGood += UpLifeOfPlayer;

        }
        
        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _pnj.LoadContent(content);
            
        }

        public void UnloadContent()
        {

        }
        
        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera, float deltaSeconds)
        {

            if (_runtimeData.AnswerBox.isVisible) return;

            _pnj.heroSprite.Scale = new Vector2(0.7f);
            if(keyboardState.IsKeyDown(Keys.E) && _runtimeData.Player != null) collisionWithPlayer(gameTime);

           
            if (_type != TypePNJ.Static)
            {
                iaMovePnj(gameTime);
                _pnj.checkMove(keyboardState);
            }
            
            
            _pnj.heroAnimations.Update(deltaSeconds);
            _pnj.heroSprite.Position = new Vector2(_pnj.position.X + _pnj.tileWidth / 2, _pnj.position.Y + _pnj.tileHeight / 2);

            
        }

        private void changeDirectionOfPnj(PNJ j)
        {
            if (j.PNJPlayer.positionVirt == j._positionSec) j._currentDirection = PnjDirection.Spawn;
            else _currentDirection = PnjDirection.PositionSec;

            j.PNJPlayer.heroAnimations.Play(Direction.NONE.ToString());
        }

        private void iaMovePnj(GameTime time)
        {

            if (_saveTime.TotalMilliseconds != 0 && _saveTime.TotalMilliseconds <= time.TotalGameTime.TotalMilliseconds - 1000) _saveTime = new TimeSpan(0);
            else if (_saveTime.TotalMilliseconds != 0) return; 

            if(PNJPlayer.separeFrame == 0 && _currentDirection == PnjDirection.Spawn)
            {
                if (PNJPlayer.positionVirt.X < _positionMain.X) PNJPlayer.MooveChamp(Direction.RIGHT);
                else if (PNJPlayer.positionVirt.X > _positionMain.X) PNJPlayer.MooveChamp(Direction.LEFT);
                else if (PNJPlayer.positionVirt.X == _positionMain.X && PNJPlayer.positionVirt.Y < _positionMain.Y) PNJPlayer.MooveChamp(Direction.DOWN);
                else if (PNJPlayer.positionVirt.X == _positionMain.X && PNJPlayer.positionVirt.Y > _positionMain.Y) PNJPlayer.MooveChamp(Direction.UP);
            }

            else if (PNJPlayer.separeFrame == 0 && _currentDirection == PnjDirection.PositionSec)
            {
                if (PNJPlayer.positionVirt.X < _positionSec.X) PNJPlayer.MooveChamp(Direction.RIGHT);
                else if (PNJPlayer.positionVirt.X > _positionSec.X) PNJPlayer.MooveChamp(Direction.LEFT);


                else if (PNJPlayer.positionVirt.X == _positionSec.X && PNJPlayer.positionVirt.Y < _positionSec.Y) PNJPlayer.MooveChamp(Direction.DOWN);
                else if (PNJPlayer.positionVirt.X == _positionSec.X && PNJPlayer.positionVirt.Y > _positionSec.Y) PNJPlayer.MooveChamp(Direction.UP);
            }

            if (PNJPlayer.separeFrame == 0 && PNJPlayer.positionVirt == _positionMain || PNJPlayer.separeFrame == 0 && PNJPlayer.positionVirt == _positionSec)
            {
                _saveTime = time.TotalGameTime;
                PnjChangeDirection(this);
            }

        }
        
        private void collisionWithPlayer(GameTime time)
        {
            if (_saveTime.TotalMilliseconds == 0)
            {
                if (_runtimeData.Player.positionVirt.X == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y + 1 == _pnj.positionVirt.Y) talk();

                else if (_runtimeData.Player.positionVirt.X == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y - 1 == _pnj.positionVirt.Y) talk();

                else if (_runtimeData.Player.positionVirt.X + 1 == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y == _pnj.positionVirt.Y) talk();

                else if (_runtimeData.Player.positionVirt.X - 1 == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y == _pnj.positionVirt.Y) talk();

                _saveTime = time.TotalGameTime;
            }
            else if (_saveTime != null && _saveTime <= time.TotalGameTime - new TimeSpan(0, 0, 4)) _saveTime = new TimeSpan(0, 0, 0); 

        }

        private void talk()
        {
            _runtimeData.AnswerBox._ask = "How Are u ? ";
            _runtimeData.AnswerBox._Answer.Add("Yes", true);
            _runtimeData.AnswerBox._Answer.Add("Nope", false);
            _runtimeData.AnswerBox.Run();
            

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _pnj.Draw(spriteBatch);
            
        }  

        public void Execute(params string[] param)
        {

        }

        private void DownLifeOfPlayer(RuntimeData runtimeData)
        {
            _runtimeData.LooseLive();
            _runtimeData.DialogBox.AddDialog("Perdu ahahahahahah", 2);
        }

        private void UpLifeOfPlayer(RuntimeData runtimeData)
        {
            _runtimeData.GainLive();
            _runtimeData.DialogBox.AddDialog("Wouahhhhh Gagné ", 2);
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
