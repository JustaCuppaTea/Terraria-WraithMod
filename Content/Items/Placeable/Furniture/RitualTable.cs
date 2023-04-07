using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WraithMod.Content.Items.Misc;

namespace WraithMod.Content.Items.Placeable.Furniture
{
    public class RitualTable : ModItem
    {
        public override void SetDefaults()
        {
            // ModContent.TileType<Tiles.Furniture.ExampleWorkbench>() retrieves the id of the tile that this item should place when used.
            // DefaultToPlaceableTile handles setting various Item values that placeable items use
            // Hover over DefaultToPlaceableTile in Visual Studio to read the documentation!
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.RitualTable>());
            Item.width = 28; // The item texture's width
            Item.height = 14; // The item texture's height
            Item.value = 150;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WorkBench)
                .AddIngredient(ModContent.ItemType<DarkDust>(), 2)
                .AddIngredient(ModContent.ItemType<RitualKnife>())
                .AddIngredient(ModContent.ItemType<BloodContained>(), 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}