using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OrthoCite.Entities.MiniGames
{
    abstract class MiniGame : IEntity
    {
        abstract public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice);
        abstract public void UnloadContent();

        abstract public void Update(GameTime gameTime, KeyboardState keyboardState);
        abstract public void Draw(SpriteBatch spriteBatch);

        abstract internal void Start();
    }
}
