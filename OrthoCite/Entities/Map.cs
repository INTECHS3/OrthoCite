using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Maps.Tiled;

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

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState)
        {

            if (keyboardState.IsKeyDown(Keys.Up)) _runtimeData.Camera.Move(new Vector2(0, +5));
            if (keyboardState.IsKeyDown(Keys.Down)) _runtimeData.Camera.Move(new Vector2(0, -5));
            if (keyboardState.IsKeyDown(Keys.Right)) _runtimeData.Camera.Move(new Vector2(+5, 0));
            if (keyboardState.IsKeyDown(Keys.Left)) _runtimeData.Camera.Move(new Vector2(-5, 0));
        }

        void IEntity.Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(textMap, _runtimeData.Camera); 


        }

    }
}
