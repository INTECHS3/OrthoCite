using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OrthoCite.Entities;
using OrthoCite.Entities.MiniGames;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Runtime.InteropServices;


namespace OrthoCite
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OrthoCite : Game
    {
        public enum nameEntity
        {
            NONE,
            DEBUG,
            MAP,
            PLATFORMER
        }

        BoxingViewportAdapter _viewportAdapter;
        Camera2D _camera;
        RuntimeData _runtimeData;
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Dictionary<nameEntity, IEntity> _entities;

        public const int SCENE_WIDTH = 1366;
        public const int SCENE_HEIGHT = 768;

        public nameEntity _entitiesSelect;
        public bool _entitiesModified;
        
        private void onInstanceChange()
        {
                switch (_entitiesSelect)
                {
                    case nameEntity.PLATFORMER:
                        _entities[nameEntity.MAP].Dispose();
                        _entities.Remove(nameEntity.MAP);
                        //_entities.Add(nameEntity.PLATFORMER, new Platformer(_runtimeData));
                        break;
                    default:
                        System.Console.WriteLine("Nothing Entities Selected");
                        break;

                }
                _entitiesModified = false;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public OrthoCite()
        {
            
            
            _runtimeData = new RuntimeData();
            _graphics = new GraphicsDeviceManager(this);

            _entities = new Dictionary<nameEntity, IEntity>();
            _entitiesModified = false;
            _entitiesSelect = nameEntity.NONE;

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
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, SCENE_WIDTH, SCENE_HEIGHT);
            _runtimeData.viewAdapter = _viewportAdapter;
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

            _runtimeData.Window = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

           _entities.Add(nameEntity.MAP,new Map(_runtimeData, this));

#if DEBUG
            _entities.Add(nameEntity.DEBUG,new DebugLayer(_runtimeData));
#endif


            foreach (KeyValuePair<nameEntity, IEntity> entity in _entities)
            {
                entity.Value.LoadContent(this.Content, this.GraphicsDevice);
            }
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (KeyValuePair<nameEntity, IEntity> entity in _entities)
            {
                entity.Value.UnloadContent();
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

            foreach (KeyValuePair<nameEntity, IEntity> entity in _entities)
            {
                entity.Value.Update(gameTime, Keyboard.GetState(), _camera);
            }

            if (_entitiesModified) onInstanceChange();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);
            
            foreach (KeyValuePair<nameEntity, IEntity> entity in _entities)
            {
                entity.Value.Draw(_spriteBatch, _viewportAdapter.GetScaleMatrix(), _camera.GetViewMatrix());
            }

            base.Draw(gameTime);
        }
    }
}
