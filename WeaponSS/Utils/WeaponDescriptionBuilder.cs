using GameData;
using Gear;
using Player;
using System.Text;


namespace WeaponStatShower.Utils
{
    internal class WeaponDescriptionBuilder
    {
        private SleepersDatas sleepersDatas;
        public PlayerDataBlock _playerDataBlock { get; set; }
        public GearIDRange idRange { get; set; }

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
            uint categoryID = idRange.GetCompID(eGearComponent.Category);

            GearCategoryDataBlock gearCatBlock = GameDataBlockBase<GearCategoryDataBlock>.GetBlock(categoryID);

            ItemDataBlock itemDataBlock = ItemDataBlock.GetBlock(gearCatBlock.BaseItem);

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

                return GearDescription + GetFormatedWeaponStats(archetypeDataBlock, itemDataBlock, isSentryGun);
            }
        }

        private string VerboseDescriptionFormatter(MeleeArchetypeDataBlock meleeArchetypeDataBlock)
        {
            StringBuilder sb = new StringBuilder();

            switch (meleeArchetypeDataBlock.persistentID)
            {
                case 1:
                    sb.AppendLine("Balanced");
                    break;
                case 2:
                case 4:
                    sb.AppendLine("Fast");
                    break;
                case 3:
                    sb.AppendLine("Slow");
                    break;
            }

            sb.AppendLine(meleeArchetypeDataBlock.CameraDamageRayLength < 1.76?"Short Range": meleeArchetypeDataBlock.CameraDamageRayLength < 3? "Medium Range" : "Long Range");

            sb.Append(meleeArchetypeDataBlock.CanHitMultipleEnemies? "Piercing\n": "");

            return sb.ToString();
        }

        private string VerboseDescriptionFormatter(ArchetypeDataBlock archetypeDataBlock, bool isSentryGun)
        {
            StringBuilder sb = new StringBuilder();

            if (isSentryGun)
                sb.AppendLine("Deployable");

            switch (archetypeDataBlock.FireMode)
            {
                case eWeaponFireMode.Auto:
                case eWeaponFireMode.SentryGunAuto:
                    sb.AppendLine("Fully Automatic (" + archetypeDataBlock.ShotDelay + ")");
                    break;
                case eWeaponFireMode.Semi:
                case eWeaponFireMode.SentryGunSemi:
                    sb.AppendLine("Semi Automatic ("+ archetypeDataBlock.ShotDelay +")");
                    break;
                case eWeaponFireMode.Burst:
                case eWeaponFireMode.SentryGunBurst:
                    if (archetypeDataBlock.BurstShotCount != 1)
                    {
                        sb.Append("Burst FireMode (");
                        sb.Append("#" + archetypeDataBlock.BurstShotCount);
                        sb.Append(DIVIDER);
                        sb.Append(FormatFloat(archetypeDataBlock.BurstDelay + (archetypeDataBlock.ShotDelay * archetypeDataBlock.BurstShotCount)));
                        
                        sb.AppendLine(")");
                    }
                    else
                    {
                        sb.AppendLine("Semi Automatic ("+ archetypeDataBlock.ShotDelay + ")");
                    }
                    break;
                case eWeaponFireMode.SemiBurst:
                    sb.AppendLine("SemiBurst FireMode ("+ archetypeDataBlock.BurstShotCount + DIVIDER + archetypeDataBlock.SpecialSemiBurstCountTimeout +")");
                    break;
                case eWeaponFireMode.SentryGunShotgunSemi:
                    sb.AppendLine("Shotgun FireMode");
                    break;
                default:
                    WeaponStatShowerPlugin.LogError("FireMode not found");
                    break;
            }

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
                    "Long Charge-up (" + FormatFloat(archetypeDataBlock.SpecialChargetupTime) + ")" :
                    "Short Charge-up (" + FormatFloat(archetypeDataBlock.SpecialChargetupTime) + ")");

            return sb.ToString()+"\n";
        }




        private string GetFormatedWeaponStats(ArchetypeDataBlock archeTypeDataBlock, ItemDataBlock itemDataBlock, bool isSentryGun = false)
        {
            if (archeTypeDataBlock == null) return string.Empty;

            StringBuilder builder = new StringBuilder();

            builder.Append("<#9D2929>");
            builder.Append($"{Short_Damage} ");
            builder.Append(FormatFloat(archeTypeDataBlock.Damage));
            builder.Append(archeTypeDataBlock.ShotgunBulletCount > 0? "(x"+ archeTypeDataBlock.ShotgunBulletCount +")" : "");
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
                builder.Append(FormatFloat(archeTypeDataBlock.DefaultReloadTime));
                builder.Append(CLOSE_COLOR_TAG);
            }

            if (!isSentryGun)
                builder.Append("\n");
            else
                builder.Append(DIVIDER);

            if (archeTypeDataBlock.PrecisionDamageMulti != 1f)
            {
                builder.Append("<#18A4A9>");
                builder.Append($"{Short_Precision} ");
                builder.Append(FormatFloat(archeTypeDataBlock.PrecisionDamageMulti));
                builder.Append(CLOSE_COLOR_TAG);
                builder.Append(DIVIDER);
            }


            builder.Append("<#AAA8FF>");
            builder.Append($"{Short_Falloff} ");
            builder.Append(((int)archeTypeDataBlock.DamageFalloff.x).ToString() + "m");
            builder.Append(CLOSE_COLOR_TAG);


            bool nonStandardStagger = archeTypeDataBlock.StaggerDamageMulti != 1f;

            if (nonStandardStagger)
            {
                builder.Append(DIVIDER);

                builder.Append("<color=green>");
                builder.Append($"{Short_Stagger} ");
                builder.Append(FormatFloat(archeTypeDataBlock.StaggerDamageMulti));
                builder.Append(CLOSE_COLOR_TAG);
            }


            bool pierce = archeTypeDataBlock.PiercingBullets;

            if (pierce)
            {
                builder.Append(DIVIDER);

                builder.Append("<#004E2C>");
                builder.Append($"{Short_PierceCount} ");
                builder.Append(archeTypeDataBlock.PiercingDamageCountLimit);
                builder.Append(CLOSE_COLOR_TAG);
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

        private static float FormatFloat(float value)
        {
            return (float)Math.Round((decimal)value, 2);
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
        public static string Short_Falloff { get; } = "Dist";
        public static string Short_Reload { get; } = "Rld";
        public static string Short_Stagger { get; } = "Stgr";
        public static string Short_Precision { get; } = "Prcsn";
        public static string Short_PierceCount { get; } = "Pierc";
        public static string Short_ShotgunPelletCount { get; } = "Pelts";
    }
}
