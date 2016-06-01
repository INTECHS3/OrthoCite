using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
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
    public class DoorGame : MiniGame
    {
        enum TypeObject
        {
            Panneau, 
            Porte
        }
        RuntimeData _runtimeData;
        public TiledMap textMap;
        Helpers.Player _player;
        SpriteFont _font;
        Random _random;

        int _gidStart;
        const int _gidSpawn = 46;
        int _district;
        int _level = 1;

        const int _fastSpeedPlayer = 8;
        const int _lowSpeedPlayer = 13;
        const int _zoom = 3;
        bool _firstUpdate;

        struct Word
        {
            public string Value;
            public bool IsValid;
        }

        struct WordCollection
        {
            public Word Valid;
            public List<Word> Invalid;

            public WordCollection(string valid)
            {
                Valid = new Word { IsValid = true, Value = valid };
                Invalid = new List<Word>();
            }

            public void AddInvalid(string invalid)
            {
                Invalid.Add(new Word { Value = invalid });
            }
        }

        List<WordCollection> _wordCollections;

        Dictionary<int, List<string>> _world;
        
        

        




        public DoorGame(RuntimeData runtimeData)
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
            _runtimeData.DoorGame = this;
            _wordCollections = new List<WordCollection>();

        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            textMap = content.Load<TiledMap>("minigames/DoorGame/sallePorte");
            _font = content.Load<SpriteFont>("minigames/platformer/font");
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
            _player.spriteFactory.Add(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 5,10 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 19, 13, 24, 13 }, isLooping: false));

            _player.LoadContent(content);
            LoadWords();

            WordCollection words = _wordCollections[_random.Next(0, _wordCollections.Count)];
            _wordCollections.Remove(words);
            instanceWorld();

        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_firstUpdate)
            {
                camera.Zoom = _zoom;
                _player.position = new Vector2(_player.positionVirt.X * textMap.TileWidth, _player.positionVirt.Y * textMap.TileHeight);
                _firstUpdate = !_firstUpdate;
            }

            _player.checkMove(keyboardState, camera);

            _player.heroAnimations.Update(deltaSeconds);
            _player.heroSprite.Position = new Vector2(_player.position.X + textMap.TileWidth / 2, _player.position.Y + textMap.TileHeight / 2);

            checkCamera(camera);

            if(_level == 5)
            {
                _runtimeData.DialogBox.AddDialog("Gagné !", 2).Show();
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
            }
            if (keyboardState.IsKeyDown(Keys.F9)) _player.collisionLayer.IsVisible = !_player.collisionLayer.IsVisible;
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            spriteBatch.Draw(textMap, gameTime: _runtimeData.GameTime);

            _player.Draw(spriteBatch);


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
            foreach(KeyValuePair<int, List<string>> e in _world)
            {
                if (i.Y == _player.positionVirt.Y - 1 && i.X == _player.positionVirt.X && i.Id == e.Key)
                {
                    
                        if (e.Value[0] == TypeObject.Panneau.ToString())
                        {
                        _runtimeData.DialogBox.SetText(e.Value[2].ToString());
                        _runtimeData.DialogBox.Show();
                        

                    }
                        else if(e.Value[0] == TypeObject.Porte.ToString() && e.Value[1] == true.ToString())
                        {
                            foreach (TiledTile a in _player.collisionLayer.Tiles)
                            {
                                if (a.Id == _gidSpawn) _player.positionVirt = new Vector2(a.X, a.Y);
                            }
                            _level += 1;
                        _runtimeData.DialogBox.SetText("Niveau " + _level);
                        _runtimeData.DialogBox.Show();
                        instanceWorld();
                        
                       
                    }
                    return true;
                }
            }
            
            return false;
        }

        internal bool CheckColRight(TiledTile i)
        {
            foreach (KeyValuePair<int, List<string>> e in _world)
            {
                if (i.Y == _player.positionVirt.Y && i.X == _player.positionVirt.X  + 1 && i.Id == e.Key)
                {
                   
                    if (e.Value[0] == TypeObject.Panneau.ToString())
                    {
                      
                    }
                    
                    return true;
                }
            }
            return false;
        }

        internal bool CheckColLeft(TiledTile i)
        {
            foreach (KeyValuePair<int, List<string>> e in _world)
            {
                if (i.Y == _player.positionVirt.Y && i.X == _player.positionVirt.X - 1 && i.Id == e.Key)
                {
                    if (e.Value[0] == TypeObject.Panneau.ToString())
                    {
                      

                    }
                    return true;
                }
            }
            return false;   
        }

        private void instanceWorld()
        {
            _world = new Dictionary<int, List<string>>();
            WordCollection words = _wordCollections[_random.Next(0, _wordCollections.Count)];
            _wordCollections.Remove(words);
            int validWord = _random.Next(1, 4);
            Console.WriteLine(validWord);
            Word word = words.Invalid[_random.Next(0, words.Invalid.Count)];
            words.Invalid.Remove(word);


            _world.Add(564, new List<string>(3));
            _world[564].Add(TypeObject.Panneau.ToString());
            if (validWord == 1 )
            {
                _world[564].Add(true.ToString());
                _world[564].Add(words.Valid.Value.ToString());
                
            }
            else
            {
                _world[564].Add(false.ToString());
                _world[564].Add(word.Value.ToString());
                
            }
            
            _world.Add(563, new List<string>(3));
            _world[563].Add(TypeObject.Porte.ToString());
            if (validWord == 1)
            {
                _world[563].Add(true.ToString());
            }
            else
            {
                _world[563].Add(false.ToString());
            }
            

            if(validWord != 1)
            {
                word = words.Invalid[_random.Next(0, words.Invalid.Count)];
                words.Invalid.Remove(word);
            }

            _world.Add(573, new List<string>(3));
            _world[573].Add(TypeObject.Panneau.ToString());
            if (validWord == 2)
            {
                _world[573].Add(true.ToString());
                _world[573].Add(words.Valid.Value.ToString());

            }
            else
            {
                _world[573].Add(false.ToString());
                _world[573].Add(word.Value.ToString());
            }

            _world.Add(565, new List<string>(3));
            _world[565].Add(TypeObject.Porte.ToString());
            if (validWord == 2)
            {
                _world[565].Add(true.ToString());
            }
            else
            {
                _world[565].Add(false.ToString());
            }

            if(validWord != 2 && validWord != 3)
            {
                word = words.Invalid[_random.Next(0, words.Invalid.Count)];
                words.Invalid.Remove(word);
            }
            
            _world.Add(574, new List<string>(3));
            _world[574].Add(TypeObject.Panneau.ToString());
            if(validWord == 3)
            {
                _world[574].Add(true.ToString());
                _world[574].Add(words.Valid.Value.ToString());
            }
            else
            {
                _world[574].Add(false.ToString());
                _world[574].Add(word.Value.ToString());
            }
 
            _world.Add(566, new List<string>(3));
            _world[566].Add(TypeObject.Porte.ToString());
            if(validWord == 3)
            {
                _world[566].Add(true.ToString());
            }
            else
            {
                _world[566].Add(false.ToString());
            }

        }

        public void LoadWords()
        {
            _wordCollections.Clear();
            XmlDocument document = new XmlDocument();
            document.Load(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\dictionaries\DoorGame.xml");
            XmlNode root = document.DocumentElement;
            XmlNode district = root.SelectSingleNode("district[@id='" + _district + "']");

            foreach (XmlNode sentence in district.SelectNodes("sentence"))
            {
                WordCollection collection = new WordCollection(sentence.SelectSingleNode("valid").InnerText);
                foreach (XmlNode invalid in sentence.SelectNodes("invalid"))
                {
                    collection.AddInvalid(invalid.InnerText);
                }

                _wordCollections.Add(collection);
            }
        }

    }
}
