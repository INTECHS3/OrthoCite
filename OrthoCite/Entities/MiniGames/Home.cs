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
using OrthoCite.Helpers;


namespace OrthoCite.Entities.MiniGames
{


    public class Home : MiniGame
    {
        RuntimeData _runtimeData;
        public TiledMap textMap;
        TiledMap actualTextMap;
        Helpers.Player _player;
        SpriteFont _font;
        SoundEffect _hurt;
        SoundEffect _success;
        Song _music;

        int _gidStart;
        const int _gidSpawn = 46;
        const int _fastSpeedPlayer = 8;
        const int _lowSpeedPlayer = 13;
        const int _zoom = 3;
        bool _firstUpdate;
       


        public Home(RuntimeData runtimeData)
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


        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            textMap = content.Load<TiledMap>("minigames/Home/Home");
            actualTextMap = textMap;
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
            addAllPnj(content, graphicsDevice);

            _player.gidCol = 633;
            _player.spriteFactory.Add(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 }));
            _player.spriteFactory.Add(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 5, 10 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 19, 13, 24, 13 }, isLooping: false));
            _player.LoadContent(content);
            Start();

        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach(TiledTile i in _player.collisionLayer.Tiles )
            {
                if(i.Id == 44 && i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y) _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
            }
            if (_firstUpdate)
            {

                camera.Zoom = _zoom;
                _player.position = new Vector2(_player.positionVirt.X * textMap.TileWidth, _player.positionVirt.Y * textMap.TileHeight);
                _firstUpdate = !_firstUpdate;
            }

            bool tmpCheckTalk = false;
            foreach (KeyValuePair<ListPnj, PNJ> i in _runtimeData.PNJ)
            {
                i.Value.Update(gameTime, keyboardState, camera, deltaSeconds);
                if (i.Value.inTalk) tmpCheckTalk = true;
            }
            if (tmpCheckTalk) return;
            _player.checkMove(keyboardState);

            _player.heroAnimations.Update(deltaSeconds);
            _player.heroSprite.Position = new Vector2(_player.position.X + textMap.TileWidth / 2, _player.position.Y + textMap.TileHeight / 2);

            checkCamera(camera);


            if (keyboardState.IsKeyDown(Keys.F9)) _player.collisionLayer.IsVisible = !_player.collisionLayer.IsVisible;
        }


       

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            //spriteBatch.Draw(actualTextMap, gameTime: _runtimeData.GameTime);
            actualTextMap.Draw(cameraMatrix);
            _player.Draw(spriteBatch);

            foreach (KeyValuePair<ListPnj, PNJ> i in _runtimeData.PNJ)
            {
                i.Value.Draw(spriteBatch);
            }


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
                if (i.Y == _player.positionVirt.Y - 1 && i.X == _player.positionVirt.X )
                {
                    return true;
                }
            return false;
        }

        internal bool CheckColRight(TiledTile i)
        {
                if (i.Y == _player.positionVirt.Y - 1 && i.X == _player.positionVirt.X + 1)
                {
                    return true;
                }
            return false;
        }

       
        internal bool CheckColLeft(TiledTile i)
        {

                if (i.Y == _player.positionVirt.Y - 1 && i.X == _player.positionVirt.X - 1 )
                {
                    return true;
                }
            return false;
        }

        private void addAllPnj(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _runtimeData.pnj.Clear();
            if(_runtimeData.gidLast == 2064)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_1, new PNJ(TypePNJ.Static, new Vector2(11, 7), new List<ItemList>(), _runtimeData, "map/pnj"));
                _runtimeData.PNJ[ListPnj.HOME_1].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 55 }));
                _runtimeData.PNJ[ListPnj.HOME_1].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 55 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_1].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 79 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_1].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 79 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_1].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 91 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_1]._talkAndAnswer.Add(new PnjDialog($"J'ai cru entendre que tu venais sauver la ville.", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_1]._talkAndAnswer.Add(new PnjDialog($"Accepterais-tu ce cadeau ?", new Dictionary<string, bool>() { { "Bien sûr !", true }, { "Je ne te fais pas confiance.", false } }));
                _runtimeData.PNJ[ListPnj.HOME_1].lookDir = Direction.DOWN;
                _runtimeData.PNJ[ListPnj.HOME_1].playerAnswerToPnj += answerHome_1;
               

            }
            else if(_runtimeData.gidLast == 2063)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_2, new PNJ(TypePNJ.Static, new Vector2(10, 7), new List<ItemList>(), _runtimeData, "map/pnj"));
                _runtimeData.PNJ[ListPnj.HOME_2].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 52 }));
                _runtimeData.PNJ[ListPnj.HOME_2].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 52 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_2].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_2].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_2].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 88 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_2]._talkAndAnswer.Add(new PnjDialog($"Salut toi !", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_2]._talkAndAnswer.Add(new PnjDialog($"J'ai trouvé cet letre par terre, tu la veux ?", new Dictionary<string, bool>() { { "Oui.", false }, { "Non.", true } }));
                _runtimeData.PNJ[ListPnj.HOME_2].lookDir = Direction.DOWN;
                _runtimeData.pnj[ListPnj.HOME_2].playerAnswerToPnj += answerHome_2;
             
            }
            else if (_runtimeData.gidLast == 2062)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_3, new PNJ(TypePNJ.Static, new Vector2(10, 8), new List<ItemList>(), _runtimeData, "map/actor"));
                _runtimeData.PNJ[ListPnj.HOME_3].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 7 }));
                _runtimeData.PNJ[ListPnj.HOME_3].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 7 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_3].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 31 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_3].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 31 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_3].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 43 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_3]._talkAndAnswer.Add(new PnjDialog($"Tu veut te reposé un peu ?", new Dictionary<string, bool>() { { "Avec plaisir !", false }, { "Tu es louche...", true } }));
                _runtimeData.PNJ[ListPnj.HOME_3].lookDir = Direction.DOWN;
                _runtimeData.pnj[ListPnj.HOME_3].playerAnswerToPnj += answerHome_3;
               
            }
            else if (_runtimeData.gidLast == 2061)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_4, new PNJ(TypePNJ.Static, new Vector2(8, 9), new List<ItemList>(), _runtimeData, "map/actor"));
                _runtimeData.PNJ[ListPnj.HOME_4].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 73 }));
                _runtimeData.PNJ[ListPnj.HOME_4].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 49 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_4].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 73 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_4].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 73 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_4].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 85 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_4]._talkAndAnswer.Add(new PnjDialog($"Tu m'as l'air louche...", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_4]._talkAndAnswer.Add(new PnjDialog($"Es-tu avec Lyrik ?", new Dictionary<string, bool>() { { "Oui.", false }, { "Non.", true } }));
                _runtimeData.PNJ[ListPnj.HOME_4].lookDir = Direction.RIGHT;
                _runtimeData.pnj[ListPnj.HOME_4].playerAnswerToPnj += answerHome_4;
                
            }
            else if (_runtimeData.gidLast == 2060)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_5, new PNJ(TypePNJ.Static, new Vector2(11, 9), new List<ItemList>(), _runtimeData, "map/pnj"));
                _runtimeData.PNJ[ListPnj.HOME_5].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 73 }));
                _runtimeData.PNJ[ListPnj.HOME_5].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 49 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_5].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 73 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_5].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 73 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_5].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 85 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_5]._talkAndAnswer.Add(new PnjDialog($"Bonjour !", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_5]._talkAndAnswer.Add(new PnjDialog($"Tu pourrais m'aidé rapidment ?", new Dictionary<string, bool>() { { "Bien sûr !", false }, { "Non, je n'ai pas confiance.", true } }));
                _runtimeData.PNJ[ListPnj.HOME_5].lookDir = Direction.LEFT;
                _runtimeData.pnj[ListPnj.HOME_5].playerAnswerToPnj += answerHome_5;
               
            }
            else if (_runtimeData.gidLast == 2059)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_6, new PNJ(TypePNJ.Static, new Vector2(9, 9), new List<ItemList>(), _runtimeData, "map/actor"));
                _runtimeData.PNJ[ListPnj.HOME_6].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 76 }));
                _runtimeData.PNJ[ListPnj.HOME_6].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 52 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_6].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_6].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_6].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 88 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_6]._talkAndAnswer.Add(new PnjDialog($"Tu m'as l'air fatigué", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_6]._talkAndAnswer.Add(new PnjDialog($"tu veux manger un bou avec moi ?", new Dictionary<string, bool>() { { "Ah, un bon repas !", false }, { "Sans façon !", true } }));
                _runtimeData.PNJ[ListPnj.HOME_6].lookDir = Direction.RIGHT;
                _runtimeData.pnj[ListPnj.HOME_6].playerAnswerToPnj += answerHome_6;
                
            }
            else if (_runtimeData.gidLast == 2058)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_7, new PNJ(TypePNJ.Static, new Vector2(11, 7), new List<ItemList>(), _runtimeData, "map/actor"));
                _runtimeData.PNJ[ListPnj.HOME_7].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 55 }));
                _runtimeData.PNJ[ListPnj.HOME_7].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 55 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_7].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 79 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_7].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 79 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_7].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 91 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_7]._talkAndAnswer.Add(new PnjDialog($"Ah, j'en ai ma claque de ce Lyrik !", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_7]._talkAndAnswer.Add(new PnjDialog($"Qu'est-ce que tu penses de lui, toi ?", new Dictionary<string, bool>() { { "Il est vraiment horrible !", true }, { "Il est gentil, au fond.", false } }));
                _runtimeData.PNJ[ListPnj.HOME_7].lookDir = Direction.DOWN;
                _runtimeData.pnj[ListPnj.HOME_7].playerAnswerToPnj += answerHome_7;
               
            }
            else if (_runtimeData.gidLast == 2057)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_8, new PNJ(TypePNJ.Static, new Vector2(11, 7), new List<ItemList>(), _runtimeData, "map/pnj"));
                _runtimeData.PNJ[ListPnj.HOME_8].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 4 }));
                _runtimeData.PNJ[ListPnj.HOME_8].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 4 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_8].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 28 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_8].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 28 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_8].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 40 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_8]._talkAndAnswer.Add(new PnjDialog($"J'ai déterré ça hier, je pense que ça pourra t'être utile !", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_8]._talkAndAnswer.Add(new PnjDialog($"Il paraît que ça porte malheur de refuser un cadeau.", new Dictionary<string, bool>() { { "C'est gentil, je vais le prendre.", true }, { "Je n'en veux pas !", false } }));
                _runtimeData.PNJ[ListPnj.HOME_8].lookDir = Direction.DOWN;
                _runtimeData.pnj[ListPnj.HOME_8].playerAnswerToPnj += answerHome_8;
               
            }
            else if (_runtimeData.gidLast == 2056)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_9, new PNJ(TypePNJ.Static, new Vector2(8, 8), new List<ItemList>(), _runtimeData, "map/actor"));
                _runtimeData.PNJ[ListPnj.HOME_9].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 25 }));
                _runtimeData.PNJ[ListPnj.HOME_9].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 1 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_9].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 25 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_9].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 25 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_9].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 37 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_9]._talkAndAnswer.Add(new PnjDialog($"Il paraît que tu es venu sauvé la ville", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_9]._talkAndAnswer.Add(new PnjDialog($"es-ce que c'est vrai ?", new Dictionary<string, bool>() { { "Oui, je vais tous vous sauver !", false }, { "Hein ? Euh... non, non.", true } }));
                _runtimeData.PNJ[ListPnj.HOME_9].lookDir = Direction.RIGHT;
                _runtimeData.pnj[ListPnj.HOME_9].playerAnswerToPnj += answerHome_9;
                
            }
            else if (_runtimeData.gidLast == 2055)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_10, new PNJ(TypePNJ.Static, new Vector2(9, 9), new List<ItemList>(), _runtimeData, "map/actor"));
                _runtimeData.PNJ[ListPnj.HOME_10].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 34 }));
                _runtimeData.PNJ[ListPnj.HOME_10].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 10 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_10].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 34 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_10].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 34 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_10].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 46 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_10]._talkAndAnswer.Add(new PnjDialog($"Tu m'as l'air d'être un aventurier !", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_10]._talkAndAnswer.Add(new PnjDialog($"Je peux compter sur toi pour nous sauver ?", new Dictionary<string, bool>() { { "Sans problèmes !", true }, { "Absolument pas !", false } }));
                _runtimeData.PNJ[ListPnj.HOME_10].lookDir = Direction.RIGHT;
                _runtimeData.pnj[ListPnj.HOME_10].playerAnswerToPnj += answerHome_10;
               
            }
            else if (_runtimeData.gidLast == 2054)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_11, new PNJ(TypePNJ.Static, new Vector2(11, 9), new List<ItemList>(), _runtimeData, "map/pnj"));
                _runtimeData.PNJ[ListPnj.HOME_11].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 79 }));
                _runtimeData.PNJ[ListPnj.HOME_11].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 55 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_11].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 79 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_11].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 79 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_11].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 91 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_11]._talkAndAnswer.Add(new PnjDialog($"sa te dit de boire un peti peu pour te reposer ?", new Dictionary<string, bool>() { { "Ce ne serait pas de refus !", false }, { "Je m'en passerai.", true } }));
                _runtimeData.PNJ[ListPnj.HOME_11].lookDir = Direction.LEFT;
                _runtimeData.pnj[ListPnj.HOME_11].playerAnswerToPnj += answerHome_11;
               
            }
            else if (_runtimeData.gidLast == 2053)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_12, new PNJ(TypePNJ.Static, new Vector2(11, 7), new List<ItemList>(), _runtimeData, "map/pnj"));
                _runtimeData.PNJ[ListPnj.HOME_12].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 52 }));
                _runtimeData.PNJ[ListPnj.HOME_12].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 52 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_12].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_12].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_12].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 88 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_12]._talkAndAnswer.Add(new PnjDialog($"Bonjour à toi !", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_12]._talkAndAnswer.Add(new PnjDialog($"J'ai retrouvé ça dans un vieux tiroir, ça t'intéresse ?", new Dictionary<string, bool>() { { "Oui.", true }, { "Pas du tout.", false } }));
                _runtimeData.PNJ[ListPnj.HOME_12].lookDir = Direction.DOWN;
                _runtimeData.pnj[ListPnj.HOME_12].playerAnswerToPnj += answerHome_12;
                
            }
            else if (_runtimeData.gidLast == 2051)
            {
                _runtimeData.PNJ.Add(ListPnj.HOME_13, new PNJ(TypePNJ.Static, new Vector2(11, 7), new List<ItemList>(), _runtimeData, "map/pnj"));
                _runtimeData.PNJ[ListPnj.HOME_13].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 52 }));
                _runtimeData.PNJ[ListPnj.HOME_13].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 52 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_13].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_13].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.HOME_13].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 88 }, isLooping: false));
                _runtimeData.pnj[ListPnj.HOME_13]._talkAndAnswer.Add(new PnjDialog($"Bonjour à toi !", new Dictionary<string, bool>()));
                _runtimeData.pnj[ListPnj.HOME_13]._talkAndAnswer.Add(new PnjDialog($"Tu es là pour te battre contre Lyrik ?", new Dictionary<string, bool>() { { "Oui.", true }, { "Pas du tout.", false } }));
                _runtimeData.PNJ[ListPnj.HOME_13].lookDir = Direction.DOWN;
                _runtimeData.pnj[ListPnj.HOME_13].playerAnswerToPnj += answerHome_13;
                
            }

            foreach (KeyValuePair<ListPnj, PNJ> i in _runtimeData.PNJ)
            {
                i.Value.PNJPlayer.collisionLayer = _player.collisionLayer;
                i.Value.PNJPlayer.lowFrame = _player.lowFrame;
                i.Value.PNJPlayer.fastFrame = _player.fastFrame;
                i.Value.PNJPlayer.typeDeplacement = TypeDeplacement.WithDirection;
                i.Value.PNJPlayer.gidCol = 889;
            }
            foreach (KeyValuePair<ListPnj, PNJ> i in _runtimeData.PNJ)
            {
                i.Value.LoadContent(content, graphicsDevice);
                i.Value.PNJPlayer.heroSprite.Scale = new Vector2(0.8f);
            }


        }

        private void answerHome_13(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach(ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if(i == ListPnj.HOME_13)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_13);
            if (TrueOrFalseAnswer)
            { 
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Parfait, ceci va t'aider !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Parfait, mais je t'es déjâ aidé", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("Traître, va-t'en !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_12(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_12)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_12);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("J'espère que tu pourras nous sauver de Lyrik !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Je t'es déjâ aidé, mais bien joué !", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("Oups, je pense que tu aurais dû accepter !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_11(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_11)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_11);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Arghh, tu as deviné que c'était un piège !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog(" Je suis triste, mes pièges ne marchent plus !", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("C'été de l'eau empoisonnée, je t'ai bien eu !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_10(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_10)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_10);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Ah, un homme qui a du coeur, prends donc ceci pour t'aider !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Je t'es déjâ aidé, mais tu es un homme qui a du coeur !", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("Encore un qui ne vient ici que pour la gloire, va-t'en !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_9(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_9)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_9);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Oh tu es avec nous alors ! Prend sa", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Oh tu es avec nous alors, mais je n'es plus rien à te donner !", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("Argh, prend sa du coup et vive Lyrik !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_8(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_8)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_8);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Tiens, fais bonne route !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Je t'es déjâ aidé, mais fais bonne route !", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("Je t'avais pourtant prévenu...", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_7(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_7)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_7);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Au moins, on est bien d'accord là-dessus !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("On est bien d'accord là-dessus, mais je n'es plus rien à te donner ! ", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("GENTIL ? Et puis quoi encore ?", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_6(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_6)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_6);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Tu savai que c'étai empoisonné !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Grrrrr", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("La nouriture été empoisonné ! Tu té fait avoir !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_5(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_5)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_5);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Pffff tu savé que c'était un piège, c'es pas drôle !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Même si tu savais que c'était un piège, je ne te redonnerais plus rien !", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("BAM ! Je té bien eu !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_4(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_4)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_4);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Tout va bien alors, prends donc ceci !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Je t'es déjâ aidé, mais tu es un bon garçon !", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("Ah, mécréant, sors tout de suite de chez moi !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_3(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_3)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_3);

            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Tu as deviner que j'étais avec Lyrik ? Tu es for !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Tu as déjâ deviné avant, tu ne peux pas te redonner une récompense", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("Hop ! Un coup dans le do pendan que tu te reposes, c'été trop facil !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_2(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_2)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_2);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Grrrr, comment tu as su que c'était un piège ?", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Je t'es déjâ donné ce que j'avais, mais comment tu as su que c'était un piège ?", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("Haha, tu est tombé dans mon piège c'été trop facile !", 3).Show();
                if (_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

        private void answerHome_1(RuntimeData r, bool TrueOrFalseAnswer)
        {
            bool talkOrNot = true;
            foreach (ListPnj i in _runtimeData.listPnjInHomeNotSend)
            {
                if (i == ListPnj.HOME_1)
                {
                    talkOrNot = false;
                }
            }
            _runtimeData.listPnjInHomeNotSend.Add(ListPnj.HOME_1);
            if (TrueOrFalseAnswer)
            {
                _success.Play();
                if (talkOrNot)
                {
                    r.GainLive();
                    _runtimeData.DialogBox.AddDialog("Tiens, bon courage pour la suite de ta quête !", 3).Show();
                }
                else _runtimeData.DialogBox.AddDialog("Je t'es déjâ aidé, mais bon courage pour la suite de ta quête !", 3).Show();
            }
            else
            {
                r.LooseLive();
                _hurt.Play();
                _runtimeData.DialogBox.AddDialog("Ah, tu dois être un sbire de Lyrik ! Prends-ça !", 3).Show();
                if(_runtimeData.Lives == 0)
                {
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
                }
            }
        }

    }
}
