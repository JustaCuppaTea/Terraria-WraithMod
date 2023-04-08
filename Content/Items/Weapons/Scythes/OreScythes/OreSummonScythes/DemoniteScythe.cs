using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using System.Security.Cryptography.X509Certificates;
using WraithMod.Content.Class;
using System;
using Terraria.Graphics.Shaders;
using WraithMod.Content.Items.Misc;
using Terraria.Audio;
using WraithMod.Content.BaseClasses;

namespace WraithMod.Content.Items.Weapons.Scythes.OreScythes.OreSummonScythes
{
    /// <summary>
    ///     Star Wrath/Starfury style weapon. Spawn projectiles from sky that aim towards mouse.
    ///     See Source code for Star Wrath projectile to see how it passes through tiles.
    ///     For a detailed sword guide see <see cref="ExampleSword" />
    /// </summary>
    public class DemoniteScythe : BaseScythe
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Holding this item summons 2 Shadow amalgamates to aid you in your fight!\nAlso, swinging shoots out mini eaters.\nLifecost " + LifeCost);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            BaseLifeCost = 14;
            LifeCost = 14;
            Item.width = 70;
            Item.height = 70;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<WraithClass>();
            Item.damage = 21;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ProjectileID.TinyEater; // ID of the projectiles the sword will shoot
            Item.shootSpeed = 8f; // Speed of the projectiles the sword will shoot

            // If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
            // Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 2 + Main.rand.Next(3);
            float rotation = MathHelper.ToRadians(45);
            position += Vector2.Normalize(velocity) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 0.8f; // Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ShadowMinion>()] < 2)
            {
                Projectile shot = Projectile.NewProjectileDirect(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<ShadowMinion>(), Item.damage, 0f, player.whoAmI);
                shot.originalDamage = Item.damage;
            }
            player.AddBuff(ModContent.BuffType<ShadowMinionBuff>(), 18000);
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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 35)
                .AddIngredient(ItemID.CrimtaneBar, 21)
                .AddIngredient(ItemID.TissueSample, 16)
                .AddIngredient(ModContent.ItemType<DarkDust>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class ShadowMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Amalgamate Minion");
            // Description.SetDefault("These shadow amalgamations will help you in your battles!");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ShadowMinion>()] > 0)
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
    
    public class ShadowMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Ball Minion");
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
            Projectile.width = 32;
            Projectile.height = 32;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
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
        public override bool MinionContactDamage()
        {
            return true;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Dust dustBlood = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0, 3, 12, default, 1f);
                dustBlood.shader = GameShaders.Armor.GetShaderFromItemId(ItemID.VioletGradientDye);
            }
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.type != ModContent.ItemType<DemoniteScythe>())
            {
                Projectile.Kill();
            }

            #region Active check
            // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<ShadowMinionBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<ShadowMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            #endregion

            #region General behavior
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -player.direction;
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
                // This code is required either way, used for finding a target
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
                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            Projectile.friendly = foundTarget;
            #endregion

            #region Movement

            // Default movement parameters (here for attacking)
            float speed = 8f;
            float inertia = 20f;

            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > 40f)
                {
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
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
}