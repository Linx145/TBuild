using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.IO;
using Terraria.DataStructures;

namespace TBuild
{
    public class TBuildLoadOperations
    {
        public static int NewSign(int x, int y, string text)
        {
            for (int i = 0; i < Main.sign.Length; i++)
            {
                if (Main.sign[i] == null)
                {
                    Main.sign[i] = new Sign();
                    Main.sign[i].x = x;
                    Main.sign[i].y = y;
                    Main.sign[i].text = text;
                    return i;
                }
            }
            return -1;
        }
        public static int NewChest(int x, int y, string name)
        {
            for (int i = 0; i < Main.chest.Length; i++)
            {
                if (Main.chest[i] == null)
                {
                    Main.chest[i] = new Chest();
                    Main.chest[i].x = x;
                    Main.chest[i].y = y;
                    Main.chest[i].name = name;
                    for (int c = 0; c < Main.chest[i].item.Length; c++)
                    {
                        Main.chest[i].item[c] = new Item();
                    }
                    return i;
                }
            }
            return -1;
        }
        internal static SerializedTile LoadTile(BinaryReader reader)
        {
            SerializedTile result = new SerializedTile();

            BitsByte flags1 = reader.ReadByte();
            BitsByte flags2 = reader.ReadByte();

            result.RedWire = flags1[1];
            result.GreenWire = flags1[2];
            result.BlueWire = flags1[3];
            result.YellowWire = flags1[4];
            result.hasActuator = flags1[5];
            result.isActuated = flags1[6];
            result.isHalfBlock = flags2[5];

            result.tileID = -1;
            result.wallID = WallID.None;
            result.frameX = -1;
            result.frameY = -1;
            result.tileColor = PaintID.None;
            result.wallColor = PaintID.None;
            result.slopeType = SlopeType.Solid;

            result.liquidType = 0;
            result.liquidAmount = 0;

            if (flags1[0]) //hasTile
            {
                if (flags1[7]) //modTile != null
                {
                    string modName = reader.ReadString();
                    string name = reader.ReadString();

                    Mod mod = ModLoader.GetMod(modName);
                    if (mod != null)
                    {
                        result.tileID = mod.Find<ModTile>(name).Type;
                    }
                }
                else
                {
                    result.tileID = reader.ReadUInt16();
                }
                if (Main.tileFrameImportant[result.tileID])
                {
                    result.frameX = reader.ReadInt16();
                    result.frameY = reader.ReadInt16();
                }
                if (flags2[2]) //tile.TileColor != PaintID.None;
                {
                    result.tileColor = reader.ReadByte();
                }
                if (flags2[4]) //tile.Slope != SlopeType.Solid;
                {
                    byte slopeTypeID = reader.ReadByte();
                    result.slopeType = (SlopeType)slopeTypeID;
                }
            }

            if (flags2[0]) //tile.WallType != WallID.None
            {
                if (flags2[1]) //modWall != null
                {
                    string modName = reader.ReadString();
                    string name = reader.ReadString();

                    Mod mod = ModLoader.GetMod(modName);
                    if (mod != null)
                    {
                        result.wallID = mod.Find<ModWall>(name).Type;
                    }
                }
                else
                {
                    result.wallID = reader.ReadUInt16();
                }
                if (flags2[3]) //tile.WallColor != PaintID.None
                {
                    result.wallColor = reader.ReadByte();
                }
            }

            if (flags2[6])//tile.LiquidAmount > 0
            {
                result.liquidType = reader.ReadInt32();
                result.liquidAmount = reader.ReadByte();
            }

            return result;
        }
        public static TBuildLoadResult Load(Point position, int version, int width, int height, TagCompound compound, bool frameTiles = true)
        {
            if (version == 4)
            {
                try
                {
                    List<int> chests = new List<int>();

                    #region load tiles
                    int tileSavesCount = compound.Get<int>("tileSaves");
                    byte[] tileData = compound.GetByteArray("tileData");

                    using (MemoryStream memoryStream = new MemoryStream(tileData))
                    {
                        using (BinaryReader reader = new BinaryReader(memoryStream))
                        {
                            for (int i = position.X; i < position.X + width + 1; i++)
                            {
                                ushort cont = 0;
                                SerializedTile serialized = default;
                                for (int j = position.Y; j < position.Y + height + 1; j++)
                                {
                                    //read tile if cont == 0
                                    if (cont == 0)
                                    {
                                        cont = reader.ReadUInt16();
                                        serialized = LoadTile(reader);
                                    }
                                    //place tile
                                    Tile tile = Main.tile[i, j];
                                    tile.HasTile = serialized.HasTile;
                                    if (serialized.HasTile)
                                    {
                                        tile.TileType = (ushort)serialized.tileID;
                                        if (Main.tileFrameImportant[tile.TileType])
                                        {
                                            tile.TileFrameX = serialized.frameX;
                                            tile.TileFrameY = serialized.frameY;
                                        }
                                        tile.TileColor = serialized.tileColor;
                                        tile.Slope = serialized.slopeType;
                                    }
                                    if (serialized.liquidAmount > 0)
                                    {
                                        tile.LiquidAmount = serialized.liquidAmount;
                                        tile.LiquidType = serialized.liquidType;
                                    }
                                    tile.RedWire = serialized.RedWire;
                                    tile.GreenWire = serialized.GreenWire;
                                    tile.BlueWire = serialized.BlueWire;
                                    tile.YellowWire = serialized.YellowWire;
                                    tile.IsHalfBlock = serialized.isHalfBlock;
                                    tile.IsActuated = serialized.isActuated;
                                    tile.HasActuator = serialized.hasActuator;
                                    tile.WallColor = serialized.wallColor;
                                    tile.WallType = serialized.wallID;

                                    cont--;
                                }
                            }
                        }
                    }
                    if (frameTiles)
                    {
                        for (int i = position.X; i < position.X + width; i++)
                        {
                            for (int j = position.Y; j < position.Y + height; j++)
                            {
                                WorldGen.SquareTileFrame(i, j);
                                WorldGen.SquareWallFrame(i, j);
                            }
                        }
                    }
                    #endregion

                    #region load chests
                    if (compound.ContainsKey("chests"))
                    {
                        IList<TagCompound> loadedChests = compound.GetList<TagCompound>("chests");
                        for (int i = 0; i < loadedChests.Count; i++)
                        {
                            Point16 chestPos = loadedChests[i].Get<Point16>("position");
                            string chestName = loadedChests[i].Get<string>("name");
                            int chest = NewChest(chestPos.X + position.X, chestPos.Y + position.Y, chestName);

                            IList<TagCompound> chestItems = loadedChests[i].GetList<TagCompound>("items");
                            for (int c = 0; c < chestItems.Count; c++)
                            {
                                Main.chest[chest].item[c] = ItemIO.Load(chestItems[c]);
                            }
                            chests.Add(chest);
                        }
                    }
                    #endregion

                    #region load signs
                    if (compound.ContainsKey("signs"))
                    {
                        IList<TagCompound> signs = compound.GetList<TagCompound>("signs");
                        for (int i = 0; i < signs.Count; i++)
                        {
                            NewSign(signs[i].Get<int>("x") + position.X, signs[i].Get<int>("y") + position.Y, signs[i].Get<string>("text"));
                        }
                    }
                    #endregion

                    return new TBuildLoadResult(chests);
                }
                catch (Exception e)
                {
                    Main.NewText(e.ToString());
                    return default;
                }
            }
            else
            {
                Main.NewText("Error: Unrecognised version!");
                return default;
            }
        }
        public static void LoadFromExternalFile(Point position, string path)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                int version = reader.ReadInt32();
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                TagCompound compound = TagIO.Read(reader);
                Load(position, version, width, height, compound);
            }
        }
        public static void Load(Mod mod, Point position, string filePathInMod)
        {
            using (MemoryStream stream = new MemoryStream(mod.GetFileBytes(filePathInMod)))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int version = reader.ReadInt32();
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    TagCompound compound = TagIO.Read(reader);
                    Load(position, version, width, height, compound);
                }
            }
        }
        public static Point GetDimensions(Mod mod, Point position, string filePathInMod)
        {
            Point result = new Point();
            using (MemoryStream stream = new MemoryStream(mod.GetFileBytes(filePathInMod)))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int version = reader.ReadInt32();
                    result.X = reader.ReadInt32();
                    result.Y = reader.ReadInt32();
                }
            }
            return result;
        }
    }
}