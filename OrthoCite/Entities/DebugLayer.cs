using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace OrthoCite.Entities
{
    class DebugLayer : IEntity
    {
        RuntimeData _runtimeData;
        FramesPerSecondCounter _fpsCounter;
        Texture2D _bgRectangle;
        Rectangle _rectangleBg;

        SpriteFont _font;

        public DebugLayer(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _fpsCounter = new FramesPerSecondCounter();
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _bgRectangle = new Texture2D(graphicsDevice, 1, 1);
            _bgRectangle.SetData(new Color[] { Color.Black });

            _rectangleBg = new Rectangle(5, 10, 250, 20);

            _font = content.Load<SpriteFont>("debug");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            spriteBatch.Draw(_bgRectangle, _rectangleBg, Color.White);
            spriteBatch.DrawString(_font, $"Debug mode - FPS: {_fpsCounter.CurrentFramesPerSecond:0}", new Vector2(10, 10), Color.White);
            spriteBatch.End();
        }
    }
}
