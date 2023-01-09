using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System.IO;
using System;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using System.Collections.Generic;

namespace TBuild
{
    public class TBXLoadResult
    {
        public readonly int[] loadedChests;

        public TBXLoadResult(int[] chests)
        {
            loadedChests = chests;
        }
    }
    public class TBuild : Mod
    {
        public static int toTilePos(float i)
        {
            return (int)Math.Floor(i / 16f);
        }
        public override void Unload()
        {
            base.Unload();
        }
        public static void SetTileNone(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            tile.IsHalfBlock = false;
            tile.HasTile = false;
            tile.TileFrameX = -1;
            tile.TileFrameY = -1;
            tile.TileColor = 0;
            tile.TileFrameNumber = 0;
            tile.TileType = 0;
            tile.IsActuated = false;
        }
        
        public TBuild()
        {
        }
        public enum MessageType
        {
            sendMarkerData, receiveMarkerData//, replaceTileAction, setTileAction
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType type = (MessageType)reader.ReadInt32();
            /*if (type == MessageType.replaceTileAction)
            {
                ushort tile1 = reader.ReadUInt16();
                ushort tile2 = reader.ReadUInt16();
                System.Threading.Tasks.Task.Run(() => { TBuildWorld.ReplaceOperation(tile1, tile2); });
            }
            else if (type == MessageType.setTileAction)
            {
                ushort tile1 = reader.ReadUInt16();
                System.Threading.Tasks.Task.Run(() => { TBuildWorld.SetTileOperation(tile1); });
            }*/
        }
    }
}