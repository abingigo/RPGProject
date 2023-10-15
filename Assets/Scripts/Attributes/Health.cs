using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using RPG.Utils;
using System;
using UnityEngine.Events;

namespace RPG.Attributes
{
    [Serializable]
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float> health;
        public bool isDead = false;
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;

        private void Awake()
        {
            health = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            health.ForceInit();
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void Heal(float healthToRestore)
        {
            health.value = Mathf.Min(health.value + healthToRestore, GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RestoreHealthOnLevelUp;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RestoreHealthOnLevelUp;
        }

        public bool TakeDamage(float damage, GameObject instigator)
        {
            print(gameObject.name + " took damage: " + damage);
            if (isDead) return isDead;
            health.value = Mathf.Max(health.value - damage, 0);
            if(damage != 0)
                takeDamage.Invoke(damage);
            if (health.value == 0)
            {
                GetComponent<Animator>().SetTrigger("Die");
                isDead = true;
                GetComponent<ActionScheduler>().CancelCurrentAction();
                AwardExperience(instigator);
                onDie.Invoke();
            }
            return isDead;
        }

        public float GetHealthPoints()
        {
            return health.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void AwardExperience(GameObject instigator)
        {
            if (instigator == null) return;
            Experience exp = instigator.GetComponent<Experience>();
            if (exp != null)
                exp.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public object CaptureState()
        {
            return health.value;
        }

        public void RestoreState(object state)
        {
            health.value = (float)state;
            TakeDamage(0, null);
        }

        public float GetPercentage()
        {
            return 100 * health.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void RestoreHealthOnLevelUp()
        {
            health.value = Mathf.Max(GetComponent<BaseStats>().GetStat(Stat.Health) * 70 / 100, health.value);
        }
    }
}
