using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OrthoCite
{
    interface IEntity
    {
        void LoadContent(ContentManager content, GraphicsDevice graphicsDevice);
        void UnloadContent();

        void Update(GameTime gameTime, KeyboardState keyboardState);
        void Draw(SpriteBatch spriteBatch);
    }
}
