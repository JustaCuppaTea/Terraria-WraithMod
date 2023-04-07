using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using System.Security.Cryptography.X509Certificates;
using WraithMod.Content.Class;
using WraithMod.Content.BaseClasses;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Enums;
using Terraria.GameContent.Shaders;
using Terraria.GameContent;
using IL.Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using WraithMod.Content.Items.Misc;

namespace WraithMod.Content.Items.Weapons.Scythes.OreScythes.OreSummonScythes
{
    /// <summary>
    ///     Star Wrath/Starfury style weapon. Spawn projectiles from sky that aim towards mouse.
    ///     See Source code for Star Wrath projectile to see how it passes through tiles.
    ///     For a detailed sword guide see <see cref="ExampleSword" />
    /// </summary>
    public class HellstoneScythe : BaseScythe
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Hit enemies get lit on fire, hitting enemies that are on fire deals more damage. Summons a magma eye that shoots fire beams at nearby enemies.\nLifecost " + LifeCost);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            BaseLifeCost = 11;
            LifeCost = 11;
            Item.width = 62;
            Item.height = 62;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<WraithClass>();
            Item.damage = 51;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ProjectileID.BallofFire; // ID of the projectiles the sword will shoot
            Item.shootSpeed = 8f; // Speed of the projectiles the sword will shoot

            // If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
            // Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }
        // This method gets called when firing your weapon/sword.
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 2 + Main.rand.Next(2); // 3, 4, or 5 shots
            float rotation = MathHelper.ToRadians(45);
            position += Vector2.Normalize(velocity) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .8f; // Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false; // return false to stop vanilla from calling Projectile.NewProjectile.
        }

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
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<MagmaEye>()] < 1)
            {
                Projectile shot = Projectile.NewProjectileDirect(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<MagmaEye>(), Item.damage, 0f, player.whoAmI);
                shot.originalDamage = Item.damage;
            }
            player.AddBuff(ModContent.BuffType<MagmaEyeMinionBuff>(), 18000);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Obsidian, 35)
                .AddIngredient(ItemID.HellstoneBar, 20)
                .AddIngredient(ModContent.ItemType<DarkDust>(), 3)
                .AddIngredient(ItemID.BlackLens, 1)
                .AddTile(TileID.Hellforge)
                .Register();
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            target.AddBuff(BuffID.OnFire3, 120);
            target.AddBuff(BuffID.OnFire, 240);
            if (target.HasBuff(BuffID.OnFire) || target.HasBuff(BuffID.OnFire3))
            {
                int damage2 = (int)(damage * 1.2);
                damage = damage2;
            }
        }
    }
    public class MagmaEyeMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magma Eye");
            Description.SetDefault("This magma eye will help you in your battles!");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<MagmaEye>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
    public class MagmaEye : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magma Eye Minion");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }
        public sealed override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 22;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            Projectile.hostile = false;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 0f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
        }
        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire3, 120);
            target.AddBuff(BuffID.OnFire, 240);
            if (target.HasBuff(BuffID.OnFire) || target.HasBuff(BuffID.OnFire3))
            {
                int damage2 = (int)(damage * 1.2);
                damage = damage2;
            }
        }
        public override void AI()
        {
            if (Main.rand.NextBool(6))
            {
                Dust dustBlood = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0.25f, 12, default(Color), 1f);
                dustBlood.shader = GameShaders.Armor.GetShaderFromItemId(ItemID.BloodbathDye);
                Dust dustTorch = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0.25f, 12, default(Color), 1f);
                dustTorch.shader = GameShaders.Armor.GetShaderFromItemId(ItemID.FlameDye);
            }
            if (Main.rand.NextBool(100))
            {
                Dust dustSmoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0, -3, 12, default(Color), 2f);
            }
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.type != ModContent.ItemType<HellstoneScythe>())
            {
                Projectile.Kill();
            }

            #region Active check
            // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<MagmaEyeMinionBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<MagmaEyeMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            #endregion

            #region General behavior
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 24f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (1 + Projectile.minionPos * 2) * -player.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();
            if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // Fix overlap with other minions
                Projectile other = Main.projectile[i];
                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X) Projectile.velocity.X -= overlapVelocity;
                    else Projectile.velocity.X += overlapVelocity;

                    if (Projectile.position.Y < other.position.Y) Projectile.velocity.Y -= overlapVelocity;
                    else Projectile.velocity.Y += overlapVelocity;
                }
            } 
            #endregion

            #region Find target
            // Starting search distance
            float distanceFromTarget = 100f;
            Vector2 targetCenter = Projectile.position;
            bool foundTarget = false;

            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
                // This code is required either way, used for finding a target
            }

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            #endregion

            #region Movement

            // Default movement parameters (here for attacking)
            float speed = 1f;
            float inertia = 2f;

            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > 40f)
                {
                    Vector2 target = targetCenter - Projectile.position;
                    Timer++;
                    if (Timer > 60)
                    {
                        // Our timer has finished, do something here:
                        // SoundEngine.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(target.X *= 0.1f, target.Y *= 0.1f), ModContent.ProjectileType<FireBeam>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                        Timer = 0;
                    }
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 2000f)
                    {
                        // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                        // and then set netUpdate to true
                        Projectile.position = idlePosition;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                // Minion doesn't have a target: return to player and idle
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    speed = 12f;
                    inertia = 60f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 4f;
                    inertia = 80f;
                }
                if (distanceToIdlePosition > 20f)
                {
                    // The immediate range around the player (when it passively floats about)

                    // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            #endregion

            #region Animation and visuals
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
            #endregion
        }
    }
    public class FireBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Torch);
			Vector2 vector30 = Projectile.position;
			vector30 -= Projectile.velocity * ((float)2 * 0.25f);
			Projectile.alpha = 255;
			Dust num368 = Dust.NewDustDirect(vector30, 1, 1, DustID.Torch);
            num368.noGravity = true;
			num368.position = vector30;
			num368.position.X += Projectile.width / 2;
			num368.position.Y += Projectile.height / 2;
			num368.scale = (float)Main.rand.Next(70, 110) * 0.013f;
			Dust dust2 = num368;
            dust2.noGravity = true;
			dust2.velocity *= 0.2f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // If collide with tile, reduce the penetrate.
            // So the projectile can reflect at most 5 times
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                // If the projectile hits the left or right side of the tile, reverse the X velocity
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }

            return false;
        }
    }
}