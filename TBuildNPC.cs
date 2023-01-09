using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBuild
{
    public class TBuildNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (ModContent.GetInstance<TBuildConfig>().stopSpawns)
            {
                spawnRate = 0;
                maxSpawns = 0;
            }
        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (ModContent.GetInstance<TBuildConfig>().stopSpawns)
            {
                pool.Clear();
            }
        }
    }
}