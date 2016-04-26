using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace OrthoCite.Entities
{
    class DebugLayer : IEntity
    {
        SpriteFont _font;
        double _refreshRate;

        public void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("debug");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            _refreshRate = gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, _refreshRate.ToString() + "ms", new Vector2(10, 10), Color.Black);
        }
    }
}
