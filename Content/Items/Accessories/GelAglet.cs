using WraithMod.Content.Class;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ID;
using static tModPorter.ProgressUpdate;
using System;
using WraithMod.Content.BaseClasses;

namespace WraithMod.Content.Items.Accessories
{
    public class GelAglet : ModItem 
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Decreases life cost by 2 and increases movement speed by 5%.\n\"Absorbs darkness of evil items.\"");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.HeldItem.ModItem is BaseScythe modItem)
            {
                modItem.LifeCost -= 2;
            }
            player.moveSpeed += 0.05f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gel, 35)
                .AddIngredient(ItemID.Aglet)
                .AddIngredient(TileID.WorkBenches)
                .Register();
        }
    }
    public class GelAgletPlayer : ModPlayer
    {
        public override void ResetEffects()
        {
            if (Player.HeldItem.ModItem is BaseScythe modItem)
            {
                modItem.LifeCost = modItem.BaseLifeCost;
            }
        }
    }
}