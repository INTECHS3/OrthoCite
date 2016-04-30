using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OrthoCite.Entities.MiniGames
{
    class Platformer : MiniGame
    {
        const int MIN_SPEED = 1;
        const int MAX_SPEED = 4;
        const int FRAMES_TO_NEXT_SPEED = 10;

        RuntimeData _runtimeData;

        Texture2D _background;
        Texture2D _platform;
        Texture2D _playerJump;
        Texture2D _playerStraight;
        bool _faceRight;

        /* Game state */
        Vector2 _playerPosition;
        int _currentSpeed = 0;
        int _framesSincePreviousSpeed = 0;

        public Platformer(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _background = content.Load<Texture2D>("minigames/platformer/background");
            _platform = content.Load<Texture2D>("minigames/platformer/platform");
            _playerJump = content.Load<Texture2D>("minigames/platformer/player-jump");
            _playerStraight = content.Load<Texture2D>("minigames/platformer/player-straight");

            _playerPosition = new Vector2((_runtimeData.Window.Width / 2) - (_playerStraight.Width / 2), _runtimeData.Window.Height - _playerStraight.Height);
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                if (_currentSpeed < MAX_SPEED)
                {
                    if (++_framesSincePreviousSpeed == FRAMES_TO_NEXT_SPEED)
                    {
                        _framesSincePreviousSpeed = 0;
                        _currentSpeed++;
                    }
                }
            }

            _playerPosition.Y -= _currentSpeed;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                _playerPosition.X -= _currentSpeed;
                _faceRight = false;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                _playerPosition.X += _currentSpeed;
                _faceRight = true;
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, new Vector2(0, 0));
            spriteBatch.Draw(_platform, new Vector2(20, 20));

            spriteBatch.Draw(_playerStraight, _playerPosition, null, null, null, 0, null, null, _faceRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            
        }

        internal override void Start()
        {
            throw new NotImplementedException();
        }
    }
}
