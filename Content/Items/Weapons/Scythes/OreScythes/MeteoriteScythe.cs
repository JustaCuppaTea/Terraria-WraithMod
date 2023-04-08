using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using System.Security.Cryptography.X509Certificates;
using WraithMod.Content.Class;
using WraithMod.Content.BaseClasses;
using Terraria.Audio;
using WraithMod.Content.Items.Misc;

namespace WraithMod.Content.Items.Weapons.Scythes.OreScythes
{
    /// <summary>
    ///     Star Wrath/Starfury style weapon. Spawn projectiles from sky that aim towards mouse.
    ///     See Source code for Star Wrath projectile to see how it passes through tiles.
    ///     For a detailed sword guide see <see cref="ExampleSword" />
    /// </summary>
    public class MeteoriteScythe : BaseScythe
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Causes meteorites to fall from the sky. Right clicking causes you to call a massive asteroid from the sky with a cooldown.\nLifecost " + LifeCost);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Player player = Main.LocalPlayer;
            BaseLifeCost = 16;
            LifeCost = 16;
            Item.width = 62;
            Item.height = 62;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<WraithClass>();
            Item.damage = 42;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<Meteorite>(); // ID of the projectiles the sword will shoot
            Item.shootSpeed = 20f; // Speed of the projectiles the sword will shoot

            // If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
            // Item.attackSpeedOnlyAffectsWeaponAnimation = true;
            if (player.altFunctionUse == 2)
            {
                SoundStyle AsteroidWoosh = new SoundStyle("WraithMod/Assets/Sounds/Projectiles/AsteroidFly");
                SoundEngine.PlaySound(AsteroidWoosh with
                {
                    Volume = 2f,
                    PitchVariance = 0.1f,
                });
            }
        }
        // This method gets called when firing your weapon/sword.

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool Cooldown = player.HasBuff<AsteroidCooldown>();
            if (player.altFunctionUse == 2 && Cooldown == false)
            {
                SoundStyle Asteroid = new SoundStyle("WraithMod/Assets/Sounds/Projectiles/AsteroidBoom");
                SoundEngine.PlaySound(Asteroid with
                {
                    Volume = 2f,
                    PitchVariance = 0.1f,
                });
                player.AddBuff(ModContent.BuffType<AsteroidCooldown>(), 480);
                Item.shootSpeed += 4f;
                Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                float ceilingLimit = target.Y;
                if (ceilingLimit > player.Center.Y - 200f)
                {
                    ceilingLimit = player.Center.Y - 200f;
                }
                // Loop these functions 3 times.
                position = player.Center - new Vector2(Main.rand.NextFloat(401) * player.direction, 600f);
                position.Y -= 100 * 1;
                Vector2 heading = target - position;

                if (heading.Y < 0f)
                {
                    heading.Y *= -1f;
                }

                if (heading.Y < 20f)
                {
                    heading.Y = 20f;
                }

                heading.Normalize();
                heading *= velocity.Length();
                heading.Y += Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(source, position, heading, ModContent.ProjectileType<Asteroid>(), damage * 4, knockback, player.whoAmI, 0f, ceilingLimit);
            }
            else if (player.altFunctionUse != 2)
            {
                Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                float ceilingLimit = target.Y;
                if (ceilingLimit > player.Center.Y - 200f)
                {
                    ceilingLimit = player.Center.Y - 200f;
                }
                // Loop these functions 3 times.
                for (int i = 0; i < 3; i++)
                {
                    position = player.Center - new Vector2(Main.rand.NextFloat(401) * player.direction, 600f);
                    position.Y -= 100 * i;
                    Vector2 heading = target - position;

                    if (heading.Y < 0f)
                    {
                        heading.Y *= -1f;
                    }

                    if (heading.Y < 20f)
                    {
                        heading.Y = 20f;
                    }

                    heading.Normalize();
                    heading *= velocity.Length();
                    heading.Y += Main.rand.Next(-40, 41) * 0.02f;
                    Projectile.NewProjectile(source, position, heading, type, damage, knockback, player.whoAmI, 0f, ceilingLimit);
                }
            }

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Obsidian, 35)
                .AddIngredient(ItemID.MeteoriteBar, 20)
                .AddIngredient(ModContent.ItemType<DarkDust>(), 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class Meteorite : ModProjectile
    {
        public int Once;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = true;
            Projectile.Size = new Vector2(50, 50);
            Projectile.penetrate = 1;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // projectile sprite faces up
            Dust Smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0, 0, 10, default(Color), 3.5f);
            Smoke.noGravity = true;
            Dust Fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 10, default(Color), 3.5f);
            Fire.noGravity = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Once++;
            if (Once <= 1)
            {
                for (int d = 0; d < 10; d++)
                {
                    Dust SmokeBoom = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y, 10, default(Color), 3.5f);
                    SmokeBoom.noGravity = true;
                    Dust FireBoom = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 10, default(Color), 3.5f);
                    FireBoom.noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                Projectile.velocity = new Vector2(0f, 0f);
                Projectile.timeLeft = 3;
                Projectile.penetrate = -1;
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.Resize(100, 100);
            }
            return false;

        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Once++;
            if (Once <= 1)
            {
                for (int d = 0; d < 10; d++)
                {
                    Dust SmokeBoom = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y, 10, default(Color), 3.5f);
                    SmokeBoom.noGravity = true;
                    Dust FireBoom = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 10, default(Color), 3.5f);
                    FireBoom.noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                Projectile.velocity = new Vector2(0f, 0f);
                Projectile.timeLeft = 3;
                Projectile.penetrate = -1;
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.Resize(100, 100);
            }
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
    public class Asteroid : ModProjectile
    {
        public int Once2;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = true;
            Projectile.Size = new Vector2(96, 96);
            Projectile.penetrate = 1;
            Projectile.aiStyle = 0;
            Projectile.scale = 2;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // projectile sprite faces up
            Dust Smoke2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0, 0, 10, default(Color), 4.5f);
            Smoke2.noGravity = true;
            Dust Fire2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 10, default(Color), 4.5f);
            Fire2.noGravity = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.LocalPlayer;
            Once2++;
            if (Once2 <= 1)
            {
                for (int d = 0; d < 30; d++)
                {
                    Dust SmokeBoom2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y, 10, default(Color), 4.5f);
                    SmokeBoom2.noGravity = true;
                    Dust FireBoom2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 10, default(Color), 4.5f);
                    FireBoom2.noGravity = true;
                }
                player.GetModPlayer<ShakeScreen>().ScreenShakeTimer = 30;
                SoundEngine.PlaySound(SoundID.Item14 with { Volume = 2.5f });
                Projectile.velocity = new Vector2(0f, 0f);
                Projectile.timeLeft = 3;
                Projectile.penetrate = -1;
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.Resize(384, 384);
            }
            return false;

        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.LocalPlayer;
            Once2++;
            if (Once2 <= 1)
            {
                target.AddBuff(BuffID.OnFire3, 180);
                for (int d = 0; d < 30; d++)
                {
                    Dust SmokeBoom2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y, 10, default(Color), 4.5f);
                    SmokeBoom2.noGravity = true;
                    Dust FireBoom2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 10, default(Color), 4.5f);
                    FireBoom2.noGravity = true;
                }
                player.GetModPlayer<ShakeScreen>().ScreenShakeTimer = 30;
                SoundEngine.PlaySound(SoundID.Item14 with { Volume = 2.5f});
                Projectile.velocity = new Vector2(0f, 0f);
                Projectile.timeLeft = 3;
                Projectile.penetrate = -1;
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.Resize(384, 384);
            }
        }
    }
    public class AsteroidCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asteroid Cooldown");
            Description.SetDefault("You cannot call down any asteroids yet.");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
    public class ShakeScreen : ModPlayer
    {
        public int ScreenShakeTimer;
        public override void ModifyScreenPosition()
        {
            if (ScreenShakeTimer > 0)
            {
                Main.screenPosition += new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                ScreenShakeTimer--;
            }
        }
    }
}