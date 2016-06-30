using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using System;
using System.Collections.Generic;


namespace OrthoCite.Helpers
{
    public enum ListPnj
    {
        QUARTIER_1_1,
        QUARTIER_1_2,
        QUARTIER_1_3,
        QUARTIER_1_4,
        QUARTIER_1_5,
        QUARTIER_1_6,
        QUARTIER_2_1,
        QUARTIER_2_2,
        QUARTIER_2_3,
        QUARTIER_2_4,
        QUARTIER_3_1,
        QUARTIER_3_2,
        QUARTIER_3_3,
        QUARTIER_3_4,
        QUARTIER_3_5,
        QUARTIER_3_6,
        QUARTIER_4_1,
        QUARTIER_4_2,
        QUARTIER_4_3,
        QUARTIER_4_4,
        QUARTIER_4_5,
        QUARTIER_4_6,
        HOME_1,
        HOME_2,
        HOME_3,
        HOME_4,
        HOME_5,
        HOME_6,
        HOME_7,
        HOME_8,
        HOME_9,
        HOME_10,
        HOME_11,
        HOME_12,
        THROWGAME,
        PORTAILBLOCK
    }

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
    
    public struct PnjDialog
    {
        public readonly string ask;
        public readonly Dictionary<string, bool> answer;
         
        public PnjDialog(string theAsk, Dictionary<string, bool> theAnswer)
        {
            ask = theAsk;
            answer = theAnswer;
        }

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
        public delegate void AttackEvent(PNJ player);
        //public event AttackEvent playerAttack;

        public delegate void AnswerPnjEvent(RuntimeData r, bool TrueOrFalseAnswer);
        public event AnswerPnjEvent playerAnswerToPnj;
        public void playerAnswerToPnjRequest(RuntimeData r, bool TrueOrFalseAnswer)
        {
            if (playerAnswerToPnj != null)
                playerAnswerToPnj(r, TrueOrFalseAnswer);
        }

        TypePNJ _type;
        List<ItemList> _item;

        
        public List<PnjDialog> _talkAndAnswer { get; set;  }
        Queue<PnjDialog> _talkAndAnswerQueue;
        public bool inTalk { get; private set; } 

        const int timeTalk = 100;

        public Vector2 _positionSec { get; set; } // IF == DYNAMIQUE -> MOOVE TO THIS 
        public Vector2 _positionMain { get; set; } // SPAWN 

        Player _pnj;

        PnjDirection _currentDirection;

        RuntimeData _runtimeData;
        string _texture;


        TimeSpan _saveTime;
        TimeSpan _gameTime;
        const int timeToNextDialog = 1; //IN SECOND 

        public Direction lookDir { get; set; }
        
        

        public PNJ(TypePNJ type, Vector2 positionSpawn, List<ItemList> item, RuntimeData runtimeData, string texture)
        {
            changeDirection += changeDirectionOfPnj;

            _type = type;
            _item = item;
            _runtimeData = runtimeData;
            _texture = texture;
            _positionMain = positionSpawn;
            _currentDirection = PnjDirection.PositionSec;
            _talkAndAnswer = new List<PnjDialog>();
            _pnj = new Player(TypePlayer.WithSpriteSheet, positionSpawn ,_runtimeData, texture);

            //playerAttack += goAttack;

            _talkAndAnswerQueue = new Queue<PnjDialog>();
            inTalk = false;
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
            _gameTime = gameTime.TotalGameTime;
            if (_runtimeData.AnswerBox.isVisible) return;

            if (inTalk)
            {
                if (keyboardState.IsKeyDown(Keys.E))talk();
                _pnj.heroAnimations.Play(lookDir.ToString());
                
                _pnj.lastDir = lookDir;
                _pnj.heroAnimations.Update(deltaSeconds);
                return;
            }
            else if (keyboardState.IsKeyDown(Keys.E) && _runtimeData.Player != null) collisionWithPlayer(gameTime);



            if (_type != TypePNJ.Static)
            {
                iaMovePnj(gameTime);
                _pnj.checkMove(keyboardState);
            }
            else { _pnj.heroAnimations.Play(lookDir.ToString()); _pnj.lastDir = lookDir; }
            
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
                if (PNJPlayer.positionVirt.X < _positionMain.X && !PNJPlayer.ColRight()) PNJPlayer.MooveChamp(Direction.RIGHT);
                else if (PNJPlayer.positionVirt.X > _positionMain.X && !PNJPlayer.ColLeft()) PNJPlayer.MooveChamp(Direction.LEFT);
                else if (PNJPlayer.positionVirt.Y < _positionMain.Y && !PNJPlayer.ColDown()) PNJPlayer.MooveChamp(Direction.DOWN);
                else if (PNJPlayer.positionVirt.Y > _positionMain.Y && !PNJPlayer.ColUp()) PNJPlayer.MooveChamp(Direction.UP);
            }

            else if (PNJPlayer.separeFrame == 0 && _currentDirection == PnjDirection.PositionSec)
            {
                if (PNJPlayer.positionVirt.X < _positionSec.X && !PNJPlayer.ColRight()) PNJPlayer.MooveChamp(Direction.RIGHT);
                else if (PNJPlayer.positionVirt.X > _positionSec.X && !PNJPlayer.ColLeft()) PNJPlayer.MooveChamp(Direction.LEFT);


                else if (PNJPlayer.positionVirt.Y < _positionSec.Y && !PNJPlayer.ColDown()) PNJPlayer.MooveChamp(Direction.DOWN);
                else if (PNJPlayer.positionVirt.Y > _positionSec.Y && !PNJPlayer.ColUp()) PNJPlayer.MooveChamp(Direction.UP);
            }

            if (PNJPlayer.separeFrame == 0 && PNJPlayer.positionVirt == _positionMain || PNJPlayer.separeFrame == 0 && PNJPlayer.positionVirt == _positionSec)
            {
                _saveTime = time.TotalGameTime;
                PnjChangeDirection(this);
            }

        }

        private void goAttack(PNJ player)
        {
            if (player.PNJPlayer.lastDir == Direction.LEFT) player.PNJPlayer.heroAnimations.Play(Direction.ATTACK_LEFT.ToString());
            else if (player.PNJPlayer.lastDir == Direction.RIGHT) player.PNJPlayer.heroAnimations.Play(Direction.ATTACK_RIGHT.ToString());
            else if (player.PNJPlayer.lastDir == Direction.UP) player.PNJPlayer.heroAnimations.Play(Direction.ATTACK_TOP.ToString());
            else if (player.PNJPlayer.lastDir == Direction.ATTACK_DOWN) player.PNJPlayer.heroAnimations.Play(Direction.ATTACK_DOWN.ToString());
        }

        private void collisionWithPlayer(GameTime time)
        {
            if (_saveTime.TotalMilliseconds == 0)
            {
                if (_runtimeData.Player.positionVirt.X == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y + 1 == _pnj.positionVirt.Y) { lookDir = Direction.UP; talk(); }

                else if (_runtimeData.Player.positionVirt.X == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y - 1 == _pnj.positionVirt.Y) { lookDir = Direction.DOWN; talk(); }

                else if (_runtimeData.Player.positionVirt.X + 1 == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y == _pnj.positionVirt.Y) { lookDir = Direction.LEFT; talk(); }

                else if (_runtimeData.Player.positionVirt.X - 1 == _pnj.positionVirt.X && _runtimeData.Player.positionVirt.Y == _pnj.positionVirt.Y) { lookDir = Direction.RIGHT; talk(); }
                
                _saveTime = time.TotalGameTime;
            }
            else if (_saveTime != null && _saveTime <= time.TotalGameTime - new TimeSpan(0, 0, 2)) _saveTime = new TimeSpan(0, 0, 0); 

        }

        private void talk()
        {
            if (_saveTime.TotalSeconds != 0 && _saveTime.TotalSeconds <= _gameTime.TotalSeconds - timeToNextDialog) _saveTime = _gameTime;
            else if (_saveTime.TotalMilliseconds == 0) _saveTime = _gameTime;
            else return;

            if (_talkAndAnswerQueue.Count == 0 && inTalk)
            {
                inTalk = false;
                _runtimeData.DialogBox.Hide();
            }
            else if (_talkAndAnswerQueue.Count == 0)
            {
                inTalk = true;
                foreach (PnjDialog tmpDialog in _talkAndAnswer)
                {
                    _talkAndAnswerQueue.Enqueue(tmpDialog);
                }
                if (_talkAndAnswerQueue.Count != 0)
                {
                    PnjDialog tmp = _talkAndAnswerQueue.Dequeue();
                    if (tmp.answer != null && tmp.answer.Count != 0)
                    {
                        _runtimeData.AnswerBox._ask = tmp.ask;
                        _runtimeData.AnswerBox._Answer = tmp.answer;
                        _runtimeData.AnswerBox.Run(this);
                    }
                    else _runtimeData.DialogBox.SetText(tmp.ask).Show();
                }
            }
            else
            {
                PnjDialog tmp = _talkAndAnswerQueue.Dequeue();
                if (tmp.answer != null && tmp.answer.Count != 0)
                {
                    _runtimeData.AnswerBox._ask = tmp.ask;
                    _runtimeData.AnswerBox._Answer = tmp.answer;
                    _runtimeData.AnswerBox.Run(this);
                }
                else _runtimeData.DialogBox.SetText(tmp.ask).Show();
            }
            
        }
        
        public void finishTalk()
        {
            _talkAndAnswerQueue.Clear();
            inTalk = false;
            _runtimeData.DialogBox.Hide();

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
