using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using OrthoCite.Helpers;
using System.Collections.Generic;
using System.Text;

namespace OrthoCite.Helpers
{
    public class AnswerBox : IEntity
    {
        const int MARGIN_LEFT_BOX = 50;
        const int MARGIN_TOP_BOX = 200;
        const int MARGIN_LEFT_TEXT = 30;
        const int MARGIN_TOP_TEXT = 10;
        const int INTERLINES = 30;

        int height;
        int width;

        public Dictionary<string, bool> _Answer { get; set; }
        Dictionary<string, Vector2> _PositionAnswer;
        public string _ask { get; set; }

        RuntimeData _runtimeData;
        Texture2D _text;
        SpriteFont _font;
        bool _isVisible = false;
        bool _firstUpdate = true;


        const string CURSOR = ">";
        Vector2 _cursorPosition;
        string _cursorSelect;

        public delegate void eventAnswerBox(RuntimeData runtimeData);
        public event eventAnswerBox heAnswerGood;
        public event eventAnswerBox heAnswerFalse;
        

        public AnswerBox(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _runtimeData.AnswerBox = this;
            _Answer = new Dictionary<string, bool>();
            _PositionAnswer = new Dictionary<string, Vector2>();
        }

        public AnswerBox Run()
        {
            if(_ask != "" && _ask != null && _Answer.Count != 0)_isVisible = true;
            return this;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _text= new Texture2D(graphicsDevice, 1, 1);
            _text.SetData(new Color[] { Color.Black });

            _font = content.Load<SpriteFont>("dialogbox/font");

            heAnswerFalse += HideAndClear;
            heAnswerGood += HideAndClear;
        }

        public bool isVisible
        {
            get { return _isVisible; }
        }

        private void HideAndClear(RuntimeData runtimeData)
        {

            _runtimeData.DialogBox.Hide();
            _isVisible = false;
            _ask = null;
            _PositionAnswer.Clear();
            _Answer.Clear();
            _firstUpdate = true;

        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
           
            if (!_isVisible) return;
            _runtimeData.DialogBox.SetText(_ask).Show();

            if(_firstUpdate)
            {
                int countAnwerNow = 0;
                string countMaxString = "";
                foreach (KeyValuePair<string, bool> i in _Answer)
                {
                    
                    _PositionAnswer.Add(i.Key, new Vector2(MARGIN_LEFT_BOX + MARGIN_LEFT_TEXT, MARGIN_TOP_BOX + MARGIN_TOP_TEXT + INTERLINES * countAnwerNow++));
                    if (countAnwerNow - 1 == 1)
                    {
                        _cursorSelect = i.Key;
                        _cursorPosition = new Vector2(_PositionAnswer[_cursorSelect].X - 10, _PositionAnswer[_cursorSelect].Y);
                    }
                    if (i.Key.Length > countMaxString.Length) countMaxString = i.Key; 
                }
                
                width = (int)_font.MeasureString(countMaxString).X + MARGIN_LEFT_TEXT * 2;
                height = MARGIN_TOP_TEXT * 2 + INTERLINES * countAnwerNow ;
                _firstUpdate = false;
            }

            if (keyboardState.IsKeyDown(Keys.Down)) tryDownCursor();
            else if (keyboardState.IsKeyDown(Keys.Up)) tryUpCursor();
            else if (keyboardState.IsKeyDown(Keys.Enter)) lookAnswer();
        }

        private void lookAnswer()
        {
            if (_Answer[_cursorSelect]) heAnswerGood(_runtimeData);
            else heAnswerFalse(_runtimeData);
        }

        private void tryDownCursor()
        {
            bool _next = false;

            foreach (KeyValuePair<string, bool> i in _Answer)
            {
                if(_next)
                {
                    _cursorSelect = i.Key;
                    _cursorPosition = new Vector2(_PositionAnswer[i.Key].X - 8, _PositionAnswer[i.Key].Y);
                }

                if (i.Key == _cursorSelect) _next = true;
            }

        }

        private void tryUpCursor()
        {
            bool _next = false;
            List<string> tmpArrayAnswer = new List<string>();
            foreach (KeyValuePair<string, bool> i in _Answer)
            {
                tmpArrayAnswer.Add(i.Key);
            }
            tmpArrayAnswer.Reverse();

            foreach (string i in tmpArrayAnswer)
            {
                if (_next)
                {
                    _cursorSelect = i;
                    _cursorPosition = new Vector2(_PositionAnswer[i].X - 8, _PositionAnswer[i].Y);
                }

                if (i == _cursorSelect) _next = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            if (!_isVisible) return;

            spriteBatch.Begin(transformMatrix: frozenMatrix);
            spriteBatch.Draw(_text, new Rectangle(MARGIN_LEFT_BOX, MARGIN_TOP_BOX, width, height), Color.White * 0.5f);

            foreach (KeyValuePair<string, bool> i in _Answer)
            {
                spriteBatch.DrawString(_font, i.Key, _PositionAnswer[i.Key], Color.White);
            }
            spriteBatch.DrawString(_font, CURSOR, _cursorPosition, Color.White);

            spriteBatch.End();
        }

        public void Execute(params string[] param)
        {

        }

        /// <summary>
        /// Counts the time text. Return int in seconds
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static int CountTimeText(string text)
        {
            int baseTime = 30;
            char[] arrayCount = text.ToCharArray();
            for(int i = 1; i < arrayCount.Length; i++)
            {
                if (baseTime * i >= arrayCount.Length) return i;
            }
            return 1;
        }
    }
}
