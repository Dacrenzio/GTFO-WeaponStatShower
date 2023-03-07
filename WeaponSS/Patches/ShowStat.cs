using BepInEx.Configuration;
using CellMenu;
using GameData;
using Gear;
using WeaponStatShower.Utils;

namespace WeaponStatShower.Patches
{
    internal class ShowStat : Patch
    {
        private const string PatchName = nameof(ShowStat);
        private const PatchType patchType = PatchType.Postfix;
        public static Patch Instance { get; private set; }
        public override string Name { get; } = PatchName;

        private static readonly ConfigDefinition ConfigEnabled = new(PatchName, "Enabled");
        private static readonly ConfigDefinition ConfigSleepers = new(PatchName, "SleepersShown");
        public override bool Enabled => WeaponStatShowerPlugin.Instance.Config.GetConfigEntry<bool>(ConfigEnabled).Value;
        private static string ShownSleepers => WeaponStatShowerPlugin.Instance.Config.GetConfigEntry<string>(ConfigSleepers).Value;
        private static WeaponDescriptionBuilder? _weaponDescriptionBuilder;



        public override void Initialize()
        {
            Instance = this;

            WeaponStatShowerPlugin.Instance.Config.Bind(ConfigEnabled, true, new ConfigDescription("Show the stats of a weapon."));
            WeaponStatShowerPlugin.Instance.Config.Bind<string>(ConfigSleepers, "STRIKER, SHOOTER, SCOUT",
                new ConfigDescription("Select which Sleepers are shown, separeted by a comma.\n Acceptable values: ALL, STRIKER, SHOOTER, SCOUT"));
        }

        public override void Execute()
        {
            this.PatchMethod<CM_InventorySlotItem>(nameof(CM_InventorySlotItem.LoadData), patchType);
        }


        public static void CM_InventorySlotItem__LoadData__Postfix(CM_InventorySlotItem __instance, GearIDRange idRange, bool clickable, bool detailedInfo)
        {
            if (__instance == null) return;

            PlayerDataBlock _playerDataBlock = PlayerDataBlock.GetBlock(1U);

            string[] sleepers = ShownSleepers.Trim().Split(',');

            _weaponDescriptionBuilder = new WeaponDescriptionBuilder(_playerDataBlock, idRange, sleepers);

            __instance.GearDescription = _weaponDescriptionBuilder.DescriptionFormatter(__instance.GearDescription);
        }
    }
}
