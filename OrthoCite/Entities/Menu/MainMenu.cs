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
using System.Xml;
using System.IO;
using System.Reflection;

namespace OrthoCite.Entities
{
    class Mainmenu : IEntity
    {

        enum GameState { mainMenu, enterName, inGame, options };

        GameState gameState;

        List<element> main = new List<element>();
        List<element> enterName = new List<element>();
        List<element> options = new List<element>();

        string whereToWrite;

        private Keys[] lastPressedKeys = new Keys[6];

        private string myName = string.Empty;
        private string myBindUp = "Up";
        private string myBindLeft = "Left";
        private string myBindRight = "Right";
        private string myBindDown = "Down";


        RuntimeData _runtimeData;


        private SpriteFont sf;
        public Mainmenu(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            main.Add(new element("fond"));
            main.Add(new element("play"));
            main.Add(new element("name"));
            main.Add(new element("franklogo"));
            main.Add(new element("joelogo"));
            main.Add(new element("optiong"));


            enterName.Add(new element("putname"));
            enterName.Add(new element("done"));
            enterName.Add(new element("joename"));
            enterName.Add(new element("namepdg"));

            options.Add(new element("fond"));
            options.Add(new element("bindUp"));
            options.Add(new element("bindDown"));
            options.Add(new element("bindLeft"));
            options.Add(new element("bindRight"));
            options.Add(new element("donx"));

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
            main.Find(x => x.AssetName == "franklogo").MoveElement(50, -180);
            main.Find(x => x.AssetName == "play").MoveElement(50, -75);
            //main.Find(x => x.AssetName == "charge").MoveElement(0, 75);
            //main.Find(x => x.AssetName == "sauvegarde").MoveElement(0, 150);
            main.Find(x => x.AssetName == "joelogo").MoveElement(-200, -30);
            main.Find(x => x.AssetName == "optiong").MoveElement(50, 75);
            main.Find(x => x.AssetName == "name").MoveElement(50, 0);



            foreach (element element in enterName)
            {
                element.LoadContent(content);
                element.CenterElement(550, 800);
                element.clickEvent += OnClick;
            }

            enterName.Find(x => x.AssetName == "putname").MoveElement(50, 0);
            enterName.Find(x => x.AssetName == "done").MoveElement(60, 100);
            enterName.Find(x => x.AssetName == "joename").MoveElement(-200, -30);
            enterName.Find(x => x.AssetName == "namepdg").MoveElement(50, -180);

            foreach (element element in options)
            {
                element.LoadContent(content);
                element.CenterElement(550, 800);
                element.clickEvent += OnClick;
            }

            options.Find(x => x.AssetName == "bindUp").MoveElement(50, -100);
            options.Find(x => x.AssetName == "bindDown").MoveElement(50, -50);
            options.Find(x => x.AssetName == "bindRight").MoveElement(50, 0);
            options.Find(x => x.AssetName == "bindLeft").MoveElement(50, 50);
            options.Find(x => x.AssetName == "donx").MoveElement(40, 130);

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
                    GetKeys();
                    break;
                case GameState.options:
                    foreach (element element in options)
                    {
                        element.Update();
                    }
                    GetKeys();
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
            XmlDocument document = new XmlDocument();
            document.Load(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\binds.xml");
            XmlNode root = document.DocumentElement;
            myBindUp = root.SelectSingleNode("bind[@key='up']").InnerText;
            myBindRight = root.SelectSingleNode("bind[@key='right']").InnerText;
            myBindLeft = root.SelectSingleNode("bind[@key='left']").InnerText;
            myBindDown = root.SelectSingleNode("bind[@key='down']").InnerText;

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
                    spriteBatch.DrawString(sf, myName, new Vector2(325, 230), Color.Black);
                    break;
                case GameState.options:
                    foreach (element element in options)
                    {
                        element.Draw(spriteBatch);
                    }
                    spriteBatch.DrawString(sf, myBindUp, new Vector2(500, 165), Color.Red);
                    spriteBatch.DrawString(sf, myBindDown, new Vector2(500, 215), Color.Red);
                    spriteBatch.DrawString(sf, myBindRight, new Vector2(500, 265), Color.Red);
                    spriteBatch.DrawString(sf, myBindLeft, new Vector2(500, 315), Color.Red);
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

            if (element == "donx")
            {
                gameState = GameState.mainMenu;
            }

            if (element == "optiong")
            {
                gameState = GameState.options;
            }

            if (element == "bindUp")
            {
                whereToWrite = "myBindUp";
            }
            if (element == "bindRight")
            {
                whereToWrite = "myBindRight";
            }
            if (element == "bindDown")
            {
                whereToWrite = "myBindDown";
            }
            if (element == "bindLeft")
            {
                whereToWrite = "myBindLeft";
            }

            if (element == "putname")
            {
                whereToWrite = "myName";
            }




        }

        private void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (whereToWrite == "myName")
            {

                if (e.Character == '\b')
                {
                    if (myName != "")
                    {
                        myName = myName.Remove(myName.Length - 1);
                    }
                }
                else
                {

                    myName += e.Character;
                }
            }

            /*if (whereToWrite == "myBindUp")
            {

                if (e.Character == '\b')
                {
                    if (myBindUp != "")
                    {
                        myBindUp = myBindUp.Remove(myBindUp.Length - 1);
                    }
                }
                else
                {
                    myBindUp = null;
                    myBindUp += e.Character;
                }
            }

            if (whereToWrite == "myBindRight")
            {

                if (e.Character == '\b')
                {
                    if (myBindRight != "")
                    {
                        myBindRight = myBindRight.Remove(myBindRight.Length - 1);
                    }
                }
                else
                {
                    myBindRight = null;
                    myBindRight += e.Character;
                }
            }



            if (whereToWrite == "myBindDown")
            {

                if (e.Character == '\b')
                {
                    if (myBindDown != "")
                    {
                        myBindDown = myBindDown.Remove(myBindDown.Length - 1);
                    }
                }
                else
                {
                    myBindDown = null;
                    myBindDown += e.Character;
                }
            }
            if (whereToWrite == "myBindLeft")
            {

                if (e.Character == '\b')
                {
                    if (myBindLeft != "")
                    {
                        myBindLeft = myBindLeft.Remove(myBindLeft.Length - 1);
                    }
                }
                else
                {
                    myBindLeft = null;
                    myBindLeft += e.Character;
                }
            }*/


        }




        public void GetKeys()
        {
            KeyboardState kbState = Keyboard.GetState();

            Keys[] pressedKeys = kbState.GetPressedKeys();

            foreach (Keys key in lastPressedKeys)
            {
                if (!pressedKeys.Contains(key))
                {
                    OnKeyUp(key);
                }
            }

            foreach (Keys key in pressedKeys)
            {
                if (!lastPressedKeys.Contains(key))
                {
                    OnKeyDown(key, whereToWrite);
                }
            }

            lastPressedKeys = pressedKeys;
        }

        public void OnKeyUp(Keys key)
        {

        }

        public void OnKeyDown(Keys key, string whereToWrite)
        {
            if (whereToWrite == "myBindUp")
            {
                if (key.ToString() != myBindRight && key.ToString() != myBindLeft && key.ToString() != myBindDown)
                {
                    myBindUp = key.ToString();
                }
            }
            else if (whereToWrite == "myBindRight")
            {
                if (key.ToString() != myBindUp && key.ToString() != myBindLeft && key.ToString() != myBindDown)
                {
                    myBindRight = key.ToString();
                }
            }
            else if (whereToWrite == "myBindDown")
            {
                if (key.ToString() != myBindUp && key.ToString() != myBindLeft && key.ToString() != myBindRight)
                {
                    myBindDown = key.ToString();
                }
            }
            else if (whereToWrite == "myBindLeft")
            {
                if (key.ToString() != myBindUp && key.ToString() != myBindRight && key.ToString() != myBindDown)
                {
                    myBindLeft = key.ToString();
                }
            }

            XmlDocument document = new XmlDocument();
            document.Load(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\binds.xml");
            XmlNode root = document.DocumentElement;
            root.SelectSingleNode("bind[@key='up']").InnerText = myBindUp;
            root.SelectSingleNode("bind[@key='right']").InnerText = myBindRight;
            root.SelectSingleNode("bind[@key='left']").InnerText = myBindLeft;
            root.SelectSingleNode("bind[@key='down']").InnerText = myBindDown;
            document.Save(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\binds.xml");
        }


    }
}
