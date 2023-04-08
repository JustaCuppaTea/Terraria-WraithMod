using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using WraithMod.Content.Items.Armour;
using Terraria.DataStructures;
using WraithMod.Content.Items.Weapons.Scythes.OreScythes;
using WraithMod.Content.Buffs;

namespace WraithMod.Content.Items.Misc
{
    public class BloodContained : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contained Blood");
            Tooltip.SetDefault("Contains human blood.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 26; // The item texture's height
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item3;
            Item.buffType = ModContent.BuffType<Rage>(); // Specify an existing buff to be applied when used.
            Item.buffTime = 540; // The amount of time the buff declared in Item.buffType will last in ticks. 540 / 60 is 9, so this buff will last 9 seconds.
            Item.consumable = true;

            Item.maxStack = 999; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
        }
        public override void OnConsumeItem(Player player)
        {
            if (player.HasBuff(ModContent.BuffType<BloodSickness>()))
            {
                player.AddBuff(ModContent.BuffType<Sick>(), 540);
            }
            player.AddBuff(ModContent.BuffType<BloodSickness>(), 540);
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

        // Researching the Example item will give you immediate access to the armour it can make and some weapons it can make.
    }
}