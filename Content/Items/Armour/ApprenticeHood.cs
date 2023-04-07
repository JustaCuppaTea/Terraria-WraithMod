using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using WraithMod.Content.Items.Armour;
using WraithMod.Content.Class;
using Terraria.GameInput;
using System.Security.Cryptography.X509Certificates;
using WraithMod.Content.Items.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using WraithMod.Content.BaseClasses;

namespace WraithMod.Content.Items.Armour
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class ApprenticeHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Increases movement speed by 15%.");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;

            // If your head equipment should draw hair while drawn, use one of the following:
            // ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            // ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            // ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Purple; // The rarity of the item
            Item.defense = 1; // The amount of defense the item will give when equipped
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.15f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ApprenticeRobe>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases dark damage by 15% and decreases life cost by 2.\n Allows you to turn invulnerable for a split second on pressing Armour Set Bonus Key with a big cooldown."; // This is the setbonus tooltip
            player.GetDamage<WraithClass>() += 0.15f;
            if (Main.rand.NextBool(6))
            {
                Dust dust = Dust.NewDustDirect(player.Center, 1, 1, DustID.Asphalt, 0, -1, Main.rand.Next(60), default(Color), Main.rand.NextFloat(2f));
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetShaderFromItemId(ItemID.GrimDye);
            }
            if (player.HeldItem.ModItem is BaseScythe modItem)
            {
                modItem.LifeCost -= 1;
            }
            bool KeyPressed = InvincibleKeybindPlayer.InvinciblePressed;
            if (KeyPressed && !player.HasBuff<InvincibleCooldown>())
            {
                player.immune = true;
                player.immuneTime = 10;
                player.AddBuff(ModContent.BuffType<InvincibleCooldown>(), 3000);
            }
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 7)
                .AddIngredient(ItemID.RottenChunk, 10)
                .AddIngredient(ModContent.ItemType<DarkDust>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 7)
                .AddIngredient(ItemID.Vertebrae, 10)
                .AddIngredient(ModContent.ItemType<DarkDust>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
        // See Common/Systems/KeybindSystem for keybind registration.
        public class InvincibleKeybindPlayer : ModPlayer
        {
        public static bool InvinciblePressed;
            public override void ProcessTriggers(TriggersSet triggersSet)
            {
            if (KeybindSystem.InvinicibilityKey.JustPressed)
            {
                InvinciblePressed = true;
            }
            if (KeybindSystem.InvinicibilityKey.JustReleased)
                {
                    InvinciblePressed = false;
                }
            }
        }
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind InvinicibilityKey { get; private set; }

        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to english users is in en-US.hjson
            InvinicibilityKey = KeybindLoader.RegisterKeybind(Mod, "Armour Set Bonus", "P");
        }

        // Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
        public override void Unload()
        {
            // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
            InvinicibilityKey = null;
        }
    }

    public class InvincibleCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Invicibility Cooldown");
            // Description.SetDefault("You cannot turn invincible yet.");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}