using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;

namespace OrthoCite.Entities
{
    class LostScreen : IEntity
    {
        RuntimeData _runtimeData;

        Texture2D _pixelTexture;
        Texture2D _coinTexture;

        Sprite _coin;

        DateTime _timeStarted;

        public LostScreen(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _runtimeData.Lives = 3;
            _runtimeData.Credits++;

            _timeStarted = DateTime.Now;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.Black });
            _coinTexture = content.Load<Texture2D>("lostscreen/coin");

            _coin = new Sprite(_coinTexture);
            _coin.Position = new Vector2(_runtimeData.Scene.Width / 2, _runtimeData.Scene.Height / 2);
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (DateTime.Now - _timeStarted >= TimeSpan.FromSeconds(5))
            {
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, _runtimeData.Scene.Width, _runtimeData.Scene.Height), Color.White);
            spriteBatch.Draw(_coin);
            spriteBatch.End();
        }

        public void Execute(params string[] param)
        {

        }
    }
}
