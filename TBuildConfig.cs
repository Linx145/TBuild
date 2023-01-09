using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace TBuild
{
    public class TBuildConfig : ModConfig
    {
        [Label("Editor Mode")]
        [DefaultValue(false)]
        [Tooltip("When enabled, allows the building tools to work, as well as /buildtools")]
        public bool editorMode;

        [Label("Stop NPC Spawning")]
        [DefaultValue(false)]
        [Tooltip("When enabled, prevent all NPC spawns from occurring")]
        public bool stopSpawns;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            return whoAmI == 0; //only server owner can change settings
        }
    }
}
