using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.Audio;
using System;
using WraithMod.Content.Class;
using WraithMod.Content.Items.Misc;
using WraithMod.Content.BaseClasses;

namespace WraithMod.Content.Items.Weapons.Scythes.OreScythes
{
    /// <summary>
    ///     Star Wrath/Starfury style weapon. Spawn projectiles from sky that aim towards mouse.
    ///     See Source code for Star Wrath projectile to see how it passes through tiles.
    ///     For a detailed sword guide see <see cref="ExampleSword" />
    /// </summary>
    public class IronScythe : BaseScythe
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Scythe");
            Tooltip.SetDefault("Sometimes creates an anvil at the position of the mouse.\nLifecost " + LifeCost);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            BaseLifeCost = 14;
            LifeCost = 14;
            Item.width = 68;
            Item.height = 68;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<WraithClass>();
            Item.damage = 32;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<AnvilProj>();
            Item.shootSpeed = 1f; // Speed of the projectiles the sword will shoot

            // If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
            // Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }
        // This method gets called when firing your weapon/sword.

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override bool? UseItem(Player player)
        {
            string DeathMessage = "";
            int decider = Main.rand.Next(4);
            if (decider == 0)
            {
                DeathMessage = " was consumed by darkness.";
            }
            else if (decider == 1)
            {
                DeathMessage = " couldn't handle the power.";
            }
            else if (decider == 2)
            {
                DeathMessage = " didn't watch their health bar.";
            }
            else if (decider == 3)
            {
                DeathMessage = " forgot to heal.";
            }
            player.Hurt(PlayerDeathReason.ByCustomReason(player.name + DeathMessage), LifeCost, 0, false, true, false, -1);
            player.immuneTime = 0;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 13)
                .AddIngredient(ItemID.IronBar, 16)
                .AddIngredient(ModContent.ItemType<DarkDust>(), 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(Item.GetSource_FromThis(), Main.MouseWorld, new Vector2(0, 0), ModContent.ProjectileType<AnvilProj>(), damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class AnvilProj : ModProjectile
    {
        int SpawnDust = 0;
        public static bool collide = false;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = true;
            Projectile.Size = new Vector2(86, 42);
            Projectile.penetrate = -1;
            Projectile.aiStyle = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnDust++;
            if (SpawnDust <= 1)
            {
                int decide = Main.rand.Next(2);
                collide = true;
                SoundStyle Anvil1 = new SoundStyle("WraithMod/Assets/Sounds/Projectiles/Anvil1");
                SoundStyle Anvil2 = new SoundStyle("WraithMod/Assets/Sounds/Projectiles/Anvil2");
                SoundStyle Anvil3 = new SoundStyle("WraithMod/Assets/Sounds/Projectiles/Anvil3");
                if (decide == 0)
                {
                    SoundEngine.PlaySound(Anvil1 with
                    {
                        Volume = 2f,
                        Pitch = 0.3f,
                        PitchVariance = 0.1f,
                    });
                }
                else if (decide == 1)
                {
                    SoundEngine.PlaySound(Anvil2 with
                    {
                        Volume = 2f,
                        Pitch = 0.4f,
                        PitchVariance = 0.1f,
                    });
                }
                else
                {
                    SoundEngine.PlaySound(Anvil3 with
                    {
                        Volume = 2f,
                        Pitch = 0.3f,
                        PitchVariance = 0.1f,
                    });
                }
                for (int d = 0; d < 10; d++)
                {
                    float Xpos = Main.rand.NextFloat(5);
                    float NegativeXpos = Xpos - Main.rand.NextFloat(5);
                    Dust.NewDust(Projectile.position, Projectile.width + 15, Projectile.height, DustID.YellowTorch, NegativeXpos, -5, Main.rand.Next(40), default, 1.75f);
                }
            }
            return false;
        }
        public override void Kill(int timeLeft)
        {
            SpawnDust = 0;
            collide = false;
        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {

            if (collide)
            {
                Timer++;
                if (Timer <= 30)
                {
                    // Our timer has finished, do something here:
                    // SoundEngine.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.
                    Projectile.Kill();
                    collide = false;
                    Timer = 0;
                }
            }
            Projectile.velocity.Y = Projectile.velocity.Y + 0.45f; // 0.1f for arrow gravity, 0.4f for knife gravity
            if (Projectile.velocity.Y > 32f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
            {
                Projectile.velocity.Y = 32f;
            }
        }
    }
    /*public class Shake : ModPlayer
    {
        public override void ModifyScreenPosition()
        {
            bool DidColllide = AnvilProj.collide;
            if (DidColllide) 
            {
                Main.screenPosition += Main.rand.NextVector2CircularEdge(3f, 3f);
            }
        }
    } */
}