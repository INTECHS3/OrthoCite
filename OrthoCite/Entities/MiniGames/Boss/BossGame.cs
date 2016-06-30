using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Animations.Tweens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace OrthoCite.Entities.MiniGames
{
    class BossGame : MiniGame
    {
        const int DISTRICT = 4;
        const int LATERAL_SPEED = 5;
        const int FIREBALL_INTERVAL = 200;

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
        bool _faceLeft;
        Sprite _enemy;
        Sprite _fireballEnemy;
        Sprite _fireballAlly;
        Sprite _attackBox;

        SpriteFont _fontWord;
        SpriteFont _fontPercentage;

        SpriteSheetAnimation _animationFireball;
        SpriteSheetAnimation _animationFireballEnemy;
        SpriteSheetAnimation _animationFireballAlly;

        SoundEffect _arghPlayer;
        SoundEffect _arghEnemy;
        SoundEffect _spell;
        SoundEffectInstance _spellInstance;

        Song _music;

        // Game state
        List<string> _words;
        List<Sprite> _fireballs;
        DateTime _lastFireball;

        string _currentSpellWord;
        string _currentSpellWordTyped;
        GameState _gameState;
        int _bossLifePercentage = 100;
        bool _waitingForInput = true;

        public BossGame(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _random = new Random();

            _words = new List<string>();
            _fireballs = new List<Sprite>();
            _lastFireball = DateTime.MaxValue;

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

            _spell = content.Load<SoundEffect>("minigames/boss/spell");
            _spellInstance = _spell.CreateInstance();
            _arghPlayer = content.Load<SoundEffect>("minigames/platformer/argh");
            _arghEnemy = content.Load<SoundEffect>("minigames/boss/arghEnemy");

            _music = content.Load<Song>("minigames/boss/music");

            var fireballTexture = content.Load<Texture2D>("minigames/boss/fireball");
            var fireballAtlas = TextureAtlas.Create(fireballTexture, 130, 50);
            _animationFireball = new SpriteSheetAnimation("fireballAnimation", fireballAtlas)
            {
                FrameDuration = 0.2f
            };

            var fireballEnemyTexture = content.Load<Texture2D>("minigames/boss/fireball_enemy");
            var fireballEnemyAtlas = TextureAtlas.Create(fireballEnemyTexture, 130, 50);
            _animationFireballEnemy = new SpriteSheetAnimation("fireballEnemyAnimation", fireballEnemyAtlas)
            {
                FrameDuration = 0.2f
            };

            _fireballEnemy = new Sprite(_animationFireballEnemy.CurrentFrame) { Origin = _player.Origin, IsVisible = false };

            var fireballAllyTexture = content.Load<Texture2D>("minigames/boss/fireball_ally");
            var fireballAllyAtlas = TextureAtlas.Create(fireballAllyTexture, 130, 50);
            _animationFireballAlly = new SpriteSheetAnimation("fireballAllyAnimation", fireballAllyAtlas)
            {
                FrameDuration = 0.2f
            };

            _fireballAlly = new Sprite(_animationFireballAlly.CurrentFrame) { Origin = _player.Origin, IsVisible = false };

            _attackBox = new Sprite(_attackBoxTexture);
            _attackBox.Position = new Vector2(_runtimeData.Scene.Width / 2, _runtimeData.Scene.Height / 2);

            _fontWord = content.Load<SpriteFont>("minigames/boss/font");
            _fontPercentage = content.Load<SpriteFont>("minigames/boss/percentagefont");

            Start();
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (_runtimeData.Lives == 0) _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
            if (_gameState != GameState.NONE) return;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                _player.Position = new Vector2(_player.Position.X - LATERAL_SPEED, _player.Position.Y);
                _faceLeft = true;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                _player.Position = new Vector2(_player.Position.X + LATERAL_SPEED, _player.Position.Y);
                _faceLeft = false;
            }

            if (_player.Position.X + _playerTexture.Width >= _runtimeData.Scene.Width - 300) _player.Position = new Vector2(_runtimeData.Scene.Width - _playerTexture.Width - 300, _player.Position.Y); // Right
            if (_player.Position.X < 0) _player.Position = new Vector2(0, _player.Position.Y); // Left

            _animationFireball.Update(gameTime);
            _animationFireballAlly.Update(gameTime);
            _fireballAlly.TextureRegion = _animationFireballAlly.CurrentFrame;
            _animationFireballEnemy.Update(gameTime);
            _fireballEnemy.TextureRegion = _animationFireballEnemy.CurrentFrame;

            if (_lastFireball == DateTime.MaxValue || DateTime.Now - _lastFireball >= TimeSpan.FromMilliseconds(FIREBALL_INTERVAL)) GenerateFireball();

            foreach (Sprite fireball in _fireballs) fireball.TextureRegion = _animationFireball.CurrentFrame;

            // Handle fireball collisions
            Sprite fireballToRemove = null;
            foreach (var fireball in _fireballs)
            {
                if ((_player.Position.X + _playerTexture.Width > fireball.Position.X && _player.Position.X <= fireball.Position.X + _playerTexture.Width) && (fireball.Position.Y + fireball.TextureRegion.Height - _playerTexture.Height >= _player.Position.Y))
                {
                    fireballToRemove = fireball;
                    _arghPlayer.Play();
                    _runtimeData.LooseLive();
                }
            }

            if (fireballToRemove != null) _fireballs.Remove(fireballToRemove);

            if (_runtimeData.Lives == 0)
            {
                _gameState = GameState.LOST;
                _runtimeData.DialogBox.AddDialog("Perdu !", 2).Show();
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
            }
            else if (_bossLifePercentage == 0)
            {
                _gameState = GameState.WON;
                _runtimeData.DialogBox.AddDialog("Gagné !", 2).Show();
                if (_runtimeData.DataSave.District == DISTRICT)
                {
                    _runtimeData.DataSave.District = DISTRICT + 1;
                    _runtimeData.DataSave.ClearMiniGames();
                    _runtimeData.DataSave.Save();
                }
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
            }
        }

        private void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (_gameState != GameState.NONE) return;
            if (!_waitingForInput) return;

            if (e.Character == '\u001b') return; // Escape

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
            _spellInstance.Play();
            _waitingForInput = false;
            _fireballAlly.IsVisible = true;
            _fireballAlly.Position = new Vector2(_player.Position.X + _playerTexture.Width, _player.Position.Y);
            _fireballAlly.Effect = SpriteEffects.FlipHorizontally;
            _fireballAlly.CreateTweenGroup(OnFireSpellOnEnemyEnd).MoveTo(new Vector2(_enemy.Position.X - _fireballAlly.TextureRegion.Width + 27, _enemy.Position.Y), 1.0f, EasingFunctions.SineEaseIn);
        }

        void OnFireSpellOnEnemyEnd()
        {
            _spellInstance.Stop();
            _arghEnemy.Play();
            _waitingForInput = true;
            _fireballAlly.IsVisible = false;
            _bossLifePercentage -= 20;
            LoadWord();
            _runtimeData.DialogBox.AddDialog("Aaaarrggh !", 2).Show();
        }

        public void FireSpellOnPlayer()
        {
            _spellInstance.Play();
            _waitingForInput = false;
            _fireballEnemy.IsVisible = true;
            _fireballEnemy.Position = new Vector2(_enemy.Position.X - _fireballEnemy.TextureRegion.Width + 27, _enemy.Position.Y);
            _fireballEnemy.Effect = SpriteEffects.None;
            _fireballEnemy.CreateTweenGroup(OnFireSpellOnPlayerEnd).MoveTo(new Vector2(_player.Position.X + _playerTexture.Width - 27, _player.Position.Y), 1.0f, EasingFunctions.SineEaseIn);
        }

        void OnFireSpellOnPlayerEnd()
        {
            _spellInstance.Stop();
            _arghPlayer.Play();
            _waitingForInput = true;
            _fireballEnemy.IsVisible = false;
            if(_runtimeData.Lives != 0)_runtimeData.LooseLive();
            LoadWord();
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

        public void GenerateFireball()
        {
            int posX = _random.Next(0, _runtimeData.Scene.Width - 300);
            Sprite fireball = new Sprite(_animationFireball.CurrentFrame) { Origin = _player.Origin, IsVisible = true };
            fireball.Rotation = -1.57f;
            fireball.Position = new Vector2(posX, 0);
            fireball.CreateTweenGroup(() => _fireballs.Remove(fireball)).MoveTo(new Vector2(posX, _runtimeData.Scene.Height + 11), 2.0f, EasingFunctions.SineEaseIn);
            _fireballs.Add(fireball);

            _lastFireball = DateTime.Now;
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            spriteBatch.Draw(_backgroundTexture, new Vector2(0, 0), Color.White);

            _player.Effect = _faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(_player);
            spriteBatch.Draw(_fireballAlly);
            spriteBatch.Draw(_fireballEnemy);
            spriteBatch.Draw(_enemy);

            int barWidth = 200;
            int barHeight = 20;
            Vector2 barPosition = new Vector2(_runtimeData.Scene.Width - barWidth - 40, 650);
            int actualBarWidth = (_bossLifePercentage * barWidth) / 100;
            spriteBatch.Draw(_pixelTexture, new Rectangle((int)barPosition.X, (int)barPosition.Y, actualBarWidth, barHeight), Color.Red);
            int percentageHeight = (int)_fontPercentage.MeasureString($"{_bossLifePercentage}%").X;
            spriteBatch.DrawString(_fontPercentage, $"{_bossLifePercentage}%", new Vector2(barPosition.X + (actualBarWidth / 2) - (percentageHeight / 2), barPosition.Y + 2), Color.White);

            spriteBatch.Draw(_attackBox);
            spriteBatch.DrawString(_fontWord, _currentSpellWord, new Vector2(670, 295), Color.Purple);
            spriteBatch.DrawString(_fontWord, _currentSpellWordTyped, new Vector2(425, 435), Color.Green);

            foreach (Sprite fireball in _fireballs)
            {
                // spriteBatch.Draw(_pixelTexture, new Rectangle((int)fireball.Position.X, (int)fireball.Position.Y - (int)fireball.GetBoundingRectangle().Height, (int)fireball.GetBoundingRectangle().Width, (int)fireball.GetBoundingRectangle().Height), null, Color.Yellow);
                spriteBatch.Draw(fireball);
            }

            spriteBatch.End();
        }

        

        public override void Execute(params string[] param)
        {
            switch (param[0])
            {
                case "exit":
                    try
                    {
                        _runtimeData.OrthoCite.Exit();
                    }
                    catch { Console.WriteLine("Can't Exit"); }
                    break;
                case "giveLife":
                    try { _runtimeData.GainLive(Int32.Parse(param[1])); Console.WriteLine($"You give {param[1]}"); }
                    catch { Console.WriteLine("use : giveLife {nbLife}"); }
                    break;
                case "clearText":
                    try { _runtimeData.DialogBox.AddDialog("SetEmpty", 2).Show(); }
                    catch { Console.WriteLine("use : error"); }
                    break;
                case "setLife":
                    try
                    {
                        if (Int32.Parse(param[1]) < _runtimeData.Lives)
                        {
                            int liveTmp = _runtimeData.Lives - Int32.Parse(param[1]);
                            _runtimeData.LooseLive(liveTmp);
                        }
                        else
                        {
                            int liveTmp = Int32.Parse(param[1]) - _runtimeData.Lives;
                            _runtimeData.GainLive(liveTmp);
                        }
                    }
                    catch { Console.WriteLine("use : error"); }
                    break;
                default:
                    Console.WriteLine($"Can't find method to invoke in {this.ToString()}");
                    break;
            }
        }

        internal override void Start()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music);
            LoadWords();
            LoadWord();
        }

        public void LoadWords()
        {
            _words.Clear();
            XmlDocument document = new XmlDocument();
            document.Load(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\dictionaries\boss.xml");
            XmlNode root = document.DocumentElement;

            foreach (XmlNode word in root.SelectNodes("word"))
            {
                _words.Add(word.InnerText);
            }
        }

        public void LoadWord()
        {
            _currentSpellWordTyped = "";
            _currentSpellWord = _words[_random.Next(0, _words.Count)];
            _words.Remove(_currentSpellWord);
        }
    }
}
