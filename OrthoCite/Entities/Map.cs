using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace OrthoCite.Entities
{
    class Map : IEntity
    {
        RuntimeData _runtimeData;
        TmxMap _map;

        Texture2D tileset;

        int tileWidth;
        int tileHeight;
        int tilesetTilesWide;
        int tilesetTilesHigh;

        public Map(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
            
        }

        void IEntity.LoadContent(ContentManager content)
        {
            _map = new TmxMap("Content/Map.tmx");
           
            tileset = content.Load<Texture2D>(_map.Tilesets[0].Name.ToString());

            tileWidth = _map.Tilesets[0].TileWidth;
            tileHeight = _map.Tilesets[0].TileHeight;

            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;
        }

        void IEntity.UnloadContent()
        {
        }

        void IEntity.Update(GameTime gameTime, KeyboardState keyboardState)
        {
        }

        void IEntity.Draw(SpriteBatch spriteBatch)
        {
           
            for (var i = 0; i < _map.Layers[0].Tiles.Count; i++)
            {
                int gid = _map.Layers[0].Tiles[i].Gid;
                _runtimeData._console.WriteLine((_map.Layers[0].Tiles[i].Gid).ToString());

                if (gid != 0)
                {
                   
                    int tileFrame = gid - 1;
                    int column = tileFrame % tilesetTilesWide;
                    int row = (tileFrame + 1 > tilesetTilesWide) ? tileFrame - column * tilesetTilesWide : 0;

                    float x = (i % _map.Width) * _map.TileWidth;
                    float y = (float)Math.Floor(i / (double)_map.Width) * _map.TileHeight;

                    Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);

                    spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);
                }
            }


        }

    }
}
