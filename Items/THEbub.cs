using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBuild.Items
{
    public class THEbub : ModItem
    {
        public override void SetDefaults()
        {
            Item.width =28;
            Item.height = 28;
            Item.maxStack = 1;

            Item.value = 0;
            Item.rare = 5;
            Item.useStyle = 1;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.consumable = false;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("THE bub");
            Tooltip.SetDefault("A bubble that allows you to travel the chaotic greater dimensions without your brains being eaten by daemons. Instantly.\nTeleport to your mouse cursor without any repercussions! Building mode only");
        }

        public override bool? UseItem(Player player)
        {
            if (ModContent.GetInstance<TBuildConfig>().editorMode)
            {
                if (Main.netMode!= 2)
                {
                    player.Teleport(Main.MouseWorld);
                }
            }
            return true;
        }
    }
}
