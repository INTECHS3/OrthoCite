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
using OrthoCite.Helpers;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

namespace OrthoCite.Entities
{
    struct District
    {
        public int NUM_DISTRICT { get; private set; }
        public Vector2 TOP_LEFT { get; private set; }
        public Vector2 DOWN_LEFT { get; private set; }
        public Vector2 TOP_RIGHT { get; private set; }
        public Vector2 DOWN_RIGHT { get; private set; }

        public District(int num, Vector2 top_left, Vector2 down_left, Vector2 top_right, Vector2 down_right)
        {
            NUM_DISTRICT = num;
            TOP_LEFT = top_left;
            DOWN_LEFT = down_left;
            TOP_RIGHT = top_right;
            DOWN_RIGHT = down_right;
        }
    }

    public class Map : IEntity
    {
        RuntimeData _runtimeData;
        public TiledMap textMap;
        TiledTileLayer _upLayer;
        Helpers.Player _player;
  
        int _gidStart;
        const int _gidSpawn = 1151;
        const int _gidCol = 889;
        const int _fastSpeedPlayer = 8;
        const int _lowSpeedPlayer = 13;
        const int _zoom = 3;
        bool _firstUpdate;

        List<District> _district;

        
        public Map(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _gidStart = _runtimeData.gidLast;

            _firstUpdate = true;

            _district = new List<District>();

            _player = new Helpers.Player(Helpers.TypePlayer.WithSpriteSheet, new Vector2(0, 0), _runtimeData, "animations/walking");
            
            _player.separeFrame = 0;
            _player.lowFrame = _lowSpeedPlayer;
            _player.fastFrame = _fastSpeedPlayer;
            _player.typeDeplacement = TypeDeplacement.WithKey;

            _runtimeData.Map = this;
            _runtimeData.Player = _player;

            MediaPlayer.Stop();
            
        }

        void IEntity.LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            textMap = content.Load<TiledMap>("map/Map");

            foreach (TiledTileLayer e in textMap.TileLayers)
            {
                if (e.Name == "Collision") _player.collisionLayer = e;
                else if (e.Name == "Up") _upLayer = e;
            }
            _player.collisionLayer.IsVisible = false;
            
            if (_gidStart != 0)
            {
                Console.Write("ok");
                foreach (TiledTile i in _player.collisionLayer.Tiles)
                {
                    if (i.Id == _gidStart) _player.positionVirt = new Vector2(i.X, i.Y + 1); 
                }
            }

            if(_player.positionVirt.Length() == 0)
            {
                foreach (TiledTile i in _player.collisionLayer.Tiles)
                {
                    if (i.Id == _gidSpawn) _player.positionVirt = new Vector2(i.X, i.Y);
                }
            }
            _runtimeData.gidLast = 0;
            _player.gidCol = _gidCol;

            _player.spriteFactory.Add(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 }));
            _player.spriteFactory.Add(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 5, 0, 10, 0 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 19, 13, 24, 13 }, isLooping: false));

            _player.LoadContent(content);

            addAllPnj(content, graphicsDevice);

            _district.Add(new District(1, new Vector2(59,40), new Vector2(59, textMap.Height), new Vector2(textMap.Width, 40), new Vector2(textMap.Width, textMap.Height)));
            _district.Add(new District(2, new Vector2(56, 0), new Vector2(56, 33), new Vector2(textMap.Width, 0), new Vector2(textMap.Width, 33)));
            _district.Add(new District(3, new Vector2(0, 0), new Vector2(0, 33), new Vector2(51,0), new Vector2(51, 33)));
            _district.Add(new District(4, new Vector2(0, 40), new Vector2(0, textMap.Height), new Vector2(46 , 40), new Vector2(46, textMap.Height)));
            
        }

        void IEntity.UnloadContent()
        {
            
        }

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {

            if(keyboardState.IsKeyDown(Keys.X)) checkDistrict();


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

            foreach(KeyValuePair<ListPnj, PNJ> i in _runtimeData.PNJ)
            {
                i.Value.Update(gameTime, keyboardState, camera, deltaSeconds);
            }


            checkCamera(camera);

            if (keyboardState.IsKeyDown(Keys.F9) && _player.separeFrame == 0) _player.collisionLayer.IsVisible = !_player.collisionLayer.IsVisible;

            //Console.WriteLine($"X : {_positionVirt.X} Y : {_positionVirt.Y} ");
        }

        
        void IEntity.Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            _upLayer.IsVisible = false;
            spriteBatch.Draw(textMap, gameTime: _runtimeData.GameTime);

            _player.Draw(spriteBatch);

            foreach (KeyValuePair<ListPnj, PNJ> i in _runtimeData.PNJ)
            {
                i.Value.Draw(spriteBatch);
            }
           

            _upLayer.IsVisible = true;
            _upLayer.Draw(spriteBatch);
            
            spriteBatch.End();
            
        }
        
        void IEntity.Execute(params string[] param)
        {
            
            switch(param[0])
            {
                case "movePlayer":
                    try{ MoveTo(new Vector2(Int32.Parse(param[1]), Int32.Parse(param[2]))); }
                    catch { Console.WriteLine("use : movePlayer {x} {y}"); }
                    break;
                case "changeDistrict":
                    try { _runtimeData.DataSave.District = Byte.Parse(param[1]);
                        _runtimeData.DataSave.Save();
                        _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
                        Console.WriteLine($"District is change to : {_runtimeData.DataSave.District}" );
                    }
                    catch { Console.WriteLine("use : changeDistrict {nbDistrict}"); }
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
                default:
                    Console.WriteLine("Can't find method to invoke in Map Class");
                    break;
            }
        }

        private void checkDistrict()
        {
            foreach (District dis in _district)
            {
                if (_player.positionVirt.X >= dis.TOP_LEFT.X && _player.positionVirt.Y >= dis.TOP_LEFT.Y &&
                    _player.positionVirt.X >= dis.DOWN_LEFT.X && _player.positionVirt.Y <= dis.DOWN_LEFT.Y &&
                    _player.positionVirt.X <= dis.TOP_RIGHT.X && _player.positionVirt.Y >= dis.TOP_RIGHT.Y &&
                    _player.positionVirt.X <= dis.DOWN_RIGHT.X && _player.positionVirt.Y <= dis.DOWN_RIGHT.Y
                    ) { _runtimeData.DistrictActual = dis.NUM_DISTRICT; break;  }
               
            }
        }


        private void addAllPnj(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _runtimeData.PNJ = new Dictionary<ListPnj, PNJ>();
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_1_1, new PNJ(TypePNJ.Dynamique, new Vector2(120, 59), new List<ItemList>(), _runtimeData, "map/pnj"));

            if(_runtimeData.DataSave.District != 4)
            {
                Vector2 tmpPostionOfPNJ = new Vector2();
                if (_runtimeData.DataSave.District == 1) tmpPostionOfPNJ = new Vector2(75, 40);
                else if (_runtimeData.DataSave.District == 2) tmpPostionOfPNJ = new Vector2(56, 15);
                else if (_runtimeData.DataSave.District == 3) tmpPostionOfPNJ = new Vector2(38, 32);

                _runtimeData.PNJ.Add(ListPnj.PORTAILBLOCK, new PNJ(TypePNJ.Static, tmpPostionOfPNJ, new List<ItemList>(), _runtimeData, "map/pnj"));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 , 1, 2}, isLooping: true));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 0, 1, 2 }, isLooping: true));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 36, 37, 38 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK]._curentTalker = TypeTalkerPNJ.Talk;
                if (_runtimeData.DataSave.District == 1) _runtimeData.PNJ[ListPnj.PORTAILBLOCK].lookDir = Direction.NONE;
                else if (_runtimeData.DataSave.District == 2) _runtimeData.PNJ[ListPnj.PORTAILBLOCK].lookDir = Direction.RIGHT;
                else if (_runtimeData.DataSave.District == 3) _runtimeData.PNJ[ListPnj.PORTAILBLOCK].lookDir = Direction.UP;
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK]._talkAndAnswer.Add($"Désolé tu n'as pas fini les mini jeux du quartier {_runtimeData.DataSave.District}, tu ne peux pas avancer !", new Dictionary<string, bool>());
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK]._talkAndAnswer.Add($"Fini et reviens me voir après ! ", new Dictionary<string, bool>());
            }

            

            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 0, 1, 2}, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 24,25 ,26 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 36, 37, 38}, isLooping: false));

            _runtimeData.PNJ[ListPnj.QUARTIER_1_1]._positionSec = new Vector2(126,64);

            _runtimeData.PNJ[ListPnj.QUARTIER_1_1]._curentTalker = TypeTalkerPNJ.AnswerTalk;
            
            _runtimeData.PNJ[ListPnj.QUARTIER_1_1]._talkAndAnswer.Add("Bienvenu sur OrthoCité", new Dictionary<string, bool>());


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

        private void checkCamera(Camera2D camera)
        {
            camera.LookAt(new Vector2(_player.position.X, _player.position.Y));
            if (OutOfScreenTop(camera)) camera.LookAt(new Vector2(_player.position.X, -_runtimeData.Scene.Height / _zoom + _runtimeData.Scene.Height / 2));
            if (OutOfScreenLeft(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / _zoom + _runtimeData.Scene.Width / 2, _player.position.Y));
            if (OutOfScreenRight(camera)) camera.LookAt(new Vector2(textMap.WidthInPixels - (_runtimeData.Scene.Width / _zoom) * 2 + _runtimeData.Scene.Width / 2, _player.position.Y));
            if (OutOfScreenBottom(camera)) camera.LookAt(new Vector2(_player.position.X, textMap.HeightInPixels - (_runtimeData.Scene.Height / _zoom) * 2 + _runtimeData.Scene.Height / 2));

            if (OutOfScreenLeft(camera) && OutOfScreenBottom(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / _zoom + _runtimeData.Scene.Width / 2, textMap.HeightInPixels - (_runtimeData.Scene.Height / _zoom) * 2 + _runtimeData.Scene.Height / 2));
            if (OutOfScreenLeft(camera) && OutOfScreenTop(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / _zoom + _runtimeData.Scene.Width / 2, -_runtimeData.Scene.Height / _zoom + _runtimeData.Scene.Height / 2));

            if (OutOfScreenRight(camera) && OutOfScreenTop(camera)) camera.LookAt(new Vector2(textMap.WidthInPixels - (_runtimeData.Scene.Width / _zoom) * 2 + _runtimeData.Scene.Width / 2, -_runtimeData.Scene.Height / _zoom + _runtimeData.Scene.Height / 2));
            if (OutOfScreenRight(camera) && OutOfScreenBottom(camera)) camera.LookAt(new Vector2(textMap.WidthInPixels - (_runtimeData.Scene.Width / _zoom) * 2 + _runtimeData.Scene.Width / 2, textMap.HeightInPixels - (_runtimeData.Scene.Height / _zoom) * 2 + _runtimeData.Scene.Height / 2));
        }

        private bool OutOfScreenTop(Camera2D camera)
        {
            if(camera.Position.Y < -_runtimeData.Scene.Height / _zoom) return true;
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
        
        public void checkIfWeLaunchInstance(TiledTile i)
        {
            if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1165)
            {
                _runtimeData.gidLast = 1165;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_PLATFORMER, 1);
            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 105)
            {
                _runtimeData.gidLast = 105;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_DOORGAME, 1);
            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 186)
            {
                _runtimeData.gidLast = 186;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_REARRANGER, 1);
            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 188)
            {
                _runtimeData.gidLast = 188;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_BOSS);
               
            }

        }

    }
}
