using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OrthoCite.Entities;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace OrthoCite
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OrthoCite : Game
    {
        RuntimeData _runtimeData;
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        readonly ArrayList _entities;
        Camera2D _camera;

        public const int SCENE_WIDTH = 1366;
        public const int SCENE_HEIGHT = 768;

        public OrthoCite()
        {
            _runtimeData = new RuntimeData();
            _graphics = new GraphicsDeviceManager(this);

            _entities = new ArrayList();

#if DEBUG
            _graphics.PreferredBackBufferWidth = 911;
            _graphics.PreferredBackBufferHeight = 512;
            _entities.Add(new DebugLayer(_runtimeData));
#else
            _graphics.PreferredBackBufferWidth = SCENE_WIDTH;
            _graphics.PreferredBackBufferHeight = SCENE_HEIGHT;
            _graphics.IsFullScreen = true;
#endif
            _entities.Add(new DebugLayer(_runtimeData));

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, SCENE_WIDTH, SCENE_HEIGHT);
            _camera = new Camera2D(viewportAdapter);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
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
