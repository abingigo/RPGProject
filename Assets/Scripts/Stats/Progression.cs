using UnityEngine;
using System.Collections.Generic;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] progressionCharacterClasses = null;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStats[] stats;
            //public float[] health;
        }

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookUpTable = null;

        public float GetStat(CharacterClass characterClass, int level, Stat stat)
        {
            BuildLookup();
            try
            {
                return lookUpTable[characterClass][stat][level - 1];
            } catch
            {
                return 0f;
            }
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            return lookUpTable[characterClass][stat].Length;
        }

        public void BuildLookup()
        {
            if (lookUpTable != null) return;

            lookUpTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass p in progressionCharacterClasses)
            {
                var statLookUpTable = new Dictionary<Stat, float[]>();
                foreach (ProgressionStats ps in p.stats)
                {
                    statLookUpTable[ps.stat] = ps.levels;
                }
                lookUpTable[p.characterClass] = statLookUpTable;
            }
        }
    }

    [System.Serializable]
    public class ProgressionStats
    {
        public Stat stat;
        public float[] levels;
    }
}
