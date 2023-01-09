using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBuild.Items
{
    public class TBuilder : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 0, 0);
            Item.rare = 7;
            Item.accessory = true;
            //item.defense = 3;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("TBuilder");
            Tooltip.SetDefault("All the building stat buffs you ever need, like seriously");
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.blockRange += 100;
            player.wallSpeed += 0.5f;
            player.tileSpeed += 0.5f;
            player.ZonePeaceCandle = true;
            if (!hideVisual)
            {
                player.rulerGrid = true;
                player.rulerLine = true;
            }
        }
    }
}