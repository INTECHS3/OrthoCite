using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Animations.Tweens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace OrthoCite.Entities
{
    class Introduction : IEntity
    {
        RuntimeData _runtimeData;

        Texture2D _backgroundTexture;
        Texture2D _logoTexture;

        Sprite _background;
        Sprite _logo;
     
        Song _song;

        bool _animationStarted;

        public Introduction(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _backgroundTexture = content.Load<Texture2D>("intro/background");
            _logoTexture = content.Load<Texture2D>("intro/logo");
            _background = new Sprite(_backgroundTexture);
            _background.Origin = new Vector2(0, 0);
            _background.Position = new Vector2(0, _runtimeData.Scene.Height - _backgroundTexture.Height);
            _logo = new Sprite(_logoTexture);
            _logo.Position = new Vector2(_runtimeData.Scene.Width / 2, -_logoTexture.Height);

            _song = content.Load<Song>("intro/music");
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (!_animationStarted)
            {
                StartAnimation();
                MediaPlayer.Play(_song);
            }
        }

        public void StartAnimation()
        {
            _animationStarted = true;
            _background.CreateTweenGroup(OnBackgroundReached).MoveTo(new Vector2(0, 0), 2.0f, EasingFunctions.SineEaseIn);
        }

        void OnBackgroundReached()
        {
            _logo.CreateTweenGroup(OnLogoMoved).MoveTo(new Vector2(_runtimeData.Scene.Width / 2, _runtimeData.Scene.Height / 2), 3.0f, EasingFunctions.SineEaseOut);
        }

        void OnLogoMoved()
        {
            _logo.CreateTweenGroup(OnLogoFaded).ScaleTo(new Vector2(4, 4), 2.0f, EasingFunctions.SineEaseIn).FadeTo(0, 2.0f, EasingFunctions.SineEaseIn);
        }

        void OnLogoFaded()
        {
            _runtimeData.OrthoCite.ChangeGameContext(GameContext.MENU);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);

            spriteBatch.Draw(_background);
            spriteBatch.Draw(_logo);

            spriteBatch.End();
        }



        public void Execute(params string[] param)
        {

        }
    }
}
