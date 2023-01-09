/*using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TBuild
{
    public class TBuildGlobalTile  : GlobalTile
    {
        //Todo: Make sure cankilltile works, test, implement loading and unloading in pinkymod-side, make a new variable called name for tilehitmap so as to allow easy iteration, maybe make it a tagcompound otherwise idk
        //public static List<TileHitmap> activeHitmaps = new List<TileHitmap>();
        public static void removeHitmap(string byName)
        {
            activeHitmaps.RemoveAll(new Predicate<TileHitmap>((TileHitmap map) => { return map.name == byName; }));
        }
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            for (int c = 0; c < activeHitmaps.Count; c++)
            {
                if (activeHitmaps[c].HasTile(i, j))
                {
                    return false;
                }
            }
            return true;
        }
        public override bool CanExplode(int i, int j, int type)
        {
            for (int c = 0; c < activeHitmaps.Count; c++)
            {
                if (activeHitmaps[c].HasTile(i, j))
                {
                    return false;
                }
            }
            return true;
        }
        
    }
}*/