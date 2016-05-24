using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace OrthoCite.Entities
{
    public class DialogBox : IEntity
    {
        struct Dialog
        {
            public WrappedText Text;
            public int Delay;
        }

        const int PADDING = 20;
        const int MARGIN_TOP = 20;
        const int MAX_MARGIN_BORDERS = 20;
        const int INTERLINE = 2;

        RuntimeData _runtimeData;
        Texture2D _blackTexture;
        SpriteFont _font;
        bool _visible = false;
        int _currentDelay;
        WrappedText _currentWrappedText;
        DateTime _timeShown;

        List<Dialog> _dialogs;

        public DialogBox(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;

            _dialogs = new List<Dialog>();
        }

        /// <summary>
        /// Adds a dialog to the dialog box.
        /// </summary>
        /// <param name="text">The text to show.</param>
        /// <param name="delay">For how long to show it.</param>
        /// <returns>The same instance of DialogBox, for chaining.</returns>
        public DialogBox AddDialog(string text, int delay)
        {
            if (delay == 0) throw new ArgumentException("The delay cannot be 0.", nameof(delay));

            Dialog dialog = new Dialog { Text = TextHelper.WrapString(_font, text, _runtimeData.Scene.Width - (MAX_MARGIN_BORDERS * 2), INTERLINE), Delay = delay };

            if (_currentWrappedText == null)
            {
                _currentWrappedText = dialog.Text;
                _currentDelay = dialog.Delay;
                _timeShown = DateTime.Now;
            }
            else
            {
                _dialogs.Add(dialog);
            }

            return this;
        }

        /// <summary>
        /// Clears the queue of dialogs, and set the text of the dialog box.
        /// </summary>
        /// <param name="text">The text to show.</param>
        /// <returns>The same instance of DialogBox, for chaining.</returns>
        public DialogBox SetText(string text)
        {
            _currentDelay = 0;
            _dialogs.Clear();
            _currentWrappedText = TextHelper.WrapString(_font, text, _runtimeData.Scene.Width - (MAX_MARGIN_BORDERS * 2), INTERLINE);
            return this;
        }

        /// <summary>
        /// Shows the dialog box.
        /// </summary>
        /// <returns>The same instance of DialogBox, for chaining.</returns>
        public DialogBox Show()
        {
            _visible = true;
            return this;
        }

        /// <summary>
        /// Hides the dialog box.
        /// </summary>
        /// <returns>The same instance of DialogBox, for chaining.</returns>
        public DialogBox Hide()
        { 
            _visible = false;
            return this;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _blackTexture = new Texture2D(graphicsDevice, 1, 1);
            _blackTexture.SetData(new Color[] { Color.Black });
            _font = content.Load<SpriteFont>("dialogbox/font");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (_currentDelay != 0 && DateTime.Now - _timeShown >= TimeSpan.FromSeconds(_currentDelay))
            {
                if (_dialogs.Count > 0)
                {
                    _currentWrappedText = _dialogs[0].Text;
                    _currentDelay = _dialogs[0].Delay;
                    _dialogs.RemoveAt(0);
                    _timeShown = DateTime.Now;
                }
                else
                {
                    _visible = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
           if (!_visible) return;

            spriteBatch.Begin(transformMatrix: frozenMatrix);
            int xOffset = (_runtimeData.Scene.Width - (int)_currentWrappedText.Bounds.X - (PADDING * 2)) / 2;
            int yOffset = MARGIN_TOP;
            spriteBatch.Draw(_blackTexture, new Rectangle(xOffset, yOffset, (int)_currentWrappedText.Bounds.X + (PADDING * 2), (int)_currentWrappedText.Bounds.Y + (PADDING * 2)), Color.White * 0.7f);

            foreach (WrappedTextLine line in _currentWrappedText.Lines)
            {
                spriteBatch.DrawString(_font, line.Text, new Vector2(xOffset + line.Position.X + PADDING, yOffset + line.Position.Y + PADDING), Color.White);
            }
            spriteBatch.End();
        }

        public void Dispose()
        {
        }

        public void Execute(params string[] param)
        {

        }
    }
}
