using GameData;
using Gear;
using Player;
using System.Text;

namespace WeaponStatShower.Utils
{
    internal class WeaponDescriptionBuilder
    {
        private SleepersDatas sleepersDatas;
        private PlayerDataBlock _playerDataBlock { get; set; }
        private GearIDRange idRange { get; set; }

        private uint categoryID;
        private GearCategoryDataBlock gearCatBlock;
        private ItemDataBlock itemDataBlock;


        public void UpdateSleepersDatas(string[] activatedSleepers)
        {
            if (activatedSleepers[0].Length == 0)
            {
                WeaponStatShowerPlugin.LogWarning("Empty String in the config file, applying Default values");
                activatedSleepers = new string[] { "STRIKER", "SHOOTER", "SCOUT" };
            }
            sleepersDatas = new SleepersDatas(activatedSleepers);
        }

        public string DescriptionFormatter(string GearDescription)
        {

            if (itemDataBlock.inventorySlot == InventorySlot.GearMelee)
            {
                MeleeArchetypeDataBlock meleeArchetypeDataBlock = MeleeArchetypeDataBlock.GetBlock(GearBuilder.GetMeleeArchetypeID(gearCatBlock));

                GearDescription = VerboseDescriptionFormatter(meleeArchetypeDataBlock);

                return GearDescription + "\n\n" + GetFormatedWeaponStats(meleeArchetypeDataBlock, itemDataBlock);
            }
            else
            {
                eWeaponFireMode weaponFireMode = (eWeaponFireMode)idRange.GetCompID(eGearComponent.FireMode);

                bool isSentryGun = categoryID == 12; // => PersistentID for Sentry Gun

                ArchetypeDataBlock archetypeDataBlock = isSentryGun
                    ? SentryGunInstance_Firing_Bullets.GetArchetypeDataForFireMode(weaponFireMode)
                    : ArchetypeDataBlock.GetBlock(GearBuilder.GetArchetypeID(gearCatBlock, weaponFireMode));

                if (archetypeDataBlock == null) return GearDescription; //case of non-weapon tools such as BIO_TRACKER/MINE_DEPLOYER

                GearDescription = VerboseDescriptionFormatter(archetypeDataBlock, isSentryGun);

                return GearDescription + GetFormatedWeaponStats(archetypeDataBlock, isSentryGun);
            }
        }

        internal string FireRateFormatter(string gearPublicName)
        {
            if (itemDataBlock.inventorySlot == InventorySlot.GearMelee)
            {
                MeleeArchetypeDataBlock meleeArchetypeDataBlock = MeleeArchetypeDataBlock.GetBlock(GearBuilder.GetMeleeArchetypeID(gearCatBlock));
                return VerbosePublicNameMelee(meleeArchetypeDataBlock);
            }
            else
            {
                eWeaponFireMode weaponFireMode = (eWeaponFireMode)idRange.GetCompID(eGearComponent.FireMode);

                bool isSentryGun = categoryID == 12; // => PersistentID for Sentry Gun

                ArchetypeDataBlock archetypeDataBlock = isSentryGun
                    ? SentryGunInstance_Firing_Bullets.GetArchetypeDataForFireMode(weaponFireMode)
                    : ArchetypeDataBlock.GetBlock(GearBuilder.GetArchetypeID(gearCatBlock, weaponFireMode));

                if (archetypeDataBlock == null) return gearPublicName; //case of non-weapon tools such as BIO_TRACKER/MINE_DEPLOYER

                return VerbosePublicNameFireMode(archetypeDataBlock);
            }
        }

        private string VerbosePublicNameMelee(MeleeArchetypeDataBlock meleeArchetypeDataBlock)
        {
            switch (meleeArchetypeDataBlock.persistentID)
            {
                case 1:
                    return "Hammer - Balanced";
                case 2:
                    return "Knife - Fast";
                case 4:
                    return "Bat - Fast";
                case 3:
                    return "Spear - Slow";
                default:
                    return "";
            }
        }

        private string VerbosePublicNameFireMode(ArchetypeDataBlock archetypeDataBlock)
        {
            eWeaponFireMode fireMode = archetypeDataBlock.FireMode;
            StringBuilder sb = new StringBuilder();
            switch (fireMode)
            {
                case eWeaponFireMode.Auto:
                case eWeaponFireMode.SentryGunAuto:

                    sb.Append("Full-A (");
                    sb.Append("<#12FF50>");
                    sb.Append(Short_RateOfFire + " ");
                    sb.Append(GetRateOfFire(archetypeDataBlock, fireMode));
                    sb.Append(CLOSE_COLOR_TAG);
                    sb.Append(")"); 
                    break;

                case eWeaponFireMode.Semi:
                case eWeaponFireMode.SentryGunSemi:

                    sb.Append("Semi-A (");
                    sb.Append("<#12FF50>");
                    sb.Append(Short_RateOfFire + " ");
                    sb.Append(GetRateOfFire(archetypeDataBlock, fireMode));
                    sb.Append(CLOSE_COLOR_TAG);
                    sb.Append(")");
                    break;

                case eWeaponFireMode.Burst:
                case eWeaponFireMode.SentryGunBurst:
                    if (archetypeDataBlock.BurstShotCount != 1)
                    {
                        sb.Append("Burst (");
                        sb.Append("<#704dfa>");
                        sb.Append("#" + archetypeDataBlock.BurstShotCount);
                        sb.Append(CLOSE_COLOR_TAG);
                        sb.Append(DIVIDER);
                        sb.Append("<#12FF50>");
                        sb.Append(Short_RateOfFire + " ");
                        sb.Append(GetRateOfFire(archetypeDataBlock, fireMode));
                        sb.Append(CLOSE_COLOR_TAG);
                        sb.Append(")");
                    }
                    else
                    {
                        sb.Append("Semi-A (");
                        sb.Append("<#12FF50>");
                        sb.Append(Short_RateOfFire + " ");
                        sb.Append(GetRateOfFire(archetypeDataBlock, eWeaponFireMode.Semi));
                        sb.Append(CLOSE_COLOR_TAG);
                        sb.Append(")");
                        break;
                    }
                    break;
                case eWeaponFireMode.SemiBurst:
                    sb.Append("S-Burst (");
                    sb.Append("<#12FF50>");
                    sb.Append("#" + archetypeDataBlock.BurstShotCount);
                    sb.Append(" every " + archetypeDataBlock.SpecialSemiBurstCountTimeout + "\'");
                    sb.Append(CLOSE_COLOR_TAG);
                    sb.Append(")"); 
                    break;

                case eWeaponFireMode.SentryGunShotgunSemi:
                    sb.Append("Shotgun-S (");
                    sb.Append("<#12FF50>");
                    sb.Append(Short_RateOfFire + " ");
                    sb.Append(GetRateOfFire(archetypeDataBlock, fireMode));
                    sb.Append(CLOSE_COLOR_TAG);
                    sb.Append(")");
                    break;
                default:
                    WeaponStatShowerPlugin.LogError("FireMode not found");
                    break;
            }

            return sb.ToString();
        }

        private string VerboseDescriptionFormatter(MeleeArchetypeDataBlock meleeArchetypeDataBlock)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(meleeArchetypeDataBlock.CameraDamageRayLength < 1.76 ? "Short Range" : meleeArchetypeDataBlock.CameraDamageRayLength < 2.5 ? "Medium Range" : "Long Range");

            sb.Append(meleeArchetypeDataBlock.CanHitMultipleEnemies? "Piercing\n": "");

            return "".Equals(sb.ToString()) ? "" : sb.ToString() + "\n";
        }

        private string VerboseDescriptionFormatter(ArchetypeDataBlock archetypeDataBlock, bool isSentryGun)
        {
            StringBuilder sb = new StringBuilder();

            if (isSentryGun)
                sb.AppendLine("Deployable");

            switch (archetypeDataBlock.ShotgunBulletSpread + archetypeDataBlock.ShotgunConeSize)
            {
                case 0: break;
                case 1:
                    sb.AppendLine("Chocked Spread");
                    break;
                case 4:
                    sb.AppendLine("Small Spread");
                    break;
                case 5:
                    sb.AppendLine("Medium Spread");
                    break;
                case 7:
                    sb.AppendLine("Large Spread");
                    break;
                case 9:
                    sb.AppendLine("Huge Spread");
                    break;
                default:
                    WeaponStatShowerPlugin.LogError(archetypeDataBlock.PublicName + ": spread not considered{" + archetypeDataBlock.ShotgunBulletSpread + "/" + archetypeDataBlock.ShotgunConeSize +"}");
                    break;
            }

            if (archetypeDataBlock.SpecialChargetupTime > 0)
                sb.AppendLine(archetypeDataBlock.SpecialChargetupTime > 0.4 ?
                    "Long Charge-up (" + FormatFloat(archetypeDataBlock.SpecialChargetupTime, 2) + ")" :
                    "Short Charge-up (" + FormatFloat(archetypeDataBlock.SpecialChargetupTime, 2) + ")");

            return "".Equals(sb.ToString())? "" : sb.ToString() + "\n";
        }




        private string GetFormatedWeaponStats(ArchetypeDataBlock archeTypeDataBlock, bool isSentryGun = false)
        {
            if (archeTypeDataBlock == null) return string.Empty;

            StringBuilder builder = new StringBuilder();

            void Divider(ref int count, StringBuilder builder)
            {
                if (count >= 4)
                {
                    builder.Append("\n");
                    count = 0;
                }
                else if (count > 0)
                    builder.Append(DIVIDER);
            }

            int count = 0;

            builder.Append("<#9D2929>");
            builder.Append($"{Short_Damage} ");
            builder.Append(FormatFloat(archeTypeDataBlock.Damage, 2));
            builder.Append(archeTypeDataBlock.ShotgunBulletCount > 0? "(x"+ archeTypeDataBlock.ShotgunBulletCount +")" : "");
            builder.Append(CLOSE_COLOR_TAG);
            count++;

            if (!isSentryGun)
            {
                builder.Append(DIVIDER);

                builder.Append("<color=orange>");
                builder.Append($"{Short_Clip} ");
                builder.Append(archeTypeDataBlock.DefaultClipSize);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            builder.Append(DIVIDER);

            builder.Append("<#FFD306>");
            builder.Append($"{Short_MaxAmmo} ");
            builder.Append(GetTotalAmmo(archeTypeDataBlock, itemDataBlock, isSentryGun));
            builder.Append(CLOSE_COLOR_TAG);
            count++;

            if (!isSentryGun)
            {
                builder.Append(DIVIDER);

                builder.Append("<#C0FF00>");
                builder.Append($"{Short_Reload} ");
                builder.Append(FormatFloat(archeTypeDataBlock.DefaultReloadTime, 2));
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }


            if (archeTypeDataBlock.PrecisionDamageMulti != 1f)
            {
                Divider(ref count, builder);
                builder.Append("<#18A4A9>");
                builder.Append($"{Short_Precision} ");
                builder.Append(FormatFloat(archeTypeDataBlock.PrecisionDamageMulti, 2));
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            Divider(ref count, builder);
            builder.Append("<#6764de>");
            builder.Append($"{Short_Falloff} ");
            builder.Append(((int)archeTypeDataBlock.DamageFalloff.x).ToString() + "m");
            builder.Append(CLOSE_COLOR_TAG);
            count++;

            bool nonStandardStagger = archeTypeDataBlock.StaggerDamageMulti != 1f;

            if (nonStandardStagger)
            {
                Divider(ref count, builder);
                builder.Append("<color=green>");
                builder.Append($"{Short_Stagger} ");
                builder.Append(FormatFloat(archeTypeDataBlock.StaggerDamageMulti, 2));
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            bool nonStandardHip = archeTypeDataBlock.HipFireSpread != 0f;

            if (nonStandardHip && archeTypeDataBlock.ShotgunBulletCount == 0)
            {
                Divider(ref count, builder);
                builder.Append("<#cc9347>");
                builder.Append($"{Short_HipSpread} ");
                builder.Append(FormatFloat(archeTypeDataBlock.HipFireSpread, 2));
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            bool nonStandardADS = archeTypeDataBlock.AimSpread != 0f;

            if (nonStandardADS && archeTypeDataBlock.ShotgunBulletCount == 0)
            {
                Divider(ref count, builder);
                builder.Append("<#e6583c>");
                builder.Append($"{Short_AimSpread} ");
                builder.Append(FormatFloat(archeTypeDataBlock.AimSpread, 2));
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }


            bool pierce = archeTypeDataBlock.PiercingBullets;

            if (pierce)
            {
                Divider(ref count, builder);
                builder.Append("<#097345>");
                builder.Append($"{Short_PierceCount} ");
                builder.Append(archeTypeDataBlock.PiercingDamageCountLimit);
                builder.Append(CLOSE_COLOR_TAG);
                count++;
            }

            builder.AppendLine("\n");

            builder.Append(sleepersDatas.VerboseKill(archeTypeDataBlock));

            return builder.ToString();
        }

        private string GetFormatedWeaponStats(MeleeArchetypeDataBlock archeTypeDataBlock, ItemDataBlock itemDataBlock)
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

            builder.AppendLine("\n");

            builder.Append(sleepersDatas.VerboseKill(archeTypeDataBlock));

            return builder.ToString();
        }

        private static float FormatFloat(float value, int v)
        {
            return (float)Math.Round((decimal)value, v);
        }



        private int GetAmmoMax(ItemDataBlock itemDataBlock)
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


        private int GetTotalAmmo(ArchetypeDataBlock archetypeDataBlock, ItemDataBlock itemDataBlock, bool isSentryGun = false)
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

        private string GetRateOfFire(ArchetypeDataBlock archetypeDataBlock, eWeaponFireMode fireMode)
        {
            float value = -1F;

            switch (fireMode)
            {
                case eWeaponFireMode.Auto:
                case eWeaponFireMode.SentryGunAuto:
                    value = 1 / archetypeDataBlock.ShotDelay;
                    break;
                case eWeaponFireMode.SentryGunShotgunSemi:
                case eWeaponFireMode.Semi:
                    value = 1 / (archetypeDataBlock.ShotDelay + archetypeDataBlock.SpecialChargetupTime);
                    break;
                case eWeaponFireMode.SentryGunBurst:
                case eWeaponFireMode.Burst:
                    float shootsPerSecondSB = 1 / (archetypeDataBlock.BurstDelay + archetypeDataBlock.SpecialChargetupTime + (archetypeDataBlock.ShotDelay * (archetypeDataBlock.BurstShotCount -1)));
                    value = (shootsPerSecondSB) * archetypeDataBlock.BurstShotCount;
                    break;
            }

            return FormatFloat(value, 1).ToString();
        }

        internal void Inizialize(GearIDRange idRange, PlayerDataBlock playerDataBlock)
        {
            this.idRange = idRange;

            _playerDataBlock = playerDataBlock;

            categoryID = idRange.GetCompID(eGearComponent.Category);

            gearCatBlock = GameDataBlockBase<GearCategoryDataBlock>.GetBlock(categoryID);

            itemDataBlock = ItemDataBlock.GetBlock(gearCatBlock.BaseItem);
        }

        private const string DIVIDER = " | ";
        private const string CLOSE_COLOR_TAG = "</color>";

        private static string Short_MeleeLight { get; } = ".Lgt";
        private static string Short_MeleeCharged { get; } = ".Hvy";

        private static string Short_MeleeCanRunWhileCharging { get; } = "Run";
        private static string Short_MeleeSleepingEnemiesMultiplier { get; } = "Slp";
        private static string Short_EnvironmentMultiplier { get; } = "Env";

        private static string Short_Damage { get; } = "Dmg";
        private static string Short_Clip { get; } = "Clp";
        private static string Short_MaxAmmo { get; } = "Max";
        private static string Short_Falloff { get; } = "Dist";
        private static string Short_Reload { get; } = "Rld";
        private static string Short_Stagger { get; } = "Stgr";
        private static string Short_Precision { get; } = "Prcn";
        private static string Short_PierceCount { get; } = "Pierc";
        private static string Short_RateOfFire { get; } = "RoF";
        private static string Short_AimSpread { get; } = "ADS";
        private static string Short_HipSpread { get; } = "HIP";
    }
}
