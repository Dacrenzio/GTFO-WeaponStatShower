using BepInEx.Configuration;
using CellMenu;
using GameData;
using Gear;
using WeaponStatShower.Utils;
using WeaponStatShower.Utils.Language;

namespace WeaponStatShower.Patches
{
    internal class ShowStat : Patch
    {
        private const string PatchName = nameof(ShowStat);
        private const PatchType patchType = PatchType.Postfix;
        public static Patch Instance { get; private set; }
        public override string Name { get; } = PatchName;

        private static readonly ConfigDefinition ConfigEnabled = new(PatchName, "Enabled");
        private static readonly ConfigDefinition Language = new(PatchName, "SleepersShown");
        private static readonly ConfigDefinition ConfigSleepers = new(PatchName, "SleepersShown");

        private static LanguageEnum LanguageValue;
        private static string? CurrShownSleepers;
        private static string PrevShownSleepers = "PLACEHOLDER";

        private static WeaponDescriptionBuilder? _weaponDescriptionBuilder;



        public override void Initialize()
        {
            Instance = this;
            WeaponStatShowerPlugin.Instance.Config.Bind(ConfigEnabled, true, new ConfigDescription("Show the stats of a weapon."));
            WeaponStatShowerPlugin.Instance.Config.Bind(Language, LanguageEnum.English, new ConfigDescription("Select the mod language."));
            WeaponStatShowerPlugin.Instance.Config.Bind<string>(ConfigSleepers, "ALL",
                new ConfigDescription("Select which Sleepers are shown, separeted by a comma.\n" +
                "Acceptable values: ALL, NONE, STRIKER, SHOOTER, SCOUT, BIG_STRIKER, BIG_SHOOTER, CHARGER, CHARGER_SCOUT"));

            _weaponDescriptionBuilder = new WeaponDescriptionBuilder();
        }

        public override void Execute()
        {
            this.PatchMethod<CM_InventorySlotItem>(nameof(CM_InventorySlotItem.LoadData), patchType);
        }


        public static void CM_InventorySlotItem__LoadData__Postfix(CM_InventorySlotItem __instance, GearIDRange idRange, bool clickable, bool detailedInfo)
        {
            if (__instance == null || !detailedInfo) return;
            if (_weaponDescriptionBuilder == null)
            {
                WeaponStatShowerPlugin.LogError("Something went wrong with the DescriptionBuilder");
                return;
            }

            WeaponStatShowerPlugin.Instance.Config.Reload();
            CurrShownSleepers = WeaponStatShowerPlugin.Instance.Config.GetConfigEntry<string>(ConfigSleepers).Value.Trim().ToUpper();
            LanguageValue = WeaponStatShowerPlugin.Instance.Config.GetConfigEntry<LanguageEnum>(Language).Value;

            if (!PrevShownSleepers.Equals(CurrShownSleepers))
            {
                _weaponDescriptionBuilder.UpdateSleepersDatas(CurrShownSleepers.Split(','), LanguageValue);
                PrevShownSleepers = CurrShownSleepers;
            }

            _weaponDescriptionBuilder.Inizialize(idRange, PlayerDataBlock.GetBlock(1U));

            __instance.GearDescription = _weaponDescriptionBuilder.DescriptionFormatter(__instance.GearDescription, LanguageValue);
            __instance.GearPublicName = _weaponDescriptionBuilder.FireRateFormatter(__instance.GearPublicName, LanguageValue);
        }
    }
}
