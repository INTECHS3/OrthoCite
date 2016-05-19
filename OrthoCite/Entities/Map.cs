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

namespace OrthoCite.Entities
{
    class Map : IEntity
    {
        RuntimeData _runtimeData;
        TiledMap textMap;
        TiledTileLayer _collisionLayer;
        TiledTileLayer _upLayer;
        SpriteSheetAnimator _heroAnimations;
        Sprite _heroSprite;

        int _gidStart;
        const int _gidSpawn = 1151;

        const int _fastSpeed = 5;
        const int _lowSpeed = 10;
        
        int _separeFrame;
        int _actualFrame;
        int _fastFrame;
        int _lowFrame;
        const int _zoom = 3;

        Vector2 _position;
        Vector2 _positionVirt;

        Direction _actualDir;
        bool _firstUpdate;
        
        enum Direction
        {
            NONE,
            LEFT,
            RIGHT,
            UP,
            DOWN
        }
        
        
        Direction _textureCharacterSelect;
        Dictionary<string, Texture2D> _textureCharacter;


        public Map(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _gidStart = _runtimeData.gidLast;


            _textureCharacter = new Dictionary<string, Texture2D>();
            _textureCharacterSelect = Direction.NONE;

            _separeFrame = 0;
            _lowFrame = _lowSpeed;
            _fastFrame = _fastSpeed;

            _firstUpdate = true;
            
        }


        void IEntity.LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            textMap = content.Load<TiledMap>("map/Map");
            
            foreach (TiledTileLayer e in textMap.TileLayers)
            {
                if (e.Name == "Collision") _collisionLayer = e;
                else if (e.Name == "Up") _upLayer = e;
            }
            _collisionLayer.IsVisible = false;
            
            if (_gidStart != 0)
            {
                foreach (TiledTile i in _collisionLayer.Tiles)
                {
                    if (i.Id == _gidStart) _positionVirt = new Vector2(i.X, i.Y + 1);
                }
            }

            if(_positionVirt.Length() == 0)
            {
                foreach (TiledTile i in _collisionLayer.Tiles)
                {
                    if (i.Id == _gidSpawn) _positionVirt = new Vector2(i.X, i.Y);
                }
            }
            _runtimeData.gidLast = 0;

            var HeroWalking = content.Load<Texture2D>("animations/Walking");
            var HeroAtlas = TextureAtlas.Create(HeroWalking, 32, 32);
            var HeroWalkingFactory = new SpriteSheetAnimationFactory(HeroAtlas);
            HeroWalkingFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            HeroWalkingFactory.Add("walkSouth", new SpriteSheetAnimationData(new[] { 5, 0, 10, 0 }, isLooping: false));
            HeroWalkingFactory.Add("walkWest", new SpriteSheetAnimationData(new[] {0 , 0, 0,  0}, isLooping: false));
            HeroWalkingFactory.Add("walkEast", new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            HeroWalkingFactory.Add("walkNorth", new SpriteSheetAnimationData(new[] { 19, 13, 24, 13 }, isLooping: false));
            _heroAnimations = new SpriteSheetAnimator(HeroWalkingFactory);
            _heroSprite = _heroAnimations.CreateSprite(_positionVirt);

            _textureCharacter.Add("RightLeft", content.Load<Texture2D>("map/champRightLeft"));
            _textureCharacter.Add("Up", content.Load<Texture2D>("map/champUp"));
            _textureCharacter.Add("Down", content.Load<Texture2D>("map/champDown"));
            _textureCharacter.Add("None", content.Load<Texture2D>("map/champNone"));
            _actualDir = Direction.NONE;
        }

        void IEntity.UnloadContent()
        {

        }

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _heroAnimations.Update(deltaSeconds);
            if (_firstUpdate)
            {
                camera.Zoom = _zoom;
                _position = new Vector2(_positionVirt.X * textMap.TileWidth, _positionVirt.Y * textMap.TileHeight);
                _firstUpdate = !_firstUpdate;
            }
            
            
            if(_separeFrame == 0 && keyboardState.GetPressedKeys().Length != 0 && _actualDir == Direction.NONE)
            {
                if (keyboardState.IsKeyDown(Keys.LeftShift)) _actualFrame = _fastFrame;
                else _actualFrame = _lowFrame;

                if (keyboardState.IsKeyDown(Keys.Down) && !ColDown())
                {
                    _actualDir = Direction.DOWN;
                    _heroAnimations.Play("walkSouth");
                }
                if (keyboardState.IsKeyDown(Keys.Up) && !ColUp())
                {
                  
                    _actualDir = Direction.UP;
                    _textureCharacterSelect = Direction.UP;
                    _heroAnimations.Play("walkSouth");
                }
                if (keyboardState.IsKeyDown(Keys.Left) && !ColLeft())
                {
                    _actualDir = Direction.LEFT;
                    _textureCharacterSelect = Direction.LEFT;
                }
                if (keyboardState.IsKeyDown(Keys.Right) && !ColRight())
                {
                    _actualDir = Direction.RIGHT;
                    _textureCharacterSelect = Direction.RIGHT;
                }
                

                if (keyboardState.IsKeyDown(Keys.F9)) _collisionLayer.IsVisible = !_collisionLayer.IsVisible;

                _separeFrame++;
            }
            else  if (_separeFrame != 0)
            {

                if(_separeFrame >= _actualFrame)
                {
                    if (_actualDir == Direction.DOWN)
                    {
                        _heroAnimations.Play("walkSouth");
                        MoveDownChamp();
                        
                    }
                    if (_actualDir == Direction.UP)
                    {
                        MoveUpChamp();
                        _heroAnimations.Play("walkNorth");
                    }
                    if (_actualDir == Direction.LEFT)
                    {
                        MoveLeftChamp();
                        _heroAnimations.Play("walkWest");
                    }
                    if (_actualDir == Direction.RIGHT)
                    {
                        MoveRightChamp();
                        _heroAnimations.Play("walkEast");
                    }
                        
                    _position = new Vector2(_positionVirt.X * textMap.TileWidth, _positionVirt.Y * textMap.TileHeight);
                    

                    _actualDir = Direction.NONE;
                    _separeFrame = 0;
                }
                else
                {
                    if (_actualDir == Direction.DOWN) _position.Y += textMap.TileHeight / _actualFrame;
                    if (_actualDir == Direction.UP) _position.Y += -(textMap.TileHeight / _actualFrame);
                    if (_actualDir == Direction.LEFT) _position.X += -(textMap.TileWidth / _actualFrame);
                    if (_actualDir == Direction.RIGHT) _position.X += textMap.TileWidth / _actualFrame;


                   _separeFrame++;
                }
                
            }

            
            _heroSprite.Position = new Vector2(_position.X + textMap.TileWidth / 2, _position.Y + textMap.TileHeight / 2);
            camera.LookAt(new Vector2(_position.X, _position.Y));
            //Console.WriteLine($"X : {_positionVirt.X} Y : {_positionVirt.Y} ");
        }

        void IEntity.Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            _upLayer.IsVisible = false;
            spriteBatch.Draw(textMap);
            spriteBatch.Draw(_heroSprite);
            

            //if (_textureCharacterSelect == Direction.RIGHT || _textureCharacterSelect == Direction.LEFT) spriteBatch.Draw(_textureCharacter["RightLeft"], _position, null, null, null, 0, null, null, _textureCharacterSelect == Direction.LEFT ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            //else if (_textureCharacterSelect == Direction.UP) _heroAnimations.Play("walkNorth");
            //else if (_textureCharacterSelect == Direction.DOWN) _heroAnimations.Play("walkSouth");
            //else if (_textureCharacterSelect == Direction.NONE) spriteBatch.Draw(_textureCharacter["None"], _position, Color.White);


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
                    catch { Console.WriteLine("Bad Params => movePlayer {x] {y}"); }
                    break;
                default:
                    Console.WriteLine("Can't find method to invoke in Map Class");
                    break;
            }
        }

        private bool OutOfScreenTop(Camera2D camera)
        {
            if(camera.Position.Y <= 0) return true;
            return false;
        }
        private bool OutOfScreenLeft(Camera2D camera)
        {
            if (camera.Position.X <= 0) return true;
            return false;
        }
        private bool OutOfScreenRight(Camera2D camera)
        {
            if (camera.Position.X >= textMap.WidthInPixels - _runtimeData.Scene.Width) return true;
            return false;
        }
        private bool OutOfScreenBottom(Camera2D camera)
        {
            if (camera.Position.Y >= textMap.HeightInPixels - _runtimeData.Scene.Height) return true;
            return false;
        }

       

        

        private void MoveUpChamp()
        {
            
            _positionVirt += new Vector2(0, -1);
        }

        private void MoveDownChamp()
        {
            if (_positionVirt.Y >= textMap.Height - 1) return;
            _positionVirt += new Vector2(0, +1);
        }

        private void MoveLeftChamp()
        {
            if (_positionVirt.X <= 0) return;
            _positionVirt += new Vector2(-1, 0);
        }

        private void MoveRightChamp()
        {
            if (_positionVirt.X >= textMap.Width - 1) return;
            _positionVirt += new Vector2(+1, 0);
        }

        private void MoveTo(Vector2 vec)
        {
            _positionVirt = vec;
        }
        private bool ColUp()
        {
            if (_positionVirt.Y <= 0) return false;
            foreach (TiledTile i in _collisionLayer.Tiles)
            {
                if (i.X == _positionVirt.X && i.Y == _positionVirt.Y - 1 && i.Id == 889) return true;
                checkIfWeLaunchInstance(i);
            }
            
            return false;
        }

        private bool ColDown()
        {
            foreach (TiledTile i in _collisionLayer.Tiles)
            {
                if (i.X == _positionVirt.X && i.Y == _positionVirt.Y + 1 && i.Id == 889) return true;
            }
            return false;
        }

        private bool ColLeft()
        {
            foreach (TiledTile i in _collisionLayer.Tiles)
            {
                if (i.X == _positionVirt.X - 1 && i.Y == _positionVirt.Y && i.Id == 889) return true;
            }
            return false;
        }

        private bool ColRight()
        {
            foreach (TiledTile i in _collisionLayer.Tiles)
            {
                if (i.X == _positionVirt.X + 1 && i.Y == _positionVirt.Y && i.Id == 889) return true;
            }
            return false;
        }

        private void checkIfWeLaunchInstance(TiledTile i)
        {
            if (i.X == _positionVirt.X && i.Y == _positionVirt.Y - 1 && i.Id == 1165)
            {
                _runtimeData.gidLast = 1165;
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MINIGAME_PLATFORMER);
            }
        }

    }
}
