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
    public class CrimtaneScythe : BaseScythe
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Holding this item summons 2 creepers to aid you in your fight!\nAlso, swinging throws out an amalgamation of the eye and brain of cthulu that splits on death.\nLifecost " + LifeCost);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            BaseLifeCost = 17;
            LifeCost = 17;
            Item.width = 70;
            Item.height = 70;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<WraithClass>();
            Item.damage = 21;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<FleshAmalgProj>(); // ID of the projectiles the sword will shoot
            Item.shootSpeed = 8f; // Speed of the projectiles the sword will shoot

            // If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
            // Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 2;
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
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CreeperMinion>()] < 2)
            {
                Projectile shot = Projectile.NewProjectileDirect(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<CreeperMinion>(), Item.damage, 0f, player.whoAmI);
                shot.originalDamage = Item.damage;
            }
            player.AddBuff(ModContent.BuffType<CreeperMinionBuff>(), 18000);
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
    public class FleshAmalgProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = 15;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 120;
            DrawOffsetX = -14;
            DrawOriginOffsetY = -7;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath22 with { Volume = 1.5f});
            for (int d = 0; d < 30; d++)
            {
                Dust.NewDustDirect(Projectile.position, 1, 1, DustID.Blood, 0f, 0f, 20, default(Color), 1.5f);
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(0, -2.5f), ModContent.ProjectileType<EyeProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(0, -1.75f), ModContent.ProjectileType<BrainProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        public override void AI()
        {
            Projectile.velocity.Y = Projectile.velocity.Y + 0.3f; // 0.1f for arrow gravity, 0.4f for knife gravity
            if (Projectile.velocity.Y > 16f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
            {
                Projectile.velocity.Y = 16f;
            }
            Projectile.alpha -= 10;
            if (Projectile.alpha <= 0) { Projectile.alpha = 0; }
            Projectile.rotation += 0.1f * (float)Projectile.direction;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 1f, 20, default(Color), 1.5f);
            if (Projectile.direction == 1)
            {
                if (Projectile.velocity.X < 0.2f)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.direction == -1)
            {
                if (Projectile.velocity.X > -0.2f)
                {
                    Projectile.Kill();
                }
            }
            Projectile.velocity *= 0.95f;
            return false;
        }
    }
    public class CreeperMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Creeper Minion");
            // Description.SetDefault("These creepers will help you in your battles!");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CreeperMinion>()] > 0)
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
    public class EyeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 90;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool(2))
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 1f, 20, default(Color), 1.5f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // projectile sprite faces up
            float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 8f; // The speed at which the projectile moves towards the target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
    public class BrainProj : ModProjectile 
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<WraithClass>();
        }
        public override void AI()
        {
            if (Main.rand.NextBool(2))
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 1f, 20, default(Color), 1.5f);
            }
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 1f, 20, default(Color), 1.5f);
            float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 5f; // The speed at which the projectile moves towards the target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 60);
        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
    public class CreeperMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Creeper Minion");
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
            if (Main.rand.NextBool(3))
            {
                Dust dustBlood = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 3, 12, default, 1f);
                dustBlood.shader = GameShaders.Armor.GetShaderFromItemId(ItemID.BloodbathDye);
            }
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.type != ModContent.ItemType<CrimtaneScythe>())
            {
                Projectile.Kill();
            }

            #region Active check
            // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<CreeperMinionBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<CreeperMinionBuff>()))
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