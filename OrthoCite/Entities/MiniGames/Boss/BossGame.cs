﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Animations.Tweens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;

namespace OrthoCite.Entities.MiniGames
{
    class BossGame : MiniGame
    {
        enum GameState
        {
            NONE,
            WON,
            LOST
        }

        RuntimeData _runtimeData;
        Random _random;

        Texture2D _backgroundTexture;
        Texture2D _playerTexture;
        Texture2D _enemyTexture;
        Texture2D _pixelTexture;
        Texture2D _attackBoxTexture;

        Sprite _player;
        Sprite _enemy;
        Sprite _fireball;
        Sprite _attackBox;

        SpriteFont _fontWord;

        SpriteSheetAnimation _animation;

        // Game state
        string _currentSpellWord;
        string _currentSpellWordTyped;
        GameState _gameState;
        int _bossLifePercentage = 100;
        bool _waitingForInput = true;

        public BossGame(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _random = new Random();

            EventInput.CharEntered += EventInput_CharEntered;
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _backgroundTexture = content.Load<Texture2D>("minigames/boss/background");
            _playerTexture = content.Load<Texture2D>("minigames/platformer/player-straight");
            _enemyTexture = content.Load<Texture2D>("minigames/boss/enemy");
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.White });
            _attackBoxTexture = content.Load<Texture2D>("minigames/boss/attackbox");

            _player = new Sprite(_playerTexture);
            _player.Origin = new Vector2(0, 0);
            _player.Position = new Vector2((_runtimeData.Scene.Width / 2) - (_playerTexture.Width / 2), _runtimeData.Scene.Height - _playerTexture.Height);
            _enemy = new Sprite(_enemyTexture);
            _enemy.Origin = new Vector2(0, 0);
            _enemy.Position = new Vector2(_runtimeData.Scene.Width - _enemyTexture.Width - 100, _runtimeData.Scene.Height - _enemyTexture.Height);

            var fireballTexture = content.Load<Texture2D>("minigames/boss/fireball");
            var fireballAtlas = TextureAtlas.Create(fireballTexture, 130, 50);
            _animation = new SpriteSheetAnimation("fireballAnimation", fireballAtlas)
            {
                FrameDuration = 0.2f
            };
            
            _fireball = new Sprite(_animation.CurrentFrame) { Origin = _player.Origin, IsVisible = false };

            _attackBox = new Sprite(_attackBoxTexture);
            _attackBox.Position = new Vector2(_runtimeData.Scene.Width / 2, _runtimeData.Scene.Height / 2);

            _fontWord = content.Load<SpriteFont>("minigames/platformer/font-result");

            Start();
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (_gameState != GameState.NONE) return;

            _animation.Update(gameTime);
            _fireball.TextureRegion = _animation.CurrentFrame;

            if (_runtimeData.Lives == 0)
            {
                _gameState = GameState.LOST;
                _runtimeData.DialogBox.AddDialog("Perdu !", 2).Show();
            }
            else if (_bossLifePercentage == 0)
            {
                _gameState = GameState.WON;
                _runtimeData.DialogBox.AddDialog("Gagné !", 2).Show();
            }

            if (_gameState != GameState.NONE) _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
        }

        private void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (_gameState != GameState.NONE) return;
            if (!_waitingForInput) return;

            if (e.Character == '\b')
            {
                if (_currentSpellWordTyped != "") _currentSpellWordTyped = _currentSpellWordTyped.Remove(_currentSpellWordTyped.Length - 1);
            }
            else
            {
                _currentSpellWordTyped += e.Character;
            }

            int currentIndex = _currentSpellWordTyped.Length - 1;
            if (currentIndex < 0) return;
            if (currentIndex > _currentSpellWord.Length - 1) return;

            if (_currentSpellWord[currentIndex] != _currentSpellWordTyped[currentIndex])
            {
                Mistyped();
            }

            if (_currentSpellWordTyped.Length == _currentSpellWord.Length)
            {
                Welltyped();
            }
        }

        public void FireSpellOnEnemy()
        {
            _waitingForInput = false;
            _fireball.IsVisible = true;
            _fireball.Position = new Vector2(_player.Position.X + _playerTexture.Width, _player.Position.Y);
            _fireball.Effect = SpriteEffects.FlipHorizontally;
            _fireball.CreateTweenGroup(OnFireSpellOnEnemyEnd).MoveTo(new Vector2(_enemy.Position.X - _fireball.TextureRegion.Width + 27, _enemy.Position.Y), 1.0f, EasingFunctions.SineEaseIn);
        }

        void OnFireSpellOnEnemyEnd()
        {
            _waitingForInput = true;
            _fireball.IsVisible = false;
            _bossLifePercentage -= 20;
            GenerateWord();
            _runtimeData.DialogBox.AddDialog("Aaaarrggh !", 2).Show();
        }

        public void FireSpellOnPlayer()
        {
            _waitingForInput = false;
            _fireball.IsVisible = true;
            _fireball.Position = new Vector2(_enemy.Position.X - _fireball.TextureRegion.Width + 27, _enemy.Position.Y);
            _fireball.Effect = SpriteEffects.None;
            _fireball.CreateTweenGroup(OnFireSpellOnPlayerEnd).MoveTo(new Vector2(_player.Position.X + _playerTexture.Width - 27, _player.Position.Y), 1.0f, EasingFunctions.SineEaseIn);
        }

        void OnFireSpellOnPlayerEnd()
        {
            _waitingForInput = true;
            _fireball.IsVisible = false;
            _runtimeData.Lives -= 1;
            GenerateWord();
            _runtimeData.DialogBox.AddDialog("Raté !", 2).Show();
        }

        public void Mistyped()
        {
            FireSpellOnPlayer();
        }

        public void Welltyped()
        {
            FireSpellOnEnemy();
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            spriteBatch.Draw(_backgroundTexture, new Vector2(0, 0), Color.White);

            spriteBatch.Draw(_player);
            spriteBatch.Draw(_fireball);
            spriteBatch.Draw(_enemy);

            int barWidth = 200;
            int barHeight = 20;
            Vector2 barPosition = new Vector2(_runtimeData.Scene.Width - barWidth - 40, 650);
            int actualBarWidth = (_bossLifePercentage * barWidth) / 100;
            spriteBatch.Draw(_pixelTexture, new Rectangle((int)barPosition.X, (int)barPosition.Y, actualBarWidth, barHeight), Color.Red);

            spriteBatch.Draw(_attackBox);
            spriteBatch.DrawString(_fontWord, _currentSpellWord, new Vector2(650, 310), Color.Red);
            spriteBatch.DrawString(_fontWord, _currentSpellWordTyped, new Vector2(450, 440), Color.Black);

            spriteBatch.End();
        }

        

        public override void Execute(params string[] param)
        {

        }


        void GenerateWord()
        {
            _currentSpellWordTyped = "";

            string[] words = new string[] { "constitution", "bateaux", "âge", "goéland", "Noël" };
            _currentSpellWord = words[_random.Next(0, words.Length)];
        }

        internal override void Start()
        {
            GenerateWord();
        }
    }
}
