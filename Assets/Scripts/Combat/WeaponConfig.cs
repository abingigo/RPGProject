using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName ="Weapon", menuName = "Weapons/MakeNewWeapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon weaponPrefab = null;
        public float range;
        public float damage;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform lefthand, Animator animator)
        {
            DestroyOldWeapon(rightHand, lefthand);
            Weapon weapon = null;
            if (weaponPrefab != null)
            {
                weapon = Instantiate(weaponPrefab, isRightHanded ? rightHand : lefthand);
                weapon.gameObject.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
                animator.runtimeAnimatorController = animatorOverride;
            else if(overrideController != null)
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform lefthand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
                oldWeapon = lefthand.Find(weaponName);
            if (oldWeapon == null)
                return;
            oldWeapon.name = "destroyed";
            Destroy(oldWeapon.gameObject);
        }

        public bool hasProjectile()
        {
            return projectile != null;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, isRightHanded ? rightHand.position : leftHand.position, Quaternion.identity);
            projectileInstance.SetTarget(target, calculatedDamage, instigator);
        }
    }
}
