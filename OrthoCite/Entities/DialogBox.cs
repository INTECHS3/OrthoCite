using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
namespace OrthoCite.Entities
{
    class DialogBox : IEntity
    {
        const int PADDING = 20;
        const int MARGIN_BOTTOM = 20;
        const int MAX_BOX_WIDTH = 1000;
        const int INTERLINE = 2;

        RuntimeData _runtimeData;
        Texture2D _blackTexture;
        SpriteFont _font;
        Point _mousePosition;
        WrappedText _wrappedText;

        public DialogBox(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _blackTexture = new Texture2D(graphicsDevice, 1, 1);
            _blackTexture.SetData(new Color[] { Color.Black });
            _font = content.Load<SpriteFont>("dialogbox/font");
            _wrappedText = TextHelper.WrapString(_font, "Salut ! Comment vas-tu ? Moi ça va. Cette boîte de dialogue se redimensionne en fonction du texte qu'on lui donne. Je sais, c'est stock. Mais Marvin est stock de toute façon.", MAX_BOX_WIDTH, INTERLINE);
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);
            int xOffset = (_runtimeData.Scene.Width - (int)_wrappedText.Bounds.X - (PADDING * 2)) / 2;
            int yOffset = (_runtimeData.Scene.Height - (int)_wrappedText.Bounds.Y - (PADDING * 2)) - MARGIN_BOTTOM;
            spriteBatch.Draw(_blackTexture, new Rectangle(xOffset, yOffset, (int)_wrappedText.Bounds.X + (PADDING * 2), (int)_wrappedText.Bounds.Y + (PADDING * 2)), Color.White);

            foreach (WrappedTextLine line in _wrappedText.Lines)
            {
                spriteBatch.DrawString(_font, line.Text, new Vector2(xOffset + line.Position.X + PADDING, yOffset + line.Position.Y + PADDING), Color.White);
            }
            spriteBatch.End();
        }

        public void Dispose()
        {
            System.Console.WriteLine($"Disose class : {this.GetType().Name}");
        }

        public void Execute(params string[] param)
        {

        }
    }
}
