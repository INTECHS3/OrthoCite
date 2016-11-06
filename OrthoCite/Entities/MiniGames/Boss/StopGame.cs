using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using OrthoCite.Helpers;

namespace OrthoCite.Entities.MiniGames
{
    struct saveWordTerminason
    {
        public string Word { get; private set; }
        public string Term { get; private set; }
        public bool TrueOrFalse { get; private set; }

        public saveWordTerminason(string TheWord, string TheTerm, bool ItsTrueOrFalse)
        {
            Word = TheWord;
            Term = TheTerm;
            TrueOrFalse = ItsTrueOrFalse;
        }
    }

    struct  positionOfSaveWordTerminason
    {
        public saveWordTerminason saveWordStruct { get; set; }
        public Vector2 positionOfWord { get; set; }

        public positionOfSaveWordTerminason(saveWordTerminason s, Vector2 pos)
        {
            saveWordStruct = s;
            positionOfWord = pos;
        }
    }

    public class StopGame : MiniGame
    {

        RuntimeData _runtimeData;
        TiledMap _textureMap;
        Player _player;
    
        SpriteFont _font;
        SpriteFont _fontCompteur;


        SoundEffect _open;
        SoundEffect _hurt;
        Song _music;

        const int GID_SPAWN = 16;
        const int ZOOM = 2;
        const int FAST_SPEED_PLAYER = 8;
        const int LOW_SPEED_PLAYER = 13;
        const int WIDTHHEIGHTTITLE = 32;
        const int LIMITTOPLAYER = 7;
        const int DISTRICT = 2;
        const int CAMERA_DECAL = 250;
        const int TIME_INTERVAL_PUSH_BUTTON = 500; //IN MILLISECONDS
        static readonly int[] BUTTONTILECOL = new int[4] { 153, 154, 155, 156 };
        

        GameTime _saveGameTime;
        Queue<List<saveWordTerminason>> wordsQueue;
        List<saveWordTerminason> actualWords;
        List<positionOfSaveWordTerminason> actualPositionWords;
        Vector2[] staticPositionSpawn = new Vector2[4] { new Vector2(22 * WIDTHHEIGHTTITLE,5 * WIDTHHEIGHTTITLE), new Vector2(22 * WIDTHHEIGHTTITLE ,7 * WIDTHHEIGHTTITLE), new Vector2(22 * WIDTHHEIGHTTITLE,9 * WIDTHHEIGHTTITLE), new Vector2(22 * WIDTHHEIGHTTITLE,11 * WIDTHHEIGHTTITLE)};
        Dictionary<int, Vector2> staticPositionButtonVirt = new Dictionary<int, Vector2>() { { 153, new Vector2(5,5)}, { 154, new Vector2(5,7)}, { 155, new Vector2(5,9)}, { 156, new Vector2(5,11)} };

        TimeSpan _saveTimePressButton;

        public StopGame(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _runtimeData.StopGame = this;

            _player = new Player(TypePlayer.WithSpriteSheet, _runtimeData, "animations/Walking");

            _runtimeData.Player = _player;

            wordsQueue = new Queue<List<saveWordTerminason>>();

            if (_runtimeData.Lives == 0) _runtimeData.GainLive(3);
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {

            _textureMap = content.Load<TiledMap>("minigames/StopGame/stopGame");
            _font = content.Load<SpriteFont>("minigames/throwgame/font");
            _fontCompteur = content.Load<SpriteFont>("minigames/throwgame/font_compteur");
            _music = content.Load<Song>("minigames/DoorGame/music");
            _open = content.Load<SoundEffect>("minigames/DoorGame/open");
            _hurt = content.Load<SoundEffect>("minigames/GuessGame/hurt");

            foreach (TiledTileLayer e in _textureMap.TileLayers)
            {
                if (e.Name == "collision") _player.collisionLayer = e;
            }
            _player.collisionLayer.IsVisible = false;


            if (_player.positionVirt.Length() == 0)
            {
                foreach (TiledTile i in _player.collisionLayer.Tiles)
                {
                    if (i.Id == GID_SPAWN) _player.positionVirt = new Vector2(i.X, i.Y);
                }
            }

            _player.gidCol = 633;
            _player.spriteFactory.Add(Helpers.Direction.NONE, new SpriteSheetAnimationData(new[] { 0 }));
            _player.spriteFactory.Add(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 5, 10 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 19, 13, 24, 13 }, isLooping: false));




            _player.separeFrame = 0;
            _player.lowFrame = LOW_SPEED_PLAYER;
            _player.fastFrame = FAST_SPEED_PLAYER;
            _player.typeDeplacement = TypeDeplacement.WithKey;

            _player.LoadContent(content);
            LoadAllWords();

            Start();


        }


        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {

           if (_runtimeData.Lives == 0) _runtimeData.OrthoCite.ChangeGameContext(GameContext.LOST_SCREEN);
           
            

            _saveGameTime = gameTime;
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            camera.Zoom = ZOOM;

            if(keyboardState.IsKeyDown(Keys.E)) checkButtonPlayer();
            checkWord();

            _player.checkMove(keyboardState);
            _player.heroAnimations.Update(deltaSeconds);
            _player.heroSprite.Position = new Vector2(_player.position.X + _textureMap.TileWidth / 2, _player.position.Y + _textureMap.TileHeight / 2);

            checkCamera(camera);
            if (keyboardState.IsKeyDown(Keys.F9)) _player.collisionLayer.IsVisible = !_player.collisionLayer.IsVisible;
        }

        private void checkButtonPlayer()
        {
            foreach(KeyValuePair<int, Vector2> i in staticPositionButtonVirt)
            {
                if ((i.Value.X == _player.positionVirt.X + 1 && i.Value.Y == _player.positionVirt.Y)) launchDestroyWord(i.Value.Y * WIDTHHEIGHTTITLE);
            }
        }

        private void launchDestroyWord(float y)
        {
            if (_saveTimePressButton.TotalMilliseconds == 0)
            {
                List<positionOfSaveWordTerminason> iTmp = new List<positionOfSaveWordTerminason>();

                _saveTimePressButton = _saveGameTime.TotalGameTime;
                if (actualPositionWords == null) return;
                foreach (positionOfSaveWordTerminason i in actualPositionWords)
                {
                    if (i.positionOfWord.Y == y) iTmp.Add(i);
                }
                foreach(positionOfSaveWordTerminason i in iTmp)
                {
                    if (i.saveWordStruct.TrueOrFalse) actionWhenTrueIsDelete();
                    actualWords.Remove(i.saveWordStruct);
                    actualPositionWords.Remove(i);
                }
            }
            else if (_saveTimePressButton.TotalMilliseconds < _saveGameTime.TotalGameTime.TotalMilliseconds - TIME_INTERVAL_PUSH_BUTTON) _saveTimePressButton = new TimeSpan(0);
            
        }

        

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            //_textureMap.Draw(spriteBatch);
            _textureMap.Draw(frozenMatrix);
            

            _player.Draw(spriteBatch);
            if(actualPositionWords != null)
            {
                foreach (positionOfSaveWordTerminason i in actualPositionWords)
                {
                    spriteBatch.DrawString(_font, i.saveWordStruct.Term, new Vector2(LIMITTOPLAYER * WIDTHHEIGHTTITLE, i.positionOfWord.Y), Color.White);
                    spriteBatch.DrawString(_font, i.saveWordStruct.Word, i.positionOfWord, Color.White);
                }
            }
            

            spriteBatch.End();

        }



        public override void Execute(params string[] param)
        {
            switch (param[0])
            {
                case "movePlayer":
                    try { MoveTo(new Vector2(Int32.Parse(param[1]), Int32.Parse(param[2]))); }
                    catch { Console.WriteLine("use : movePlayer {x} {y}"); }
                    break;
                case "exit":
                    try
                    {
                        _runtimeData.OrthoCite.Exit();
                    }
                    catch { Console.WriteLine("Can't Exit"); }
                    break;
                case "giveLife":
                    try { _runtimeData.GainLive(Int32.Parse(param[1])); Console.WriteLine($"You give {param[1]}"); }
                    catch { Console.WriteLine("use : giveLife {nbLife}"); }
                    break;
                case "clearText":
                    try { _runtimeData.DialogBox.AddDialog("SetEmpty", 2).Show(); }
                    catch { Console.WriteLine("use : error"); }
                    break;
                case "setLife":
                    try
                    {
                        if (Int32.Parse(param[1]) < _runtimeData.Lives)
                        {
                            int liveTmp = _runtimeData.Lives - Int32.Parse(param[1]);
                            _runtimeData.LooseLive(liveTmp);
                        }
                        else
                        {
                            int liveTmp = Int32.Parse(param[1]) - _runtimeData.Lives;
                            _runtimeData.GainLive(liveTmp);
                        }
                    }
                    catch { Console.WriteLine("use : error"); }
                    break;
                default:
                    Console.WriteLine($"Can't find method to invoke in {this.ToString()}");
                    break;
            }
        }

        internal override void Start()
        {

        }

        private void checkWord()
        {
            if(actualWords == null && wordsQueue.Count != 0)
            {
                
                actualWords = wordsQueue.Dequeue();
                actualPositionWords = new List<positionOfSaveWordTerminason>();

                int count = 0;
                new Random().Shuffle(staticPositionSpawn);
                foreach(saveWordTerminason wordStruct in actualWords)
                {
                    actualPositionWords.Add(new positionOfSaveWordTerminason(wordStruct, staticPositionSpawn[count++]));
                }
            }
            else if(actualWords != null)
            {
                List<positionOfSaveWordTerminason> structToDel = new List<positionOfSaveWordTerminason>();

                foreach(positionOfSaveWordTerminason pair in actualPositionWords)
                {
                    if (pair.positionOfWord.X == LIMITTOPLAYER * WIDTHHEIGHTTITLE)
                    {
                        structToDel.Add(pair);
                        if (!pair.saveWordStruct.TrueOrFalse) actionWhenFalsePass();
                    }
                }

                foreach(positionOfSaveWordTerminason s in structToDel)
                {
                    actualWords.Remove(s.saveWordStruct);
                    actualPositionWords.Remove(s);
                }

                if(actualWords.Count == 0)
                {
                    actualWords = null;
                    actualPositionWords = null;
                    return;
                }

                for(int i = 0; i < actualPositionWords.Count; i++)
                {
                    positionOfSaveWordTerminason tmpVar = actualPositionWords[i];
                    tmpVar.positionOfWord += new Vector2(-1, 0);
                    actualPositionWords[i] = tmpVar;
                }
                


            }
            else if(wordsQueue.Count == 0)
            {
                if (_runtimeData.DistrictActual == DISTRICT && _runtimeData.DataSave.District == DISTRICT)
                {
                    _runtimeData.DataSave.District = DISTRICT + 1;
                    _runtimeData.DataSave.ClearMiniGames();
                    _runtimeData.DataSave.Save();
                }
                _runtimeData.DialogBox.AddDialog("Arggggh, tu m'as battu.", 4).Show();
                _runtimeData.OrthoCite.ChangeGameContext(GameContext.MAP);
            }

        }

        private void actionWhenFalsePass()
        {
            _runtimeData.DialogBox.AddDialog("Une mauvaise terminaison est passé ahahah, Lyrick sera toujour maitre du monde", 3);
            _runtimeData.LooseLive();
            _hurt.Play();
        }

        private void actionWhenTrueIsDelete()
        {
            _runtimeData.DialogBox.AddDialog("Tu as detruit une bonne terminaison, l'orthographe n'existera jamais !", 3);
            _runtimeData.LooseLive();
            _hurt.Play();
        }

        private void checkCamera(Camera2D camera)
        {
            camera.LookAt(new Vector2(_player.position.X + CAMERA_DECAL, _player.position.Y));

        }
        private bool OutOfScreenTop(Camera2D camera)
        {
            if (camera.Position.Y < -_runtimeData.Scene.Height / ZOOM) return true;
            return false;
        }
        private bool OutOfScreenLeft(Camera2D camera)
        {
            if (camera.Position.X <= -_runtimeData.Scene.Width / ZOOM) return true;
            return false;
        }
        private bool OutOfScreenRight(Camera2D camera)
        {
            if (camera.Position.X >= _textureMap.WidthInPixels - (_runtimeData.Scene.Width / ZOOM) * 2) return true;
            return false;
        }
        private bool OutOfScreenBottom(Camera2D camera)
        {
            if (camera.Position.Y >= _textureMap.HeightInPixels - (_runtimeData.Scene.Height / ZOOM) * 2) return true;
            return false;
        }

        public void MoveTo(Vector2 vec)
        {
            _player.positionVirt = vec;
            _player.UpdateThePosition();
        }

        private void LoadAllWords()
        {
            XmlDocument _saveOfWordXml = new XmlDocument();

            _saveOfWordXml.Load(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\dictionaries\StopGame.xml");

            XmlNode elementWords = _saveOfWordXml.DocumentElement;
            XmlNode districtElement = elementWords.SelectSingleNode("district");

            foreach (XmlNode group in districtElement.SelectNodes("group"))
            {
                List<saveWordTerminason> tmpList = new List<saveWordTerminason>();

                foreach (XmlNode word in group.SelectNodes("word"))
                {
                    bool tmpBool = false;
                    if (word.Attributes["typeWorld"].Value == "true") tmpBool = true;

                    tmpList.Add(new saveWordTerminason(word.InnerText, word.Attributes["term"].Value, tmpBool));
                }

                wordsQueue.Enqueue(tmpList);
            }
        }

        internal bool CheckColDown(TiledTile i)
        {
            for(int e = 0; e < BUTTONTILECOL.Length; e++)
            {
                if (i.Id == BUTTONTILECOL[e] && i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y + 1) return true;
            }
            return false;
        }

        internal bool CheckColUp(TiledTile i)
        {
            for (int e = 0; e < BUTTONTILECOL.Length; e++)
            {
                if (i.Id == BUTTONTILECOL[e] && i.X == _player.positionVirt.X && i.Y == _player.positionVirt.Y - 1) return true;
            }
            return false;
        }

        internal bool CheckColRight(TiledTile i)
        {
            for (int e = 0; e < BUTTONTILECOL.Length; e++)
            {
                if (i.Id == BUTTONTILECOL[e] && i.X == _player.positionVirt.X + 1 && i.Y == _player.positionVirt.Y) return true;
            }
            return false;
        }
    }
    static class RandomExtensions
    {
        public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}




