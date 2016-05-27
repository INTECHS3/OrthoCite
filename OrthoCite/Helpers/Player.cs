using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System.Collections.Generic;
using MonoGame.Extended;

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

    public enum TypeDeplacement
    {
        WithKey,
        WithDirection
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
        public TypeDeplacement typeDeplacement { get; set; }
        public TiledTileLayer collisionLayer { set; get; }
        

        public int separeFrame { set; get; }
        public int actualFrame { set; get; }
        public int fastFrame { set; get; }
        public int lowFrame { set; get; }
        
        public int tileWidth;
        public int tileHeight;
        public int mapHeight;
        public int mapWidth; 

        public int gidCol { get; set; }

        string _texture;

        public Player(TypePlayer TypePlaYer, RuntimeData runtimeData, string texture)
             : this(TypePlaYer, new Vector2(0,0), runtimeData, texture){} 

        public Player(TypePlayer TypePlaYer, Vector2 PositionVirt, RuntimeData runtimeData, string texture)
        {
            
            positionVirt = PositionVirt;
            typePlayer = TypePlaYer;
            _runtimeData = runtimeData;
            _texture = texture;
            spriteFactory = new Dictionary<Direction, SpriteSheetAnimationData>();
        }


        public void LoadContent(ContentManager content)
        {
            tileHeight = collisionLayer.TileHeight;
            tileWidth = collisionLayer.TileWidth;
            mapHeight = collisionLayer.Height;
            mapWidth = collisionLayer.Width;

            if (typePlayer == TypePlayer.WithSpriteSheet)
            {
                heroTexture = content.Load<Texture2D>(_texture);
                var HeroAtlas = TextureAtlas.Create(heroTexture, 32, 32);
                var HeroWalkingFactory = new SpriteSheetAnimationFactory(HeroAtlas);

                HeroWalkingFactory.Add(Direction.NONE.ToString(), spriteFactory[Direction.NONE]);
                HeroWalkingFactory.Add(Direction.DOWN.ToString(), spriteFactory[Direction.DOWN]);
                HeroWalkingFactory.Add(Direction.LEFT.ToString(), spriteFactory[Direction.LEFT]);
                HeroWalkingFactory.Add(Direction.RIGHT.ToString(), spriteFactory[Direction.RIGHT]);
                HeroWalkingFactory.Add(Direction.UP.ToString(), spriteFactory[Direction.UP]);

                heroAnimations = new SpriteSheetAnimator(HeroWalkingFactory);
                heroSprite = heroAnimations.CreateSprite(positionVirt);

                actualDir = Direction.NONE;
                lastDir = actualDir;
            }
            else if (typePlayer == TypePlayer.WithTexture2D)
            {
                heroTexture = content.Load<Texture2D>(_texture);
            }

            position = new Vector2(positionVirt.X * tileWidth, positionVirt.Y * tileHeight);
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

        public void checkMove(KeyboardState keyboardState, Camera2D camera)
        {
            
         
            if (separeFrame == 0)
            {
                if(typeDeplacement == TypeDeplacement.WithKey && actualDir == Helpers.Direction.NONE && keyboardState.GetPressedKeys().Length != 0)
                {
                    if (keyboardState.IsKeyDown(Keys.LeftShift)) actualFrame = fastFrame;
                    else actualFrame = lowFrame;

                    if (keyboardState.IsKeyDown(Keys.Down))
                    {
                        if (!ColDown()) actualDir = Helpers.Direction.DOWN;
                        lastDir = Helpers.Direction.DOWN;
                        heroAnimations.Play(Helpers.Direction.DOWN.ToString());

                    }
                    else if (keyboardState.IsKeyDown(Keys.Up))
                    {
                        if (!ColUp()) actualDir = Helpers.Direction.UP;
                        lastDir = Helpers.Direction.UP;
                        heroAnimations.Play(Helpers.Direction.UP.ToString());
                    }
                    else if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        if (!ColLeft()) actualDir = Helpers.Direction.LEFT;
                        lastDir = Helpers.Direction.LEFT;
                        heroAnimations.Play(Helpers.Direction.LEFT.ToString());
                    }
                    else if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        if (!ColRight()) actualDir = Helpers.Direction.RIGHT;
                        lastDir = Helpers.Direction.RIGHT;
                        heroAnimations.Play(Helpers.Direction.RIGHT.ToString());
                    }

                    separeFrame++;
                }
                else if(typeDeplacement == TypeDeplacement.WithDirection && actualDir != Direction.NONE)
                {
                    if (actualDir == Direction.RIGHT && ColRight())
                    {
                        actualDir = Direction.NONE;
                        return;
                    }
                    else if (actualDir == Direction.LEFT && ColLeft())
                    {
                        actualDir = Direction.NONE;
                        return;
                    }
                    else if(actualDir == Direction.UP && ColUp())
                    {
                        actualDir = Direction.NONE;
                        return;
                    }
                    else if(actualDir == Direction.DOWN && ColDown())
                    {
                        actualDir = Direction.NONE;
                        return;
                    }
                    lastDir = actualDir;
                    heroAnimations.Play(actualDir.ToString());
                    separeFrame++;
                }                
                
            }
            else if (separeFrame != 0)
            {

                if (separeFrame >= actualFrame)
                {
                    if (actualDir == Helpers.Direction.DOWN) MoveDownChamp();
                    if (actualDir == Helpers.Direction.UP)MoveUpChamp();
                    if (actualDir == Helpers.Direction.LEFT)MoveLeftChamp();
                    if (actualDir == Helpers.Direction.RIGHT)MoveRightChamp();
                    

                    position = new Vector2(positionVirt.X * tileWidth, positionVirt.Y * tileHeight);


                    actualDir = Helpers.Direction.NONE;
                    separeFrame = 0;
                }
                else
                {
                    if (actualDir == Helpers.Direction.DOWN)
                    {
                        position += new Vector2(0, tileHeight / actualFrame);
                       }
                    if (actualDir == Helpers.Direction.UP)
                    {
                        position += new Vector2(0, -(tileHeight / actualFrame));
                        }
                    if (actualDir == Helpers.Direction.LEFT)
                    {
                        position += new Vector2(-(tileWidth / actualFrame), 0);
                        }
                    if (actualDir == Helpers.Direction.RIGHT)
                    {

                        position += new Vector2(tileWidth / actualFrame, 0);
                        
                    }
                    separeFrame++;
                }

            }
        }

        public bool ColUp()
        {
            if (positionVirt.Y <= 0) return true;
            foreach (TiledTile i in collisionLayer.Tiles)
            {
                if (i.X == positionVirt.X && i.Y == positionVirt.Y - 1 && i.Id == gidCol) return true;
                if(_runtimeData.Map != null) _runtimeData.Map.checkIfWeLaunchInstance(i); 
            }

            foreach(KeyValuePair<Entities.ListPnj,PNJ> i in _runtimeData.PNJ)
            {
                if (positionVirt.X == i.Value.PNJPlayer.positionVirt.X && positionVirt.Y - 1 == i.Value.PNJPlayer.positionVirt.Y) return true;
            }

            return false;
        }

        public bool ColDown()
        {

            if (positionVirt.Y >= mapHeight - 1) return true;
            foreach (TiledTile i in collisionLayer.Tiles)
            {
                if (i.X == positionVirt.X && i.Y == positionVirt.Y + 1 && i.Id == gidCol) return true;
            }
            foreach (KeyValuePair<Entities.ListPnj, PNJ> i in _runtimeData.PNJ)
            {
                if (positionVirt.X == i.Value.PNJPlayer.positionVirt.X && positionVirt.Y + 1 == i.Value.PNJPlayer.positionVirt.Y) return true;
            }
            return false;
        }

        public bool ColLeft()
        {
            if (positionVirt.X <= 0) return true;
            foreach (TiledTile i in collisionLayer.Tiles)
            {
                if (i.X == positionVirt.X - 1 && i.Y == positionVirt.Y && i.Id == gidCol) return true;
            }
            foreach (KeyValuePair<Entities.ListPnj, PNJ> i in _runtimeData.PNJ)
            {
                if (positionVirt.X - 1 == i.Value.PNJPlayer.positionVirt.X && _runtimeData.Player.positionVirt.Y == i.Value.PNJPlayer.positionVirt.Y) return true;
            }
            return false;
        }

        public bool ColRight()
        {
            if (positionVirt.X >= mapWidth - 1) return true;
            foreach (TiledTile i in collisionLayer.Tiles)
            {
                if (i.X == positionVirt.X + 1 && i.Y == positionVirt.Y && i.Id == gidCol) return true;
            }
            foreach (KeyValuePair<Entities.ListPnj, PNJ> i in _runtimeData.PNJ)
            {
                if (positionVirt.X + 1 == i.Value.PNJPlayer.positionVirt.X && _runtimeData.Player.positionVirt.Y == i.Value.PNJPlayer.positionVirt.Y) return true;
            }
            return false;
        }


    }
}
