using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WraithMod.Content.Buffs
{
    public class BloodSickness : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Sickness");
            // Description.SetDefault("If you drink more blood you will be sick.");
            Main.debuff[Type] = true;
        }
    }
}
