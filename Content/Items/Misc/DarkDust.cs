using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using WraithMod.Content.Items.Armour;
using Terraria.DataStructures;
using WraithMod.Content.Items.Weapons.Scythes.OreScythes;

namespace WraithMod.Content.Items.Misc
{
    public class DarkDust : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4)); // animation for 5 ticks 4 frames
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

            ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while in the player's inventory
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
            DisplayName.SetDefault("Evil Essence");
            Tooltip.SetDefault("Infused with both evils of the terraria world.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.width = 32; // The item texture's width
            Item.height = 26; // The item texture's height
            Item.rare = ItemRarityID.Purple;

            Item.maxStack = 999; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RottenChunk, 2)
                .AddIngredient(ItemID.VileMushroom, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Vertebrae, 2)
                .AddIngredient(ItemID.ViciousMushroom, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe(3)
                .AddIngredient(ItemID.VilePowder, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe(3)
               .AddIngredient(ItemID.ViciousPowder, 1)
               .AddTile(TileID.WorkBenches)
               .Register();
        }

        // Researching the Example item will give you immediate access to the armour it can make and some weapons it can make.
    }
}