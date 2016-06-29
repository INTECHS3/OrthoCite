using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using OrthoCite.Helpers;
using System.Collections.Generic;

namespace OrthoCite.Entities
{
    class MenuInGame : IEntity
    {
        RuntimeData _runtimeData;
        bool _isVisible;

        Rectangle _recContourMap;
        Texture2D _bgRectangleContour;
        TiledMap _tileMap;
        Rectangle _rec;
        TimeSpan _saveTime;
        Texture2D _bgRectangle;
        SpriteFont _font;
        Button _leaveButton;
        Button _clearSaveButton;

        Texture2D _textPlayerPosition;

        public MenuInGame(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
        }
        void IEntity.LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _leaveButton = new Button(new Vector2(700, 130), "menuingame/leave", _runtimeData);
            _leaveButton.LoadContent(content, graphicsDevice);
            _leaveButton.onClick += _runtimeData.OrthoCite.Leave;

            _clearSaveButton = new Button(new Vector2(700, 50), "menuingame/clear", _runtimeData);
            _clearSaveButton.LoadContent(content, graphicsDevice);
            _clearSaveButton.onClick += _runtimeData.OrthoCite.ClearSave;


            _tileMap = content.Load<TiledMap>("map/Map");
            foreach(TiledLayer e in _tileMap.TileLayers)
            {
                if (e.Name == "Collision") e.IsVisible = false;
                
            }
            _isVisible = false;
            

            _bgRectangle = new Texture2D(graphicsDevice, 1, 1);
            _textPlayerPosition = new Texture2D(graphicsDevice, 1, 1);
            _textPlayerPosition.SetData(new Color[] { Color.Red });
            _bgRectangle.SetData(new Color[] { Color.Black });
            _rec = new Rectangle(0, 0, _runtimeData.Scene.Width, _runtimeData.Scene.Height);
            _recContourMap = new Rectangle(0, 0, _tileMap.WidthInPixels + 50, _tileMap.HeightInPixels + 50);
            _bgRectangleContour = new Texture2D(graphicsDevice, 1, 1);
            _bgRectangleContour.SetData(new Color[] { Color.Aqua });

            _font = content.Load<SpriteFont>("debug");


        }

        void IEntity.UnloadContent()
        {

        }

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            
            if (_saveTime.TotalMilliseconds == 0) { if (keyboardState.IsKeyDown(Keys.Escape)) { _isVisible = !_isVisible; _saveTime = gameTime.TotalGameTime; }  }
            else if (_saveTime.TotalMilliseconds <= gameTime.TotalGameTime.TotalMilliseconds - 400) _saveTime = new TimeSpan(0, 0, 0);

            if (_isVisible)
            { _leaveButton.Update(gameTime, keyboardState, camera, 0f);
                _clearSaveButton.Update(gameTime, keyboardState, camera, 0f);
            }
            
        }

        void IEntity.Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            
            spriteBatch.Begin(transformMatrix: frozenMatrix);

            if (_isVisible)
            {
                spriteBatch.Draw(_bgRectangle, _rec, Color.White * 0.7f);
                _leaveButton.Draw(spriteBatch);
                _clearSaveButton.Draw(spriteBatch);
            }
            spriteBatch.End();

            frozenMatrix.Scale = new Vector3(0.1f);
           
           
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            if (_isVisible)
            {
                spriteBatch.Draw(_bgRectangleContour, _recContourMap, Color.Aqua);
                _tileMap.Draw(spriteBatch, new Rectangle(500,500, 1, 1), gameTime: _runtimeData.GameTime);
                if (_runtimeData.Player != null) spriteBatch.Draw(_textPlayerPosition, new Rectangle((int)_runtimeData.Player.position.X, (int)_runtimeData.Player.position.Y, 75 ,75 ), Color.White);
            }
            spriteBatch.End();
        }

        void IEntity.Execute(params string[] param)
        {
            
        }

        public bool IsVIsible
        {
            get { return _isVisible; }
        } 

       
        
    }
}
