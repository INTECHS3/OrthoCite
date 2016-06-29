using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OrthoCite.Entities;
using OrthoCite.Entities.MiniGames;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Runtime.InteropServices;
using System;
using MonoGame.Extended.Animations;
using System.IO;
using System.Reflection;
using OrthoCite.Helpers;
using System.Threading.Tasks;

namespace OrthoCite
{
    public enum GameContext
    {
        NONE,
        INTRO,
        RULES,
        LOST_SCREEN,
        MAP,
        MINIGAME_PLATFORMER,
        MINIGAME_BOSS,
        MINIGAME_DOORGAME,
        MINIGAME_REARRANGER,
        MINIGAME_GUESSGAME,
        MINIGAME_THROWGAME,
        MINIGAME_STOPGAME,
        HOME
    }


    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OrthoCite : Game
    {

        const GameContext STARTING_ENTITY = GameContext.INTRO;



        BoxingViewportAdapter _viewportAdapter;
        Camera2D _camera;
        RuntimeData _runtimeData;
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        List<IEntity> _entities;

        const int SCENE_WIDTH = 1366;
        const int SCENE_HEIGHT = 768;

        int _miniGameDistrict;
        GameContext _pendingRulesMiniGame;
        GameContext _gameContext;
        public bool _gameContextChanged;
        GameQueue _queue;
        
        public static void writeSpacerConsole() => System.Console.WriteLine("===========================================");

        
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public OrthoCite()
        {
                    
            _runtimeData = new RuntimeData();
            _runtimeData.OrthoCite = this;
            _runtimeData.DataSave = new DataSave(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\datasaves");
            _graphics = new GraphicsDeviceManager(this);
            _queue = new GameQueue();

            _entities = new List<IEntity>();

            ChangeGameContext(STARTING_ENTITY, _runtimeData.DistrictActual);

#if DEBUG
            _graphics.PreferredBackBufferWidth = 928;
            _graphics.PreferredBackBufferHeight = 512;
            AllocConsole();
            System.Console.WriteLine("=== OrthoCite debug console ===");
#else
            _graphics.PreferredBackBufferWidth = SCENE_WIDTH;
            _graphics.PreferredBackBufferHeight = SCENE_HEIGHT;
            _graphics.IsFullScreen = true;
#endif

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            EventInput.Initialize(Window);
            Components.Add(new AnimationComponent(this));
            _viewportAdapter = new BoxingViewportAdapter(Window, _graphics, SCENE_WIDTH, SCENE_HEIGHT);
            _runtimeData.ViewAdapter = _viewportAdapter;
            _runtimeData.Scene = new Rectangle(0, 0, SCENE_WIDTH, SCENE_HEIGHT);
            _runtimeData.DialogBox = new DialogBox(_runtimeData);
            _camera = new Camera2D(_viewportAdapter);

            base.Initialize();


           
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

            foreach (var entity in _entities)
            {
                entity.LoadContent(this.Content, this.GraphicsDevice);
            }
            recordConsole(_queue);


           

        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (var entity in _entities)
            {
                entity.UnloadContent();
            }
        }
       
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F12))
            {
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
            }

            string[] message;
            if((message = _queue.Pull()) != null)
            {
                foreach(IEntity i in _entities)
                {
                    i.Execute(message);
                }
            }
            
            

            foreach (var entity in _entities)
            {
                entity.Update(gameTime, Keyboard.GetState(), _camera);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _runtimeData.GameTime = gameTime;
            _graphics.GraphicsDevice.Clear(Color.Black);
            
            foreach (var entity in _entities)
            {
                entity.Draw(_spriteBatch, _viewportAdapter.GetScaleMatrix(), _camera.GetViewMatrix());
            }

            if (_gameContextChanged) PopulateEntitiesFromGameContext();

            base.Draw(gameTime);
        }

        public void ChangeGameContext(GameContext context, int district = 1)
        {
            _gameContext = context;
            _gameContextChanged = true;
            _miniGameDistrict = _runtimeData.DistrictActual;
        }

        public void RulesDone()
        {
            _gameContextChanged = true;
        }

        void PopulateEntitiesFromGameContext()
        {
            _entities.Clear();
            writeSpacerConsole();
            IsMouseVisible = true;
            Console.Write("Context switched to ");

            if (_pendingRulesMiniGame != GameContext.NONE)
            {
                _gameContext = _pendingRulesMiniGame;
                _pendingRulesMiniGame = GameContext.NONE;
            }
            else
            {
                if (_gameContext != GameContext.INTRO && _gameContext != GameContext.LOST_SCREEN && _gameContext != GameContext.MAP && _gameContext != GameContext.RULES && _gameContext != GameContext.HOME)
                {
                    // if minigame asked and rules not shown
                    _pendingRulesMiniGame = _gameContext;
                    _gameContext = GameContext.RULES;
                }
            }

            switch (_gameContext)
            {
                case GameContext.INTRO:
                    Console.WriteLine("introduction");
                    _entities.Add(new Introduction(_runtimeData));
                    break;
                case GameContext.LOST_SCREEN:
                    Console.WriteLine("lost screen");
                    _entities.Add(new LostScreen(_runtimeData));
                    break;
                case GameContext.MAP:
                    Console.WriteLine("map");
                    _entities.Add(new Map(_runtimeData));
                    break;
                case GameContext.RULES:
                    Console.WriteLine("rules");
                    Rules rules = new Rules(_runtimeData);
                    rules.LoadContent(this.Content, this.GraphicsDevice);
                    rules.SetMiniGame(_pendingRulesMiniGame);
                    _entities.Add(rules);
                    break;
                case GameContext.MINIGAME_PLATFORMER:
                    Console.WriteLine("platformer minigame");
                    Platformer platformer = new Platformer(_runtimeData);
                    platformer.SetDistrict(_miniGameDistrict);
                    platformer.LoadContent(this.Content, this.GraphicsDevice); // content needs to be loaded before calling start
                    platformer.Start();
                    _entities.Add(platformer);
                    break;
                case GameContext.MINIGAME_DOORGAME:
                    Console.WriteLine("DoorGame");
                    DoorGame doorGame = new DoorGame(_runtimeData);
                    doorGame.SetDistrict(_miniGameDistrict);
                    _entities.Add(doorGame);
                    break;
                case GameContext.MINIGAME_REARRANGER:
                    Console.WriteLine("Rearranger");
                    Rearranger rearranger = new Rearranger(_runtimeData);
                    rearranger.SetDistrict(_miniGameDistrict);
                    _entities.Add(rearranger);
                    break;
                case GameContext.MINIGAME_BOSS:
                    if (okToGoInBoss(1))
                    {
                        _runtimeData.DialogBox.AddDialog("Impossible d'acceder à ce niveaux, tu n'as pas debloquer les niveaux du district !", 4).Show();
                        _entities.Add(new Map(_runtimeData));
                        break;
                    }
                    Console.WriteLine("boss minigame");
                    _entities.Add(new BossGame(_runtimeData));
                    break;
                case GameContext.MINIGAME_THROWGAME:
                    if (okToGoInBoss(3))
                    {
                        _runtimeData.DialogBox.AddDialog("Impossible d'acceder à ce niveaux, tu n'as pas debloquer les niveaux du district !", 4).Show();
                        _entities.Add(new Map(_runtimeData));
                        break;
                    }
                    Console.WriteLine("ThrowGame");
                    _entities.Add(new ThrowGame(_runtimeData));
                    break;
                case GameContext.MINIGAME_GUESSGAME:
                    if (okToGoInBoss(1))
                    {
                        _runtimeData.DialogBox.AddDialog("Impossible d'acceder à ce niveaux, tu n'as pas debloquer les niveaux du district !", 4).Show();
                        _entities.Add(new Map(_runtimeData));
                        break;
                    }
                    Console.WriteLine("GuessGame");
                    _entities.Add(new GuessGame(_runtimeData));
                    break;
                case GameContext.MINIGAME_STOPGAME:
                    if (okToGoInBoss(2))
                    {
                        _runtimeData.DialogBox.AddDialog("Impossible d'acceder à ce niveaux, tu n'as pas debloquer les niveaux du district !", 4).Show();
                        _entities.Add(new Map(_runtimeData));
                        break;
                    }
                    Console.WriteLine("StopGame");
                    _entities.Add(new StopGame(_runtimeData));
                    break;
                case GameContext.HOME:
                    Console.WriteLine("Home");
                    _entities.Add(new Home(_runtimeData));
                    break;
            }

            if (_gameContext != GameContext.INTRO)
            {
                _entities.Add(_runtimeData.DialogBox);
                _entities.Add(new Lives(_runtimeData));
                _entities.Add(new MenuInGame(_runtimeData));
                _entities.Add(new AnswerBox(_runtimeData));
            }
            
            _gameContextChanged = false;

#if DEBUG
            _entities.Add(new DebugLayer(_runtimeData));
#endif

            LoadContent();
        }

        void recordConsole(GameQueue queue)
        {
            Task.Factory.StartNew(() => {
                string cmdTmp = Console.ReadLine();
                queue.Push(cmdTmp.Split(' '));
                recordConsole(queue);
            });
        }

        internal void Leave(Button button)
        {
            Console.WriteLine("Exit");
            Exit();
          
        }
        internal void ClearSave(Button button)
        {
            Console.WriteLine("Clear Save");
            _runtimeData.DataSave.Clear();
            _runtimeData.DataSave.Save();
            Console.WriteLine("Reload");
            ChangeGameContext(GameContext.MAP);
        }

        private bool okToGoInBoss(int nbDistrict)
        {

            if (nbDistrict <= _runtimeData.DataSave.District && _runtimeData.DataSave.MiniGameIsValidated(DataSaveMiniGame.DOORGAME) && _runtimeData.DataSave.MiniGameIsValidated(DataSaveMiniGame.REARRANGER) && _runtimeData.DataSave.MiniGameIsValidated(DataSaveMiniGame.PLATFORMER)) return false;

            
            return true;

        }
    }
}
