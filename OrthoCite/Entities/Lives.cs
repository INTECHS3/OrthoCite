using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
namespace OrthoCite.Entities
{
    class Lives : IEntity
    {
        const int MARGIN_TOP = 20;
        const int MARGIN_RIGHT = 20;
        const int SPACE_BETWEEN_LIVES = 10;

        RuntimeData _runtimeData;
        Texture2D _heartTexture;

        public Lives(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _heartTexture = content.Load<Texture2D>("lives/heart");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            for (int i = 0; i < _runtimeData.Lives; i++)
            {
                spriteBatch.Draw(_heartTexture, new Vector2(_runtimeData.Scene.Width - _heartTexture.Width - MARGIN_RIGHT - i * (_heartTexture.Width + SPACE_BETWEEN_LIVES), MARGIN_TOP));
            }
            spriteBatch.End();
        }

        public void Execute(params string[] param)
        {

        }
    }
}
