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

namespace OrthoCite
{
    public enum GameContext
    {
        INTRO,
        MENU,
        LOST_SCREEN,
        MAP,
        MINIGAME_PLATFORMER,
        MINIGAME_BOSS,
        MINIGAME_DOORGAME
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OrthoCite : Game
    {
        const GameContext STARTING_ENTITY = GameContext.MENU;

        BoxingViewportAdapter _viewportAdapter;
        Camera2D _camera;
        RuntimeData _runtimeData;
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        List<IEntity> _entities;

        const int SCENE_WIDTH = 1366;
        const int SCENE_HEIGHT = 768;

        GameContext _gameContext;
        public bool _gameContextChanged;
        

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

            _entities = new List<IEntity>();

            ChangeGameContext(STARTING_ENTITY);

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

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, SCENE_WIDTH, SCENE_HEIGHT);
            _runtimeData.ViewAdapter = _viewportAdapter;
            _runtimeData.Scene = new Rectangle(0, 0, SCENE_WIDTH, SCENE_HEIGHT);
            _runtimeData.Lives = 3;
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

#if DEBUG
            if(Keyboard.GetState().IsKeyDown(Keys.F11)) { recordConsole();  }
#endif
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

        public void ChangeGameContext(GameContext context)
        {
            _gameContext = context;
            _gameContextChanged = true;
        }

        void PopulateEntitiesFromGameContext()
        {
            _entities.Clear();
            writeSpacerConsole();
            Console.Write("Context switched to ");

            switch (_gameContext)
            {
                case GameContext.INTRO:
                    Console.WriteLine("introduction");
                    _entities.Add(new Introduction(_runtimeData));
                    break;
                case GameContext.MENU:
                    Console.WriteLine("menu");
                    _entities.Add(new Mainmenu(_runtimeData));
                    break;
                case GameContext.LOST_SCREEN:
                    Console.WriteLine("lost screen");
                    _entities.Add(new LostScreen(_runtimeData));
                    break;
                case GameContext.MAP:
                    Console.WriteLine("map");
                    _entities.Add(new Map(_runtimeData));
                    break;
                case GameContext.MINIGAME_PLATFORMER:
                    Console.WriteLine("platformer minigame");
                    _entities.Add(new Platformer(_runtimeData));
                    break;
                case GameContext.MINIGAME_BOSS:
                    Console.WriteLine("boss minigame");
                    _entities.Add(new BossGame(_runtimeData));
                    break;
                case GameContext.MINIGAME_DOORGAME:
                    Console.WriteLine("DoorGame");
                    _entities.Add(new DoorGame(_runtimeData));
                    break;
            }

            if (_gameContext != GameContext.MENU && _gameContext != GameContext.INTRO)
            {
                _entities.Add(_runtimeData.DialogBox);
                _entities.Add(new Lives(_runtimeData));
            }

            _gameContextChanged = false;

#if DEBUG
            _entities.Add(new DebugLayer(_runtimeData));
#endif

            LoadContent();
        }

        private void recordConsole()
        {
            string cmdTmp = System.Console.ReadLine();
            string[] cmd = cmdTmp.Split(' ');
            foreach (var entity in _entities)
            {
                entity.Execute(cmd);
            }
        }
    }
}
