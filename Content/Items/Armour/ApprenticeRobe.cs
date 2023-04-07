using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using WraithMod.Content.Class;
using Terraria.GameContent.Creative;
using WraithMod.Content.Items.Misc;

namespace WraithMod.Content.Items.Armour
{
    // See also: ExampleCostume
    [AutoloadEquip(EquipType.Body)]
    public class ApprenticeRobe : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Apprentice Robe");
            Tooltip.SetDefault("Increases life regen by 4.");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            // By passing this (the ModItem) into the item parameter we can reference it later in GetEquipSlot with just the item's name
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 14;
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Purple;
            Item.defense = 3; // The amount of defense the item will give when equipped
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 15)
                .AddIngredient(ItemID.RottenChunk, 13)
                .AddIngredient(ModContent.ItemType<DarkDust>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 15)
                .AddIngredient(ItemID.Vertebrae, 13)
                .AddIngredient(ModContent.ItemType<DarkDust>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 8;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            // By changing the equipSlot to the leg equip texture slot, the leg texture will now be drawn on the player
            // We're changing the leg slot so we set this to true
            robes = true;
            // Here we can get the equip slot by name since we referenced the item when adding the texture
            // You can also cache the equip slot in a variable when you add it so this way you don't have to call GetEquipSlot
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }
    }
}