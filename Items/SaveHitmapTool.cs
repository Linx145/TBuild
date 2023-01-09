using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Threading;

namespace TBuild.Items
{
    public class SaveHitmapTool : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.maxStack = 1;
            Item.value = 0;
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.consumable = false;
            Item.autoReuse = false;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Save Hitmap Tool");
            Tooltip.SetDefault("Saves the selection as a hitmap, if any.\nFile will be saved as whatever you set in /filename, and by default saves on the desktop");
        }

        public void DoHitmapSave(object obj)
        {
            TBuildWorld.SaveHitmap();
        }
        public override bool? UseItem(Player player)
        {
            if (!TBuildWorld.isSaving)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoHitmapSave));
                return true;//TBuildWorld.SaveSelection();
            }
            else return null;
        }
    }
}
