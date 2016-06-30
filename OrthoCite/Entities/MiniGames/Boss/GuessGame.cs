using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using Microsoft.Xna.Framework.Media;

using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using OrthoCite.Helpers;


namespace OrthoCite.Entities.MiniGames
{


    public class GuessGame : MiniGame
    {
        RuntimeData _runtimeData;
        public TiledMap textMap;
        TiledMap actualTextMap;
        Helpers.Player _player;
        SpriteFont _font;
        Random _random;
        SoundEffect _hurt;
        SoundEffect _success;
        Song _music;
        XmlDocument document;
        List<Word> _word;
        Word _current;
        TimeSpan _timer;
        TimeSpan _actualTime;
        TimeSpan _saveTime;
        TimeSpan _popTime;
        TimeSpan _invincible;
        TiledTileLayer _poppers;

        int _gidStart;
        const int _gidSpawn = 46;
        const int _fastSpeedPlayer = 8;
        const int _lowSpeedPlayer = 13;
        const int DISTRICT = 1;
        const int _ligne = 9;
        const int columns = 17;
        const int _zoom = 3;
        int count = 0;
        const int TIME_FINISH = 10;
        const int TIMER_POP = 2;
        const int TIME_INVICIBLE = 2;
        int[] _calculator;
        int _randomNumber;
        int _randomNumber2;
        bool _firstUpdate;
        int _district = 0;
        bool _update = false;
        bool _invinc;
        int number = 0;

        int[] _r =        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,};


        struct Word
        {
            public string Value;
            public bool IsValid;
        }

      

            public GuessGame(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _gidStart = _runtimeData.gidLast;

            _firstUpdate = true;

            _player = new Helpers.Player(Helpers.TypePlayer.WithSpriteSheet, new Vector2(0, 0), _runtimeData, "animations/walking");
            _runtimeData.Player = _player;
            _player.separeFrame = 0;
            _player.lowFrame = _lowSpeedPlayer;
            _player.fastFrame = _fastSpeedPlayer;
            _player.typeDeplacement = TypeDeplacement.WithKey;
            _random = new Random();
            _runtimeData.GuessGame = this;
            _word = new List<Word>();
            _current = new Word();
            _popTime = new TimeSpan();
            _invincible = new TimeSpan();
           

        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            textMap = content.Load<TiledMap>("minigames/GuessGame/GuessGame");
            actualTextMap = textMap;
            _poppers = new TiledTileLayer(textMap, graphicsDevice, "Poppers", 17, 17, _r);
            _poppers.IsVisible = true;
            _font = content.Load<SpriteFont>("minigames/Rearranger/font");
            _music = content.Load<Song>("minigames/Rearranger/music");
            _hurt = content.Load<SoundEffect>("minigames/GuessGame/hurt");
            _success = content.Load<SoundEffect>("minigames/GuessGame/success");
            foreach (TiledTileLayer e in textMap.TileLayers)
            {
                if (e.Name == "collision") _player.collisionLayer = e;
            }
            _player.collisionLayer.IsVisible = false;

            if (_gidStart != 0)
            {
                foreach (TiledTile i in _player.collisionLayer.Tiles)
                {
                    if (i.Id == _gidStart) _player.positionVirt = new Vector2(i.X, i.Y + 1);
                }
            }

            if (_player.positionVirt.Length() == 0)
            {
                foreach (TiledTile i in _player.collisionLayer.Tiles)
                {
                    if (i.Id == _gidSpawn) _player.positionVirt = new Vector2(i.X, i.Y);
                }
            }
            _runtimeData.gidLast = 0;

            _player.gidCol = 633;
            _player.spriteFactory.Add(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 }));
            _player.spriteFactory.Add(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 5, 10 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 19, 13, 24, 13 }, isLooping: false));
            _calculator = new int[_ligne];
            _calculator[0] = 0;
            _district = 1;
            for(int i = 1; i < _ligne; i++ )
            {
                _calculator[i] = _calculator[i - 1] + columns;
            }
            _player.LoadContent(content);
            LoadWords();
            Start();
            instanceWorld();

        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (_invincible.TotalMilliseconds == 0)
            {
                _invincible = gameTime.TotalGameTime;
            }
            else if (gameTime.TotalGameTime.Subtract(_invincible).TotalSeconds >= TIME_INVICIBLE)
            {
                _invinc = false;
            }
            foreach (TiledTile e in _poppers.Tiles)
            {
                if(e.Id == 1614 && e.X == _player.positionVirt.X && e.Y == _player.positionVirt.Y)
                {
                    if (!_invinc)
                    {
                        _runtimeData.LooseLive();
                        _hurt.Play();
                        if (_runtimeData.Lives == 0)
                        {
                            _runtimeData.DialogBox.AddDialog("tu as perdu !", 2);
                            _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                        }
                        invincible();
                    }
                }
            }
             if (_timer.TotalMilliseconds == 0)
             {
                 _timer = gameTime.TotalGameTime;
             }
             else if (gameTime.TotalGameTime.Subtract(_timer).TotalSeconds >= TIME_FINISH)
             {
                 _runtimeData.LooseLive();
                _hurt.Play();

                 if (_runtimeData.Lives == 0)
                 {
                     _runtimeData.DialogBox.AddDialog("tu as perdu !", 2);
                     _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                 } 
                 instanceWorld();
             }

            if (_popTime.TotalMilliseconds == 0)
            {
                _popTime = gameTime.TotalGameTime;
            }
            else if (gameTime.TotalGameTime.Subtract(_popTime).TotalSeconds >= TIMER_POP)
            {
                mapPop();
            }
            _actualTime = gameTime.TotalGameTime;
            if (_update)
            {
                _runtimeData.DialogBox.SetText(_current.Value).Show();
                _update = false;
            }
            if (_saveTime.TotalMilliseconds == 0 && keyboardState.GetPressedKeys().Length != 0)
            {
                if (keyboardState.IsKeyDown(Keys.E)) checkIfWrong();
                if (keyboardState.IsKeyDown(Keys.A)) checkIfRight();
                _saveTime = gameTime.TotalGameTime;
            }
            else if (_saveTime.TotalMilliseconds != 0 && _saveTime.TotalMilliseconds <= gameTime.TotalGameTime.TotalMilliseconds - 200) _saveTime = new TimeSpan(0, 0, 0);

            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_firstUpdate)
            {
                
                camera.Zoom = _zoom;
                _player.position = new Vector2(_player.positionVirt.X * textMap.TileWidth, _player.positionVirt.Y * textMap.TileHeight);
                _firstUpdate = !_firstUpdate;
            }
            

                _player.checkMove(keyboardState);

            _player.heroAnimations.Update(deltaSeconds);
            _player.heroSprite.Position = new Vector2(_player.position.X + textMap.TileWidth / 2, _player.position.Y + textMap.TileHeight / 2);

            checkCamera(camera);

           
            if (keyboardState.IsKeyDown(Keys.F9)) _player.collisionLayer.IsVisible = !_player.collisionLayer.IsVisible;
        }

        private void invincible()
        {
            _invinc = true;
            _invincible = new TimeSpan();
        }

        private void mapPop()
        {
            bool flag = false;
            if (count >= 80) return;
            do
            {
                _randomNumber = _random.Next(72, 81);
                _randomNumber2 = _random.Next(0, _ligne);
                if (_r[_randomNumber + _calculator[_randomNumber2]] == 0)
                {
                    count++;
                    flag = true;
                    _r[_randomNumber + _calculator[_randomNumber2]] = 1614;
                    _poppers.IsVisible = false;
                    _poppers = textMap.CreateTileLayer("Letter", 17, 17, _r);
                    _poppers.IsVisible = true;
                    _popTime = new TimeSpan();
                }
                
            } while (!flag);
            
        }

        private void checkIfWrong()
        {
            if(!_current.IsValid)
            {
                number++;
                if(number == 30)
                {
                    _success.Play();
                    _runtimeData.DialogBox.AddDialog("Tu as gagné !", 2).Show();
                    if(_runtimeData.DataSave.District == DISTRICT)
                    {
                        _runtimeData.DataSave.District = DISTRICT + 1;
                        _runtimeData.DataSave.ClearMiniGames();
                        _runtimeData.DataSave.Save();

                    }
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
                }
                instanceWorld();
            }
            else
            {
                _runtimeData.LooseLive();
                _hurt.Play();

                if(_runtimeData.Lives == 0)
                {
                    _runtimeData.DialogBox.AddDialog("tu as perdu !", 2);
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
                instanceWorld();
            }
        }

        private void checkIfRight()
        {
            if (_current.IsValid)
            {
                number++;
                if(number == 30)
                {
                    _success.Play();
                    _runtimeData.DialogBox.AddDialog("Tu as gagné !", 2).Show();
                    if (_runtimeData.DataSave.District == DISTRICT)
                    {
                        _runtimeData.DataSave.District = DISTRICT + 1;
                        _runtimeData.DataSave.ClearMiniGames();
                        _runtimeData.DataSave.Save();

                    }
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
                }
                instanceWorld();
               
            }
            else
            {
                _runtimeData.LooseLive();
                _hurt.Play();

                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.DialogBox.AddDialog("tu as perdu !", 2);
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
                instanceWorld();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            spriteBatch.Draw(actualTextMap, gameTime: _runtimeData.GameTime);
            _player.Draw(spriteBatch);


            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: frozenMatrix);


            int time = TIME_FINISH - Convert.ToInt32((_actualTime.Subtract(_timer).TotalSeconds));
            spriteBatch.DrawString(_font, time.ToString(), new Vector2(_runtimeData.Scene.Width - 50, _runtimeData.Scene.Height - 50), Color.White);
            spriteBatch.DrawString(_font, number.ToString() + " / 30", new Vector2(_runtimeData.Scene.Width - 125, _runtimeData.Scene.Height - 100), Color.White);
            spriteBatch.End();
        }

        public override void Execute(params string[] param)
        {
            switch (param[0])
            {
                case "movePlayer":
                    try { MoveTo(new Vector2(Int32.Parse(param[1]), Int32.Parse(param[2]))); }
                    catch { Console.WriteLine("use : movePlayer {x] {y}"); }
                    break;
                default:
                    Console.WriteLine("Can't find method to invoke in Map Class");
                    break;
            }
        }

        internal override void Start()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music);
        }

        private void checkCamera(Camera2D camera)
        {
            camera.LookAt(new Vector2(_player.position.X, _player.position.Y));
            if (OutOfScreenTop(camera)) camera.LookAt(new Vector2(_player.position.X, -_runtimeData.Scene.Height / _zoom + _runtimeData.Scene.Height / 2));
            if (OutOfScreenLeft(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / _zoom + _runtimeData.Scene.Width / 2, _player.position.Y));
            if (OutOfScreenRight(camera)) camera.LookAt(new Vector2(textMap.WidthInPixels - (_runtimeData.Scene.Width / _zoom) * 2 + _runtimeData.Scene.Width / 2, _player.position.Y));
            if (OutOfScreenBottom(camera)) camera.LookAt(new Vector2(_player.position.X, textMap.HeightInPixels - (_runtimeData.Scene.Height / _zoom) * 2 + _runtimeData.Scene.Height / 2));

            if (OutOfScreenLeft(camera) && OutOfScreenBottom(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / _zoom + _runtimeData.Scene.Width / 2, textMap.HeightInPixels - (_runtimeData.Scene.Height / _zoom) * 2 + _runtimeData.Scene.Height / 2));
            if (OutOfScreenLeft(camera) && OutOfScreenTop(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / _zoom + _runtimeData.Scene.Width / 2, -_runtimeData.Scene.Height / _zoom + _runtimeData.Scene.Height / 2));

            if (OutOfScreenRight(camera) && OutOfScreenTop(camera)) camera.LookAt(new Vector2(textMap.WidthInPixels - (_runtimeData.Scene.Width / _zoom) * 2 + _runtimeData.Scene.Width / 2, textMap.HeightInPixels - (_runtimeData.Scene.Height / _zoom) * 2 + _runtimeData.Scene.Height / 2));
            if (OutOfScreenRight(camera) && OutOfScreenBottom(camera)) camera.LookAt(new Vector2(textMap.WidthInPixels - (_runtimeData.Scene.Width / _zoom) * 2 + _runtimeData.Scene.Width / 2, textMap.HeightInPixels - (_runtimeData.Scene.Height / _zoom) * 2 + _runtimeData.Scene.Height / 2));

        }

        private bool OutOfScreenTop(Camera2D camera)
        {
            if (camera.Position.Y < -_runtimeData.Scene.Height / _zoom) return true;
            return false;
        }
        private bool OutOfScreenLeft(Camera2D camera)
        {
            if (camera.Position.X <= -_runtimeData.Scene.Width / _zoom) return true;
            return false;
        }
        private bool OutOfScreenRight(Camera2D camera)
        {
            if (camera.Position.X >= textMap.WidthInPixels - (_runtimeData.Scene.Width / _zoom) * 2) return true;
            return false;
        }
        private bool OutOfScreenBottom(Camera2D camera)
        {
            if (camera.Position.Y >= textMap.HeightInPixels - (_runtimeData.Scene.Height / _zoom) * 2) return true;
            return false;
        }


        public void MoveTo(Vector2 vec)
        {
            _player.positionVirt = vec;
        }

        internal bool CheckColUp(TiledTile i)
        {
            foreach (TiledTile e in _poppers.Tiles)
            {
                if (i.Y == _player.positionVirt.Y - 1 && i.X == _player.positionVirt.X && e.Id == 1614)
                {
                    if (!_invinc)
                    {
                        _runtimeData.LooseLive();
                        _hurt.Play();
                        if (_runtimeData.Lives == 0)
                        {
                            _runtimeData.DialogBox.AddDialog("tu as perdu !", 2);
                            _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                        }
                        invincible();
                    }
                    return true;
                }
            }
            

            return false;
        }

        internal bool CheckColRight(TiledTile i)
        {
            foreach (TiledTile e in _poppers.Tiles)
            {
                if (i.Y == _player.positionVirt.Y - 1 && i.X == _player.positionVirt.X + 1 && e.Id == 1614)
                {
                    if (!_invinc)
                    {
                        _runtimeData.LooseLive();
                        _hurt.Play();
                        if (_runtimeData.Lives == 0)
                        {
                            _runtimeData.DialogBox.AddDialog("tu as perdu !", 2);
                            _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                        }
                        invincible();
                    }
                    return true;
                }
            }


            return false;
        }

        private void instanceWorld()
        {

            _timer = new TimeSpan(0);
            _current = _word[_random.Next(0, _word.Count)];
            _word.Remove(_current);
            _update = true;
            
        }

        internal bool CheckColLeft(TiledTile i)
        {

            foreach (TiledTile e in _poppers.Tiles)
            {
                if (i.Y == _player.positionVirt.Y - 1 && i.X == _player.positionVirt.X - 1 && e.Id == 1614)
                {
                    if (!_invinc)
                    {
                        _runtimeData.LooseLive();
                        _hurt.Play();
                        if (_runtimeData.Lives == 0)
                        {
                            _runtimeData.DialogBox.AddDialog("tu as perdu !", 2);
                            _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                        }
                        invincible();
                    }
                    return true;
                }
            }


            return false;
        }

        public void LoadWords()
        {
            Word add;
            add = new Word();
            _word.Clear();
            document = new XmlDocument();
            document.Load(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\dictionaries\GuessGame.xml");
            XmlNode root = document.DocumentElement;
            XmlNode district = root.SelectSingleNode("district[@id='" + _district + "']");

            foreach (XmlNode sentence in district.SelectNodes("sentence"))
            {
                foreach (XmlNode invalid in sentence.SelectNodes("invalid"))
                {
                    add.IsValid = false;
                    add.Value = invalid.InnerText;
                    _word.Add(add);
                }
                foreach (XmlNode valid in sentence.SelectNodes("valid"))
                {
                    add.IsValid = true;
                    add.Value = valid.InnerText;
                    _word.Add(add);
                }

            }
        }
    }
}
