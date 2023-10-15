using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using RPG.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        Health target;
        [SerializeField] float timeBetweenAttacks = 5f;
        float timeSinceLastAttack = Mathf.Infinity;
        float chaseSpeedFraction;
        [SerializeField] Transform rightHandTransform = null, leftHandTransform = null;
        private WeaponConfig currentWeaponConfig = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        Weapon SetupDefaultWeapon()
        {
            return EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target != null && !target.isDead)
            {
                if (GetIsInRange(target.transform))
                {
                    transform.LookAt(target.transform);
                    GetComponent<MovePlayer>().Cancel();
                    AttackBehaviour();
                }
                else
                    GetComponent<MovePlayer>().MoveTo(target.transform.position, chaseSpeedFraction);
            }
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(targetTransform.position, transform.position) <= currentWeaponConfig.range;
        }

        public Weapon EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
            return currentWeapon.value;
        }

        public Health GetTarget()
        {
            return target;
        }

        public bool CanAttack(GameObject t)
        {
            return t != null && !t.GetComponent<Health>().isDead && (GetComponent<MovePlayer>().canMoveTo(t.transform.position) || GetIsInRange(t.transform));
        }

        private void AttackBehaviour()
        {
            if(timeSinceLastAttack >= timeBetweenAttacks)
            {
                GetComponent<Animator>().ResetTrigger("StopAttack");
                GetComponent<Animator>().SetTrigger("Attack");
                timeSinceLastAttack = 0;
            }
        }

        public void Attack(Health t, float chaseFraction = 1f)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = t;
            chaseSpeedFraction = chaseFraction;
        }

        public void Cancel()
        {
            GetComponent<Animator>().SetTrigger("StopAttack");
            target = null;
            GetComponent<MovePlayer>().Cancel();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
                yield return currentWeaponConfig.damage;
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
                yield return currentWeaponConfig.GetPercentageBonus();
        }

        //Animation event
        void Hit()
        {
            if (target == null) return;
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeapon.value != null)
                currentWeapon.value.OnHit();
            if (currentWeaponConfig.hasProjectile())
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            else if (target.TakeDamage(damage, gameObject))
                Cancel();
        }

        void Shoot()
        {
            Hit();
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            WeaponConfig weapon = Resources.Load<WeaponConfig>(state as string);
            EquipWeapon(weapon);
        }
    }
}
