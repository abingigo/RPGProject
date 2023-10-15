using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health target = null;
        [SerializeField] float speed = 1f;
        float damage = 1f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] GameObject[] destroyOnHit = null;
        GameObject instigator = null;
        [SerializeField] UnityEvent onHit;

        private void Start()
        {
            Destroy(gameObject, 7f);
        }

        private void Update()
        {
            if (target)
            {
                if(isHoming && !target.isDead)
                    transform.LookAt(getAimLocation());
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }

        public void SetTarget(Health target, float damage, GameObject instigator)
        {
            this.target = target;
            this.damage = damage;
            transform.LookAt(getAimLocation());
            this.instigator = instigator;
        }

        private Vector3 getAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) 
                return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Health>() != target) return;
            if (target.isDead) return;
            target.TakeDamage(damage, instigator);
            speed = 0;
            onHit.Invoke();
            if(hitEffect != null)
                Instantiate(hitEffect, getAimLocation(), transform.rotation);
            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }    
            Destroy(gameObject, 0.2f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }
    }
}
