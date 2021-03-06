﻿using Microsoft.Xna.Framework;
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

        bool _itVisible;
        int _frameCounter;

        public DebugLayer(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _fpsCounter = new FramesPerSecondCounter();
            _itVisible = true;
            _frameCounter = 0;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _bgRectangle = new Texture2D(graphicsDevice, 1, 1);
            _bgRectangle.SetData(new Color[] { Color.Black });

            _rectangleBg = new Rectangle(5, 10, 250, 20);
            _rectangleBgPosition = new Rectangle(5, 30, 430, 70);
            _font = content.Load<SpriteFont>("debug");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            
            if (keyboardState.IsKeyDown(Keys.F4) && isDrawable()) _itVisible = !_itVisible;

            _fpsCounter.Update(gameTime);
            _camera = camera;
            var mouseState = Mouse.GetState();
            _mousePosition = _runtimeData.ViewAdapter.PointToScreen(mouseState.X, mouseState.Y);
            _frameCounter++;
        }

        private bool isDrawable()
        {
            if (_frameCounter >= 10)
            {
                _frameCounter = 0;
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            if(_itVisible)
            {
                spriteBatch.Begin(transformMatrix: frozenMatrix);
                spriteBatch.Draw(_bgRectangle, _rectangleBg, Color.White);
                spriteBatch.Draw(_bgRectangle, _rectangleBgPosition, Color.White); 
                spriteBatch.DrawString(_font, $"Debug mode - FPS: {_fpsCounter.FramesPerSecond:0}", new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(_font, $"Debug mode - Camera Position: X: {_camera.Position.X} Y: {_camera.Position.Y}", new Vector2(10, 30), Color.White);
                spriteBatch.DrawString(_font, $"Debug mode - Mouse Position: X: {_mousePosition.X} Y: {_mousePosition.Y}", new Vector2(10, 50), Color.White);
                if(_runtimeData.Player != null) { spriteBatch.DrawString(_font, $"Debug mode - Player Position: X: {_runtimeData.Player.positionVirt.X} Y: {_runtimeData.Player.positionVirt.Y}", new Vector2(10, 70), Color.White); }
                
                
                spriteBatch.End();
            }
            
        }

        public void Dispose()
        {
            System.Console.WriteLine($"Disose class : {this.GetType().Name}");
        }

        public void Execute(params string[] param)
        {

        }
    }
}
