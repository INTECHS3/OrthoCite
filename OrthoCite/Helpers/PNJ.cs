using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Collections;


namespace OrthoCite.Helpers
{
    public enum TypePNJ
    {
        Static,
        Dynamique
    }

    public enum ItemList
    {

    }
    
    public class PNJ
    {

        TypePNJ _type;
        List<ItemList> _item;

        public List<string> _talk { get; set; }
        int i;
        const int timeTalk = 50;
        int a;

        Player _pnj;


        RuntimeData _runtimeData;
        string _texture;

        public bool talkAndStop { get; set; }


        //TALKABLE, TEXT, NEDD LESS PARAMS CONTRUCTOR

        public PNJ(TypePNJ type, Vector2 positionSpawn, List<ItemList> item, RuntimeData runtimeData, string texture)
        {
            _type = type;
            _item = item;
            _runtimeData = runtimeData;
            _texture = texture;
            _talk = new List<string>();
            _pnj = new Player(TypePlayer.WithSpriteSheet, positionSpawn ,_runtimeData, texture);
        }
        
        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {

            _pnj.collisionLayer = _runtimeData.Player.collisionLayer;

            _pnj.separeFrame = _runtimeData.Player.separeFrame;
            _pnj.lowFrame = _runtimeData.Player.lowFrame;
            _pnj.fastFrame = _runtimeData.Player.fastFrame;

            _pnj.typeDeplacement = TypeDeplacement.WithDirection;

            

            _pnj.LoadContent(content);
        }

        public void UnloadContent()
        {

        }
        
        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera, float deltaSeconds)
        {
            if (talkAndStop) talk();
            else
            {
                if (_type != TypePNJ.Static)
                {

                    _pnj.checkMove(keyboardState, camera);
                }
            }
            
            _pnj.heroAnimations.Update(deltaSeconds);
            _pnj.heroSprite.Position = new Vector2(_pnj.position.X + _pnj.tileWidth / 2, _pnj.position.Y + _pnj.tileHeight / 2);
        }

        private void talk()
        {

            if (i == 0 && a < _talk.Count)
            {
                _runtimeData.DialogBox.SetText(_talk[a]);
                _runtimeData.DialogBox.Show();
                a++;
                i++;
            }
            else if (a >= _talk.Count) talkAndStop = false;
            else if (i != 0 && i < timeTalk) i++;
            else if (i == timeTalk) i = 0;
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _pnj.Draw(spriteBatch);
        }  

        public void Execute(params string[] param)
        {

        }

        public void spriteFactory(Direction dir, SpriteSheetAnimationData spriteData)
        {
            _pnj.spriteFactory.Add(dir, spriteData);
        }

        
    }
}
