using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace WraithMod.Content.Buffs
{
    public class Sick : ModBuff
    {
        public static bool Active;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Sickness");
            // Description.SetDefault("If you drink more blood you will be sick.");
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.direction == 1)
            {
                for (int d = 0; d < 40; d++)
                {
                    Dust.NewDust(new Vector2(player.position.X, player.position.Y + 10), player.width, player.height, DustID.GreenBlood, 15f, 0f, 150, default(Color), 1.5f);
                    Dust.NewDust(new Vector2(player.position.X, player.position.Y + 10), player.width, player.height, DustID.Confetti_Yellow, 15f, 0f, 150, default(Color), 1.5f);
                }
            }
            if (player.direction == -1)
            {
                for (int d = 0; d < 40; d++)
                {
                    Dust.NewDust(new Vector2(player.position.X, player.position.Y + 10), player.width, player.height, DustID.GreenBlood, -15f, 0f, 150, default(Color), 1.5f);
                    Dust.NewDust(new Vector2(player.position.X, player.position.Y + 10), player.width, player.height, DustID.Confetti_Yellow, -15f, 0f, 150, default(Color), 1.5f);
                }
            }
            player.GetDamage(DamageClass.Generic) *= 0.5f; // attack decreased by 50%
            player.moveSpeed *= 0.5f;
            Active = true;
        }
    }
    public class SickPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            if (Sick.Active)
            {
                Player.statDefense -= (int)(Player.statDefense * 0.5f); // defense decreased by 50%
            }
        }
        public override void ResetEffects()
        {
            Sick.Active = false;
        }
    }

}
