using System;
using UnityEngine;
using RPG.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,100)]
        LazyValue<int> currentLevel;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        public event Action onLevelUp;
        [SerializeField] bool shouldUseModifiers = false;
        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
                experience.onExperienceGained += UpdateLevel;
        }

        private void OnDisable()
        {
            if (experience != null)
                experience.onExperienceGained -= UpdateLevel;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        internal float GetStat(Stat stat)
        {
            return (progression.GetStat(characterClass, CalculateLevel(), stat) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetPercentageModifier(Stat stat)
        {
            float total = 0;
            if (!shouldUseModifiers) return total;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                    total += modifier;
            return total;
        }

        private float GetAdditiveModifiers(Stat stat)
        {
            float total = 0;
            if (!shouldUseModifiers) return total;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                    total += modifier;
            return total;
        }

        public int GetLevel()
        {
            if (currentLevel.value < 1)
                currentLevel.value = CalculateLevel();
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            float currentXP = GetComponent<Experience>().experiencePoints;
            float penultimateLevel = progression.GetLevels(Stat.Experience, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float xpToLevelUp = progression.GetStat(characterClass, level, Stat.Experience);
                if (currentXP < xpToLevelUp)
                    return level;
            }
            return (int)penultimateLevel + 1;
        }
    }
}
