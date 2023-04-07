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
    public class Rage : ModBuff
    {
        public static bool Active;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rage");
            // Description.SetDefault("Defense decreased by 10% however, damage is increased by 10%.");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) *= 1.1f;
            Active = true;
        }
    }
    public class DefensePlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            if (Rage.Active)
            {
                Player.statDefense -= (int)(Player.statDefense * 0.1f);
            }
        }
        public override void ResetEffects()
        {
            Rage.Active = false;
        }
    }
}
