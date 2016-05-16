using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace OrthoCite.Entities.MiniGames
{
    abstract class MiniGame : IEntity
    {
        abstract public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice);
        abstract public void UnloadContent();

        abstract public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera);
        abstract public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix);

        abstract public void Execute(params string[] param);
        abstract public void Dispose();

        abstract internal void Start();
    }
}
