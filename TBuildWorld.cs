using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.IO;
using Terraria.ModLoader.IO;

namespace TBuild
{
    public class TBuildWorld : ModSystem
    {
        public static string fileName;
        public static Point markerPos1;
        public static Point markerPos2;
        public const int version = 4;
        public const int hitmapSaveVersion = 1;
        public static bool saveProjectiles;
        public static bool saveNPCs;

        //version 2: No TileEntitySupport
        //version 3: Compressed

        public static void ReplaceWallOperation(ushort wall1, ushort wall2)
        {
            int wallsChanged = 0;
            Main.NewText(wall1);
            Main.NewText(wall2);
            if (markerPos1 == markerPos2)
            {
                return;
            }
            for (int i = markerPos1.X; i < markerPos2.X + 1; i++)
            {
                for (int j = markerPos1.Y; j < markerPos2.Y + 1; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile.WallType == wall1)//(Main.tile[i, j].wall == WallID.None && wall1 == ushort.MaxValue) || (Main.tile[i, j].wall != WallID.None && Main.tile[i, j].wall == wall1))
                    {
                        tile.WallType = wall2;
                        if (wall2 == WallID.None)
                        {
                            tile.WallColor = 0;
                        }
                        WorldGen.SquareWallFrame(i, j);
                        wallsChanged++;
                    }
                }
            }
            Main.NewText("walls changed: " + wallsChanged.ToString(), Colors.RarityPink);
        }

        public static void ReplaceOperation(ushort tile1, ushort tile2)
        {
            int tilesChanged = 0;
            if (markerPos1 == markerPos2)
            {
                return;
            }
            for (int i = markerPos1.X; i < markerPos2.X + 1; i++)
            {
                for (int j = markerPos1.Y; j < markerPos2.Y + 1; j++)
                {
                    if ((!Main.tile[i, j].HasTile && tile1 == ushort.MaxValue) || (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tile1))
                    {
                        if (tile2 == ushort.MaxValue)
                        {
                            TBuild.SetTileNone(i, j);
                        }
                        else
                        {
                            Tile tile = Main.tile[i, j];
                            tile.TileType = tile2;
                            tile.HasTile = true;
                        }
                        WorldGen.SquareTileFrame(i, j);
                        tilesChanged++;
                    }
                }
            }
            Main.NewText("tiles changed: " + tilesChanged.ToString(), Colors.RarityPink);
        }

        public static void SetWallOperation(ushort wall)
        {
            int tilesChanged = 0;
            if (markerPos1 == markerPos2)
            {
                return;
            }
            for (int i = markerPos1.X; i < markerPos2.X + 1; i++)
            {
                for (int j = markerPos1.Y; j < markerPos2.Y + 1; j++)
                {
                    Tile tile = Main.tile[i, j];
                    tile.WallType = wall;
                    if (wall == WallID.None)
                    {
                        tile.WallColor = 0;
                    }
                    WorldGen.SquareWallFrame(i, j);
                    tilesChanged++;
                }
            }
            Main.NewText("walls changed: " + tilesChanged.ToString(), Colors.RarityPink);
        }
        public static void SetTileOperation(ushort tile1)
        {
            int tilesChanged = 0;
            if (markerPos1 == markerPos2)
            {
                return;
            }
            for (int i = markerPos1.X; i < markerPos2.X + 1; i++)
            {
                for (int j = markerPos1.Y; j < markerPos2.Y + 1; j++)
                {
                    if (tile1 == ushort.MaxValue)
                    {
                        TBuild.SetTileNone(i, j);
                    }
                    else
                    {
                        Tile tile = Main.tile[i, j];
                        tile.TileType = tile1;
                        tile.HasTile = true;
                    }
                    WorldGen.SquareTileFrame(i, j);
                    tilesChanged++;
                }
            }
            Main.NewText("tiles changed: " + tilesChanged.ToString(), Colors.RarityPink);
        }

        public static Rectangle saveArea
        {
            get
            {
                return new Rectangle(markerPos1.X, markerPos1.Y, markerPos2.X - markerPos1.X + 1, markerPos2.Y - markerPos1.Y + 1);
            }
        }

        public static bool isSaving = false;



        public static void SaveHitmap()
        {
            if (markerPos1 == markerPos2)
            {
                return;
            }
            Main.NewText("Save Started!");
            isSaving = true;

            //we do not need to write the actual amt of shorts written because we can bypass that by simply incrementing the j value by amount read during read operations
            using (BinaryWriter writer = new BinaryWriter(File.Create(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/" + fileName + ".tbm")))//BoolCompressingWriter writer = new BoolCompressingWriter(File.Create(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/" + fileName + ".tbm")))
            {
                writer.Write(hitmapSaveVersion);
                writer.Write(markerPos2.X - markerPos1.X + 1);
                writer.Write(markerPos2.Y - markerPos1.Y + 1);

                bool cachedTileActive = Main.tile[markerPos1.X, markerPos1.Y].HasTile;
                //negative count = how many tiles are inactive
                //positive count = how many tiles are active
                short cont = 0;
                for (int i = markerPos1.X; i < markerPos2.X + 1; i++)
                {
                    for (int j = markerPos1.Y; j < markerPos2.Y + 1; j++)
                    {
                        if (Main.tile[i, j].HasTile == cachedTileActive)
                        {
                            if (cachedTileActive)
                            {
                                cont++;
                            }
                            else
                            {
                                cont--;
                            }
                            if (j == markerPos2.Y)
                            {
                                writer.Write(cont);
                                cont = 0;
                            }
                        }
                        else
                        {
                            writer.Write(cont);
                            cachedTileActive = Main.tile[i, j].HasTile;
                            cont = 0;
                            if (cachedTileActive)
                            {
                                cont++;
                            }
                            else
                            {
                                cont--;
                            }
                        }
                    }
                }
            }

            Main.NewText("Save Complete!");
            isSaving = false;
        }

        public void Initialize()
        {
            fileName = "TBuild";
            markerPos1 = Point.Zero;
            markerPos2 = Point.Zero;
        }

        public override void OnWorldLoad()
        {
            Initialize();
        }
        public override void OnWorldUnload()
        {
            Initialize();
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(markerPos1.X);
            writer.Write(markerPos1.Y);
            writer.Write(markerPos2.X);
            writer.Write(markerPos2.Y);
            writer.Write(saveProjectiles);
            writer.Write(saveNPCs);
        }
        public override void NetReceive(BinaryReader reader)
        {
            markerPos1.X = reader.ReadInt32();
            markerPos1.Y = reader.ReadInt32();
            markerPos2.X = reader.ReadInt32();
            markerPos2.Y = reader.ReadInt32();
            saveProjectiles = reader.ReadBoolean();
            saveNPCs = reader.ReadBoolean();
        }
        public override void PostUpdateWorld()
        {
            if (markerPos1 != markerPos2 && markerPos1 != Point.Zero && markerPos2 != Point.Zero)
            {
                if (TBuildWorld.markerPos2.X < TBuildWorld.markerPos1.X)// || TBuildWorld.markerPos2.Y < TBuildWorld.markerPos1.Y)
                {
                    int stored = TBuildWorld.markerPos2.X;
                    TBuildWorld.markerPos2.X = TBuildWorld.markerPos1.X;
                    TBuildWorld.markerPos1.X = stored;
                }
                if (TBuildWorld.markerPos2.Y < TBuildWorld.markerPos1.Y)// || TBuildWorld.markerPos2.Y < TBuildWorld.markerPos1.Y)
                {
                    int stored = TBuildWorld.markerPos2.Y;
                    TBuildWorld.markerPos2.Y = TBuildWorld.markerPos1.Y;
                    TBuildWorld.markerPos1.Y = stored;
                }
            }
        }
        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            if (markerPos1 != markerPos2)
            {
                if (markerPos1 != Point.Zero)
                {
                    Vector2 drawPos1 = new Vector2(markerPos1.X * 16, markerPos1.Y * 16) - Main.screenPosition;
                    Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Markers").Value, drawPos1, new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }

                if (markerPos2 != Point.Zero)
                {
                    Vector2 drawPos2 = new Vector2(markerPos2.X * 16, markerPos2.Y * 16) - Main.screenPosition;
                    Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Markers").Value, drawPos2, new Rectangle(16, 0, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }

            /*for (int c = 0; c < TBuildGlobalTile.activeHitmaps.Count; c++)
            {
                Main.spriteBatch.Draw(Main.magicPixel, new Rectangle(TBuildGlobalTile.activeHitmaps[c].area.X - (int)Main.screenPosition.X, TBuildGlobalTile.activeHitmaps[c].area.Y - (int)Main.screenPosition.Y, TBuildGlobalTile.activeHitmaps[c].area.Width, TBuildGlobalTile.activeHitmaps[c].area.Height), Color.Red * 0.25f);
            }*/

            Main.spriteBatch.End();
        }
    }
}
