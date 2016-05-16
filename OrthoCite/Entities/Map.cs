using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OrthoCite.Entities
{
    class Map : IEntity
    {
        RuntimeData _runtimeData;

        TiledMap textMap;
        TiledTileLayer _collisionLayer;
        int _gidStart;
        const int _gidSpawn = 1151;

        int _speed;
        int _walk;
        const int _zoom = 3;

        Vector2 _position;
        Vector2 _positionVirt;

        OrthoCite _o;

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


        public Map(RuntimeData runtimeData, OrthoCite o, int i)
        {
            _o = o;
            _runtimeData = runtimeData;
            _gidStart = i;


            _textureCharacter = new Dictionary<string, Texture2D>();
            _textureCharacterSelect = Direction.NONE;

            _walk = 0;
            _speed = 5;
        }


        void IEntity.LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            textMap = content.Load<TiledMap>("map/Map");
            
            foreach (TiledTileLayer e in textMap.TileLayers)
            {
                if (e.Name == "Collision") _collisionLayer = e;
            }

            if(_gidStart != 0)
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
            
            

            _textureCharacter.Add("RightLeft", content.Load<Texture2D>("map/champRightLeft"));
            _textureCharacter.Add("Up", content.Load<Texture2D>("map/champUp"));
            _textureCharacter.Add("Down", content.Load<Texture2D>("map/champDown"));
            _textureCharacter.Add("None", content.Load<Texture2D>("map/champNone"));
            
        }

        void IEntity.UnloadContent()
        {
        }

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            camera.Zoom = _zoom;
            

            if(_walk == 0)
            {
                if (keyboardState.IsKeyDown(Keys.Down) && !ColDown()) MoveDownChamp();
                if (keyboardState.IsKeyDown(Keys.Up) && !ColUp()) MoveUpChamp();
                if (keyboardState.IsKeyDown(Keys.Left) && !ColLeft()) MoveLeftChamp();
                if (keyboardState.IsKeyDown(Keys.Right) && !ColRight()) MoveRightChamp();
                _walk++;
            }
            else
            {
                _walk++;
                if (_walk >= 5 && keyboardState.IsKeyDown(Keys.LeftShift)) _walk = 0;
                else if (_walk >= 10) _walk = 0;
            }
            
            _position = new Vector2(_positionVirt.X * textMap.TileWidth, _positionVirt.Y * textMap.TileHeight);
            camera.LookAt(new Vector2(_position.X, _position.Y));
            //Console.WriteLine($"X : {_positionVirt.X} Y : {_positionVirt.Y} ");
        }

        void IEntity.Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);
            
            spriteBatch.Draw(textMap);
            //_collisionLayer.IsVisible = true;
            //_collisionLayer.Draw(spriteBatch);

            if (_textureCharacterSelect == Direction.RIGHT || _textureCharacterSelect == Direction.LEFT) spriteBatch.Draw(_textureCharacter["RightLeft"], _position, null, null, null, 0, null, null, _textureCharacterSelect == Direction.LEFT ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            else if (_textureCharacterSelect == Direction.UP) spriteBatch.Draw(_textureCharacter["Up"], _position, Color.White);
            else if (_textureCharacterSelect == Direction.DOWN) spriteBatch.Draw(_textureCharacter["Down"], _position, Color.White);
            else if (_textureCharacterSelect == Direction.NONE) spriteBatch.Draw(_textureCharacter["None"], _position, Color.White);

            spriteBatch.End();

        }
        
        void IEntity.Dispose()
        {
            //GOING FIX;
        }

        void IEntity.Execute(params string[] param)
        {
            
            switch(param[0])
            {
                case "movePlayer":
                    try{ MoveTo(new Vector2(Int32.Parse(param[1]), Int32.Parse(param[2]))); }
                    catch { Console.WriteLine("Bad Params"); }
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
            if (camera.Position.X >= textMap.WidthInPixels - _runtimeData.Window.Width - _speed) return true;
            return false;
        }
        private bool OutOfScreenBottom(Camera2D camera)
        {
            if (camera.Position.Y >= textMap.HeightInPixels - _runtimeData.Window.Height - _speed) return true;
            return false;
        }

       

        

        private void MoveUpChamp()
        {
            if (_positionVirt.Y <= 0) return;
            _positionVirt += new Vector2(0, -1);
            _textureCharacterSelect = Direction.UP;
        }

        private void MoveDownChamp()
        {
            if (_positionVirt.Y >= textMap.Height - 1) return;
            _positionVirt += new Vector2(0, +1);
            _textureCharacterSelect = Direction.DOWN;
        }

        private void MoveLeftChamp()
        {
            if (_positionVirt.X <= 0) return;
            _positionVirt += new Vector2(-1, 0);
            _textureCharacterSelect = Direction.LEFT;
        }

        private void MoveRightChamp()
        {
            if (_positionVirt.X >= textMap.Width - 1) return;
            _positionVirt += new Vector2(+1, 0);
            _textureCharacterSelect = Direction.RIGHT;
        }

        private void MoveTo(Vector2 vec)
        {
            _positionVirt = vec;
        }
        private bool ColUp()
        {
            foreach(TiledTile i in _collisionLayer.Tiles)
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
            if (i.X == _positionVirt.X && i.Y == _positionVirt.Y - 1 && i.Id == 1190)
            {
                _o._gidLastForMap = 1190;
                _o._entitiesSelect = OrthoCite.nameEntity.PLATFORMER;
                _o._entitiesModified = true;
            }
        }

    }
}
