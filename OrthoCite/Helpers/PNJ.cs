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

        Player _pnj;
        Vector2 _positionSpawn;

        RuntimeData _runtimeData;
        string _texture;

        
        //TALKABLE, TEXT, NEDD LESS PARAMS CONTRUCTOR
        
        public PNJ(TypePNJ type, Vector2 positionSpawn, List<ItemList> item, RuntimeData runtimeData, string texture)
        {
            _type = type;
            _positionSpawn = positionSpawn;
            _item = item;
            _runtimeData = runtimeData;
            _texture = texture;
        }
        
        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _pnj.heroTexture = content.Load<Texture2D>(_texture);
            var HeroAtlas = TextureAtlas.Create(_pnj.heroTexture, 32, 32);
            var HeroWalkingFactory = new SpriteSheetAnimationFactory(HeroAtlas);

            HeroWalkingFactory.Add(Direction.NONE.ToString(), new SpriteSheetAnimationData(new[] { 0 }));
            HeroWalkingFactory.Add(Direction.DOWN.ToString(), new SpriteSheetAnimationData(new[] { 5, 0, 10, 0 }, isLooping: false));
            HeroWalkingFactory.Add(Direction.LEFT.ToString(), new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            HeroWalkingFactory.Add(Direction.RIGHT.ToString(), new SpriteSheetAnimationData(new[] { 32, 26, 37, 26 }, isLooping: false));
            HeroWalkingFactory.Add(Direction.UP.ToString(), new SpriteSheetAnimationData(new[] { 19, 13, 24, 13 }, isLooping: false));

            _pnj.heroAnimations = new SpriteSheetAnimator(HeroWalkingFactory);
            _pnj.heroSprite = _pnj.heroAnimations.CreateSprite(_pnj.position);
        }   
        
        public void UnloadContent()
        {

        }
        
        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if(_type == TypePNJ.Static) { _pnj.position = _positionSpawn; }
            else if (_type == TypePNJ.Dynamique)
            {

            }
        }   
        
        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: cameraMatrix);
            

            spriteBatch.End();
        }  

        public void Execute(params string[] param)
        {

        }
    }
}
