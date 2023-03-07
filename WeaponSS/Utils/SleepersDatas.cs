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
                switch (monster.Trim())
                {
                    case "NONE": 
                        EnemyDatas.Clear(); //just to be sure
                        return;
                    case "ALL":
                        EnemyDatas.TryAdd("STRK", new float[] { 20, 3, 2 , 0 });
                        EnemyDatas.TryAdd("SHTR", new float[] { 30, 5, 2 , 0 });
                        EnemyDatas.TryAdd("SCOUT", new float[] { 42, 3, 2 , 0 });
                        EnemyDatas.TryAdd("B-STRK", new float[] { 120, 1.5F, 2, 0 });
                        EnemyDatas.TryAdd("B-SHTR", new float[] { 150, 2, 2, 0 });
                        EnemyDatas.TryAdd("CHRG", new float[] { 30, 1, 2, 1 });
                        EnemyDatas.TryAdd("C-SCOUT", new float[] { 60, 1, 2, 1 });
                        return;
                    case "STRIKER":
                        EnemyDatas.TryAdd("STRK", new float[] { 20, 3, 2 ,0});
                        break;
                    case "SHOOTER":
                        EnemyDatas.TryAdd("SHTR", new float[] { 30, 5, 2 ,0});
                        break;
                    case "SCOUT":
                        EnemyDatas.TryAdd("SCOUT", new float[] { 42, 3, 2 , 0 });
                        break;
                    case "BIG_STRIKER":
                        EnemyDatas.TryAdd("B-STRK", new float[] { 120, 1.5F, 2 , 0 });
                        break;
                    case "BIG_SHOOTER":
                        EnemyDatas.TryAdd("B-SHTR", new float[] { 150, 2, 2 , 0 });
                        break;
                    case "CHARGER":
                        EnemyDatas.TryAdd("CHRG", new float[] { 30, 1, 2 , 1 });
                        break;
                    case "CHARGER_SCOUT":
                        EnemyDatas.TryAdd("C-SCOUT", new float[] { 60, 1, 2 , 1 });
                        break;
                    default:
                        WeaponStatShowerPlugin.LogWarning("You inserted an incorrect value in the config: " + monster.Trim());
                        break;
                }
            }
        }

        public string VerboseKill(ArchetypeDataBlock archetypeDataBlock, bool isShotgun)
        {
            StringBuilder builder = new StringBuilder();
            float damage = isShotgun ? archetypeDataBlock.Damage * archetypeDataBlock.ShotgunBulletCount : archetypeDataBlock.Damage;
            float prcsnMultiplier = archetypeDataBlock.PrecisionDamageMulti;
            int count = 0;

            for (int i = 0; i < EnemyDatas.Count; i++)
            {
                string enemyName = EnemyDatas.Keys.ElementAt(i);
                List<char> killPlace = new List<char>();
                float[] currEnemyDatas = EnemyDatas[enemyName];
                
                if (canKillOnOccipit(damage, prcsnMultiplier,currEnemyDatas))
                    killPlace.Add('o');

                if (canKillOnHead(damage, prcsnMultiplier, currEnemyDatas))
                    killPlace.Add('h');

                if (canKillOnBack(damage, prcsnMultiplier, currEnemyDatas))
                    killPlace.Add('b');

                if (canKillOnChest(damage, currEnemyDatas))
                    killPlace.Add('c');


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

        private bool canKillOnChest(float damage, float[] currEnemyDatas)
        {
            if (isArmored(currEnemyDatas))
            {
                return damage * 0.7 >= currEnemyDatas[0];
            }
            else
            {
                return damage >= currEnemyDatas[0];
            }
        }

        

        private bool canKillOnBack(float damage, float prcsnMultiplier, float[] currEnemyDatas)
        {
            if (isArmored(currEnemyDatas))
            {
                return damage * prcsnMultiplier * currEnemyDatas[2] >= currEnemyDatas[0];
            }
            else
            {
                return damage * currEnemyDatas[2] >= currEnemyDatas[0];
            }
        }

        private bool canKillOnHead(float damage, float prcsnMultiplier, float[] currEnemyDatas)
        {
            if (isArmored(currEnemyDatas))
            {
                return damage * 0.7 * currEnemyDatas[1] >= currEnemyDatas[0];
            }
            else
            {
                return damage * prcsnMultiplier * currEnemyDatas[1] >= currEnemyDatas[0];
            }
        }

        private bool canKillOnOccipit(float damage, float prcsnMultiplier, float[] currEnemyDatas)
        {
            if (isArmored(currEnemyDatas))
            {
                return damage * 0.7 * currEnemyDatas[2] * currEnemyDatas[1] >= currEnemyDatas[0];
            }
            else
            {
                return damage * prcsnMultiplier * currEnemyDatas[2] * currEnemyDatas[1] >= currEnemyDatas[0];

            }
        }

        private bool isArmored(float[] currEnemyDatas)
        {
            return currEnemyDatas[3] == 1f;
        }
    }
}
