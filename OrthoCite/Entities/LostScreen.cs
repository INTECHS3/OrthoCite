using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;

namespace OrthoCite.Entities
{
    class LostScreen : IEntity
    {
        enum State
        {
            BEGINNING,
            LOST,
            CREDIT,
            LIVE1,
            LIVE2,
            LIVE3,
            SWITCH_MAP
        }

        RuntimeData _runtimeData;

        State _state;

        Texture2D _pixelTexture;
        Texture2D _coinTexture;

        Sprite _coin;

        SpriteFont _font;

        SoundEffect _ding;
        SoundEffect _newLive;

        DateTime _timeStarted;

        public LostScreen(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;

            _state = State.BEGINNING;
            MediaPlayer.Stop();
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.Black });
            _coinTexture = content.Load<Texture2D>("lostscreen/coin");

            _font = content.Load<SpriteFont>("lostscreen/font");

            _ding = content.Load<SoundEffect>("lostscreen/ding");
            _newLive = content.Load<SoundEffect>("lostscreen/newLive");

            _coin = new Sprite(_coinTexture);
            _coin.Position = new Vector2(_runtimeData.Scene.Width / 2, _runtimeData.Scene.Height / 2);
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (_state == State.BEGINNING)
            {
                _timeStarted = DateTime.Now;
                _state = State.LOST;
            }

            TimeSpan interval = DateTime.Now - _timeStarted;

            switch(_state)
            {
                case State.LOST:
                    if (interval < TimeSpan.FromSeconds(2)) return;
                    _timeStarted = DateTime.Now;
                    _state = State.CREDIT;
                    break;
                case State.CREDIT:
                    if (interval < TimeSpan.FromSeconds(1)) return;
                    _runtimeData.Credits++;
                    _ding.Play();
                    _timeStarted = DateTime.Now;
                    _state = State.LIVE1;
                    break;
                case State.LIVE1:
                    if (interval < TimeSpan.FromSeconds(1)) return;
                    _runtimeData.Lives++;
                    _newLive.Play();
                    _timeStarted = DateTime.Now;
                    _state = State.LIVE2;
                    break;
                case State.LIVE2:
                    if (interval < TimeSpan.FromMilliseconds(300)) return;
                    _runtimeData.Lives++;
                    _newLive.Play();
                    _timeStarted = DateTime.Now;
                    _state = State.LIVE3;
                    break;
                case State.LIVE3:
                    if (interval < TimeSpan.FromMilliseconds(300)) return;
                    _runtimeData.Lives++;
                    _newLive.Play();
                    _timeStarted = DateTime.Now;
                    _state = State.SWITCH_MAP;
                    break;
                case State.SWITCH_MAP:
                    if (interval < TimeSpan.FromSeconds(2)) return;
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, _runtimeData.Scene.Width, _runtimeData.Scene.Height), Color.White);

            if (_state == State.LOST)
            {
                spriteBatch.DrawString(_font, "PERDU !", new Vector2(570, 350), Color.White);
            }
            else
            {
                spriteBatch.Draw(_coin);
                spriteBatch.DrawString(_font, "x " + _runtimeData.Credits.ToString(), new Vector2(630, 450), Color.White);
            }

            spriteBatch.End();
        }

        public void Execute(params string[] param)
        {

        }
    }
}
