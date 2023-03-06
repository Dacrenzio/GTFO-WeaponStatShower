using BepInEx.Configuration;
using CellMenu;
using GameData;
using Gear;
using Player;
using WeaponStatShower.Utils;

namespace WeaponStatShower.Patches
{
    internal class ShowStat : Patch
    {
        private const string PatchName = nameof(ShowStat);
        private const PatchType patchType = PatchType.Postfix;
        private static readonly ConfigDefinition ConfigEnabled = new(PatchName, "Enabled");
        public static Patch Instance { get; private set; }
        public override string Name { get; } = PatchName;
        public override bool Enabled => WeaponStatShowerPlugin.Instance.Config.GetConfigEntry<bool>(ConfigEnabled).Value;

        private static WeaponDescriptionBuilder _weaponDescriptionBuilder;



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
            if (__instance == null) return;

            PlayerDataBlock _playerDataBlock = PlayerDataBlock.GetBlock(1U);

            _weaponDescriptionBuilder = new WeaponDescriptionBuilder(_playerDataBlock, idRange);

            __instance.GearDescription = _weaponDescriptionBuilder.DescriptionFormatter(__instance.GearDescription);
        }
    }
}
