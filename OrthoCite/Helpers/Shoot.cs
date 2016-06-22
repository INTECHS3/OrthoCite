using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using OrthoCite.Helpers;
using System.Collections.Generic;

namespace OrthoCite.Helpers
{
    /// <summary>
    /// Use Shoot only for ThrowGame
    /// </summary>
    public class Shoot
    {
        public int SpeedOfShoot { get; set; }
        Direction _directShoot { get; set; }
        public Vector2 Position { get; private set; }
        
        RuntimeData _runtimeData;
        Dictionary<Direction, Texture2D> _textShoot;


        public Shoot(RuntimeData runtimeData, Direction directShoot, Dictionary<Direction, Texture2D> textShoot)
        {
            _runtimeData = runtimeData;
            _directShoot = directShoot;
            _textShoot = textShoot;

            Load();
        }

        public void Load()
        {
            if (_directShoot == Direction.RIGHT || _directShoot == Direction.LEFT) Position = new Vector2(_runtimeData.Player.position.X, _runtimeData.Player.tileHeight / 2 /2+ _runtimeData.Player.position.Y);
            else Position = new Vector2(_runtimeData.Player.position.X, _runtimeData.Player.position.Y);
        }


        public void Update()
        {
            if (_directShoot == Direction.NONE) _directShoot = Direction.RIGHT;

            if (_directShoot == Direction.DOWN) Position += new Vector2(0, 5);
            else if (_directShoot == Direction.UP) Position += new Vector2(0, -5);
            else if (_directShoot == Direction.LEFT) Position += new Vector2(-5, 0);
            else if (_directShoot == Direction.RIGHT) Position += new Vector2(5, 0); 

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_textShoot[_directShoot], Position, Color.White);
        }
    }
}
