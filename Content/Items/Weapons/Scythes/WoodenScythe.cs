using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using System.Security.Cryptography.X509Certificates;
using WraithMod.Content.Class;
using WraithMod.Content.BaseClasses;

namespace WraithMod.Content.Items.Weapons.Scythes
{
    /// <summary>
    ///     Star Wrath/Starfury style weapon. Spawn projectiles from sky that aim towards mouse.
    ///     See Source code for Star Wrath projectile to see how it passes through tiles.
    ///     For a detailed sword guide see <see cref="ExampleSword" />
    /// </summary>
    public class WoodenScythe : BaseScythe
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Shoots out a spinning scythe\nLifecost " + LifeCost);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            BaseLifeCost = 10;
            LifeCost = 10;
            Item.width = 62;
            Item.height = 62;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<WraithClass>();
            Item.damage = 9;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<PurityStick>(); // ID of the projectiles the sword will shoot
            Item.shootSpeed = 8f; // Speed of the projectiles the sword will shoot

            // If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
            // Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }
        // This method gets called when firing your weapon/sword.

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override bool? UseItem(Player player)
        {
            string DeathMessage = "";
            int decider = Main.rand.Next(4);
            if (decider == 0 )
            {
                DeathMessage = " was consumed by darkness.";
            } else if (decider == 1 ) 
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
            player.Hurt(PlayerDeathReason.ByCustomReason(player.name + DeathMessage), LifeCost, 0, false, true, -1, false, 1000, 2, 0);
            player.immuneTime = 0;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 35)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class PurityStick : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(30, 80);
            Projectile.penetrate = 4;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            
            Projectile.rotation += 0.25f * (float)Projectile.direction;

            // Here, you can put the logic for the projectile's movement and behavior

            // Create dust particles
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(50), default(Color), Main.rand.NextFloat(4));
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(50), default(Color), Main.rand.NextFloat(1.5f));// Creates fire dust particles at the projectile's position

            // Change the color of the projectile
            Lighting.AddLight(Projectile.position, 0.5f, 1f, 0.5f); // Adds a subtle light effect to the projectile
            Lighting.AddLight(Projectile.position, 0.5f, 1f, 0.5f); // Adds a subtle light effect to the projectile
            Projectile.alpha += 5; // Gradually increases the projectile's transparency over time
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }
    }
}