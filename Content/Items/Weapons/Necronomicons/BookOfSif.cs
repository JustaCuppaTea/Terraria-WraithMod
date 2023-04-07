using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WraithMod.Content.BaseClasses;
using WraithMod.Content.Class;
using WraithMod.Content.Items.Weapons.Scythes.OreScythes.OreSummonScythes;
using WraithMod.Content.Tiles.Furniture;

namespace WraithMod.Content.Items.Weapons.Necronomicons
{
    public class BookOfSif : BaseScythe
    {
        public static int TimeUse;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Book of Sif I");
            // Tooltip.SetDefault("Summons a necronomicon of Sif that summons the power of nature to aid you, shooting dayblooms and other herbs at your mouse.\nBlood Sacrifice " + LifeCost);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            BaseLifeCost = 30;
            LifeCost = 30;
            Item.width = 62;
            Item.height = 62;

            Item.useStyle = ItemUseStyleID.MowTheLawn;
            Item.useTime = 40;
            TimeUse = Item.useTime;
            Item.noUseGraphic = true;
            Item.useAnimation = 40;
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<WraithClass>();
            Item.damage = 9;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<BookOfSifProj>(); // ID of the projectiles the sword will shoot
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
            player.Hurt(PlayerDeathReason.ByCustomReason(player.name + DeathMessage), LifeCost, 0, false, true, -1, false, 1000, 2, 0);
            player.immuneTime = 0;
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 35)
                .AddTile(ModContent.TileType<RitualTable>())
                .Register();
        }
        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
        }
    }

    public class BookOfSifProj : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(64, 36);
            Projectile.penetrate = -1;
            Projectile.aiStyle = 0;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int d = 0; d < 50; d++)
            {
                Dust yellow = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, 0f, 150, default(Color), 1.5f);
                yellow.noGravity = true;
            }
            for (int d = 0; d < 50; d++)
            {
                Dust green = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, 0f, 150, default(Color), 1.5f);
                green.noGravity = true;
            }
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.velocity = Vector2.Zero;
            if (player.direction == 1)
            {
                Projectile.spriteDirection = 1;
                Projectile.position = new Vector2(player.position.X + 20, player.position.Y);
            }
            else if (player.direction == -1)
            {
                Projectile.spriteDirection = -1;
                Projectile.position = new Vector2(player.position.X - 60, player.position.Y);
            }
            if (player.HeldItem.type != ModContent.ItemType<BookOfSif>())
            {
                Projectile.Kill();
            }
            if (player.HeldItem.type == ModContent.ItemType<BookOfSif>())
            {
                Projectile.timeLeft += 10;
            }
            Timer++;
            if (Timer > BookOfSif.TimeUse)
            {
                // Our timer has finished, do something here:
                // SoundEngine.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.
                for (int d = 0; d < 10; d++)
                {
                    Dust yellow = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, 0f, 150, default(Color), 1.5f);
                    yellow.noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.MaxMana);
                int type2 = Main.rand.Next(new int[] { ModContent.ProjectileType<Blinkroot>(), ModContent.ProjectileType<Daybloom>(), ModContent.ProjectileType<Deathweed>(), ModContent.ProjectileType<Fireblossom>(), ModContent.ProjectileType<Moonglow>(), ModContent.ProjectileType<Shiverthorn>(), ModContent.ProjectileType<Waterleaf>()});
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, type2, Projectile.damage, Projectile.knockBack, Projectile.owner);
                Timer = 0;
            }
            Dust green2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, 0f, 150, default(Color), 1.5f);
            green2.noGravity = true;
        }
        // Some advanced drawing because the texture image isn't centered or symetrical
        // If you dont want to manually drawing you can use vanilla projectile rendering offsets
        // Here you can check it https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#horizontal-sprite-example
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            float offsetX = 20f;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            // If sprite is vertical
            // float offsetY = 20f;
            // origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);


            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
    public class Daybloom : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(12, 16);
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            Projectile.rotation += 0.4f * (float)Projectile.direction;
            if (Projectile.owner == Main.myPlayer)
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 12f;
            Player player = Main.player[Projectile.owner];
            Dust daybloom = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0, 0, 50, default(Color), 1.5f);
            daybloom.noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor) // holy moly the bloom effect actually works
        {
            Asset<Texture2D> bloomTex = ModContent.Request<Texture2D>("WraithMod/Content/Items/Weapons/Necronomicons/DayBloomGlow");
            Color color = Color.Lerp(Color.White, Color.Yellow, 20f);
            var bloom = color;
            bloom.A = 0;
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.75f, 0.75f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, speed * 4f, Scale: 1.5f);
                d.noGravity = true;
                Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                Dust e = Dust.NewDustPerfect(Projectile.Center, DustID.GreenFairy, speed2 * 4f, Scale: 1.5f);
                e.noGravity = true;
            }
        }
    }
    public class Blinkroot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(16, 20);
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            Projectile.rotation += 0.3f * (float)Projectile.direction;
            if (Projectile.owner == Main.myPlayer)
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 7f;
            Player player = Main.player[Projectile.owner];
            Dust blinkroot = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DesertTorch, 0, 0, 50, default(Color), 1.5f);
            blinkroot.noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor) // holy moly the bloom effect actually works
        {
            Asset<Texture2D> bloomTex = ModContent.Request<Texture2D>("WraithMod/Content/Items/Weapons/Necronomicons/BlinkrootGlow");
            Color color = Color.Lerp(Color.White, Color.Red, 20f);
            var bloom = color;
            bloom.A = 0;
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.75f, 0.75f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.OrangeTorch, speed * 4f, Scale: 2.5f);
                d.noGravity = true;
                Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                Dust e = Dust.NewDustPerfect(Projectile.Center, DustID.GreenBlood, speed2 * 4f, Scale: 1.5f);
                e.noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int d = 0; d < 10; d++)
            {
                Dust blinkroot = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DesertTorch, 0, 0, 50, default(Color), 1.5f);
                blinkroot.noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 1.3f;
        }
    }
    public class Deathweed : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(16, 18);
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            Projectile.rotation += 0.5f * (float)Projectile.direction;
            if (Projectile.owner == Main.myPlayer)
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 16f;
            Player player = Main.player[Projectile.owner];
            Dust deathweed = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptTorch, 0, 0, 50, default(Color), 1.5f);
            deathweed.noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor) // holy moly the bloom effect actually works
        {
            Asset<Texture2D> bloomTex = ModContent.Request<Texture2D>("WraithMod/Content/Items/Weapons/Necronomicons/DeathweedGlow");
            Color color = Color.Lerp(Color.Purple, Color.DarkRed, 20f);
            Color color2 = Color.Lerp(color, Color.White, 20f);
            var bloom = color2;
            bloom.A = 0;
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.75f, 0.75f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Blood, speed * 4f, Scale: 1.5f);
                d.noGravity = true;
                Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                Dust e = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, speed2 * 4f, Scale: 2.5f);
                e.noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int d = 0; d < 5; d++)
            {
                Dust blood = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 50, default(Color), 2f);
                blood.noGravity = true;
            }
            for (int d = 0; d < 2; d++)
            {
                Dust deathweed = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptTorch, 0, 0, 50, default(Color), 1.5f);
                deathweed.noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.8f;
        }
    }
    public class Fireblossom : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(12, 20);
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            Projectile.rotation += 0.5f * (float)Projectile.direction;
            if (Projectile.owner == Main.myPlayer)
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 13f;
            Player player = Main.player[Projectile.owner];
            Dust firebloom = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 50, default(Color), 1.5f);
            firebloom.noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor) // holy moly the bloom effect actually works
        {
            Asset<Texture2D> bloomTex = ModContent.Request<Texture2D>("WraithMod/Content/Items/Weapons/Necronomicons/FireblossomGlow");
            Color color = Color.Lerp(Color.Orange, Color.OrangeRed, 20f);
            Color color2 = Color.Lerp(color, Color.White, 20f);
            var bloom = color2;
            bloom.A = 10;
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.75f, 0.75f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.OrangeTorch, speed * 4f, Scale: 2.5f);
                d.noGravity = true;
                Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                Dust e = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed2 * 4f, Scale: 2.5f);
                e.noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 150);
            for (int d = 0; d < 7; d++)
            {
                Dust ash = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Asphalt, 0, 0, 50, default(Color), 2f);
                ash.noGravity = true;
            }
            for (int d = 0; d < 3; d++)
            {
                Dust firebloom = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 50, default(Color), 1.5f);
                firebloom.noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.6f;
        }
    }
    public class Moonglow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(12, 20);
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            Projectile.rotation += 0.4f * (float)Projectile.direction;
            if (Projectile.owner == Main.myPlayer)
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 10f;
            Player player = Main.player[Projectile.owner];
            Dust moonglow = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0, 0, 50, default(Color), 1.5f);
            moonglow.noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor) // holy moly the bloom effect actually works
        {
            Asset<Texture2D> bloomTex = ModContent.Request<Texture2D>("WraithMod/Content/Items/Weapons/Necronomicons/MoonglowGlow");
            Color color = Color.Lerp(Color.White, Color.LightSkyBlue, 20f);
            var bloom = color;
            bloom.A = 10;
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.75f, 0.75f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueCrystalShard, speed * 4f, Scale: 1.5f);
                d.noGravity = true;
                Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                Dust e = Dust.NewDustPerfect(Projectile.Center, DustID.GreenFairy, speed2 * 4f, Scale: 1.5f);
                e.noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int d = 0; d < 10; d++)
            {
                Dust moonglow = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0, 0, 50, default(Color), 1.5f);
                moonglow.noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 1.1f;
        }
    }
    public class Shiverthorn : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(16, 18);
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            Projectile.rotation += 0.5f * (float)Projectile.direction;
            if (Projectile.owner == Main.myPlayer)
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 13f;
            Player player = Main.player[Projectile.owner];
            Dust shiverthorn = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0, 0, 50, default(Color), 1.5f);
            shiverthorn.noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor) // holy moly the bloom effect actually works
        {
            Asset<Texture2D> bloomTex = ModContent.Request<Texture2D>("WraithMod/Content/Items/Weapons/Necronomicons/ShiverthornGlow");
            Color color = Color.Lerp(Color.LightBlue, Color.CadetBlue, 20f);
            Color color2 = Color.Lerp(color, Color.White, 20f);
            var bloom = color2;
            bloom.A = 10;
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 150);
            for (int d = 0; d < 7; d++)
            {
                Dust ice = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, 0, 0, 50, default(Color), 2f);
                ice.noGravity = true;
            }
            for (int d = 0; d < 3; d++)
            {
                Dust firebloom = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0, 0, 50, default(Color), 1.5f);
                firebloom.noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.75f, 0.75f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueFlare, speed * 4f, Scale: 1.5f);
                d.noGravity = true;
                Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                Dust e = Dust.NewDustPerfect(Projectile.Center, DustID.WhiteTorch, speed2 * 4f, Scale: 2.5f);
                e.noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.6f;
        }
    }
    public class Waterleaf : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(16, 16);
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = 0;
        }
        public override void AI()
        {
            Projectile.rotation += 0.5f * (float)Projectile.direction;
            if (Projectile.owner == Main.myPlayer)
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 13f;
            Player player = Main.player[Projectile.owner];
            Dust waterleaf = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0, 0, 50, default(Color), 1.5f);
            waterleaf.noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor) // holy moly the bloom effect actually works
        {
            Asset<Texture2D> bloomTex = ModContent.Request<Texture2D>("WraithMod/Content/Items/Weapons/Necronomicons/WaterleafGlow");
            Color color = Color.Lerp(Color.White, Color.Aquamarine, 20f);
            var bloom = color;
            bloom.A = 10;
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.75f, 0.75f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Water, speed * 4f, Scale: 1.5f);
                d.noGravity = true;
                Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                Dust e = Dust.NewDustPerfect(Projectile.Center, DustID.GreenFairy, speed2 * 4f, Scale: 1.5f);
                e.noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int d = 0; d < 7; d++)
            {
                Dust waterleaf = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0, 0, 50, default(Color), 2f);
                waterleaf.noGravity = true;
            }
            for (int d = 0; d < 3; d++)
            {
                Dust sand = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, 0, 0, 50, default(Color), 1.5f);
                sand.noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.9f;
        }
    }
}
