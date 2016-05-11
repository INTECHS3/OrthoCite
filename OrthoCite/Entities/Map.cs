using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended;

namespace OrthoCite.Entities
{
    class Map : IEntity
    {
        RuntimeData _runtimeData;
        TiledMap textMap;
        

        public Map(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            
        }

        void IEntity.LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            textMap = content.Load<TiledMap>("Map");
        }

        void IEntity.UnloadContent()
        {
        }

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {

            if (keyboardState.IsKeyDown(Keys.Up)) camera.Move(new Vector2(0, +5));
            if (keyboardState.IsKeyDown(Keys.Down)) camera.Move(new Vector2(0, -5));
            if (keyboardState.IsKeyDown(Keys.Right)) camera.Move(new Vector2(+5, 0));
            if (keyboardState.IsKeyDown(Keys.Left)) camera.Move(new Vector2(-5, 0));
        }

        void IEntity.Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);
            spriteBatch.Draw(textMap);
            spriteBatch.End();
        }

    }
}
