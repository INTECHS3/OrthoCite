using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended;
using System;

namespace OrthoCite.Entities
{
    class Map : IEntity
    {
        RuntimeData _runtimeData;
        TiledMap textMap;
        TiledLayer _collision;

        public Map(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            
        }

        void IEntity.LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            textMap = content.Load<TiledMap>("Map");
            foreach (TiledLayer e in textMap.TileLayers)
            {
                if (e.Name == "Collision") _collision = e;
            }
           
            
        }

        void IEntity.UnloadContent()
        {
        }

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {

            if (keyboardState.IsKeyDown(Keys.Up) && !OutOfScreenTop(camera)) camera.Move(new Vector2(0, -5));
            if (keyboardState.IsKeyDown(Keys.Down) &&  !OutOfScreenBottom(camera)) camera.Move(new Vector2(0, +5));
            if (keyboardState.IsKeyDown(Keys.Right) && !OutOfScreenRight(camera)) camera.Move(new Vector2(+5, 0));
            if (keyboardState.IsKeyDown(Keys.Left) && !OutOfScreenLeft(camera)) camera.Move(new Vector2(-5, 0));
        }

        void IEntity.Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);
            spriteBatch.Draw(textMap);
            
            _collision.IsVisible = true;
            _collision.Draw(spriteBatch);
            spriteBatch.End();
        }

        private bool OutOfScreenTop(Camera2D camera)
        {
            if(camera.Position.Y <= 0)return true;
            return false;
        }
        private bool OutOfScreenLeft(Camera2D camera)
        {
            if (camera.Position.X <= 0) return true;
            return false;
        }
        private bool OutOfScreenRight(Camera2D camera)
        {
            if (camera.Position.X >= textMap.WidthInPixels - _runtimeData.Window.Width) return true;
            return false;
        }
        private bool OutOfScreenBottom(Camera2D camera)
        {
            if (camera.Position.Y >= textMap.HeightInPixels - _runtimeData.Window.Height) return true;
            return false;
        }


    }
}
