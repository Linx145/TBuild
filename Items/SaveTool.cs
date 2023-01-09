using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Threading.Tasks;

namespace TBuild.Items
{
    public class SaveTool : ModItem
    {
        public override void SetDefaults()
        {

            Item.width = 20;
            Item.height = 22;
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
            DisplayName.SetDefault("Save Tool");
            Tooltip.SetDefault("Saves the selection, if any.\nFile will be saved as whatever you set in /filename, and by default saves on the desktop");
        }

        public override bool? UseItem(Player player)
        {
            if (!TBuildWorld.isSaving)
            {
                Task.Run(TBuildSaveOperations.SaveSelection);
            }
            return true;
        }
    }
}
