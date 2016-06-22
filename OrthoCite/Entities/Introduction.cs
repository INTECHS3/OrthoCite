using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Animations.Tweens;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;

namespace OrthoCite.Entities
{
    class Introduction : IEntity
    {
        RuntimeData _runtimeData;

        TiledMap _map;

        Texture2D _overlayTexture;
        Texture2D _logoTexture;

        Sprite _logo;
     
        Song _song;

        SpriteFont _font;

        bool _animationStarted;
        uint _mapState = 0;
        TweenAnimation<Camera2D> _cameraAnimation;
        int _fontOpacity = 0;
        bool _fadingIn = true;

        public Introduction(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _map = content.Load<TiledMap>("map/Map");
            foreach (TiledTileLayer layer in _map.TileLayers)
            {
                if (layer.Name == "Collision")
                {
                    layer.IsVisible = false;
                    break;
                }
            }

            _overlayTexture = new Texture2D(graphicsDevice, 1, 1);
            _overlayTexture.SetData(new Color[] { Color.Black * 0.4f });
            _logoTexture = content.Load<Texture2D>("intro/logo");
            _logo = new Sprite(_logoTexture);
            _logo.Position = new Vector2(_runtimeData.Scene.Width / 2, -_logoTexture.Height);

            _song = content.Load<Song>("intro/music");

            _font = content.Load<SpriteFont>("intro/font");
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (!_animationStarted)
            {
                camera.Zoom = 2f;
                StartAnimation(camera);
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(_song);
            }

            if (_fontOpacity == 0) _fadingIn = true;
            else if (_fontOpacity == 100) _fadingIn = false;

            if (_fadingIn) _fontOpacity += 1;
            else _fontOpacity -= 1;

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                MediaPlayer.Stop();
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
                _cameraAnimation.Stop();
            }
        }

        public void StartAnimation(Camera2D camera)
        {
            _animationStarted = true;
            _logo.CreateTweenGroup(() => OnLogoMoved(camera)).MoveTo(new Vector2(_runtimeData.Scene.Width / 2, _runtimeData.Scene.Height / 2 - 50), 3.0f, EasingFunctions.SineEaseOut);
        }

        void MoveMap(Camera2D camera)
        {
            float delay = 0;
            float x = 0;
            float y = 0;
            switch (_mapState)
            {
                case 0:
                    x = _map.WidthInPixels - _runtimeData.Scene.Width;
                    y = _map.HeightInPixels - _runtimeData.Scene.Height;
                    delay = 12.0f;
                    _mapState++;
                    break;
                case 1:
                    x = 0;
                    y = _map.HeightInPixels - _runtimeData.Scene.Height;
                    delay = 10.0f;
                    _mapState++;
                    break;
                case 2:
                    x = _map.WidthInPixels - _runtimeData.Scene.Width;
                    y = 0;
                    delay = 12.0f;
                    _mapState++;
                    break;
                case 3:
                    x = 0;
                    y = 0;
                    delay = 10.0f;
                    _mapState = 0;
                    break;
            }
            _cameraAnimation = camera.CreateTweenGroup(() => MoveMap(camera)).MoveTo(new Vector2(x, y), delay, EasingFunctions.SineEaseInOut);
        }

        void OnLogoMoved(Camera2D camera)
        {
            MoveMap(camera);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);
            spriteBatch.Draw(_map);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: frozenMatrix);
            spriteBatch.Draw(_overlayTexture, new Rectangle(0, 0, _runtimeData.Scene.Width, _runtimeData.Scene.Height), Color.White);

            spriteBatch.Draw(_logo);

            spriteBatch.DrawString(_font, "Appuyez sur la touche espace...", new Vector2(480, 550), Color.White * ((float)_fontOpacity / 100));
            spriteBatch.End();
        }

        public void Execute(params string[] param)
        {

        }
    }
}
