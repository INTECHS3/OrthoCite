using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System.Collections.Generic;

namespace OrthoCite.Helpers
{
    public enum Direction
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public enum TypePlayer
    {
        WithSpriteSheet,
        WithTexture2D
    }

    public class Player
    {
        RuntimeData _runtimeData;

        public SpriteSheetAnimator heroAnimations { set; get; }
        public Sprite heroSprite { set; get; }
        public Texture2D heroTexture { set; get; }

        public Vector2 positionVirt { set; get; }
        public Vector2 position { set; get; }

        public Direction actualDir { set; get; }
        public Direction lastDir { set; get; }
        public Dictionary<Direction, SpriteSheetAnimationData> spriteFactory { get; set; }
        public TypePlayer typePlayer { set; get; }

        string _texture;

        public Player(TypePlayer TypePlaYer, RuntimeData runtimeData, string texture)
             : this(TypePlaYer, new Vector2(0,0), runtimeData, texture){} 

        public Player(TypePlayer TypePlaYer, Vector2 PositionVirt, RuntimeData runtimeData, string texture)
        {
            positionVirt = PositionVirt;
            position = new Vector2(0,0);
            typePlayer = TypePlaYer;
            _runtimeData = runtimeData;
            _texture = texture;
            spriteFactory = new Dictionary<Direction, SpriteSheetAnimationData>();
        }


        public void LoadContent(ContentManager content)
        {
            if (typePlayer == TypePlayer.WithSpriteSheet)
            {
                var HeroWalking = content.Load<Texture2D>(_texture);
                var HeroAtlas = TextureAtlas.Create(HeroWalking, 32, 32);
                var HeroWalkingFactory = new SpriteSheetAnimationFactory(HeroAtlas);

                HeroWalkingFactory.Add(Helpers.Direction.NONE.ToString(), spriteFactory[Direction.NONE]);
                HeroWalkingFactory.Add(Helpers.Direction.DOWN.ToString(), spriteFactory[Direction.DOWN]);
                HeroWalkingFactory.Add(Helpers.Direction.LEFT.ToString(), spriteFactory[Direction.LEFT]);
                HeroWalkingFactory.Add(Helpers.Direction.RIGHT.ToString(), spriteFactory[Direction.RIGHT]);
                HeroWalkingFactory.Add(Helpers.Direction.UP.ToString(), spriteFactory[Direction.UP]);

                heroAnimations = new SpriteSheetAnimator(HeroWalkingFactory);
                heroSprite = heroAnimations.CreateSprite(positionVirt);

                actualDir = Direction.NONE;
                lastDir = actualDir;
            }
            else if (typePlayer == TypePlayer.WithTexture2D)
            {
                heroTexture = content.Load<Texture2D>(_texture);
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if(typePlayer == TypePlayer.WithSpriteSheet)
            {
                if (lastDir == Direction.LEFT) heroSprite.Effect = SpriteEffects.FlipHorizontally;
                else heroSprite.Effect = SpriteEffects.None;

                spriteBatch.Draw(heroSprite);
            }
            else if(typePlayer == TypePlayer.WithTexture2D)
            {
                spriteBatch.Draw(heroTexture, position, Color.White);
            }
        }

        public void MoveUpChamp()
        {

            positionVirt += new Vector2(0, -1);
        }

        public void MoveDownChamp()
        {
            positionVirt += new Vector2(0, +1);
        }

        public void MoveLeftChamp()
        {
            positionVirt += new Vector2(-1, 0);
        }

        public void MoveRightChamp()
        {
            positionVirt += new Vector2(+1, 0);
        }
    }
}
