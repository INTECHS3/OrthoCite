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
            checkDistrict();


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
            Dictionary<string, bool> tmpAnswer = new Dictionary<string, bool>();
            _runtimeData.PNJ = new Dictionary<ListPnj, PNJ>();
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_1_1, new PNJ(TypePNJ.Dynamique, new Vector2(120, 59), new List<ItemList>(), _runtimeData, "map/pnj"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_1_2, new PNJ(TypePNJ.Static, new Vector2(69, 47), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_1_3, new PNJ(TypePNJ.Dynamique, new Vector2(67, 60), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_1_4, new PNJ(TypePNJ.Static, new Vector2(82, 51), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_1_5, new PNJ(TypePNJ.Dynamique, new Vector2(75, 66), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_1_6, new PNJ(TypePNJ.Static, new Vector2(68, 56), new List<ItemList>(), _runtimeData, "map/pnj"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_2_1, new PNJ(TypePNJ.Static, new Vector2(70, 11), new List<ItemList>(), _runtimeData, "map/pnj"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_2_2, new PNJ(TypePNJ.Dynamique, new Vector2(64, 7), new List<ItemList>(), _runtimeData, "map/pnj"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_2_3, new PNJ(TypePNJ.Static, new Vector2(76, 23), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_2_4, new PNJ(TypePNJ.Static, new Vector2(84, 12), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_3_1, new PNJ(TypePNJ.Static, new Vector2(26, 25), new List<ItemList>(), _runtimeData, "map/pnj"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_3_2, new PNJ(TypePNJ.Static, new Vector2(27, 29), new List<ItemList>(), _runtimeData, "map/pnj"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_3_3, new PNJ(TypePNJ.Static, new Vector2(22, 31), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_3_4, new PNJ(TypePNJ.Dynamique, new Vector2(44, 16), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_3_5, new PNJ(TypePNJ.Static, new Vector2(26, 14), new List<ItemList>(), _runtimeData, "map/pnj"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_3_6, new PNJ(TypePNJ.Dynamique, new Vector2(44, 7), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_4_1, new PNJ(TypePNJ.Static, new Vector2(44, 50), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_4_2, new PNJ(TypePNJ.Static, new Vector2(37, 47), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_4_3, new PNJ(TypePNJ.Static, new Vector2(36, 60), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_4_4, new PNJ(TypePNJ.Static, new Vector2(25, 48), new List<ItemList>(), _runtimeData, "map/actor"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_4_5, new PNJ(TypePNJ.Static, new Vector2(27, 60), new List<ItemList>(), _runtimeData, "map/pnj"));
            _runtimeData.PNJ.Add(ListPnj.QUARTIER_4_6, new PNJ(TypePNJ.Static, new Vector2(13, 45), new List<ItemList>(), _runtimeData, "map/pnj"));
            if (_runtimeData.DataSave.District != 4)
            {
                Vector2 tmpPostionOfPNJ = new Vector2();
                if (_runtimeData.DataSave.District == 1) tmpPostionOfPNJ = new Vector2(75, 40);
                else if (_runtimeData.DataSave.District == 2) tmpPostionOfPNJ = new Vector2(56, 15);
                else if (_runtimeData.DataSave.District == 3) tmpPostionOfPNJ = new Vector2(38, 32);

                _runtimeData.PNJ.Add(ListPnj.PORTAILBLOCK, new PNJ(TypePNJ.Static, tmpPostionOfPNJ, new List<ItemList>(), _runtimeData, "map/pnj"));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0}, isLooping: true));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 0}, isLooping: true));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 24}, isLooping: false));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 24 }, isLooping: false));
                _runtimeData.PNJ[ListPnj.PORTAILBLOCK].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 36 }, isLooping: false));
                if (_runtimeData.DataSave.District == 1) _runtimeData.PNJ[ListPnj.PORTAILBLOCK].lookDir = Direction.NONE;
                else if (_runtimeData.DataSave.District == 2) _runtimeData.PNJ[ListPnj.PORTAILBLOCK].lookDir = Direction.RIGHT;
                else if (_runtimeData.DataSave.District == 3) _runtimeData.PNJ[ListPnj.PORTAILBLOCK].lookDir = Direction.UP;
                tmpAnswer.Clear();
                tmpAnswer.Add("Oui", true);
                tmpAnswer.Add("Non", false);

                _runtimeData.pnj[ListPnj.PORTAILBLOCK]._talkAndAnswerQueue.Enqueue(new PnjDialog($"Désolé tu n'as pas fini les mini jeux du quartier {_runtimeData.DataSave.District}, tu ne peux pas avancer !", tmpAnswer));
                //_runtimeData.PNJ[ListPnj.PORTAILBLOCK]._talkAndAnswer.Add($"Désolé tu n'as pas fini les mini jeux du quartier {_runtimeData.DataSave.District}, tu ne peux pas avancer !", new Dictionary<string, bool>());
                //_runtimeData.PNJ[ListPnj.PORTAILBLOCK]._talkAndAnswer.Add($"Fini et reviens me voir après ! ", new Dictionary<string, bool>());

                tmpAnswer.Clear();
            }

            

            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 0, 1, 2}, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 24,25 ,26 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_1].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 36, 37, 38}, isLooping: false));

            _runtimeData.PNJ[ListPnj.QUARTIER_1_1]._positionSec = new Vector2(126,64);
            
            
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_1]._talkAndAnswer.Add("Bienvenu sur OrthoCité", new Dictionary<string, bool>());

            _runtimeData.PNJ[ListPnj.QUARTIER_1_2].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 58 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_2].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 58}, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_2].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 70 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_2].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 82 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_2].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 94 }, isLooping: false));
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_2]._talkAndAnswer.Add($"Si tout va mal dans cette ville c'est à cause de Lyrik !", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_2]._talkAndAnswer.Add($"Il a volé les lettres sacrées et les utilisent pour tout chambouler ! ", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_2]._talkAndAnswer.Add($"il faut que quelqu'un nous sauve ! ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_1_2].lookDir = Direction.DOWN;

            _runtimeData.PNJ[ListPnj.QUARTIER_1_3].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 1 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_3].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 58 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_3].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_3].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_3].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 58 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_3]._positionSec = new Vector2(73, 60);
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_3]._talkAndAnswer.Add($"Il fait beau aujourd'hui !", new Dictionary<string, bool>());
            

            _runtimeData.PNJ[ListPnj.QUARTIER_1_4].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 16 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_4].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 4 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_4].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 16 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_4].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_4].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 58 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_4].lookDir = Direction.LEFT;
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_4]._talkAndAnswer.Add($"Tu sais que pour savoir si tu dois écrire 'a' ou 'à' dans une phrase", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_4]._talkAndAnswer.Add($"tu n'as qu'à remplacer le 'a' par 'avait' et si ça fonctionne c'est qu'il ne faut pas mettre d'accent ! ", new Dictionary<string, bool>());
           // _runtimeData.PNJ[ListPnj.QUARTIER_1_4]._talkAndAnswer.Add($"Sinon il faudra écrire 'à' !", new Dictionary<string, bool>());

            _runtimeData.PNJ[ListPnj.QUARTIER_1_5].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 10 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_5].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 9, 10, 11 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_5].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 33,34,35 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_5].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 33,34,35 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_5].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 45, 46, 47 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_5]._positionSec = new Vector2(78, 63);
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_5]._talkAndAnswer.Add($"L'orthographe nous permet de bien nous comprendre entre nous", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_5]._talkAndAnswer.Add($"c'est super important ! ", new Dictionary<string, bool>());

            _runtimeData.PNJ[ListPnj.QUARTIER_1_6].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 85 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_6].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 49 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_6].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 61 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_6].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 61 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_6].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 85 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_1_6].lookDir = Direction.UP;
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_6]._talkAndAnswer.Add($"Avant il y avait beaucoup de gens en ville", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_1_6]._talkAndAnswer.Add($"mais Lyrik les a tous fait fuir ! ", new Dictionary<string, bool>());

            _runtimeData.PNJ[ListPnj.QUARTIER_2_1].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 52 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_1].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 52 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_1].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 64 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_1].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_1].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 88 }, isLooping: false));
            //_runtimeData.PNJ[ListPnj.QUARTIER_2_1]._talkAndAnswer.Add($"Dis moi, tu savais que pour savoir si il faut écrire 'ou' ou 'où'", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_2_1]._talkAndAnswer.Add($"il suffit de le remplacer par 'ou bien' et si ça fonctionne ", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_2_1]._talkAndAnswer.Add($"c'est qu'il faut écrire 'ou'. C'est dingue ! ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_2_1].lookDir = Direction.DOWN;

            _runtimeData.PNJ[ListPnj.QUARTIER_2_2].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 10 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_2].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 9, 10, 11 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_2].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 33, 34, 35 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_2].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 33, 34, 35 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_2].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 55, 56, 57 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_2]._positionSec = new Vector2(76, 7);
            //_runtimeData.PNJ[ListPnj.QUARTIER_2_2]._talkAndAnswer.Add($"Ils ont volé toutes les lettres sacrées !", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_2_2]._talkAndAnswer.Add($"J'espère que tu pourras les récupérer, on compte tous sur toi. ", new Dictionary<string, bool>());

            _runtimeData.PNJ[ListPnj.QUARTIER_2_3].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 43 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_3].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 7 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_3].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 19 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_3].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 31 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_3].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 43 }, isLooping: false));
            //_runtimeData.PNJ[ListPnj.QUARTIER_2_3]._talkAndAnswer.Add($"J'espère que toutes ces épreuves ne sont pas trop dures pour toi.", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_2_3].lookDir = Direction.LEFT;

            _runtimeData.PNJ[ListPnj.QUARTIER_2_4].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 67 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_4].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 55 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_4].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 67 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_4].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 79 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_2_4].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 91 }, isLooping: false));
            //_runtimeData.PNJ[ListPnj.QUARTIER_2_4]._talkAndAnswer.Add($"tu n'é pas assez for pour attindre Lyrik ! ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_2_4].lookDir = Direction.LEFT;

            _runtimeData.PNJ[ListPnj.QUARTIER_3_1].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 7 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_1].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 7 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_1].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 19 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_1].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 31 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_1].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 43 }, isLooping: false));
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_1]._talkAndAnswer.Add($"Pour savoir si tu dois écrire 'se' ou 'ce' ", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_1]._talkAndAnswer.Add($"remplace le par 'cela', si ça fonctionne, c'est qu'il faut écrire 'ce' ", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_1]._talkAndAnswer.Add($"sinon tu devras l'écrire 'se' ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_3_1].lookDir = Direction.DOWN;

            _runtimeData.PNJ[ListPnj.QUARTIER_3_2].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 85 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_2].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 49 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_2].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 61 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_2].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 73 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_2].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 85 }, isLooping: false));
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_2]._talkAndAnswer.Add($"Toute fasson l'orthografe cé nul !", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_3_2].lookDir = Direction.UP;

            _runtimeData.PNJ[ListPnj.QUARTIER_3_3].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 88 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_3].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 52 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_3].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 64 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_3].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 76 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_3].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 88 }, isLooping: false));
           // _runtimeData.PNJ[ListPnj.QUARTIER_3_3]._talkAndAnswer.Add($"Tu savais qu'avant l'écriture n'existait pas ?", new Dictionary<string, bool>());
           // _runtimeData.PNJ[ListPnj.QUARTIER_3_3]._talkAndAnswer.Add($"Tout était raconté à l'oral ! ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_3_3].lookDir = Direction.UP;

            _runtimeData.PNJ[ListPnj.QUARTIER_3_4].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 49 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_4].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 48, 49, 50 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_4].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 60, 61, 62 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_4].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 72, 73, 74 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_4].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 84, 85, 86 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_4]._positionSec = new Vector2(44, 23);
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_4]._talkAndAnswer.Add($"Tu es là pour nous sauver ?", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_4]._talkAndAnswer.Add($"Merci beaucoup de ton aide !", new Dictionary<string, bool>());

            _runtimeData.PNJ[ListPnj.QUARTIER_3_5].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 4 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_5].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 4 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_5].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 16 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_5].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 28 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_5].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 40 }, isLooping: false));
           // _runtimeData.PNJ[ListPnj.QUARTIER_3_5]._talkAndAnswer.Add($"Ce quartier, c'est mon préféré", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_5]._talkAndAnswer.Add($"C'est le plus beau ! ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_3_5].lookDir = Direction.DOWN;

            _runtimeData.PNJ[ListPnj.QUARTIER_3_6].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 1 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_6].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 0, 1, 2 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_6].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_6].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 24, 25, 26 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_6].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 36, 37, 38 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_3_6]._positionSec = new Vector2(48, 5);
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_6]._talkAndAnswer.Add($"Je viens de comprendre quand je dois écrire 'tous' ou tout'", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_6]._talkAndAnswer.Add($"Quand ce qui suit est au singulier j'écris 'tout' ! Par exemple : tout le village. ", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_3_6]._talkAndAnswer.Add($"Quand ce qui suit est au pluriel j'écris 'tous' ! Par exemple : tous les matins. ", new Dictionary<string, bool>());

            _runtimeData.PNJ[ListPnj.QUARTIER_4_1].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 22 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_1].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 10 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_1].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 22 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_1].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 34 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_1].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 46 }, isLooping: false));
            //_runtimeData.PNJ[ListPnj.QUARTIER_4_1]._talkAndAnswer.Add($"Tu dois être vraiment fort pour être arrivé jusqu'ici !", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_4_1].lookDir = Direction.LEFT;

            _runtimeData.PNJ[ListPnj.QUARTIER_4_2].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 43 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_2].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 7 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_2].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 19 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_2].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 31 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_2].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 43 }, isLooping: false));
            //_runtimeData.PNJ[ListPnj.QUARTIER_4_2]._talkAndAnswer.Add($"Je sais quand mettre 'là' ou 'la' dans une phrase ! ", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_4_2]._talkAndAnswer.Add($"Si tu peux le remplacer par 'par ici' tu devras l'écrire 'là' ! par exemple : Reste là ! ", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_4_2]._talkAndAnswer.Add($"Sinon tu devras l'écrire 'la'. ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_4_2].lookDir = Direction.UP;

            _runtimeData.PNJ[ListPnj.QUARTIER_4_3].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 28 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_3].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 4 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_3].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 16 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_3].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 28 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_3].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 40 }, isLooping: false));
           // _runtimeData.PNJ[ListPnj.QUARTIER_4_3]._talkAndAnswer.Add($"Lyrik essaie d'instaurer le chaos dans le monde", new Dictionary<string, bool>());
           // _runtimeData.PNJ[ListPnj.QUARTIER_4_3]._talkAndAnswer.Add($"mais par chance, tu vas l'en empêcher ! ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_4_3].lookDir = Direction.RIGHT;

            _runtimeData.PNJ[ListPnj.QUARTIER_4_4].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 82 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_4].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 58 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_4].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 70 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_4].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 82 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_4].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 94 }, isLooping: false));
            //_runtimeData.PNJ[ListPnj.QUARTIER_4_4]._talkAndAnswer.Add($"je komprend pas pourkoi on écrit alor kon peu parler...", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_4_4].lookDir = Direction.RIGHT;

            _runtimeData.PNJ[ListPnj.QUARTIER_4_5].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 49 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_5].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 49 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_5].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 61 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_5].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 73 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_5].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 85 }, isLooping: false));
           // _runtimeData.PNJ[ListPnj.QUARTIER_4_5]._talkAndAnswer.Add($"Je prie pour qu'on récupère nos lettres sacrées !", new Dictionary<string, bool>());
           // _runtimeData.PNJ[ListPnj.QUARTIER_4_5]._talkAndAnswer.Add($"De plus en en plus de gens commencent à écrire n'importe comment et à dire n'importe quoi ! ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_4_5].lookDir = Direction.DOWN;

            _runtimeData.PNJ[ListPnj.QUARTIER_4_6].spriteFactory(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 10 }));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_6].spriteFactory(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 10 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_6].spriteFactory(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 22 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_6].spriteFactory(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 34 }, isLooping: false));
            _runtimeData.PNJ[ListPnj.QUARTIER_4_6].spriteFactory(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 46 }, isLooping: false));
           // _runtimeData.PNJ[ListPnj.QUARTIER_4_6]._talkAndAnswer.Add($"Prépare toi bien avant d'entrer !", new Dictionary<string, bool>());
            //_runtimeData.PNJ[ListPnj.QUARTIER_4_6]._talkAndAnswer.Add($"ça ne va pas être facile. ", new Dictionary<string, bool>());
            _runtimeData.PNJ[ListPnj.QUARTIER_4_6].lookDir = Direction.DOWN;


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
            //DISTRICT 1
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
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_GUESSGAME);
               
            }
            //DISTRICT 2
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1050)
            {
                _runtimeData.gidLast = 1050;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_PLATFORMER);

            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1051)
            {
                _runtimeData.gidLast = 1051;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_DOORGAME);

            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1052)
            {
                _runtimeData.gidLast = 1052;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_REARRANGER);

            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1053)
            {
                _runtimeData.gidLast = 1053;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_BOSS); // LAST GAME A FINIR

            }
            //DISTRICT 3
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1329)
            {
                _runtimeData.gidLast = 1329;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_PLATFORMER);

            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1330)
            {
                _runtimeData.gidLast = 1330;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_DOORGAME);

            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1331)
            {
                _runtimeData.gidLast = 1331;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_REARRANGER);

            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1332)
            {
                _runtimeData.gidLast = 1332;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_THROWGAME); 

            }
            //DISTRICT 4
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1594)
            {
                _runtimeData.gidLast = 1594;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_PLATFORMER);

            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1595)
            {
                _runtimeData.gidLast = 1595;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_DOORGAME);

            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1596)
            {
                _runtimeData.gidLast = 1596;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_REARRANGER);

            }
            else if (i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1 && i.Id == 1597)
            {
                _runtimeData.gidLast = 1597;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_BOSS);

            }

        }

    }
}
