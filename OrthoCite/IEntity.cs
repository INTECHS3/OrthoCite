using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace OrthoCite
{
    interface IEntity
    {
        void LoadContent(ContentManager content, GraphicsDevice graphicsDevice);
        void UnloadContent();

        void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera);
        void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix);
        
        void Execute(params string[] param);
    }
}
