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

namespace OrthoCite.Helpers
{
    public class Button
    {
        Vector2 _position;
        Texture2D _texture;
        string _textString;
        Point MouseInput;
        MouseState MouseState;
        RuntimeData _runtimeData;

        int Opacity;
        const int OpacityGo = 0;
        const int OpacityBasic = 100;
        bool isClick;
        bool Inverse;

        public delegate void ClickButton(Button button);
        public event ClickButton onClick;
        private void buttonWasClick() { if (onClick != null) onClick(this); }
    

        public Button(Vector2 position, string texture, RuntimeData runtimedata)
        {
            _position = position;
            _textString = texture;
            _runtimeData = runtimedata;
            Opacity = 100;
        }


        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _texture = content.Load<Texture2D>(_textString);
           
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera, float deltaSeconds)
        {   
            
                 
            if(!isClick)
            {
                MouseState = Mouse.GetState();
                MouseInput = _runtimeData.ViewAdapter.PointToScreen(MouseState.X, MouseState.Y);

                if (MouseInput.X < _position.X + _texture.Width &&
                        MouseInput.X > _position.X &&
                        MouseInput.Y < _position.Y + _texture.Height &&
                        MouseInput.Y > _position.Y &&
                        MouseState.LeftButton == ButtonState.Pressed)
                    isClick = !isClick;
            }
            else
            {
                if (Opacity > OpacityGo && !Inverse) { Opacity = Opacity - 10; if (Opacity == OpacityGo) Inverse = !Inverse; }
                else if (Opacity < OpacityBasic && Inverse) { Opacity = Opacity + 10; if (Opacity == OpacityBasic) { Inverse = !Inverse; isClick = !isClick; buttonWasClick(); } }
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float tmpOpacity = (float)Opacity / 100f;
            spriteBatch.Draw(_texture, _position, Color.White * tmpOpacity);
        }

        
    }
}
