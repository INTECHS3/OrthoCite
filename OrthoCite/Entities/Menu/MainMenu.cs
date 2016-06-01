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
            if (File.Exists("test.xml"))
            {

                using (XmlReader reader = XmlReader.Create("test.xml"))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                case "binds":

                                    break;
                                case "bindup":

                                    string attributeUp = reader["touche"];
                                    //tabXml[0] = attributeUp;
                                    if (attributeUp != null)
                                    {
                                        myBindUp = attributeUp;
                                    }

                                    break;
                                case "bindright":

                                    string attributeRight = reader["touche"];
                                    //tabXml[1] = attributeRight;
                                    if (attributeRight != null)
                                    {
                                        myBindRight = attributeRight;
                                    }

                                    break;
                                case "bindleft":

                                    string attributeLeft = reader["touche"];
                                    //tabXml[2] = attributeLeft;
                                    if (attributeLeft != null)
                                    {
                                        myBindLeft = attributeLeft;
                                    }
                                    break;

                                case "binddown":
                                    string attributeDown = reader["touche"];
                                    //tabXml[3] = attributeDown;
                                    if (attributeDown != null)
                                    {
                                        myBindDown = attributeDown;
                                    }

                                    break;
                            }
                        }
                    }
                }
            }

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
            if (File.Exists("test.xml"))
            {

                using (XmlReader reader = XmlReader.Create("test.xml"))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                case "binds":

                                    break;
                                case "bindup":

                                    string attributeUp = reader["touche"];
                                    break;
                                case "bindright":

                                    string attributeRight = reader["touche"];
                                    break;
                                case "bindleft":

                                    string attributeLeft = reader["touche"];
                                    break;

                                case "binddown":
                                    string attributeDown = reader["touche"];
                                    break;
                            }
                        }
                    }
                }
            }
            if (whereToWrite == "myBindUp")
            {
                if (key == Keys.Back && myBindUp.Length > 0)
                {
                    myBindUp = myBindUp.Remove(myBindUp.Length - 1);
                }
                else if (key.ToString() != myBindRight && key.ToString() != myBindLeft && key.ToString() != myBindDown)
                {
                    myBindUp = key.ToString();
                }
            }
            else if (whereToWrite == "myBindRight")
            {
                if (key == Keys.Back && myBindRight.Length > 0)
                {
                    myBindRight = myBindRight.Remove(myBindRight.Length - 1);
                }
                else if (key.ToString() != myBindUp && key.ToString() != myBindLeft && key.ToString() != myBindDown)
                {
                    myBindRight = key.ToString();
                }
                else
                {

                }
            }
            else if (whereToWrite == "myBindDown")
            {
                if (key == Keys.Back && myBindDown.Length > 0)
                {
                    myBindDown = myName.Remove(myBindDown.Length - 1);
                }
                else if (key.ToString() != myBindUp && key.ToString() != myBindLeft && key.ToString() != myBindRight)
                {
                    myBindDown = key.ToString();
                }
                else
                {

                }
            }
            else if (whereToWrite == "myBindLeft")
                if (key == Keys.Back && myBindLeft.Length > 0)
                {
                    myBindLeft = myName.Remove(myBindLeft.Length - 1);
                }
                else if (key.ToString() != myBindUp && key.ToString() != myBindRight && key.ToString() != myBindDown)
                {
                    myBindLeft = key.ToString();
                }
                else
                {

                }



            XmlWriter xmlWriter = XmlWriter.Create("test.xml");

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("binds");

            xmlWriter.WriteStartElement("bindup");
            xmlWriter.WriteAttributeString("touche", myBindUp);
            xmlWriter.WriteString("Bind Up");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("bindright");
            xmlWriter.WriteAttributeString("touche", myBindRight);
            xmlWriter.WriteString("Bind Right");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("bindleft");
            xmlWriter.WriteAttributeString("touche", myBindLeft);
            xmlWriter.WriteString("Bind Left");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("binddown");
            xmlWriter.WriteAttributeString("touche", myBindDown);
            xmlWriter.WriteString("Bind Down");


            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }


    }
}
