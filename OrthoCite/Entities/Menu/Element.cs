using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoCite
{
    class element
    {
        private Texture2D Texture;

        private Rectangle Rect;

        private string assetName;

        public string AssetName
        {
            get
            {
                return assetName;
            }

            set
            {
                assetName = value;
            }
        }

        public delegate void ElementClicked(string element);

        public event ElementClicked clickEvent;

        public element(string assetName)
        {
            this.AssetName = assetName;
        }

        public void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("menu/"+AssetName);
            Rect = new Rectangle(0, 0, Texture.Width, Texture.Height);
        }

        public void Update()
        {
            if (Rect.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                clickEvent(AssetName);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect, Color.White);
        }

        public void CenterElement(int height, int width)
        {
            Rect = new Rectangle((width / 2) - (this.Texture.Width / 2), (height / 2) - (this.Texture.Height / 2), this.Texture.Width, this.Texture.Height);
        }

        public void MoveElement(int x, int y)
        {
            Rect = new Rectangle(Rect.X += x, Rect.Y += y, Rect.Width, Rect.Height);
        }
    }
}
