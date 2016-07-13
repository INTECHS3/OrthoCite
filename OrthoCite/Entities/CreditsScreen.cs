using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Animations.Tweens;

namespace OrthoCite.Entities
{
    class CreditsScreen : IEntity
    {
        const float SCROLL_TIME = 45.0f;

        RuntimeData _runtimeData;

        Texture2D _pixelTexture;
        Texture2D _logoTexture;
        Texture2D _intechTexture;
        Texture2D _directxTexture;
        Texture2D _dotnetTexture;
        Texture2D _monogameTexture;
        Texture2D _extendedTexture;
        Texture2D _gplTexture;

        SpriteFont _fontBig;
        SpriteFont _fontLittle;

        Song _music;

        uint _currentY = 0;
        uint _moveCameraTo = 0;
        bool _cameraMoved = false;

        public CreditsScreen(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _runtimeData.DialogBox.Hide();

            MediaPlayer.Stop();
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.Black });
            _logoTexture = content.Load<Texture2D>("intro/logo");
            _intechTexture = content.Load<Texture2D>("credits/intech");
            _directxTexture = content.Load<Texture2D>("credits/directx");
            _dotnetTexture = content.Load<Texture2D>("credits/dotnet");
            _monogameTexture = content.Load<Texture2D>("credits/monogame");
            _extendedTexture = content.Load<Texture2D>("credits/extended");
            _gplTexture = content.Load<Texture2D>("credits/gnu");

            _fontBig = content.Load<SpriteFont>("lostscreen/font");
            _fontLittle = content.Load<SpriteFont>("credits/little_font");

            _music = content.Load<Song>("credits/music");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (_moveCameraTo != 0 && !_cameraMoved)
            {
                camera.Position = new Vector2(0, 0);
                camera.Zoom = 1f;
                camera.CreateTweenGroup(Done).MoveTo(new Vector2(0, _moveCameraTo), SCROLL_TIME, EasingFunctions.Linear);
                _cameraMoved = true;
                MediaPlayer.Play(_music);
            }
        }

        public void Done()
        {
            _runtimeData.DialogBox.AddDialog("Tu as terminé le jeu, bravo !", 5).AddDialog("Tu peux maintenant continuer à jouer aux mini-jeux.", 5).Show();
            _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);
            spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, _runtimeData.Scene.Width, _runtimeData.Scene.Height), Color.White);

            _currentY = 0;
            DrawSprite(spriteBatch, _logoTexture, (uint)_runtimeData.Scene.Height);

            DrawText(spriteBatch, _fontBig, "TU AS SAUVE LA VILLE", 50);
            DrawText(spriteBatch, _fontLittle, "Grace a toi, l'orthographe est conservee !", 10);
            DrawText(spriteBatch, _fontLittle, "Lyrik est maintenant enferme a l'autre bout de la planete.", 20);
            DrawText(spriteBatch, _fontLittle, "Il ne reviendra plus jamais !", 20);
            DrawText(spriteBatch, _fontLittle, "Merci. Merci pour tout...", 10);

            DrawText(spriteBatch, _fontBig, "CHEF DE PROJET", 50);
            DrawText(spriteBatch, _fontLittle, "Valentin Gauthey", 20);

            DrawText(spriteBatch, _fontBig, "DEVELOPPEURS", 50);
            DrawText(spriteBatch, _fontLittle, "Marvin Roger", 20);
            DrawText(spriteBatch, _fontLittle, "Mathieu Boissady", 10);
            DrawText(spriteBatch, _fontLittle, "Thibaut Miginiac", 10);
            DrawText(spriteBatch, _fontLittle, "Valentin Gauthey", 10);

            DrawText(spriteBatch, _fontBig, "DESIGN MAP", 50);
            DrawText(spriteBatch, _fontLittle, "Mathieu Boissady", 20);

            DrawText(spriteBatch, _fontBig, "UN PROJET", 50);
            DrawSprite(spriteBatch, _intechTexture, 20);
            DrawText(spriteBatch, _fontLittle, "de semestre 3 en ingenierie logicielle", 10);

            DrawText(spriteBatch, _fontBig, "REALISE AVEC", 50);
            DrawSprite(spriteBatch, _directxTexture, 20);
            DrawText(spriteBatch, _fontLittle, "DirectX", 10);
            DrawSprite(spriteBatch, _dotnetTexture, 10);
            DrawText(spriteBatch, _fontLittle, "Framework .NET", 10);
            DrawSprite(spriteBatch, _monogameTexture, 10);
            DrawText(spriteBatch, _fontLittle, "MonoGame", 10);
            DrawText(spriteBatch, _fontLittle, "sous licence Microsoft Public", 10);
            DrawSprite(spriteBatch, _extendedTexture, 10);
            DrawText(spriteBatch, _fontLittle, "MonoGame Extended", 10);
            DrawText(spriteBatch, _fontLittle, "sous licence MIT", 10);

            DrawText(spriteBatch, _fontBig, "RESSOURCES GRAPHIQUES", 50);
            DrawText(spriteBatch, _fontLittle, "Sprite Database", 20);
            DrawText(spriteBatch, _fontLittle, "The Legend of Zelda: A Link to the Past", 10);
            DrawText(spriteBatch, _fontLittle, "The Legend of Zelda: The Minish Cap", 10);

            DrawText(spriteBatch, _fontBig, "MUSIQUES", 50);
            DrawText(spriteBatch, _fontLittle, "DOFUS", 20);
            DrawText(spriteBatch, _fontLittle, "The Legend of Zelda: Twilight Princess", 10);
            DrawText(spriteBatch, _fontLittle, "World of Warcraft", 10);

            DrawText(spriteBatch, _fontBig, "BRUITAGES", 50);
            DrawText(spriteBatch, _fontLittle, "Universal Soundbank", 20);
            DrawText(spriteBatch, _fontLittle, "The Legend of Zelda: A Link to the Past", 10);
            DrawText(spriteBatch, _fontLittle, "The Legend of Zelda: Ocarina of Time", 10);
            DrawText(spriteBatch, _fontLittle, "The Legend of Zelda: The Minish Cap", 10);

            DrawText(spriteBatch, _fontBig, "JEU DISTRIBUE SOUS LICENCE", 50);
            DrawSprite(spriteBatch, _gplTexture, 20);
            DrawText(spriteBatch, _fontLittle, "GNU GPL", 10);

            spriteBatch.End();
            _moveCameraTo = _currentY;
        }

        public void DrawSprite(SpriteBatch spriteBatch, Texture2D texture, uint margin)
        {
            _currentY += margin;

            spriteBatch.Draw(texture, new Vector2((_runtimeData.Scene.Width / 2) - (texture.Width / 2), _currentY), Color.White);

            _currentY += (uint)texture.Height;
        }

        public void DrawText(SpriteBatch spriteBatch, SpriteFont font, string text, uint margin)
        {
            _currentY += margin;

            Vector2 textSize = font.MeasureString(text);

            spriteBatch.DrawString(font, text, new Vector2((_runtimeData.Scene.Width / 2) - (textSize.X / 2), _currentY), Color.White);

            _currentY += (uint)textSize.Y;
        }

        public void Execute(params string[] param)
        {

        }
    }
}
