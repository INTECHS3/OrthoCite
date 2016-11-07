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
    public class Rearranger : MiniGame
    {
        RuntimeData _runtimeData;
        public TiledMap textMap;
        public TiledMap textMap2;
        TiledMap actualTextMap;
        Helpers.Player _player;
        SpriteFont _font;
        Random _random;
        SoundEffect _open;
        SoundEffect _fail;
        SoundEffect _success;
        Song _music;
        List<Texture2D> _alphabet;
        TiledTileLayer _visible;
        Dictionary<int, Texture2D> _alphaSprite;
        Dictionary<int, int> _letters;
        int[] _currentWord;
        int[] _trueWord;
        TimeSpan _saveTime;
        TimeSpan _timer;
        const int TIME_FINISH = 60;
        TimeSpan _actualTime;

        int _gidStart;
        Texture2D _letterToDraw;
        bool _gotLetter = false;
        const int _gidSpawn = 46;
        int compteur;
        Dictionary<int, int> _table;
        int _tileToChange;
        int _letToDraw = 0;
        const int MARGIN_RIGHT = 20;
        const int MARGIN_TOP = 80;
        int _district;
        int _level = 1;
        const int _fastSpeedPlayer = 8;
        const int _lowSpeedPlayer = 13;
        const int _zoom = 3;
        bool _firstUpdate;
        List<string> _wordCollections;
        string _word;
       List<int> _world;
        char[] _wordSplit;
        int _tile;
        int _tileAlpha = 1809;
        int[] _r = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 832, 832, 832, 832, 832, 832, 832, 832, 832, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,};
        TiledTileLayer _i;
        

        public Rearranger(RuntimeData runtimeData)
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
            _wordCollections = new List<string>();
            _alphabet = new List<Texture2D>();
            _world = new List<int>();
            _runtimeData.Rearranger = this;
            _alphaSprite = new Dictionary<int, Texture2D>();
            _letters = new Dictionary<int, int>();
            _table = new Dictionary<int, int>();
            _timer = new TimeSpan(0);


            _player.spriteFactory.Add(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 }));
            _player.spriteFactory.Add(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 5, 10 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 19, 13, 24, 13 }, isLooping: false));

        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            textMap = content.Load<TiledMap>("minigames/Rearranger/Rearranger1");
            _i = new TiledTileLayer(textMap, graphicsDevice,"Letter", 17, 17, _r);
            _i.IsVisible = true;
            actualTextMap = textMap;
            _font = content.Load<SpriteFont>("minigames/Rearranger/font");
            _music = content.Load<Song>("minigames/Rearranger/music");
            _open = content.Load<SoundEffect>("minigames/Rearranger/open");
            _fail = content.Load<SoundEffect>("minigames/Rearranger/fail");
            _success = content.Load<SoundEffect>("minigames/Rearranger/success");
            _alphabet.Clear();
            for (int i = 0; i <= 25; i++)
            {
                _alphabet.Add(content.Load<Texture2D>("minigames/Rearranger/" + i));
            }
            
            foreach (TiledTileLayer e in textMap.TileLayers)
            {
                if (e.Name == "collision") _player.collisionLayer = e;
            }
            _player.collisionLayer.IsVisible = false;

            foreach (TiledTileLayer e in textMap.TileLayers)
            {
                if (e.Name == "Visible") _visible = e;
            }

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
            _alphaSprite.Clear();
            for(int i = 0; i <_alphabet.Count; i++)
            {
                _alphaSprite.Add(_tileAlpha + i, _alphabet[i]);
            }
           
            _player.gidCol = 633;

            _player.LoadContent(content);
            LoadWords();
            instanceWorld();
            Start();
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {

            if (_timer.TotalMilliseconds == 0)
            {
                _timer = gameTime.TotalGameTime;
            }
            else if (gameTime.TotalGameTime.Subtract(_timer).TotalSeconds >= TIME_FINISH)
            {
                for (int a = 140; a < 149; a++)
                {
                    _r[a] = 832;
                }
                foreach (TiledTile a in _player.collisionLayer.Tiles)
                {
                    if (a.Id == _gidSpawn) _player.positionVirt = new Vector2(a.X, a.Y);
                }
                _runtimeData.DialogBox.AddDialog("Le temps est écoulé ! le mot était " + _word, 2);
                _runtimeData.LooseLive();

                if(_runtimeData.Lives == 0)
                {
                    _runtimeData.DialogBox.AddDialog("tu as perdu !", 2);
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
                }
                instanceWorld();
            }
            _actualTime = gameTime.TotalGameTime;

            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_firstUpdate)
            {
                camera.Zoom = _zoom;
                _player.position = new Vector2(_player.positionVirt.X * textMap.TileWidth, _player.positionVirt.Y * textMap.TileHeight);
                _firstUpdate = !_firstUpdate;
            }

            if (_saveTime.TotalMilliseconds == 0 && keyboardState.GetPressedKeys().Length != 0)
            {
                if (keyboardState.IsKeyDown(Keys.E)) checkIfLetters();
                if (keyboardState.IsKeyDown(Keys.A)) verifLetters();
                _saveTime = gameTime.TotalGameTime;
            }
            else if (_saveTime.TotalMilliseconds != 0 && _saveTime.TotalMilliseconds <= gameTime.TotalGameTime.TotalMilliseconds - 200) _saveTime = new TimeSpan(0,0,0);

            _player.checkMove(keyboardState);

            _player.heroAnimations.Update(deltaSeconds);
            _player.heroSprite.Position = new Vector2(_player.position.X + textMap.TileWidth / 2, _player.position.Y + textMap.TileHeight / 2);

            checkCamera(camera);

            if (_level == 10)
            {
                _runtimeData.DialogBox.AddDialog("Gagné !", 2).Show();

                if (_runtimeData.DataSave.District == _district)
                {
                    _runtimeData.DataSave.ValidateMiniGame(DataSaveMiniGame.REARRANGER);
                    _runtimeData.DataSave.Save();
                }
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
            }
            if (keyboardState.IsKeyDown(Keys.F9)) _player.collisionLayer.IsVisible = !_player.collisionLayer.IsVisible;

            if(keyboardState.IsKeyDown(Keys.B))
            {
                IReadOnlyList<TiledLayer> m = actualTextMap.Layers;
                foreach (TiledLayer i in m)
                {
                    Console.WriteLine(i.Name);
                }
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            //spriteBatch.Draw(actualTextMap, gameTime: _runtimeData.GameTime);
            actualTextMap.Draw(cameraMatrix);

            _player.Draw(spriteBatch);
            

            spriteBatch.End();
            if(_gotLetter)
            {
                spriteBatch.Begin(transformMatrix: frozenMatrix);
                spriteBatch.Draw(_letterToDraw, new Vector2(_runtimeData.Scene.Width - MARGIN_RIGHT - 1 * (_letterToDraw.Width), MARGIN_TOP));
                spriteBatch.End();
            }


            spriteBatch.Begin(transformMatrix: frozenMatrix);
            
           
           int  time = TIME_FINISH - Convert.ToInt32((_actualTime.Subtract(_timer).TotalSeconds));
            spriteBatch.DrawString(_font, time.ToString(), new Vector2(_runtimeData.Scene.Width - 50, _runtimeData.Scene.Height - 50), Color.White);

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
            
            
            
            MediaPlayer.Play(_music);
            
            MediaPlayer.IsRepeating = true;


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
            _player.UpdateThePosition();
        }

        internal bool CheckColUp(TiledTile i)
        {
            foreach (int e in _world)
            {
                if (i.Y == _player.positionVirt.Y - 1 && i.X == _player.positionVirt.X && i.Id == e && e != 44)
                {
                    return true;

                }
                
                if (i.Y == _player.positionVirt.Y - 1 && i.X == _player.positionVirt.X && i.Id == e)
                {
                    foreach (TiledTile a in _player.collisionLayer.Tiles)
                    {
                        if (a.Id == _gidSpawn) _player.positionVirt = new Vector2(a.X, a.Y);
                    }
                    _level++;
                    if (_level < 6)
                    {
                        _runtimeData.DialogBox.AddDialog("Niveau " + _level, 2).Show();
                        for (int a = 140; a < 149; a++)
                        {
                            _r[a] = 832;
                        }
                        _open.Play();
                        instanceWorld();
                        return false;
                    }
                    else if(_level == 6)
                    {
                        _runtimeData.DialogBox.AddDialog("Gagné !", 2).Show();

                        if (_runtimeData.DataSave.District == _district)
                        {
                            _runtimeData.DataSave.ValidateMiniGame(DataSaveMiniGame.REARRANGER);
                            _runtimeData.DataSave.Save();
                        }
                        _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
                    }

                }
            }
            return false;
        }




        internal bool CheckColRight(TiledTile i)
        {

            if (i.Y == _player.positionVirt.Y && i.X == _player.positionVirt.X + 1)
            {
                return true;
            }

            return false;
        }

        internal bool CheckColLeft(TiledTile i)
        {

            if (i.Y == _player.positionVirt.Y && i.X == _player.positionVirt.X - 1)
            {
                return true;
            }

            return false;
        }

        public int returnTileAlphabet(char e)
        {
            int tile;

            if (e == 'a') tile = 1809;
            else if (e == 'b') tile = 1810;
            else if (e == 'c') tile = 1811;
            else if (e == 'd') tile = 1812;
            else if (e == 'e') tile = 1813;
            else if (e == 'f') tile = 1814;
            else if (e == 'g') tile = 1815;
            else if (e == 'h') tile = 1816;
            else if (e == 'i') tile = 1817;
            else if (e == 'j') tile = 1818;
            else if (e == 'k') tile = 1819;
            else if (e == 'l') tile = 1820;
            else if (e == 'm') tile = 1821;
            else if (e == 'n') tile = 1822;
            else if (e == 'o') tile = 1823;
            else if (e == 'p') tile = 1824;
            else if (e == 'q') tile = 1825;
            else if (e == 'r') tile = 1826;
            else if (e == 's') tile = 1827;
            else if (e == 't') tile = 1828;
            else if (e == 'u') tile = 1829;
            else if (e == 'v') tile = 1830;
            else if (e == 'x') tile = 1831;
            else if (e == 'y') tile = 1832;
            else if (e == 'z') tile = 1833;
            else tile = 0;

            return tile;
        }

        private void instanceWorld()
        {
            _timer = new TimeSpan(0);
            _world.Clear();
            _gotLetter = false;
            _world.Add(37);
            _world.Add(44);
            compteur = 0;
            _visible.IsVisible = true;
            _i.IsVisible = true;
            _letters.Clear();
            _table.Clear();
            _word = _wordCollections[_random.Next(0, _wordCollections.Count)];
            _wordCollections.Remove(_word);
            _wordSplit = _word.ToCharArray();
#if DEBUG
            Console.WriteLine(_word);
#endif
            foreach (int e in _r)
            {
                if (e == 832)
                {

                    _table.Add(compteur, 832);
                }
                compteur++;
            }

            int numberMax = _wordSplit.Length;
            int number = 0;
            int randomNumber;
            bool changed = false;
            bool inverted = false;
            _currentWord = new int[_wordSplit.Length];
            _trueWord = new int[_wordSplit.Length];

            

            foreach (char e in _wordSplit)
            {
                    _tile = returnTileAlphabet(e);
                    _trueWord[number] = _tile;
                    number++;
                    if (number == 1)
                    {
                        _r[140] = _tile;
                    _currentWord[140 - 140] = _tile;
                    _table.Remove(140);
                    _i.IsVisible = false;
                        _i = textMap.CreateTileLayer("Letter", 17, 17, _r);
                    }
                    else if (number == _wordSplit.Length )
                    {
                        _r[140 + _wordSplit.Length - 1] = _tile;
                    _currentWord[_wordSplit.Length - 1] = _tile;
                    _table.Remove(140 + _wordSplit.Length - 1);
                    _i.IsVisible = false;
                    _i = textMap.CreateTileLayer("Letter", 17, 17, _r);
                    }
                    else
                    {

                        randomNumber = _random.Next(141, 141 + _wordSplit.Length - 2);
                        do
                        {
                            if (_r[randomNumber] == 832)
                            {
                                _r[randomNumber] = _tile;
                            _currentWord[randomNumber - 140] = _tile;
                            _table.Remove(randomNumber);
                            _letters.Add(randomNumber, _tile);
                            _i.IsVisible = false;
                            _i = textMap.CreateTileLayer("Letter", 17, 17, _r);
                            
                                changed = true;

                            }
                            else
                            {
                                if (randomNumber == 140 + _wordSplit.Length - 2) inverted = true;

                                if (!inverted)
                                {
                                    randomNumber++;
                                }
                                else
                                {
                                    randomNumber--;
                                }
                            }
                        } while (changed == false);

                    inverted = false;
                    changed = false;
                }

                
                }
                
                
            
        }

        public void LoadWords()
        {
            XmlDocument document = new XmlDocument();
            document.Load(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\dictionaries\Rearranger.xml");
            XmlNode root = document.DocumentElement;
            XmlNode district = root.SelectSingleNode("district[@id='" + _district + "']");

            foreach (XmlNode sentence in district.SelectNodes("sentence"))
            {
                _wordCollections.Add(sentence.SelectSingleNode("valid").InnerText);
            }
        }

        public void checkIfLetters()
        {
            bool change = false;
            int count = 0;
            foreach (TiledTile e in _i.Tiles)
            {
                if(!_gotLetter)
                foreach (KeyValuePair<int, int> g in _letters)
                {
                    if (count == g.Key && e.X == _player.positionVirt.X && e.Y == _player.positionVirt.Y - 1)
                    {

                        _letToDraw = g.Value;
                        _tileToChange = g.Key;
                        foreach (KeyValuePair<int, Texture2D> i in _alphaSprite)
                        {
                            if (i.Key == _letToDraw)
                            {
                                _letterToDraw = i.Value;
                            }
                        }

                         editLayer(_tileToChange);
                            _gotLetter = !_gotLetter;
                            change = true;
                            break;

                    }

                }

                if (change) return;  
                if (_gotLetter)
                { 
                    foreach(KeyValuePair<int, int> a in _table)
                    {
                        if (count == a.Key && e.X == _player.positionVirt.X && e.Y == _player.positionVirt.Y -1)
                        {
                            _tileToChange = a.Key;
                                editLayer(_tileToChange, _letToDraw);
                                _gotLetter = !_gotLetter;
                            return;
                        }
                    }
                }
                count++;

            }
            
        }

        public void editLayer(int tileToChange)
        {
            if (!_gotLetter)
            {
                _r[tileToChange] = 832;
                if(tileToChange - 140 < _wordSplit.Length) _currentWord[tileToChange - 140] = 0;
                _letters.Remove(tileToChange);
                _table.Add(tileToChange, 832);
                    _i.IsVisible = false;
                _i = textMap.CreateTileLayer("Letter", 17, 17, _r);


            }
        }
        public void verifLetters()
        {
            bool good = true;
            for(int i = 0; i < _currentWord.Length; i++)
            {
                if (_currentWord[i] != _trueWord[i]) good = false;
            }
            if(good)
            {
                _runtimeData.DialogBox.AddDialog("Bravo, tu as trouvé le mot !", 2).Show();
                _world.Remove(37);
                _success.Play();
                _visible.IsVisible = false;
                _i.IsVisible = false;
                
            }
            else if(!good)
            {
                _runtimeData.DialogBox.AddDialog("Oh non, le mot était " + _word + " !", 2).Show();
                _fail.Play();
                _runtimeData.LooseLive();
                if (_runtimeData.Lives == 0)
                {
                    _i.IsVisible = false;
                    _runtimeData.DialogBox.AddDialog("Tu n'as plus de vie !", 2).Show();
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
                else
                {
                    for(int i = 140; i<149; i++)
                    {
                        _r[i] = 832;
                    }
                    foreach (TiledTile a in _player.collisionLayer.Tiles)
                    {
                        if (a.Id == _gidSpawn) _player.positionVirt = new Vector2(a.X, a.Y);
                    }
                    instanceWorld();

                }

            }
        }
        public void SetDistrict(int district)
        {
            _district = district;
        }
        public void editLayer(int tileToChange, int letterToDraw)
        {
            if (_gotLetter)
            {
                _r[tileToChange] = letterToDraw;
                if(tileToChange - 140 < _wordSplit.Length)
                {
                    _currentWord[tileToChange - 140] = letterToDraw;
                }
                
                _table.Remove(tileToChange);
                _letters.Add(tileToChange, letterToDraw);
                _i.IsVisible = false;
                _i = textMap.CreateTileLayer("Letter", 17, 17, _r);


            }    
        }
    }
}

