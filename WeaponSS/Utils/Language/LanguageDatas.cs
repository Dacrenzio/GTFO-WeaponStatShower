using WeaponStatShower.Utils.Language.Models;

namespace WeaponStatShower.Utils.Language
{
    public class LanguageDatas
    {
        public FiremodeLanguageModel firemode { get; set; }
        public MeleeLanguageModel melee { get; set; }
        public SpreadLanguageModel spread { get; set; }
        public SleepersLanguageModel sleepers { get; set; }
        public string damage { get; set; }
        public string clip { get; set; }
        public string maxAmmo { get; set; }
        public string falloff { get; set; }
        public string reload { get; set; }
        public string stagger { get; set; }
        public string precision { get; set; }
        public string pierceCount { get; set; }
        public string rateOfFire { get; set; }
        public string aimDSpread { get; set; }
        public string hipSpread { get; set; }
        public string deployable { get; set; }
        public string longChargeUp { get; set; }
        public string shortChargeUp { get; set; }
    }
}
