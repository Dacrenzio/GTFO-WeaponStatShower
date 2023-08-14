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
                        EnemyDatas.TryAdd(striker, new float[] { 20, 3, 2, 0 });
                        EnemyDatas.TryAdd(shooter, new float[] { 30, 5, 2, 0 });
                        EnemyDatas.TryAdd(scout, new float[] { 42, 3, 2, 0 });
                        EnemyDatas.TryAdd(big_striker, new float[] { 120, 1.5F, 2, 0 });
                        EnemyDatas.TryAdd(big_shooter, new float[] { 150, 2, 2, 0 });
                        EnemyDatas.TryAdd(charger, new float[] { 30, 1, 2, 1 });
                        EnemyDatas.TryAdd(charger_scout, new float[] { 60, 1, 2, 1 });
                        return;
                    case "STRIKER":
                        EnemyDatas.TryAdd("STRK", new float[] { 20, 3, 2, 0 });
                        break;
                    case "SHOOTER":
                        EnemyDatas.TryAdd("SHTR", new float[] { 30, 5, 2, 0 });
                        break;
                    case "SCOUT":
                        EnemyDatas.TryAdd("SCOUT", new float[] { 42, 3, 2, 0 });
                        break;
                    case "BIG_STRIKER":
                        EnemyDatas.TryAdd("B-STRK", new float[] { 120, 1.5F, 2, 0 });
                        break;
                    case "BIG_SHOOTER":
                        EnemyDatas.TryAdd("B-SHTR", new float[] { 150, 2, 2, 0 });
                        break;
                    case "CHARGER":
                        EnemyDatas.TryAdd("CHRG", new float[] { 30, 1, 2, 1 });
                        break;
                    case "CHARGER_SCOUT":
                        EnemyDatas.TryAdd("C-SCOUT", new float[] { 60, 1, 2, 1 });
                        break;
                    default:
                        WeaponStatShowerPlugin.LogWarning("You inserted an incorrect value in the config: " + monster.Trim());
                        break;
                }
            }
        }

        public string VerboseKill(ArchetypeDataBlock archetypeDataBlock)
        {
            StringBuilder builder = new StringBuilder();
            float damage = archetypeDataBlock.Damage * (archetypeDataBlock.ShotgunBulletCount > 0 ? archetypeDataBlock.ShotgunBulletCount : 1);
            float prcsnMultiplier = archetypeDataBlock.PrecisionDamageMulti;
            int count = 0;

            for (int i = 0; i < EnemyDatas.Count; i++)
            {
                string enemyName = EnemyDatas.Keys.ElementAt(i);
                List<char> killPlace = new List<char>();
                float[] currEnemyDatas = EnemyDatas[enemyName];

                if (canKillOnOccipit(damage, prcsnMultiplier, currEnemyDatas))
                    killPlace.Add(occipit);

                if (canKillOnHead(damage, prcsnMultiplier, currEnemyDatas))
                    killPlace.Add(head);

                if (canKillOnBack(damage, prcsnMultiplier, currEnemyDatas))
                    killPlace.Add(back);

                if (canKillOnChest(damage, currEnemyDatas))
                    killPlace.Add(chest);


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

        internal string? VerboseKill(MeleeArchetypeDataBlock archeTypeDataBlock)
        {
            StringBuilder builder = new StringBuilder();
            float damage = archeTypeDataBlock.ChargedAttackDamage * archeTypeDataBlock.ChargedSleeperMulti;
            float prcsnMultiplier = archeTypeDataBlock.ChargedPrecisionMulti;
            int count = 0;

            for (int i = 0; i < EnemyDatas.Count; i++)
            {
                string enemyName = EnemyDatas.Keys.ElementAt(i);
                if (enemyName.Contains("SCOUT"))
                    damage /= archeTypeDataBlock.ChargedSleeperMulti;

                List<char> killPlace = new List<char>();
                float[] currEnemyDatas = EnemyDatas[enemyName];

                if (canKillOnOccipit(damage, prcsnMultiplier, currEnemyDatas))
                    killPlace.Add(occipit);

                if (canKillOnHead(damage, prcsnMultiplier, currEnemyDatas))
                    killPlace.Add(head);

                if (canKillOnBack(damage, prcsnMultiplier, currEnemyDatas))
                    killPlace.Add(back);

                if (canKillOnChest(damage, currEnemyDatas))
                    killPlace.Add(chest);


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
            return damage >= currEnemyDatas[0];
        }



        private bool canKillOnBack(float damage, float prcsnMultiplier, float[] currEnemyDatas)
        {
            return damage * currEnemyDatas[2] >= currEnemyDatas[0];
        }

        private bool canKillOnHead(float damage, float prcsnMultiplier, float[] currEnemyDatas)
        {
            if (isArmored(currEnemyDatas))
            {
                return damage * currEnemyDatas[1] >= currEnemyDatas[0];
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
                return damage * currEnemyDatas[2] * currEnemyDatas[1] >= currEnemyDatas[0];
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
