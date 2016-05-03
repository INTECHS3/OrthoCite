using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace OrthoCite.Entities.MiniGames
{
    class Platformer : MiniGame
    {
        const int MIN_SPEED = 1;
        const int MAX_SPEED = 6;
        const int FRAMES_TO_NEXT_SPEED = 10;
        const int LATERAL_SPEED = 5;

        const int PLATFORM_MIN_TOP_BOTTOM_OFFSET = 10;

        RuntimeData _runtimeData;
        Random _random;

        Texture2D _background;
        Texture2D _platform;
        Texture2D _playerJump;
        Texture2D _playerStraight;

        SpriteFont _font;

        /* Game state */
        List<Vector2> _grid;
        Vector2 _playerPosition;
        int _currentSpeed = 0;
        int _framesSincePreviousSpeed = FRAMES_TO_NEXT_SPEED - 1;
        bool _isLanded = true;
        bool _onPlatform = false;
        bool _faceRight;
        List<Rectangle> _platforms;

        enum Direction
        {
            NONE = 0,
            UP = -1,
            DOWN = 1
        }

        Direction _direction = Direction.NONE;

        public Platformer(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _platforms = new List<Rectangle>();
            _random = new Random();
            _grid = new List<Vector2>();
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _background = content.Load<Texture2D>("minigames/platformer/background");
            _platform = content.Load<Texture2D>("minigames/platformer/platform");
            _playerJump = content.Load<Texture2D>("minigames/platformer/player-jump");
            _playerStraight = content.Load<Texture2D>("minigames/platformer/player-straight");

            _font = content.Load<SpriteFont>("debug");

            _playerPosition = new Vector2((_runtimeData.Window.Width / 2) - (_playerStraight.Width / 2), _runtimeData.Window.Height - _playerStraight.Height);

            Start();
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            /* Handle move */
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                _isLanded = false;
                if (-_currentSpeed < MAX_SPEED)
                {
                    if (++_framesSincePreviousSpeed == FRAMES_TO_NEXT_SPEED)
                    {
                        _framesSincePreviousSpeed = 0;
                        _currentSpeed--;
                    }
                }
            }
            else if (!_isLanded)
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

            _playerPosition.Y += _currentSpeed;

            if (_currentSpeed == 0) _direction = Direction.NONE;
            else _direction = _currentSpeed > 0 ? Direction.DOWN : Direction.UP;


            if (keyboardState.IsKeyDown(Keys.Left))
            {
                _playerPosition.X -= LATERAL_SPEED;
                _faceRight = false;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                _playerPosition.X += LATERAL_SPEED;
                _faceRight = true;
            }

            /* Handle collisions */

            // Borders
            if (_playerPosition.X + _playerStraight.Width > _runtimeData.Window.Width) _playerPosition.X = _runtimeData.Window.Width - _playerStraight.Width; // Right
            if (_playerPosition.X < 0) _playerPosition.X = 0; // Left
            if (_playerPosition.Y + _playerStraight.Height > _runtimeData.Window.Height) // Bottom
            {
                _playerPosition.Y = _runtimeData.Window.Height - _playerStraight.Height;
                _currentSpeed = 0;
                _isLanded = true;
            }
            if (_playerPosition.Y < 0) // Top
            {
                _playerPosition.Y = 0;
                _currentSpeed = 0;
            }

            // Platforms
            if (_direction != Direction.UP)
            {
                bool wasOnPlatform = _onPlatform;
                bool stillOnPlatform = false;
                foreach (var platform in _platforms)
                {
                    if ((_playerPosition.X + _playerStraight.Width > platform.X && _playerPosition.X <= platform.X + _platform.Width) && (platform.Y >= _playerPosition.Y + _playerStraight.Height - MAX_SPEED && platform.Y <= _playerPosition.Y + _playerStraight.Height))
                    {
                        _playerPosition.Y = platform.Y - _playerStraight.Height;
                        _currentSpeed = 0;
                        _isLanded = true;
                        _onPlatform = true;
                        stillOnPlatform = true;
                    }
                }

                if (wasOnPlatform && !stillOnPlatform)
                {
                    _onPlatform = false;
                    _isLanded = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, new Vector2(0, 0));
            foreach (var platform in _platforms)
            {
                spriteBatch.Draw(_platform, new Vector2(platform.X, platform.Y), _direction == Direction.UP ? Color.White * 0.7f : Color.White);
            }

            spriteBatch.Draw(_isLanded ? _playerStraight : _playerJump, _playerPosition, null, null, null, 0, null, null, _faceRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            spriteBatch.DrawString(_font, $"Direction {(_direction == Direction.DOWN ? "Down" : _direction == Direction.UP ? "Up" : "None")}", new Vector2(10, 300), Color.White);

        }

        internal override void Start()
        {
            /* Generate grid */
            int columns = _runtimeData.Window.Width / (_platform.Width + _playerStraight.Width + PLATFORM_MIN_TOP_BOTTOM_OFFSET * 2);
            int lines = _runtimeData.Window.Height / (_platform.Height + _playerStraight.Height);
            lines--; // FIX ME

            int xOffset = (_runtimeData.Window.Width % (columns * (_platform.Width + _playerStraight.Width + PLATFORM_MIN_TOP_BOTTOM_OFFSET * 2) - _playerStraight.Width - PLATFORM_MIN_TOP_BOTTOM_OFFSET * 2)) / 2;
            int yOffset = PLATFORM_MIN_TOP_BOTTOM_OFFSET + _playerStraight.Height;

            for (int line = 0; line < lines; line++)
            {
                for (int column = 0; column < columns; column++)
                {
                    _grid.Add(new Vector2(xOffset + column * (_platform.Width + _playerStraight.Width + PLATFORM_MIN_TOP_BOTTOM_OFFSET * 2), yOffset + line * (_platform.Height + _playerStraight.Height)));
                }
            }

            /* Generate platforms */

            int count = 10;
            for (int i = 0; i < count; i++)
            {
                int randomGridIndex = _random.Next(0, _grid.Count);
                Vector2 position = _grid[randomGridIndex];
                _grid.RemoveAt(randomGridIndex);
                _platforms.Add(new Rectangle((int)position.X, (int)position.Y, _platform.Width, _platform.Height));
            }
        }
    }
}
