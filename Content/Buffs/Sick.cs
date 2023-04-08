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
            DisplayName.SetDefault("Sickness");
            Description.SetDefault("Defense, attack, and movement speed reduced.");
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.direction == 1)
            {
                for (int d = 0; d < 20; d++)
                {
                    Dust.NewDust(new Vector2(player.position.X + 10, player.position.Y + 10), player.width, 4, DustID.GreenBlood, 7.5f, 0f, 150, default(Color), 1.5f);
                }
                for (int d = 0; d < 10; d++)
                {
                    Dust.NewDust(new Vector2(player.position.X + 10, player.position.Y + 10), player.width, 4, DustID.Confetti_Yellow, 10f, 0f, 150, default(Color), 1.25f);
                }
            }
            if (player.direction == -1)
            {
                for (int d = 0; d < 20; d++)
                {
                    Dust.NewDust(new Vector2(player.position.X - 10, player.position.Y + 10), player.width, 4, DustID.GreenBlood, -7.5f, 0f, 150, default(Color), 1.5f);
                }
                for (int d = 0; d < 10; d++)
                {
                    Dust.NewDust(new Vector2(player.position.X - 10, player.position.Y + 10), player.width, 4, DustID.Confetti_Yellow, -10f, 0f, 150, default(Color), 1.25f);
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
