using BepInEx.Configuration;
using CellMenu;
using GameData;
using Gear;
using Player;
using System.Runtime.CompilerServices;
using System.Text;

namespace WeaponStatShower
{
    internal class ShowStat : Patch
    {
        private const string PatchName = nameof(ShowStat);
        private const PatchType patchType = PatchType.Postfix;
        private static readonly ConfigDefinition ConfigEnabled = new(PatchName, "Enabled");
        public static Patch Instance { get; private set; }
        private static PlayerDataBlock _playerDataBlock;
        public override string Name { get; } = PatchName;
        public override bool Enabled => WeaponStatShowerPlugin.Instance.Config.GetConfigEntry<bool>(ConfigEnabled).Value;


        public override void Initialize()
        {
            Instance = this;
            WeaponStatShowerPlugin.Instance.Config.Bind(ConfigEnabled, true, new ConfigDescription("Show the stats of a weapon."));
        }

        public override void Execute()
        {
            WeaponStatShowerPlugin.LogInfo("Before getting the Execution");
            this.PatchMethod<CM_InventorySlotItem>(nameof(CM_InventorySlotItem.LoadData), PatchType.Postfix);
        }


        public static void CM_InventorySlotItem__LoadData__Postfix(CM_InventorySlotItem __instance, GearIDRange idRange, bool clickable, bool detailedInfo)
        {
            uint categoryID = idRange.GetCompID(eGearComponent.Category);

            GearCategoryDataBlock gearCatBlock = GameDataBlockBase<GearCategoryDataBlock>.GetBlock(categoryID);

            ItemDataBlock itemDataBlock = ItemDataBlock.GetBlock(gearCatBlock.BaseItem);

            _playerDataBlock = PlayerDataBlock.GetBlock(1U);

            if (itemDataBlock.inventorySlot == InventorySlot.GearMelee)
            {
                PopulateMelee(__instance, gearCatBlock, itemDataBlock);
            }
            else
            {
                eWeaponFireMode weaponFireMode = (eWeaponFireMode)idRange.GetCompID(eGearComponent.FireMode);

                bool isSentryGun = categoryID == 12; // => PersistentID of the Sentry Gun Category

                ArchetypeDataBlock archetypeDataBlock = isSentryGun
                    ? SentryGunInstance_Firing_Bullets.GetArchetypeDataForFireMode(weaponFireMode)
                    : ArchetypeDataBlock.GetBlock(GearBuilder.GetArchetypeID(gearCatBlock, weaponFireMode));

                __instance.GearDescription = ChargeUpFormat(__instance, archetypeDataBlock);

                __instance.GearDescription += "\n\n" + GetFormatedWeaponStats(archetypeDataBlock, itemDataBlock, isSentryGun);
            }
        }

        private static String ChargeUpFormat(CM_InventorySlotItem __instance, ArchetypeDataBlock archetypeDataBlock)
        {
            if (archetypeDataBlock.SpecialChargetupTime > 0)
            {
                String chargeString = archetypeDataBlock.SpecialChargetupTime > 0.3 ? "Long Charge-Up" : "Short Charge-Up";

                String[] lines = __instance.GearDescription.Split("\n");
                foreach (String line in lines)
                {
                    if (line.Contains("CHARGE"))
                    {
                        return __instance.GearDescription.Replace(line, chargeString);
                    }
                }

                return __instance.GearDescription += "\n" + chargeString;
            }
            return __instance.GearDescription;
        }

        private static void PopulateMelee(CM_InventorySlotItem __instance, GearCategoryDataBlock gearCatBlock, ItemDataBlock itemDataBlock)
        {
            MeleeArchetypeDataBlock meleeArchetypeDataBlock = MeleeArchetypeDataBlock.GetBlock(GearBuilder.GetMeleeArchetypeID(gearCatBlock));

            __instance.GearDescription = __instance.GearDescription + "\n\n" + GetFormatedWeaponStats(meleeArchetypeDataBlock, itemDataBlock);
        }



        public static string GetFormatedWeaponStats(MeleeArchetypeDataBlock archeTypeDataBlock, ItemDataBlock itemDataBlock)
        {
            if (archeTypeDataBlock == null) return string.Empty;

            StringBuilder builder = new StringBuilder();

            void Divider(ref int count, StringBuilder builder)
            {
                if (count >= 3)
                {
                    builder.Append("\n");
                    count = 0;
                }
                else if (count > 0)
                    builder.Append(DIVIDER);
            }

            builder.Append("<#9D2929>");
            builder.Append($"{Short_Damage}{Short_MeleeLight} ");
            builder.Append(archeTypeDataBlock.LightAttackDamage);
            builder.Append(CLOSE_COLOR_TAG);

            builder.Append(DIVIDER);

            builder.Append("<color=orange>");
            builder.Append($"{Short_Damage}{Short_MeleeCharged} ");
            builder.Append(archeTypeDataBlock.ChargedAttackDamage);
            builder.Append(CLOSE_COLOR_TAG);

            int count = 2;

            if (!archeTypeDataBlock.AllowRunningWhenCharging)
            {
                builder.Append(DIVIDER);

                builder.Append("<#FFD306>");
                builder.Append($"{Short_MeleeCanRunWhileCharging} ");
                builder.Append(archeTypeDataBlock.AllowRunningWhenCharging);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            if (archeTypeDataBlock.LightStaggerMulti != 1f)
            {
                Divider(ref count, builder);

                builder.Append("<#C0FF00>");
                builder.Append($"{Short_Stagger}{Short_MeleeLight} ");
                builder.Append(archeTypeDataBlock.LightStaggerMulti);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            if (archeTypeDataBlock.ChargedStaggerMulti != 1f)
            {
                Divider(ref count, builder);

                builder.Append("<color=green>");
                builder.Append($"{Short_Stagger}{Short_MeleeCharged} ");
                builder.Append(archeTypeDataBlock.ChargedStaggerMulti);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            if (archeTypeDataBlock.LightPrecisionMulti != 1f)
            {
                Divider(ref count, builder);

                builder.Append("<#004E2C>");
                builder.Append($"{Short_Precision}{Short_MeleeLight} ");
                builder.Append(archeTypeDataBlock.LightPrecisionMulti);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            if (archeTypeDataBlock.ChargedPrecisionMulti != 1f)
            {
                Divider(ref count, builder);

                builder.Append("<#55022B>");
                builder.Append($"{Short_Precision}{Short_MeleeCharged} ");
                builder.Append(archeTypeDataBlock.ChargedPrecisionMulti);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            if (archeTypeDataBlock.LightSleeperMulti != 1f)
            {
                Divider(ref count, builder);

                builder.Append("<#A918A7>");
                builder.Append($"{Short_MeleeSleepingEnemiesMultiplier}{Short_MeleeLight} ");
                builder.Append(archeTypeDataBlock.LightSleeperMulti);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            if (archeTypeDataBlock.ChargedSleeperMulti != 1f)
            {
                Divider(ref count, builder);

                builder.Append("<#025531>");
                builder.Append($"{Short_MeleeSleepingEnemiesMultiplier}{Short_MeleeCharged} ");
                builder.Append(archeTypeDataBlock.ChargedSleeperMulti);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            if (archeTypeDataBlock.LightEnvironmentMulti != 1f)
            {
                Divider(ref count, builder);

                builder.Append("<#18A4A9>");
                builder.Append($"{Short_EnvironmentMultiplier}{Short_MeleeLight} ");
                builder.Append(archeTypeDataBlock.LightEnvironmentMulti);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            if (archeTypeDataBlock.ChargedEnvironmentMulti != 1f)
            {
                Divider(ref count, builder);

                builder.Append("<#75A2AA>");
                builder.Append($"{Short_EnvironmentMultiplier}{Short_MeleeCharged} ");
                builder.Append(archeTypeDataBlock.ChargedEnvironmentMulti);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            return builder.ToString();
        }

        public static string GetFormatedWeaponStats(ArchetypeDataBlock archeTypeDataBlock, ItemDataBlock itemDataBlock, bool isSentryGun = false)
        {
            if (archeTypeDataBlock == null) return string.Empty;

            StringBuilder builder = new StringBuilder();



            builder.Append("<#9D2929>");
            builder.Append($"{Short_Damage} ");
            builder.Append(archeTypeDataBlock.Damage);
            builder.Append(CLOSE_COLOR_TAG);

            if (!isSentryGun)
            {
                builder.Append(DIVIDER);

                builder.Append("<color=orange>");
                builder.Append($"{Short_Clip} ");
                builder.Append(archeTypeDataBlock.DefaultClipSize);
                builder.Append(CLOSE_COLOR_TAG);
            }

            builder.Append(DIVIDER);

            builder.Append("<#FFD306>");
            builder.Append($"{Short_MaxAmmo} ");
            builder.Append(GetTotalAmmo(archeTypeDataBlock, itemDataBlock, isSentryGun));
            builder.Append(CLOSE_COLOR_TAG);

            if (!isSentryGun)
            {
                builder.Append(DIVIDER);

                builder.Append("<#C0FF00>");
                builder.Append($"{Short_Reload} ");
                builder.Append(archeTypeDataBlock.DefaultReloadTime);
                builder.Append(CLOSE_COLOR_TAG);
            }

            builder.Append("\n");

            bool nonStandardStagger = archeTypeDataBlock.StaggerDamageMulti != 1f;

            if (nonStandardStagger)
            {
                builder.Append("<color=green>");
                builder.Append($"{Short_Stagger} ");
                builder.Append(archeTypeDataBlock.StaggerDamageMulti);
                builder.Append(CLOSE_COLOR_TAG);
            }


            bool pierce = archeTypeDataBlock.PiercingBullets;

            if (pierce)
            {
                if (nonStandardStagger)
                    builder.Append(DIVIDER);

                builder.Append("<#004E2C>");
                builder.Append($"{Short_PierceCount} ");
                builder.Append(archeTypeDataBlock.PiercingDamageCountLimit);
                builder.Append(CLOSE_COLOR_TAG);

                if (nonStandardStagger)
                    builder.Append("\n");
            }

            bool isShotgun = archeTypeDataBlock.ShotgunBulletCount > 0;

            if (isShotgun)
            {
                if (!pierce && nonStandardStagger)
                    builder.Append(DIVIDER);

                builder.Append("<#55022B>");
                builder.Append($"{Short_ShotgunPelletCount} ");
                builder.Append(archeTypeDataBlock.ShotgunBulletCount);
                builder.Append(CLOSE_COLOR_TAG);

                builder.Append(DIVIDER);

                builder.Append("<#A918A7>");
                builder.Append($"{Short_ShotgunSpread} ");
                builder.Append(archeTypeDataBlock.ShotgunBulletSpread);
                builder.Append(CLOSE_COLOR_TAG);

                builder.Append("\n");
            }

            if (archeTypeDataBlock.BurstShotCount > 1 && archeTypeDataBlock.FireMode == eWeaponFireMode.Burst)
            {
                if (!isShotgun && nonStandardStagger)
                    builder.Append(DIVIDER);

                builder.Append("<#025531>");
                builder.Append($"{Short_BurstShotCount} ");
                builder.Append(archeTypeDataBlock.BurstShotCount);
                builder.Append(CLOSE_COLOR_TAG);

                builder.Append(DIVIDER);

                builder.Append("<#18A4A9>");
                builder.Append($"{Short_BurstDelay} ");
                builder.Append(archeTypeDataBlock.BurstDelay);
                builder.Append(CLOSE_COLOR_TAG);
            }

            if (archeTypeDataBlock.SpecialChargetupTime > 0)
            {
                builder.Append(DIVIDER);

                builder.Append("<#AAA8FF>");
                builder.Append($"{Short_ChargeUp} ");
                builder.Append(archeTypeDataBlock.SpecialChargetupTime);
                builder.Append(CLOSE_COLOR_TAG);
            }

            return builder.ToString();
        }



        public static int GetAmmoMax(ItemDataBlock itemDataBlock)
        {
            var ammoType = PlayerAmmoStorage.GetAmmoTypeFromSlot(itemDataBlock.inventorySlot);
            switch (ammoType)
            {
                case AmmoType.Standard:
                    return _playerDataBlock.AmmoStandardMaxCap;
                case AmmoType.Special:
                    return _playerDataBlock.AmmoSpecialMaxCap;
                case AmmoType.Class:
                    return _playerDataBlock.AmmoClassMaxCap;
                case AmmoType.CurrentConsumable:
                    return itemDataBlock.ConsumableAmmoMax;
            }
            return -1;
        }

        public static int GetTotalAmmo(ArchetypeDataBlock archetypeDataBlock, ItemDataBlock itemDataBlock, bool isSentryGun = false)
        {
            var max = GetAmmoMax(itemDataBlock);

            var costOfBullet = archetypeDataBlock.CostOfBullet;

            if (isSentryGun)
            {
                costOfBullet = costOfBullet * itemDataBlock.ClassAmmoCostFactor;

                if (archetypeDataBlock.ShotgunBulletCount > 0f)
                {
                    costOfBullet *= archetypeDataBlock.ShotgunBulletCount;
                }
            }

            var maxBullets = (int)(max / costOfBullet);

            if (isSentryGun)
                return maxBullets;

            return maxBullets + archetypeDataBlock.DefaultClipSize;
        }


        public const string DIVIDER = " | ";
        public const string CLOSE_COLOR_TAG = "</color>";

        public static string Short_MeleeLight { get; } = ".Lgt";
        public static string Short_MeleeCharged { get; } = ".Hvy";

        public static string Short_MeleeCanRunWhileCharging { get; } = "Run";
        public static string Short_MeleeSleepingEnemiesMultiplier { get; } = "Slp";
        public static string Short_EnvironmentMultiplier { get; } = "Env";

        public static string Short_Damage { get; } = "Dmg";
        public static string Short_Clip { get; } = "Clp";
        public static string Short_MaxAmmo { get; } = "Max";
        public static string Short_Reload { get; } = "Rld";
        public static string Short_Stagger { get; } = "Stgr";
        public static string Short_Precision { get; } = "Pcsn";
        public static string Short_PierceCount { get; } = "Prc";
        public static string Short_ShotgunPelletCount { get; } = "Sh.Plts";
        public static string Short_ShotgunSpread { get; } = "Sh.Sprd";
        public static string Short_BurstShotCount { get; } = "Brst.Sht";
        public static string Short_BurstDelay { get; } = "Brst.Dly";
        public static string Short_ChargeUp { get; } = "Chrg";
    }
}
