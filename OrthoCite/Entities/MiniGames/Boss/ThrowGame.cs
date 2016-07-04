using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using OrthoCite.Helpers;

namespace OrthoCite.Entities.MiniGames
{

    struct WorldCollection
    {
        public string World { get; set; }
        public bool ValidOrNot { get; set; }
    }

    public class ThrowGame : MiniGame
    {

        RuntimeData _runtimeData;
        TiledMap _textureMap;
        Player _player;
        PNJ _badBoy;
        SpriteFont _font;
        SpriteFont _fontCompteur;


        SoundEffect _open;
        Song _music;
     
        const int GID_SPAWN = 1203;
        const int ZOOM = 3;
        const int FAST_SPEED_PLAYER = 8;
        const int LOW_SPEED_PLAYER = 13;
        const float SPEED_OF_SHOOT_WORD = 1.5f;
        const int SPEED_OF_SPAWN_WORD = 2000;
        const int INTERVAL_OF_TOUCH_BAD_TILE = 2000;
        const int BAD_TILE = 1622;
        const int TIME_OF_THIS_GAME = 2; //IN MINUTES
        const int DISTRICT = 3;

        List<Shoot> _ball;
        Dictionary<WorldCollection, Vector2> _badBoyShootActual;
        TimeSpan _saveTimeFire;
        TimeSpan _saveTimeShootWord;

        XmlDocument _saveOfWordXml;
        Queue<WorldCollection> _words;

        Dictionary<Direction, Texture2D> _shootTexture;

        TimeSpan _saveTimeBadTile;
        GameTime _saveGameTime;

        TimeSpan _saveCountTimeGame;

        public ThrowGame(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _runtimeData.ThrowGame = this;

            _player = new Player(TypePlayer.WithSpriteSheet, _runtimeData, "animations/Walking_V2");

            _runtimeData.Player = _player;
            
            _saveOfWordXml = new XmlDocument();
            _words = new Queue<WorldCollection>();
            _ball = new List<Shoot>();
            _badBoyShootActual = new Dictionary<WorldCollection, Vector2>();
            _shootTexture = new Dictionary<Direction, Texture2D>();
            if(_runtimeData.Lives == 0) _runtimeData.GainLive(3);
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {

            _textureMap = content.Load<TiledMap>("minigames/throwgame/throwGame");
            _font = content.Load<SpriteFont>("minigames/throwgame/font");
            _fontCompteur = content.Load<SpriteFont>("minigames/throwgame/font_compteur");
            _music = content.Load<Song>("minigames/throwgame/music");
            _open = content.Load<SoundEffect>("minigames/DoorGame/open");

            foreach (TiledTileLayer e in _textureMap.TileLayers)
            {
                if (e.Name == "Collision") _player.collisionLayer = e;
            }
            _player.collisionLayer.IsVisible = false;


            if (_player.positionVirt.Length() == 0)
            {
                foreach (TiledTile i in _player.collisionLayer.Tiles)
                {
                    if (i.Id == GID_SPAWN) _player.positionVirt = new Vector2(i.X, i.Y);
                }
            }

            _player.gidCol = 1809;
            _player.spriteFactory.Add(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 }));
            _player.spriteFactory.Add(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 27, 28, 29, 30 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 1, 2, 3, 0 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 1, 2, 3, 0}, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 13, 15, 17, 18 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.ATTACK_TOP, new SpriteSheetAnimationData(new[] { 19, 20, 13}, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.ATTACK_DOWN, new SpriteSheetAnimationData(new[] { 32, 33, 26}, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.ATTACK_LEFT, new SpriteSheetAnimationData(new[] { 5, 6, 0}, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.ATTACK_RIGHT, new SpriteSheetAnimationData(new[] { 5, 6, 0}, isLooping: false));


            _badBoy = new PNJ(TypePNJ.Dynamique, new Vector2(3, 4), new List<ItemList>(), _runtimeData, "map/pnj");
            _badBoy._positionSec = new Vector2(12, 4);
            _badBoy.spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 }));
            _badBoy.spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 0, 1, 2 }, isLooping: false));
            _badBoy.spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
            _badBoy.spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
            _badBoy.spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 36, 37, 38 }, isLooping: false));


            _player.separeFrame = 0;
            _player.lowFrame = LOW_SPEED_PLAYER;
            _player.fastFrame = FAST_SPEED_PLAYER;
            _player.typeDeplacement = TypeDeplacement.WithKey;

            _player.LoadContent(content);

            _badBoy.PNJPlayer.separeFrame = 0;
            _badBoy.PNJPlayer.lowFrame = LOW_SPEED_PLAYER;
            _badBoy.PNJPlayer.fastFrame = FAST_SPEED_PLAYER;
            _badBoy.PNJPlayer.typeDeplacement = TypeDeplacement.WithDirection;
            _badBoy.PNJPlayer.collisionLayer = _player.collisionLayer;
            _badBoy.PNJPlayer.gidCol = 1809;
            _badBoy.LoadContent(content, graphicsDevice);


            _shootTexture.Add(Direction.UP, content.Load<Texture2D>("minigames/throwgame/ATTACK_TOP"));
            _shootTexture.Add(Direction.DOWN, content.Load<Texture2D>("minigames/throwgame/ATTACK_DOWN"));
            _shootTexture.Add(Direction.LEFT, content.Load<Texture2D>("minigames/throwgame/ATTACK_LEFT"));
            _shootTexture.Add(Direction.RIGHT, content.Load<Texture2D>("minigames/throwgame/ATTACK_RIGHT"));
            _runtimeData.PNJ.Clear();
            _runtimeData.PNJ.Add(ListPnj.THROWGAME,_badBoy);

            LoadAllWord();
            Start();
            
            
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (_runtimeData.Lives == 0) _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
            if (_words.Count == 0 || CountTimeOfGame(gameTime))
            {
                if (_runtimeData.DataSave.District == DISTRICT)
                {
                    _runtimeData.DataSave.District = DISTRICT + 1;
                    _runtimeData.DataSave.ClearMiniGames();
                    _runtimeData.DataSave.Save();
                }
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);

            }

            CountTimeOfGame(gameTime);

            _saveGameTime = gameTime;
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            camera.Zoom = ZOOM;

            badBoyShoot(gameTime);
            badBoyCheckShoot();

            if (keyboardState.IsKeyDown(Keys.Space)) launchAShoot(gameTime);
            checkShoot();

            foreach(Shoot i in _ball)
            {
                i.Update();
            }

            _badBoy.Update(gameTime, keyboardState, camera, deltaSeconds);

            _player.checkMove(keyboardState);
            _player.heroAnimations.Update(deltaSeconds);
            _player.heroSprite.Position = new Vector2(_player.position.X + _textureMap.TileWidth / 2, _player.position.Y + _textureMap.TileHeight / 2);

            checkCamera(camera);
            if (keyboardState.IsKeyDown(Keys.F9)) _player.collisionLayer.IsVisible = !_player.collisionLayer.IsVisible;
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            _textureMap.Draw(spriteBatch);
            foreach (Shoot i in _ball)
            {
                i.Draw(spriteBatch);
            }

            _player.Draw(spriteBatch);
            _badBoy.Draw(spriteBatch);


            foreach (KeyValuePair<WorldCollection, Vector2> i in _badBoyShootActual)
            {
                spriteBatch.DrawString(_font, i.Key.World, i.Value, Color.White);
            }
            spriteBatch.End();

            string tmpTime =  (TIME_OF_THIS_GAME * 60 - Math.Floor(_saveGameTime.TotalGameTime.Subtract(_saveCountTimeGame).TotalSeconds)).ToString();
            spriteBatch.Begin(transformMatrix:frozenMatrix);
            spriteBatch.DrawString(_fontCompteur, tmpTime, new Vector2(_runtimeData.Scene.Width - _fontCompteur.MeasureString(tmpTime).X, _runtimeData.Scene.Height - _fontCompteur.MeasureString(tmpTime).Y), Color.White);
            spriteBatch.End();
        }



        public override void Execute(params string[] param)
        {
            switch (param[0])
            {
                case "movePlayer":
                    try { MoveTo(new Vector2(Int32.Parse(param[1]), Int32.Parse(param[2]))); }
                    catch { Console.WriteLine("use : movePlayer {x} {y}"); }
                    break;
                case "exit":
                    try
                    {
                        _runtimeData.OrthoCite.Exit();
                    }
                    catch { Console.WriteLine("Can't Exit"); }
                    break;
                case "giveLife":
                    try { _runtimeData.GainLive(Int32.Parse(param[1])); Console.WriteLine($"You give {param[1]}"); }
                    catch { Console.WriteLine("use : giveLife {nbLife}"); }
                    break;
                case "clearText":
                    try { _runtimeData.DialogBox.AddDialog("SetEmpty", 2).Show(); }
                    catch { Console.WriteLine("use : error"); }
                    break;
                case "setLife":
                    try
                    {
                        if (Int32.Parse(param[1]) < _runtimeData.Lives)
                        {
                            int liveTmp = _runtimeData.Lives - Int32.Parse(param[1]);
                            _runtimeData.LooseLive(liveTmp);
                        }
                        else
                        {
                            int liveTmp = Int32.Parse(param[1]) - _runtimeData.Lives;
                            _runtimeData.GainLive(liveTmp);
                        }
                    }
                    catch { Console.WriteLine("use : error"); }
                    break;
                default:
                    Console.WriteLine($"Can't find method to invoke in {this.ToString()}");
                    break;
            }
        }

        internal override void Start()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music);
        }


        private bool CountTimeOfGame(GameTime gameTime)
        {
            if (_saveCountTimeGame.TotalMilliseconds == 0) _saveCountTimeGame = gameTime.TotalGameTime;
            else if (_saveCountTimeGame.TotalMinutes <= gameTime.TotalGameTime.TotalMinutes - TIME_OF_THIS_GAME) return true;
            return false;
        }


        private void badBoyCheckShoot()
        {
            List<WorldCollection> _tmpToUpdate = new List<WorldCollection>();

            foreach (KeyValuePair<WorldCollection, Vector2> i in _badBoyShootActual)
            {
                _tmpToUpdate.Add(i.Key);
            }

            foreach(WorldCollection i in _tmpToUpdate)
            {
                _badBoyShootActual[i] += new Vector2(0, SPEED_OF_SHOOT_WORD);
            }
            
            List<WorldCollection> _tmpToDel = new List<WorldCollection>();

            foreach(KeyValuePair<WorldCollection, Vector2> i in _badBoyShootActual)
            {
                if(i.Value.Y >= _textureMap.HeightInPixels - 150)
                {
                    if (i.Key.ValidOrNot) _runtimeData.DialogBox.AddDialog("Bien Joué !", 1);
                    else if (_runtimeData.LooseLive()) _runtimeData.DialogBox.AddDialog("Dommage !", 1);
                    else _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                    
                    _tmpToDel.Add(i.Key);
                }
            }

            foreach (WorldCollection i in _tmpToDel)
            {
                _badBoyShootActual.Remove(i);
            }


        }

        private void badBoyShoot(GameTime gameTime)
        {
            if (_saveTimeShootWord.TotalMilliseconds == 0 && _words.Count != 0)
            {
                WorldCollection worldCollec = _words.Dequeue();

                if(_badBoyShootActual.ContainsKey(worldCollec)) { _words.Enqueue(worldCollec); return; }

                    _badBoyShootActual.Add(worldCollec, new Vector2(_badBoy.PNJPlayer.position.X, _badBoy.PNJPlayer.position.Y));
                    _saveTimeShootWord = gameTime.TotalGameTime;

            }
            else if (_saveTimeShootWord.TotalMilliseconds <= gameTime.TotalGameTime.TotalMilliseconds - SPEED_OF_SPAWN_WORD) _saveTimeShootWord = new TimeSpan(0);
        }

        private void checkShoot()
        {
            List<Shoot> shootToDel = new List<Shoot>();
            bool clearDico = false;

            foreach(KeyValuePair<WorldCollection, Vector2> i in _badBoyShootActual)
            {
                foreach(Shoot e in _ball)
                {
                    if (e.Position.X <= 0 || e.Position.Y <= 0 || e.Position.X >= _textureMap.WidthInPixels || e.Position.Y >= _textureMap.HeightInPixels) shootToDel.Add(e);

                        if (e.Position.X < i.Value.X + _font.MeasureString(i.Key.World).X &&
                           e.Position.X > i.Value.X &&
                           e.Position.Y < i.Value.Y + _font.MeasureString(i.Key.World).Y &&
                            e.Position.Y > i.Value.Y - _textureMap.TileHeight) 
                        {
                            shootToDel.Add(e);
                            clearDico = true;
                        if (i.Key.ValidOrNot) { _runtimeData.LooseLive(); _runtimeData.DialogBox.AddDialog("Non, c'etait un bon mot !", 2); }
                        else _runtimeData.DialogBox.AddDialog("Bah mauvais, vas t'en ! ", 1).Show();
                        }
                    
                }
                if (clearDico) break;
            }
            if (clearDico) _badBoyShootActual.Clear();

            foreach(Shoot e in shootToDel)
            {
                _ball.Remove(e);
            }

            
        }

        private void launchAShoot(GameTime gameTime)
        {
            if (_saveTimeFire.TotalMilliseconds == 0)
            {
                _saveTimeFire = gameTime.TotalGameTime;
                _ball.Add(new Shoot(_runtimeData , _player.lastDir, _shootTexture));
                _player.AttackEventNow();
            }
            else if (_saveTimeFire.TotalMilliseconds <= gameTime.TotalGameTime.TotalMilliseconds - 200) _saveTimeFire = new TimeSpan(0);
        }

        public void CheckBadTile(TiledTile i)
        {
            if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y && i.Id == BAD_TILE)
            {
                if (_saveTimeBadTile.TotalMilliseconds == 0)
                {
                    _saveTimeBadTile = _saveGameTime.TotalGameTime;
                    _runtimeData.LooseLive();
                }
                else if (_saveTimeBadTile.TotalMilliseconds <= _saveGameTime.TotalGameTime.TotalMilliseconds - INTERVAL_OF_TOUCH_BAD_TILE) _saveTimeBadTile = new TimeSpan(0);
            }
        }

        private void checkCamera(Camera2D camera)
        {
            camera.LookAt(new Vector2(_player.position.X, _player.position.Y));
            if (OutOfScreenTop(camera)) camera.LookAt(new Vector2(_player.position.X, -_runtimeData.Scene.Height / ZOOM + _runtimeData.Scene.Height / 2));
            if (OutOfScreenLeft(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / ZOOM + _runtimeData.Scene.Width / 2, _player.position.Y));
            if (OutOfScreenRight(camera)) camera.LookAt(new Vector2(_textureMap.WidthInPixels - (_runtimeData.Scene.Width / ZOOM) * 2 + _runtimeData.Scene.Width / 2, _player.position.Y));
            if (OutOfScreenBottom(camera)) camera.LookAt(new Vector2(_player.position.X, _textureMap.HeightInPixels - (_runtimeData.Scene.Height / ZOOM) * 2 + _runtimeData.Scene.Height / 2));

            if (OutOfScreenLeft(camera) && OutOfScreenBottom(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / ZOOM + _runtimeData.Scene.Width / 2, _textureMap.HeightInPixels - (_runtimeData.Scene.Height / ZOOM) * 2 + _runtimeData.Scene.Height / 2));
            if (OutOfScreenLeft(camera) && OutOfScreenTop(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / ZOOM + _runtimeData.Scene.Width / 2, -_runtimeData.Scene.Height / ZOOM + _runtimeData.Scene.Height / 2));

            if (OutOfScreenRight(camera) && OutOfScreenTop(camera)) camera.LookAt(new Vector2(_textureMap.WidthInPixels - (_runtimeData.Scene.Width / ZOOM) * 2 + _runtimeData.Scene.Width / 2, -_runtimeData.Scene.Height / ZOOM + _runtimeData.Scene.Height / 2));
            if (OutOfScreenRight(camera) && OutOfScreenBottom(camera)) camera.LookAt(new Vector2(_textureMap.WidthInPixels - (_runtimeData.Scene.Width / ZOOM) * 2 + _runtimeData.Scene.Width / 2, _textureMap.HeightInPixels - (_runtimeData.Scene.Height / ZOOM) * 2 + _runtimeData.Scene.Height / 2));

        }

        private bool OutOfScreenTop(Camera2D camera)
        {
            if (camera.Position.Y < -_runtimeData.Scene.Height / ZOOM) return true;
            return false;
        }
        private bool OutOfScreenLeft(Camera2D camera)
        {
            if (camera.Position.X <= -_runtimeData.Scene.Width / ZOOM) return true;
            return false;
        }
        private bool OutOfScreenRight(Camera2D camera)
        {
            if (camera.Position.X >= _textureMap.WidthInPixels - (_runtimeData.Scene.Width / ZOOM) * 2) return true;
            return false;
        }
        private bool OutOfScreenBottom(Camera2D camera)
        {
            if (camera.Position.Y >= _textureMap.HeightInPixels - (_runtimeData.Scene.Height / ZOOM) * 2) return true;
            return false;
        }

        public void MoveTo(Vector2 vec)
        {
            _player.positionVirt = vec;
            _player.UpdateThePosition();
        }


        public void LoadAllWord()
        {
            _saveOfWordXml.Load(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\dictionaries\ThrowGame.xml");

            XmlNode elementWords = _saveOfWordXml.DocumentElement;
            XmlNode districtElement = elementWords.SelectSingleNode("district");

            foreach (XmlNode world in districtElement.SelectNodes("word"))
            {
                WorldCollection i = new WorldCollection();
                i.World = world.InnerText;
                if (world.Attributes["typeWorld"].Value == "true") i.ValidOrNot = true;
                else i.ValidOrNot = false;

                _words.Enqueue(i);
                
            }
        }

    }
}



