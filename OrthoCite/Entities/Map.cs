using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace OrthoCite.Entities
{
    class Map : IEntity
    {
        RuntimeData _runtimeData;

        public Map(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
        }

        void IEntity.LoadContent(ContentManager content)
        {
        }

        void IEntity.UnloadContent()
        {
        }

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState)
        {
        }

        void IEntity.Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
