using BepInEx.Configuration;

namespace WeaponStatShower
{
    public static class BIEExtensions
    {
        public static ConfigEntry<T> GetConfigEntry<T>(this ConfigFile configFile, ConfigDefinition definition)
        {
            if (!configFile.TryGetEntry<T>(definition, out var entry))
            {
                throw new InvalidOperationException("Config entry has not been added yet.");
            }

            return entry;
        }
    }
}