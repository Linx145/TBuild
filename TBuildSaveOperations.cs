using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria.GameContent.Tile_Entities;
using Terraria.DataStructures;
using Terraria.ID;
using System.IO;

namespace TBuild
{
    public static class TBuildSaveOperations
    {
        public static bool IsEqual(Tile A, Tile B)
        {
            if (Main.tileFrameImportant[A.TileType])
            {
                return
                    (A.HasTile == B.HasTile) &&
                    (A.TileType == B.TileType) &&
                    (A.WallType == B.WallType) &&
                    (A.TileFrameX == B.TileFrameX) &&
                    (A.TileFrameY == B.TileFrameY) &&
                    (A.RedWire == B.RedWire) &&
                    (A.GreenWire == B.GreenWire) &&
                    (A.BlueWire == B.BlueWire) &&
                    (A.YellowWire == B.YellowWire) &&
                    (A.TileColor == B.TileColor) &&
                    (A.WallColor == B.WallColor) &&
                    (A.LiquidAmount == B.LiquidAmount) &&
                    (A.LiquidType == B.LiquidType) &&
                    (A.Slope == B.Slope) &&
                    (A.IsActuated == B.IsActuated) &&
                    (A.HasActuator == B.HasActuator) &&
                    (A.IsHalfBlock == B.IsHalfBlock);
            }
            else
            {
                return
                    (A.HasTile == B.HasTile) &&
                    (A.TileType == B.TileType) &&
                    (A.WallType == B.WallType) &&
                    (A.RedWire == B.RedWire) &&
                    (A.GreenWire == B.GreenWire) &&
                    (A.BlueWire == B.BlueWire) &&
                    (A.YellowWire == B.YellowWire) &&
                    (A.TileColor == B.TileColor) &&
                    (A.WallColor == B.WallColor) &&
                    (A.LiquidAmount == B.LiquidAmount) &&
                    (A.LiquidType == B.LiquidType) &&
                    (A.Slope == B.Slope) &&
                    (A.IsActuated == B.IsActuated) &&
                    (A.HasActuator == B.HasActuator) &&
                    (A.IsHalfBlock == B.IsHalfBlock);
            }
        }
        public static void SaveTile(BinaryWriter writer, Tile tile)
        {
            ModTile modTile = ModContent.GetModTile(tile.TileType);
            ModWall modWall = ModContent.GetModWall(tile.WallType);

            BitsByte flags1 = new BitsByte();
            flags1[0] = tile.HasTile;
            flags1[1] = tile.RedWire;
            flags1[2] = tile.GreenWire;
            flags1[3] = tile.BlueWire;
            flags1[4] = tile.YellowWire;
            flags1[5] = tile.HasActuator;
            flags1[6] = tile.IsActuated;
            flags1[7] = modTile != null;

            BitsByte flags2 = new BitsByte();
            flags2[0] = tile.WallType != WallID.None;
            flags2[1] = modWall != null;
            flags2[2] = tile.TileColor != PaintID.None;
            flags2[3] = tile.WallColor != PaintID.None;
            flags2[4] = tile.Slope != SlopeType.Solid;
            flags2[5] = tile.IsHalfBlock;
            flags2[6] = tile.LiquidAmount > 0;

            writer.Write(flags1);
            writer.Write(flags2);
            if (tile.HasTile)
            {
                if (modTile != null)
                {
                    writer.Write(modTile.Mod.Name);
                    writer.Write(modTile.Name);
                }
                else
                {
                    writer.Write(tile.TileType);
                }
                if (Main.tileFrameImportant[tile.TileType])
                {
                    writer.Write(tile.TileFrameX);
                    writer.Write(tile.TileFrameY);
                }
                if (tile.TileColor != PaintID.None)
                {
                    writer.Write(tile.TileColor);
                }
                if (tile.Slope != SlopeType.Solid)
                {
                    writer.Write((byte)tile.Slope);
                }
            }
            if (tile.WallType != WallID.None)
            {
                if (modWall != null)
                {
                    writer.Write(modWall.Mod.Name);
                    writer.Write(modWall.Name);
                }
                else
                {
                    writer.Write(tile.WallType);
                }
                if (tile.WallColor != PaintID.None)
                {
                    writer.Write(tile.WallColor);
                }
            }
            if (tile.LiquidAmount > 0)
            {
                writer.Write(tile.LiquidType);
                writer.Write(tile.LiquidAmount);
            }
        }
        public static void SaveSelection()
        {
            if (TBuildWorld.markerPos1 == TBuildWorld.markerPos2)
            {
                return;
            }
            Main.NewText("Save Started!");
            TBuildWorld.isSaving = true;
            TagCompound mainCompound = new TagCompound();

            #region handle tiles
            int tileSavesCount = 0;

            using (MemoryStream memory = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memory))
                {
                    for (int i = TBuildWorld.markerPos1.X; i <= TBuildWorld.markerPos2.X; i++)
                    {
                        Tile firstTile = Framing.GetTileSafely(i, TBuildWorld.markerPos1.Y);
                        ushort cont = 0;

                        for (int j = TBuildWorld.markerPos1.Y; j <= TBuildWorld.markerPos2.Y; j++)
                        {
                            if (IsEqual(Main.tile[i, j], firstTile))
                            {
                                cont++;
                            }
                            else
                            {
                                tileSavesCount++;
                                writer.Write(cont);
                                SaveTile(writer, firstTile);
                                firstTile = Framing.GetTileSafely(i, j);
                                cont = 1; //set to 1 because we will move on and not be revisiting this tile, unlike when during the first initialization
                            }
                        }
                        //save the rest
                        writer.Write(cont);
                        SaveTile(writer, firstTile);
                        tileSavesCount++;
                    }
                }

                mainCompound.Add("tileData", memory.ToArray());
            }

            mainCompound.Add("tileSaves", tileSavesCount);
            #endregion

            #region handle chests
            List<TagCompound> chests = new List<TagCompound>();
            for (int m = 0; m < Main.chest.Length; m++)
            {
                Chest chest = Main.chest[m];
                if (chest != null)
                {
                    if (TBuildWorld.saveArea.Contains(chest.x, chest.y))
                    {
                        TagCompound chestCompound = new TagCompound();
                        chestCompound.Add("position", new Point16(chest.x - TBuildWorld.markerPos1.X, chest.y - TBuildWorld.markerPos1.Y));
                        chestCompound.Add("name", chest.name);

                        List<TagCompound> itemCompound = new List<TagCompound>();
                        for (int k = 0; k < chest.item.Length; k++)
                        {
                            Item item = chest.item[k];
                            itemCompound.Add(ItemIO.Save(item));
                        }
                        chestCompound.Add("items", itemCompound);
                        chests.Add(chestCompound);
                        Main.NewText(string.Concat("Saved Chest at (", chest.x, ", ", chest.y, ") with stored coordinates ", new Point(chest.x - TBuildWorld.markerPos1.X, chest.y - TBuildWorld.markerPos1.Y).ToString()));
                    }
                }
            }
            mainCompound.Add("chests", chests);
            #endregion

            #region signs
            List<TagCompound> signs = new List<TagCompound>();
            for (int i = 0; i < Main.sign.Length; i++)
            {
                if (Main.sign[i] != null && TBuildWorld.saveArea.Contains(new Point(Main.sign[i].x, Main.sign[i].y)))
                {
                    signs.Add(new TagCompound()
                    {
                        {
                            "text", Main.sign[i].text
                        },
                        {
                            "x", Main.sign[i].x - TBuildWorld.markerPos1.X
                        },
                        {
                            "y", Main.sign[i].y - TBuildWorld.markerPos1.Y
                        }
                    });
                    Main.NewText(string.Concat("Added sign at (", Main.sign[i].x, ", ", Main.sign[i].y, ")"));
                }
            }
            if (signs.Count > 0) mainCompound.Add("signs", signs);
            #endregion

            using (BinaryWriter writer = new BinaryWriter(File.Create(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/" + TBuildWorld.fileName + ".tbx")))
            {
                writer.Write(TBuildWorld.version);
                writer.Write(TBuildWorld.markerPos2.X - TBuildWorld.markerPos1.X);
                writer.Write(TBuildWorld.markerPos2.Y - TBuildWorld.markerPos1.Y);
                TagIO.Write(mainCompound, writer);
            }
            Main.NewText("Save Complete!");
        }
    }
}
