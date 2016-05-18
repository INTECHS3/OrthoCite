using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        SpriteFont _fontResult;

        struct Word
        {
            public string Value;
            public bool IsValid;
        }

        struct WordCollection
        {
            public Word Valid;
            public List<Word> Invalid;

            public WordCollection(string valid)
            {
                Valid = new Word { IsValid = true, Value = valid };
                Invalid = new List<Word>();
            }

            public void AddInvalid(string invalid)
            {
                Invalid.Add(new Word { Value = invalid });
            }
        }

        struct Platform
        {
            public Rectangle Coords;
            public Word Word;
        }

        /* Game state */
        List<Vector2> _grid;
        Vector2 _playerPosition;
        int _currentSpeed = 0;
        int _framesSincePreviousSpeed = FRAMES_TO_NEXT_SPEED - 1;
        bool _isLanded = true;
        bool _onPlatform = false;
        bool _faceRight;
        List<Platform> _platforms;
        bool _lost = false;
        bool _won = false;

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
            _platforms = new List<Platform>();
            _random = new Random();
            _grid = new List<Vector2>();
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _background = content.Load<Texture2D>("minigames/platformer/background");
            _platform = content.Load<Texture2D>("minigames/platformer/platform");
            _playerJump = content.Load<Texture2D>("minigames/platformer/player-jump");
            _playerStraight = content.Load<Texture2D>("minigames/platformer/player-straight");

            _font = content.Load<SpriteFont>("minigames/platformer/font");
            _fontResult = content.Load<SpriteFont>("minigames/platformer/font-result");

            _playerPosition = new Vector2((_runtimeData.Scene.Width / 2) - (_playerStraight.Width / 2), _runtimeData.Scene.Height - _playerStraight.Height);

            Start();
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            
            camera.Position = new Vector2(0, 0);
            camera.Zoom = 1;

            if(keyboardState.IsKeyDown(Keys.F12))
            {
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
            }

            if (_won || _lost) return;

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
            if (_playerPosition.X + _playerStraight.Width >= _runtimeData.Scene.Width) _playerPosition.X = _runtimeData.Scene.Width - _playerStraight.Width; // Right
            if (_playerPosition.X < 0) _playerPosition.X = 0; // Left
            if (_playerPosition.Y + _playerStraight.Height >= _runtimeData.Scene.Height) // Bottom
            {
                _playerPosition.Y = _runtimeData.Scene.Height - _playerStraight.Height;
                _currentSpeed = 0;
                _direction = Direction.NONE;
                _isLanded = true;
            }
            if (_playerPosition.Y <= 0) // Top
            {
                _playerPosition.Y = 0;
                _direction = Direction.NONE;
                _currentSpeed = 0;
            }

            // Platforms
            if (_direction != Direction.UP)
            {
                bool wasOnPlatform = _onPlatform;
                bool stillOnPlatform = false;
                foreach (var platform in _platforms)
                {
                    if ((_playerPosition.X + _playerStraight.Width > platform.Coords.X && _playerPosition.X <= platform.Coords.X + _platform.Width) && (platform.Coords.Y >= _playerPosition.Y + _playerStraight.Height - _currentSpeed && platform.Coords.Y <= _playerPosition.Y + _playerStraight.Height))
                    {
                        // On platform
                        _playerPosition.Y = platform.Coords.Y - _playerStraight.Height;
                        _currentSpeed = 0;
                        _isLanded = true;
                        _onPlatform = true;
                        stillOnPlatform = true;

                        if (keyboardState.IsKeyDown(Keys.E))
                        {
                            if (platform.Word.IsValid)
                            {
                                _lost = true;
                            }
                            else
                            {
                                _platforms.Remove(platform);
                                if (_platforms.Count == 1) _won = true;
                                break;
                            }

                        }
                    }
                }

                if (wasOnPlatform && !stillOnPlatform)
                {
                    _onPlatform = false;
                    _isLanded = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);
            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
            foreach (var platform in _platforms)
            {
                spriteBatch.Draw(_platform, new Vector2(platform.Coords.X, platform.Coords.Y), _direction == Direction.UP ? Color.White * 0.7f : Color.White);
                spriteBatch.DrawString(_font, platform.Word.Value, new Vector2(platform.Coords.X + 20, platform.Coords.Y + 20), Color.White);
            }

            spriteBatch.Draw(_isLanded ? _playerStraight : _playerJump, _playerPosition, null, null, null, 0, null, null, _faceRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            if (_lost) spriteBatch.DrawString(_fontResult, "Perdu", _playerPosition, Color.Red);
            if (_won) spriteBatch.DrawString(_fontResult, "Bravo", _playerPosition, Color.Green);
            spriteBatch.End();
        }

        public override void Dispose()
        {
            System.Console.WriteLine($"Disose class : {this.GetType().Name}");
        }

        public override void Execute(params string[] param)
        {

        }

        internal override void Start()
        {
            /* Generate grid */
            int columns = _runtimeData.Scene.Width / (_platform.Width + _playerStraight.Width + PLATFORM_MIN_TOP_BOTTOM_OFFSET * 2);
            int lines = _runtimeData.Scene.Height / (_platform.Height + _playerStraight.Height);
            lines--; // FIX ME

            int xOffset = (_runtimeData.Scene.Width % (columns * (_platform.Width + _playerStraight.Width + PLATFORM_MIN_TOP_BOTTOM_OFFSET * 2) - _playerStraight.Width - PLATFORM_MIN_TOP_BOTTOM_OFFSET * 2)) / 2;
            int yOffset = PLATFORM_MIN_TOP_BOTTOM_OFFSET + _playerStraight.Height;

            for (int line = 0; line < lines; line++)
            {
                for (int column = 0; column < columns; column++)
                {
                    _grid.Add(new Vector2(xOffset + column * (_platform.Width + _playerStraight.Width + PLATFORM_MIN_TOP_BOTTOM_OFFSET * 2), yOffset + line * (_platform.Height + _playerStraight.Height)));
                }
            }

            /* Generate platforms */

            WordCollection words = new WordCollection("orthographe");
            words.AddInvalid("ortographe");
            words.AddInvalid("ortograf");
            words.AddInvalid("aurtographe");
            words.AddInvalid("orthaugraphe");

            int count = 5;
            for (int i = 0; i < count; i++)
            {
                Vector2 position = _grid[_random.Next(0, _grid.Count)];
                _grid.Remove(position);

                Platform platform = new Platform();
                platform.Coords = new Rectangle((int)position.X, (int)position.Y, _platform.Width, _platform.Height);

                if (i == 0) platform.Word = words.Valid;
                else
                {
                    Word invalidWord = words.Invalid[_random.Next(0, words.Invalid.Count)];
                    words.Invalid.Remove(invalidWord);
                    platform.Word = invalidWord;
                }

                _platforms.Add(platform);
            }
        }
    }
}
