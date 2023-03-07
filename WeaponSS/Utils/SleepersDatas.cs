using GameData;
using System.Text;

namespace WeaponStatShower.Utils
{
    internal class SleepersDatas
    {
        private readonly Dictionary<string, float[]> EnemyDatas = new Dictionary<string, float[]>();

        public SleepersDatas(string[] activatedSleepers)
        {
            foreach (string monster in activatedSleepers)
            {
                switch (monster.ToUpper().Trim())
                {
                    case "ALL":
                        EnemyDatas.TryAdd("STRK", new float[] { 20, 3, 2 });
                        EnemyDatas.TryAdd("SHTR", new float[] { 30, 5, 2 });
                        EnemyDatas.TryAdd("SCOUT", new float[] { 42, 3, 2 });
                        EnemyDatas.TryAdd("B-STRK", new float[] { 120, 1.5F, 2 });
                        EnemyDatas.TryAdd("B-SHTR", new float[] { 150, 2, 2 });
                        EnemyDatas.TryAdd("CHRG", new float[] { 30, 1, 2 });
                        return;
                    case "STRIKER":
                        EnemyDatas.TryAdd("STRK", new float[] { 20, 3, 2 });
                        break;
                    case "SHOOTER":
                        EnemyDatas.TryAdd("SHTR", new float[] { 30, 5, 2 });
                        break;
                    case "SCOUT":
                        EnemyDatas.TryAdd("SCOUT", new float[] { 42, 3, 2 });
                        break;
                    case "BIG_STRIKER":
                        EnemyDatas.TryAdd("B-STRK", new float[] { 120, 1.5F, 2 });
                        break;
                    case "BIG_SHOOTER":
                        EnemyDatas.TryAdd("B-SHTR", new float[] { 150, 2, 2 });
                        break;
                    case "CHARGER":
                        EnemyDatas.TryAdd("CHRG", new float[] { 30, 1, 2 });
                        break;
                    default:
                        WeaponStatShowerPlugin.LogWarning("You inserted an incorrect value in the config.");
                        break;
                }
            }
        }

        internal string VerboseKill(ArchetypeDataBlock archetypeDataBlock, bool isShotgun)
        {
            StringBuilder builder = new StringBuilder();
            float damage = isShotgun ? archetypeDataBlock.Damage * archetypeDataBlock.ShotgunBulletCount : archetypeDataBlock.Damage;
            int count = 0;

            for (int i = 0; i < EnemyDatas.Count; i++)
            {
                string enemyName = EnemyDatas.Keys.ElementAt(i);
                List<char> killPlace = new List<char>();
                float[] currEnemyDatas = EnemyDatas[enemyName];
                if (damage * currEnemyDatas[1] * currEnemyDatas[2] * archetypeDataBlock.PrecisionDamageMulti >= currEnemyDatas[0])
                {
                    killPlace.Add('o');
                    if (damage * currEnemyDatas[1] >= currEnemyDatas[0])
                    {
                        killPlace.Add('h');
                        if (damage * currEnemyDatas[2] >= currEnemyDatas[0])
                        {
                            killPlace.Add('b');
                            if (damage > currEnemyDatas[0])
                            {
                                killPlace.Add('c');
                            }
                        }
                    }
                }

                if (killPlace.Count > 0)
                {
                    if (count % 2 == 1)
                        builder.Append(" | ");

                    killPlace.Reverse();
                    builder.Append(enemyName + ": [" + string.Join(",", killPlace.ToArray()) + "]");

                    if (count++ % 2 == 1 && i != EnemyDatas.Count - 1)
                    {
                        builder.Append('\n');
                    }
                }
            }

            return builder.ToString();
        }
    }
}
