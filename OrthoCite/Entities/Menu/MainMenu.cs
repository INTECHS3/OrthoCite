using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;

namespace OrthoCite.Entities
{
    class Mainmenu : IEntity
    {
        enum GameState { mainMenu, enterName, inGame };

        GameState gameState;

        List<element> main = new List<element>();
        List<element> enterName = new List<element>();

        private string myName = string.Empty;
        RuntimeData _runtimeData;


        private SpriteFont sf;
        public Mainmenu(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            main.Add(new element("fond"));
            main.Add(new element("play"));
            main.Add(new element("name"));
            main.Add(new element("charge"));
            main.Add(new element("sauvegarde"));
            main.Add(new element("franklogo"));
            main.Add(new element("joelogo"));


            //enterName.Add(new element("entrename"));
            enterName.Add(new element("putname"));
            enterName.Add(new element("done"));
            enterName.Add(new element("joename"));
            enterName.Add(new element("namepdg"));

            EventInput.CharEntered += EventInput_CharEntered;
        }

        void IEntity.LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            sf = content.Load<SpriteFont>("debug");
            foreach (element element in main)
            {
                element.LoadContent(content);
                element.CenterElement(550, 800);
                element.clickEvent += OnClick;
            }
            main.Find(x => x.AssetName == "franklogo").MoveElement(0, -180);
            main.Find(x => x.AssetName == "play").MoveElement(0, -75);
            main.Find(x => x.AssetName == "charge").MoveElement(0, 75);
            main.Find(x => x.AssetName == "sauvegarde").MoveElement(0, 150);
            main.Find(x => x.AssetName == "joelogo").MoveElement(-250, -30);


            foreach (element element in enterName)
            {
                element.LoadContent(content);
                element.CenterElement(550, 800);
                element.clickEvent += OnClick;
            }

            enterName.Find(x => x.AssetName == "putname").MoveElement(0, 0);
            enterName.Find(x => x.AssetName == "done").MoveElement(0, 100);
            enterName.Find(x => x.AssetName == "joename").MoveElement(-250, -30);
            enterName.Find(x => x.AssetName == "namepdg").MoveElement(0, -180);
        }

        void IEntity.UnloadContent()
        {

        }

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            switch (gameState)
            {
                case GameState.mainMenu:
                    foreach (element element in main)
                    {
                        element.Update();
                    }
                    break;
                case GameState.enterName:
                    foreach (element element in enterName)
                    {
                        element.Update();
                    }
                    break;
                case GameState.inGame:
                    _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
                    break;
                default:
                    break;
            }


        }

        void IEntity.Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.mainMenu:
                    foreach (element element in main)
                    {
                        element.Draw(spriteBatch);
                    }

                    break;
                case GameState.enterName:
                    foreach (element element in enterName)
                    {
                        element.Draw(spriteBatch);
                    }
                    spriteBatch.DrawString(sf, myName, new Vector2(275, 230), Color.Black);
                    break;
                case GameState.inGame:
                    break;
                default:
                    break;
            }
            spriteBatch.End();

        }

        void IEntity.Execute(params string[] param)
        {
        }

        public void OnClick(string element)
        {
            if (element == "play")
            {
                gameState = GameState.inGame;
            }

            if (element == "sauvegarde")
            {
                gameState = GameState.inGame;
            }

            if (element == "name")
            {
                gameState = GameState.enterName;
            }

            if (element == "done")
            {
                gameState = GameState.mainMenu;
            }
        }

        private void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (e.Character == '\b')
            {
                if (myName != "") myName = myName.Remove(myName.Length - 1);
            }
            else
            {
                myName += e.Character;
            }
        }

        public void OnKeyDown(Keys key)
        {
            if (key == Keys.Back && myName.Length > 0)
            {
                myName = myName.Remove(myName.Length - 1);
            }
            else
            {
                myName += key.ToString();
            }
        }

    }
}
