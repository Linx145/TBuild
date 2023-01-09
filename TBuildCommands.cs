using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Reflection;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.IO;
using Terraria.ModLoader.IO;

namespace TBuild
{
    public class SelectionWidth : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }
        public override string Command => "selectionwidth";
        public override string Description => "displays the dimensions of the current selection. Can also be used to measure distances!";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (Main.netMode != 2)
            {
                if (TBuildWorld.markerPos1 != Point.Zero)
                {
                    Main.NewText(string.Concat("Width: ", TBuildWorld.markerPos2.X - TBuildWorld.markerPos1.X, ", Height: ", TBuildWorld.markerPos2.Y - TBuildWorld.markerPos1.Y), Colors.RarityPink);
                }
            }
        }
    }
    public class BuildTools : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }
        public override string Command => "buildtools";
        public override string Description => "gives the player all TBuild tools";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (ModContent.GetInstance<TBuildConfig>().editorMode)
            {
                var source = caller.Player.GetSource_Loot();
                caller.Player.QuickSpawnItem(source, Mod.Find<ModItem>("SaveTool").Type);
                caller.Player.QuickSpawnItem(source, Mod.Find<ModItem>("SaveHitmapTool").Type);
                caller.Player.QuickSpawnItem(source, Mod.Find<ModItem>("SelectorTool").Type);
                caller.Player.QuickSpawnItem(source, Mod.Find<ModItem>("WorldSelector").Type);
                caller.Player.QuickSpawnItem(source, Mod.Find<ModItem>("TBuilder").Type);
                caller.Player.QuickSpawnItem(source, Mod.Find<ModItem>("THEbub").Type);
            }
        }
    }
    public class setfilename : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.World; }
        }

        public override string Command
        {
            get { return "filename"; }
        }

        public override string Description
        {
            get { return "Changes the name of what the build should be saved as"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length > 0)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (int i = 0; i < args.Length; i++)
                {
                    builder.Append(args[i]);
                    builder.Append(" ");
                }

                string single = builder.ToString();
                if (single.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                {
                    Main.NewText("Invalid filename!");
                }
                else
                {
                    TBuildWorld.fileName = single;
                    Main.NewText("file name set: " + single);
                }
            }
            else Main.NewText("invalid arguments!");
        }
    }
    public class set : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "set"; }
        }
        public override string Description
        {
            get { return "Sets all tiles in the selection to <argument1>, where both arguments are the internal text name of the tile"; }
        }
        public override string Usage => "All inputs are case sensitive. Vanilla inputs are searched from Terraria.TileID. To specify modded tiles for argument1, use <mod name>:<internal tile name>. To specify empty tiles, use 'air'.";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            bool toBreak = false;
            //FieldInfo tileToBeReplacedInfo = typeof(TileID).GetField(args[0], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            ushort tile1 = 0;

            string[] argument1 = args[0].Split(new char[1] { ':' }, 2);

            if (argument1.Length == 1)
            {
                //Main.NewText("argument1[0]: " + argument1[0]);
                if (argument1[0] != "air")
                {
                    //Main.NewText("searching for tile 1 under name: " + argument1[0]);
                    FieldInfo info = typeof(TileID).GetField(argument1[0], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    if (info != null)
                    {
                        tile1 = (ushort)info.GetValue(null);
                        //Main.NewText("vanilla tile 1: " + tile1.ToString());
                    }
                    else
                    {
                        Main.NewText("Error! unrecognised vanilla tile: " + argument1[0].ToString(), Colors.RarityRed);
                        toBreak = true;
                    }
                }
                else
                {
                    tile1 = ushort.MaxValue;
                }
            }
            else
            {
                int potential = 0;
                Mod selectMod = ModLoader.GetMod(argument1[0]);
                if (selectMod != null)
                {
                    potential = selectMod.Find<ModTile>(argument1[1]).Type;
                }
                if (potential != 0)
                {
                    tile1 = (ushort)potential;
                }
                else
                {
                    Main.NewText(string.Concat("Error! Unrecognised tile \"", argument1[1], "\"", " from mod \"", argument1[0], "\""), Colors.RarityRed);
                    toBreak = true;
                }
            }

            if (!toBreak)
            {
                /*if (Main.netMode == 1)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((int)TBuild.MessageType.setTileAction);
                    packet.Write(tile1);
                    packet.Send();
                }
                else if (Main.netMode == 0)
                {*/
                System.Threading.Tasks.Task.Run(() => { TBuildWorld.SetTileOperation(tile1); });
                //}
            }
            //TBuildGlobalTile.activeHitmaps.Clear();
        }
    }
    public class setWall : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "setwall"; }
        }
        public override string Description
        {
            get { return "Sets all tiles in the selection to <argument1>, where both arguments are the internal text name of the tile"; }
        }
        public override string Usage => "All inputs are case sensitive. Vanilla inputs are searched from Terraria.TileID. To specify modded tiles for argument1, use <mod name>:<internal tile name>. To specify empty tiles, use 'air'.";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            bool toBreak = false;
            //FieldInfo tileToBeReplacedInfo = typeof(TileID).GetField(args[0], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            ushort wall1 = 0;

            string[] argument1 = args[0].Split(new char[1] { ':' }, 2);

            if (argument1.Length == 1)
            {
                //Main.NewText("argument1[0]: " + argument1[0]);
                if (argument1[0] != "air")
                {
                    //Main.NewText("searching for tile 1 under name: " + argument1[0]);
                    FieldInfo info = typeof(WallID).GetField(argument1[0], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    if (info != null)
                    {
                        wall1 = (ushort)info.GetValue(null);
                        //Main.NewText("vanilla tile 1: " + tile1.ToString());
                    }
                    else
                    {
                        Main.NewText("Error! unrecognised vanilla tile: " + argument1[0].ToString(), Colors.RarityRed);
                        toBreak = true;
                    }
                }
                else
                {
                    wall1 = WallID.None;
                }
            }
            else
            {
                int potential = 0;
                Mod selectMod = ModLoader.GetMod(argument1[0]);
                if (selectMod != null)
                {
                    potential = selectMod.Find<ModWall>(argument1[1]).Type;
                }
                if (potential != 0)
                {
                    wall1 = WallID.None;
                }
                else
                {
                    Main.NewText(string.Concat("Error! Unrecognised wall \"", argument1[1], "\"", " from mod \"", argument1[0], "\""), Colors.RarityRed);
                    toBreak = true;
                }
            }

            if (!toBreak)
            {
                /*if (Main.netMode == 1)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((int)TBuild.MessageType.setTileAction);
                    packet.Write(tile1);
                    packet.Send();
                }
                else if (Main.netMode == 0)
                {*/
                System.Threading.Tasks.Task.Run(() => { TBuildWorld.SetWallOperation(wall1); });
                //}
            }
            //TBuildGlobalTile.activeHitmaps.Clear();
        }
    }

    public class replace : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "replace"; }
        }

        public override string Description
        {
            get { return "Replaces all tiles in the selection of type <argument1> with type <argument2>, where both arguments are the internal text name of the tile"; }
        }
        public override string Usage => "<argument1> specifies the tile to be replaced, <argument2> specifies the tile it should be replaced with. All inputs are case sensitive. Vanilla inputs are searched from Terraria.TileID. To specify modded tiles for either argument, use <mod name>:<internal tile name>. To specify empty tiles, use 'air'.";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            bool toBreak = false;
            //FieldInfo tileToBeReplacedInfo = typeof(TileID).GetField(args[0], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            ushort tile1 = 0;
            ushort tile2 = 0;

            string[] argument1 = args[0].Split(new char[1] { ':' }, 2);
            string[] argument2 = args[1].Split(new char[1] { ':' }, 2);

            if (argument1.Length == 1)
            {
                //Main.NewText("argument1[0]: " + argument1[0]);
                if (argument1[0] != "air")
                {
                    //Main.NewText("searching for tile 1 under name: " + argument1[0]);
                    FieldInfo info = typeof(TileID).GetField(argument1[0], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    if (info != null)
                    {
                        tile1 = (ushort)info.GetValue(null);
                        //Main.NewText("vanilla tile 1: " + tile1.ToString());
                    }
                    else
                    {
                        Main.NewText("Error! unrecognised vanilla tile: " + argument1[0].ToString(), Colors.RarityRed);
                        toBreak = true;
                    }
                }
                else
                {
                    tile1 = ushort.MaxValue;
                }
            }
            else
            {
                int potential = 0;
                Mod selectMod = ModLoader.GetMod(argument1[0]);
                if (selectMod != null)
                {
                    potential = selectMod.Find<ModTile>(argument1[1]).Type;
                }
                if (potential != 0)
                {
                    tile1 = (ushort)potential;
                }
                else
                {
                    Main.NewText(string.Concat("Error! Unrecognised tile \"", argument1[1], "\"", " from mod \"", argument1[0], "\""), Colors.RarityRed);
                    toBreak = true;
                }
            }

            if (argument2.Length == 1)
            {
                if (argument2[0] != "air")
                {
                    FieldInfo info = typeof(TileID).GetField(argument2[0], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    if (info != null)
                    {
                        tile2 = (ushort)info.GetValue(null);
                    }
                    else
                    {
                        Main.NewText("Error! unrecognised vanilla tile: " + argument2[0].ToString(), Colors.RarityRed);
                        toBreak = true;
                    }
                }
                else
                {
                    tile2 = ushort.MaxValue;
                }
            }
            else
            {
                int potential = 0;
                Mod selectMod = ModLoader.GetMod(argument2[0]);
                if (selectMod != null)
                {
                    potential = selectMod.Find<ModTile>(argument2[1]).Type;
                }

                if (potential != 0)
                {
                    tile2 = (ushort)potential;
                }
                else
                {
                    Main.NewText(string.Concat("Error! Unrecognised tile \"", argument2[1], "\"", " from mod \"", argument2[0], "\""), Colors.RarityRed);
                    toBreak = true;
                }

            }

            if (!toBreak)
            {
                /*if (Main.netMode == 1)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((int)TBuild.MessageType.replaceTileAction);
                    packet.Write(tile1);
                    packet.Write(tile2);
                    packet.Send();
                }
                else if (Main.netMode == 0)
                {*/
                System.Threading.Tasks.Task.Run(() => { TBuildWorld.ReplaceOperation(tile1, tile2); });
                //}
            }
            //TBuildGlobalTile.activeHitmaps.Clear();
        }
    }

    public class replacewall : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "replacewall"; }
        }

        public override string Description
        {
            get { return "Replaces all walls in the selection of type <argument1> with type <argument2>, where both arguments are the internal text name of the wall"; }
        }
        public override string Usage => "<argument1> specifies the wall to be replaced, <argument2> specifies the wall it should be replaced with. All inputs are case sensitive. Vanilla inputs are searched from Terraria.WallID. To specify modded wall for either argument, use <mod name>:<internal wall name>. To specify no wall, use 'air' or 'None'. HINT: Remember to check whether a wall is unsafe or not!";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            bool toBreak = false;

            ushort wall1 = 0;
            ushort wall2 = 0;

            string[] argument1 = args[0].Split(new char[1] { ':' }, 2);
            string[] argument2 = args[1].Split(new char[1] { ':' }, 2);

            if (argument1.Length == 1)
            {
                if (argument1[0] != "air")
                {

                    FieldInfo info = typeof(WallID).GetField(argument1[0], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    if (info != null)
                    {
                        wall1 = (ushort)info.GetValue(null);
                    }
                    else
                    {
                        Main.NewText("Error! unrecognised vanilla wall: " + argument1[0].ToString(), Colors.RarityRed);
                        toBreak = true;
                    }
                }
                else
                {
                    wall1 = WallID.None;
                }
            }
            else
            {
                int potential = 0;
                Mod selectMod = ModLoader.GetMod(argument1[0]);
                if (selectMod != null)
                {

                    potential = selectMod.Find<ModWall>(argument1[1]).Type;
                }
                if (potential != 0)
                {
                    wall1 = (ushort)potential;
                }
                else
                {
                    Main.NewText(string.Concat("Error! Unrecognised wall \"", argument1[1], "\"", " from mod \"", argument1[0], "\""), Colors.RarityRed);
                    toBreak = true;
                }
            }

            if (argument2.Length == 1)
            {
                if (argument2[0] != "air")
                {
                    FieldInfo info = typeof(WallID).GetField(argument2[0], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    if (info != null)
                    {
                        wall2 = (ushort)info.GetValue(null);
                    }
                    else
                    {
                        Main.NewText("Error! unrecognised vanilla wall: " + argument2[0].ToString(), Colors.RarityRed);
                        toBreak = true;
                    }
                }
                else
                {
                    wall2 = WallID.None;// ushort.MaxValue;
                }
            }
            else
            {
                int potential = 0;
                Mod selectMod = ModLoader.GetMod(argument2[0]);
                if (selectMod != null)
                {
                    potential = selectMod.Find<ModWall>(argument2[1]).Type;
                }

                if (potential != 0)
                {
                    wall2 = (ushort)potential;
                }
                else
                {
                    Main.NewText(string.Concat("Error! Unrecognised wall \"", argument2[1], "\"", " from mod \"", argument2[0], "\""), Colors.RarityRed);
                    toBreak = true;
                }

            }

            if (!toBreak)
            {
                /*if (Main.netMode == 1)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((int)TBuild.MessageType.replaceWallAction);
                    packet.Write(wall1);
                    packet.Write(wall2);
                    packet.Send();
                }
                else if (Main.netMode == 0)
                {*/
                System.Threading.Tasks.Task.Run(() => { TBuildWorld.ReplaceWallOperation(wall1, wall2); });
                //}
            }
        }
    }

    public class TilePos : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "tilepos"; }
        }

        public override string Description
        {
            get { return "Gets position of tile at cursor"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (Main.netMode != 2)
            {
                Main.NewText(string.Concat("Cursor tile position: ", Player.tileTargetX, ", ", Player.tileTargetY), Colors.RarityPink);
            }
        }
    }
    public class SetMarkerPos : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "setmarkerpos"; }
        }

        public override string Description
        {
            get { return "Sets the position of either boundary marker with a coordinate"; }
        }

        public override string Usage => "Sets the position of boundary marker <argument 1>, to x value of <argument2>, y value of <argument3>, where a value of 1 for <argument1> refers to the top left marker, a value of 2 refers to the bottom right marker. Argument 2 and 3 must be more than or equal 0, and less than the world size.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (Main.netMode != 2)
            {
                if (args.Length != 3)
                {
                    Main.NewText("Error! Must have 3 arguments.", Colors.RarityOrange);
                    return;
                }
                uint marker = 0;
                uint x = 0;
                uint y = 0;
                if (!uint.TryParse(args[0], out marker) || (marker < 1 || marker > 2))
                {
                    Main.NewText("Error! Unknown marker: " + args[0], Colors.RarityOrange);
                    return;
                }
                if (!uint.TryParse(args[1], out x) || (x > Main.maxTilesX))
                {
                    Main.NewText("Error! Invalid x coordinate: " + args[1], Colors.RarityOrange);
                    return;
                }
                if (!uint.TryParse(args[2], out y) || (y > Main.maxTilesY))
                {
                    Main.NewText("Error! Invalid y coordinate: " + args[2], Colors.RarityOrange);
                    return;
                }

                if (marker == 1)
                {
                    if (TBuildWorld.markerPos2 != default(Point) && (x > TBuildWorld.markerPos2.X || y > TBuildWorld.markerPos2.Y))
                    {
                        Main.NewText("Error! Coordinates of boundary marker 1 cannot be greater than that of boundary marker 2!", Colors.RarityOrange);
                        return;
                    }
                    TBuildWorld.markerPos1 = new Point((int)x, (int)y);
                }
                else if (marker == 2)
                {
                    if ((x < TBuildWorld.markerPos1.X || y < TBuildWorld.markerPos1.Y))
                    {
                        Main.NewText("Error! Coordinates of boundary marker 2 cannot be less than that of boundary marker 1!", Colors.RarityOrange);
                        return;
                    }
                    TBuildWorld.markerPos2 = new Point((int)x, (int)y);
                }
            }
        }
    }
    public class QuitWithoutSaving : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "q"; }
        }

        public override string Description
        {
            get { return "quits game without saving world"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (Main.netMode == 0)
                WorldFile.CacheSaveTime();
            Main.invasionProgress = 0;
            Main.invasionProgressDisplayLeft = 0;
            Main.invasionProgressAlpha = 0.0f;
            Main.menuMode = 10;
            Main.gameMenu = true;
            Main.ActivePlayerFileData.StopPlayTimer();
            Player.SavePlayer(Main.ActivePlayerFileData, false);
            if (Main.netMode == 0)
            {
                //WorldFile.saveWorld();
            }
            else
            {
                Netplay.Disconnect = true;
                Main.netMode = 0;
            }
            Main.fastForwardTime = false;
            Main.menuMode = 0;
        }
    }
    public class LoadTest : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "LoadTest"; }
        }

        public override string Description
        {
            get { return "Load a test setup"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            TBuildLoadOperations.LoadFromExternalFile(new Point(Player.tileTargetX, Player.tileTargetY), Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/TBuild.tbx");//LoadHitmapFromExternalFile(mod, Point.Zero, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/TBuild.tbm");
            //TBuildGlobalTile.activeHitmaps.Add(map);
        }
    }
}