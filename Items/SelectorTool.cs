using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBuild.Items
{
    public class SelectorTool : ModItem
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
            DisplayName.SetDefault("Selector Tool");
            Tooltip.SetDefault("Left click to set first selection position, right click to set second selection position.");
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Point pos = new Point(TBuild.toTilePos(Main.screenPosition.X + Main.mouseX), TBuild.toTilePos(Main.screenPosition.Y + Main.mouseY));

                if (TBuildWorld.markerPos2 == pos)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(pos.ToVector2() * 16, 16, 16, DustID.Torch, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default(Color), Main.rand.NextFloat(1.25f, 1.5f));
                    }
                    TBuildWorld.markerPos2 = Point.Zero;
                }
                else
                TBuildWorld.markerPos2 = pos;
            }
            else
            {
                Point pos = new Point(TBuild.toTilePos(Main.screenPosition.X + Main.mouseX), TBuild.toTilePos(Main.screenPosition.Y + Main.mouseY));

                if (TBuildWorld.markerPos1 == pos)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(pos.ToVector2() * 16, 16, 16, 74, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default(Color), Main.rand.NextFloat(1.25f, 1.5f));
                    }
                    TBuildWorld.markerPos1 = Point.Zero;
                }
                else
                    TBuildWorld.markerPos1 = pos;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
