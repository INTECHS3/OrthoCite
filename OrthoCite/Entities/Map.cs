using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using MonoGame.Extended.Maps.Tiled;

namespace OrthoCite.Entities
{
    class Map : IEntity
    {
        RuntimeData _runtimeData;
        TiledMap textMap;
        
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public Map(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
           // AllocConsole();
            
        }

        void IEntity.LoadContent(ContentManager content)
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
