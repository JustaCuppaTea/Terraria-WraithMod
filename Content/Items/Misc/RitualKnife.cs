using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using WraithMod.Content.Class;
using Terraria.DataStructures;
using Terraria.Audio;
using WraithMod.Content.Tiles.Furniture;

namespace WraithMod.Content.Items.Misc
{
    public class RitualKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Deals damage to you and if you have a bottle in your inventory, fills the bottle with blood, if you have no bottle in your inventory, you lose blood with no reward."); // The (English) text shown below your weapon's name.

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42; // The item texture's width.
            Item.height = 12; // The item texture's height.

            Item.useStyle = ItemUseStyleID.EatFood; // The useStyle of the Item.
            Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
            Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
            Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

            Item.DamageType = ModContent.GetInstance<SacrificeClass>(); // Whether your item is part of the melee class.
            Item.damage = 25; // The damage your item deals.
            Item.knockBack = 6; // The force of knockback of the weapon. Maximum is 20
            Item.crit = 6; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

            Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
            Item.rare = ItemRarityID.Red; // Give this item our custom rarity.
            Item.UseSound = SoundID.Item1; // The sound when the weapon is being used.
        }
        public override bool? UseItem(Player player)
        {
            string DeathMessage = "";
            int decider = Main.rand.Next(1);
            if (decider == 0)
            {
                DeathMessage = " sacrificed themselves.";
            }
            else if (decider == 1)
            {
                DeathMessage = " gave their lives to the gods.";
            }
            player.Hurt(PlayerDeathReason.ByCustomReason(player.name + DeathMessage), Item.damage, 0, false, true, false, -1);
            player.immuneTime = 0;
            bool hasBottle = player.HasItem(31);
            if (hasBottle)
            {
                player.ConsumeItem(31, false);
                Item.NewItem(Item.GetSource_FromThis(), player.position, ModContent.ItemType<BloodContained>(), 1, false, 0, false, false);
            }
            if (!hasBottle)
            {
                if (player.direction == 1)
                {
                    for (int d = 0; d < 50; d++)
                    {
                        Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 15, 0, 10, default(Color), 1.75f);
                    }
                }
                if (player.direction == 2)
                {
                    for (int d = 0; d < 50; d++)
                    {
                        Dust.NewDust(player.position, player.width, player.height, DustID.Blood, -15, 0, 10, default(Color), 1.75f);
                    }
                }
                SoundEngine.PlaySound(SoundID.NPCDeath22 with { Volume = 1.5f });
            }
            return true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                // Emit dusts when the sword is swung
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.RedTorch);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            // Inflict the OnFire debuff for 1 second onto any NPC/Monster that this hits.
            // 60 frames = 1 second
            target.AddBuff(BuffID.ShadowFlame, 60);
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}