using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OrthoCite.Entities;
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
        Camera2D _camera;
        RuntimeData _runtimeData;
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        readonly ArrayList _entities;

        public const int SCENE_WIDTH = 1366;
        public const int SCENE_HEIGHT = 768;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public OrthoCite()
        {
            _entities = new ArrayList();
            _graphics = new GraphicsDeviceManager(this);

#if DEBUG
            _graphics.PreferredBackBufferWidth = SCENE_WIDTH;
            _graphics.PreferredBackBufferHeight = SCENE_HEIGHT;
#else
         _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
         _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
          _graphics.IsFullScreen = true;
#endif

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

#if DEBUG
            _entities.Add(new DebugLayer(_runtimeData));
            AllocConsole();
#else
         
#endif
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

            var viewportAdapter = new BoxingViewportAdapter(Window ,_graphics.GraphicsDevice, SCENE_WIDTH, SCENE_HEIGHT);
            _camera = new Camera2D(viewportAdapter);

            _runtimeData = new RuntimeData(_camera);

            _entities.Add(new Map(_runtimeData));

            foreach (IEntity entity in _entities)
            {
                entity.LoadContent(this.Content);
            }

        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (IEntity entity in _entities)
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
            
            foreach (IEntity entity in _entities)
            {
                entity.Update(gameTime, Keyboard.GetState());
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            //Draw your stuff

            _graphics.GraphicsDevice.Clear(Color.Black);

            var transformMatrix = _camera.GetViewMatrix();
            _spriteBatch.Begin(transformMatrix: transformMatrix);
           
            foreach (IEntity entity in _entities)
            {
                entity.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            // Draw render target

            base.Draw(gameTime);
        }
    }
}
