using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBuild.Items
{
    public class WorldSelector : ModItem
    {
        public override void SetDefaults()
        {

            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 1;

            Item.value = 0;
            Item.rare = 2;
            Item.useStyle = 1;
            Item.useTime = 20;
            Item.useAnimation = 20; ;
            Item.consumable = false;
            Item.autoReuse = false;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("World Selector Tool");
            Tooltip.SetDefault("Select the whole world!");
        }

        public override bool? UseItem(Player player)
        {
            TBuildWorld.markerPos1 = Point.Zero;
            TBuildWorld.markerPos2 = new Point(Main.maxTilesX - 1, Main.maxTilesY - 1);
            Main.NewText("Selection Update");
            return true;
        }
    }
}