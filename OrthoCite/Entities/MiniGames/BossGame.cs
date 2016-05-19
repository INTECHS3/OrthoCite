using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace OrthoCite.Entities.MiniGames
{
    class BossGame : MiniGame
    {
        const int MIN_SPEED = 1;
        const int MAX_SPEED = 6;
        const int FRAMES_TO_NEXT_SPEED = 10;
        const int LATERAL_SPEED = 5;

        RuntimeData _runtimeData;
        Random _random;

        Texture2D _background;
        Texture2D _platform;
        Texture2D _playerJump;
        Texture2D _playerStraight;
        Texture2D _pixelTexture;

        SpriteFont _fontWord;

        // Game state
        string _currentSpellWord;
        string _currentSpellWordTyped;
        Vector2 _playerPosition;
        bool _lost = false;
        bool _won = false;
        int _bossLifePercentage = 100;

        public BossGame(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _random = new Random();

            EventInput.CharEntered += EventInput_CharEntered;
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _background = content.Load<Texture2D>("minigames/platformer/background");
            _platform = content.Load<Texture2D>("minigames/platformer/platform");
            _playerJump = content.Load<Texture2D>("minigames/platformer/player-jump");
            _playerStraight = content.Load<Texture2D>("minigames/platformer/player-straight");
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.White });

            _fontWord = content.Load<SpriteFont>("minigames/platformer/font-result");

            _playerPosition = new Vector2((_runtimeData.Scene.Width / 2) - (_playerStraight.Width / 2), _runtimeData.Scene.Height - _playerStraight.Height);

            Start();
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
        }

        private void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (e.Character == '\b')
            {
                if (_currentSpellWordTyped != "") _currentSpellWordTyped = _currentSpellWordTyped.Remove(_currentSpellWordTyped.Length - 1);
            }
            else
            {
                _currentSpellWordTyped += e.Character;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);

            spriteBatch.Draw(_playerStraight, _playerPosition);

            int barWidth = 300;
            int barHeight = 50;
            Vector2 barPosition = new Vector2(_runtimeData.Scene.Width - barWidth - 20, 20);
            int actualBarWidth = (_bossLifePercentage * barWidth) / 100;
            spriteBatch.Draw(_pixelTexture, new Rectangle((int)barPosition.X, (int)barPosition.Y, actualBarWidth, barHeight), Color.Red);
            spriteBatch.DrawString(_fontWord, _currentSpellWord, new Vector2(400, 200), Color.DarkSlateGray);
            spriteBatch.DrawString(_fontWord, _currentSpellWordTyped, new Vector2(400, 250), Color.Black);

            spriteBatch.End();
        }

        

        public override void Execute(params string[] param)
        {

        }

        internal override void Start()
        {
            _currentSpellWord = "test";
            _currentSpellWordTyped = "";
        }
    }
}
