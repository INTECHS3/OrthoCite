using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OrthoCity
{
    interface IEntity
    {
        void LoadContent(ContentManager content);
        void UnloadContent();

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
