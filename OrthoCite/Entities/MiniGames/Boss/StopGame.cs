using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using OrthoCite.Helpers;

namespace OrthoCite.Entities.MiniGames
{ 
    public class StopGame : MiniGame
    {

        RuntimeData _runtimeData;
        TiledMap _textureMap;
        Player _player;

        SpriteFont _font;
        SpriteFont _fontCompteur;


        SoundEffect _open;
        Song _music;

        const int GID_SPAWN = 16;
        const int ZOOM = 3;
        const int FAST_SPEED_PLAYER = 8;
        const int LOW_SPEED_PLAYER = 13;
        
        const int DISTRICT = 2;

        GameTime _saveGameTime;

        

        public StopGame(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            _runtimeData.StopGame = this;

            _player = new Player(TypePlayer.WithSpriteSheet, _runtimeData, "animations/Walking_V2");

            _runtimeData.Player = _player;

            if (_runtimeData.Lives == 0) _runtimeData.GainLive(3);
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {

            _textureMap = content.Load<TiledMap>("minigames/StopGame/stopGame");
            _font = content.Load<SpriteFont>("minigames/throwgame/font");
            _fontCompteur = content.Load<SpriteFont>("minigames/throwgame/font_compteur");
            _music = content.Load<Song>("minigames/DoorGame/music");
            _open = content.Load<SoundEffect>("minigames/DoorGame/open");

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
            _player.spriteFactory.Add(Helpers.Direction.DOWN, new SpriteSheetAnimationData(new[] { 27, 28, 29, 30 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.LEFT, new SpriteSheetAnimationData(new[] { 1, 2, 3, 0 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.RIGHT, new SpriteSheetAnimationData(new[] { 1, 2, 3, 0 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.UP, new SpriteSheetAnimationData(new[] { 13, 15, 17, 18 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.ATTACK_TOP, new SpriteSheetAnimationData(new[] { 19, 20, 13 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.ATTACK_DOWN, new SpriteSheetAnimationData(new[] { 32, 33, 26 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.ATTACK_LEFT, new SpriteSheetAnimationData(new[] { 5, 6, 0 }, isLooping: false));
            _player.spriteFactory.Add(Helpers.Direction.ATTACK_RIGHT, new SpriteSheetAnimationData(new[] { 5, 6, 0 }, isLooping: false));



            _player.separeFrame = 0;
            _player.lowFrame = LOW_SPEED_PLAYER;
            _player.fastFrame = FAST_SPEED_PLAYER;
            _player.typeDeplacement = TypeDeplacement.WithKey;

            _player.LoadContent(content);
            

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
            
            
            

            _player.checkMove(keyboardState);
            _player.heroAnimations.Update(deltaSeconds);
            _player.heroSprite.Position = new Vector2(_player.position.X + _textureMap.TileWidth / 2, _player.position.Y + _textureMap.TileHeight / 2);

            checkCamera(camera);
            if (keyboardState.IsKeyDown(Keys.F9)) _player.collisionLayer.IsVisible = !_player.collisionLayer.IsVisible;
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            _textureMap.Draw(spriteBatch);

            _player.Draw(spriteBatch);

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
                default:
                    Console.WriteLine("Can't find method to invoke in Map Class");
                    break;
            }
        }

        internal override void Start()
        {

        }

        private void checkCamera(Camera2D camera)
        {
            camera.LookAt(new Vector2(_player.position.X, _player.position.Y));
            if (OutOfScreenTop(camera)) camera.LookAt(new Vector2(_player.position.X, -_runtimeData.Scene.Height / ZOOM + _runtimeData.Scene.Height / 2));
            if (OutOfScreenLeft(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / ZOOM + _runtimeData.Scene.Width / 2, _player.position.Y));
            if (OutOfScreenRight(camera)) camera.LookAt(new Vector2(_textureMap.WidthInPixels - (_runtimeData.Scene.Width / ZOOM) * 2 + _runtimeData.Scene.Width / 2, _player.position.Y));
            if (OutOfScreenBottom(camera)) camera.LookAt(new Vector2(_player.position.X, _textureMap.HeightInPixels - (_runtimeData.Scene.Height / ZOOM) * 2 + _runtimeData.Scene.Height / 2));

            if (OutOfScreenLeft(camera) && OutOfScreenBottom(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / ZOOM + _runtimeData.Scene.Width / 2, _textureMap.HeightInPixels - (_runtimeData.Scene.Height / ZOOM) * 2 + _runtimeData.Scene.Height / 2));
            if (OutOfScreenLeft(camera) && OutOfScreenTop(camera)) camera.LookAt(new Vector2(-_runtimeData.Scene.Width / ZOOM + _runtimeData.Scene.Width / 2, -_runtimeData.Scene.Height / ZOOM + _runtimeData.Scene.Height / 2));

            if (OutOfScreenRight(camera) && OutOfScreenTop(camera)) camera.LookAt(new Vector2(_textureMap.WidthInPixels - (_runtimeData.Scene.Width / ZOOM) * 2 + _runtimeData.Scene.Width / 2, -_runtimeData.Scene.Height / ZOOM + _runtimeData.Scene.Height / 2));
            if (OutOfScreenRight(camera) && OutOfScreenBottom(camera)) camera.LookAt(new Vector2(_textureMap.WidthInPixels - (_runtimeData.Scene.Width / ZOOM) * 2 + _runtimeData.Scene.Width / 2, _textureMap.HeightInPixels - (_runtimeData.Scene.Height / ZOOM) * 2 + _runtimeData.Scene.Height / 2));

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
        }


    }
}




