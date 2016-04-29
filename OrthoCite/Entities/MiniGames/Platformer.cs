using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OrthoCite.Entities.MiniGames
{
    class Platformer : MiniGame
    {
        Texture2D _background;
        Texture2D _platform;

        public Platformer()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            _background = content.Load<Texture2D>("minigames/platformer/background");
            _platform = content.Load<Texture2D>("minigames/platformer/platform");
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(_platform, new Vector2(20, 20), Color.White);
        }

        internal override void Start()
        {
            throw new NotImplementedException();
        }
    }
}
