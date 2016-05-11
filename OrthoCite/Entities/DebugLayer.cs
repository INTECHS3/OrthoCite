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
        Rectangle _rectangleBgPosition;
        Camera2D _camera;
        SpriteFont _font;
        Point _mousePosition;



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
            _rectangleBgPosition = new Rectangle(5, 30, 430, 50);
            _font = content.Load<SpriteFont>("debug");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            _camera = camera;
            var mouseState = Mouse.GetState();
            _mousePosition = _runtimeData.viewAdapter.PointToScreen(mouseState.X, mouseState.Y);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            spriteBatch.Draw(_bgRectangle, _rectangleBg, Color.White);
            spriteBatch.Draw(_bgRectangle, _rectangleBgPosition, Color.White);
            spriteBatch.DrawString(_font, $"Debug mode - FPS: {_fpsCounter.CurrentFramesPerSecond:0}", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(_font, $"Debug mode - Camera Position: X: {_camera.Position.X} Y: {_camera.Position.Y}", new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(_font, $"Debug mode - Mouse Position: X: {_mousePosition.X} Y: {_mousePosition.Y}", new Vector2(10, 50), Color.White);
            spriteBatch.End();
        }
    }
}
