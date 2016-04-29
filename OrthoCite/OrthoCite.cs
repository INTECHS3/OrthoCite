using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OrthoCite.Entities;
using MonoGameConsole;
using IndependentResolutionRendering;

namespace OrthoCite
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OrthoCite : Game
    {
        RuntimeData _runtimeData;
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        readonly ArrayList _entities;

        public const int SCENE_WIDTH = 1366;
        public const int SCENE_HEIGHT = 768;

        public OrthoCite()
        {
            _entities = new ArrayList();
            _graphics = new GraphicsDeviceManager(this);

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
            Resolution.Init(ref _graphics);
            Resolution.SetVirtualResolution(SCENE_WIDTH, SCENE_HEIGHT);

#if DEBUG
            Resolution.SetResolution(911, 512, false);

            _entities.Add(new DebugLayer(_runtimeData));
#else
            Resolution.SetResolution(_graphics.GraphicsDevice.DisplayMode.Width, _graphics.GraphicsDevice.DisplayMode.Height, true);
#endif
            _entities.Add(new DebugLayer(_runtimeData));

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

            Services.AddService(typeof(SpriteBatch), _spriteBatch);

            GameConsole console = new GameConsole(this, _spriteBatch, new GameConsoleOptions
            {
                ToggleKey = (int)Keys.F10,
                Font = Content.Load<SpriteFont>("console"),
                FontColor = Color.White,
                Prompt = "console $",
                PromptColor = Color.LightGreen,
                CursorColor = Color.Green,
                BackgroundColor = new Color(Color.Black, 150),
                PastCommandOutputColor = Color.Aqua,
                BufferColor = Color.Orange,
                Margin = 100
            });

            _runtimeData = new RuntimeData(console);

            /// Ex to add command : console.AddCommand("positionWorld", a => { var X = int.Parse(a[0]); var Y = int.Parse(a[1]); world.Position.X = X; world.Position.Y = Y; return String.Format("Teleporte the player to X :  {0} - Y : {1}", X, Y); }, "Change X et Yposition");
            /// positionWorld = name of command
            /// a = array of argument command
            /// return string  = Text return when command was execute

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
            Resolution.BeginDraw();
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());
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
