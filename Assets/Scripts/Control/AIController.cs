using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f, shoutDistance = 10f;
        GameObject player;
        LazyValue<Vector3> guardLocation;
        float timeSinceSawPlayer = Mathf.Infinity, suspicionTime = 3f, timeSinceReachedWaypoint = Mathf.Infinity, dwellTime = 2f, timeSinceAggravated = Mathf.Infinity;
        [SerializeField] PatrolPath patrolPath;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.4f, chaseSpeedFraction = 0.8f;
        int waypointNumber = 0;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            guardLocation = new LazyValue<Vector3>(GetPosition);
        }

        private void Start()
        {
            guardLocation.ForceInit();
        }

        Vector3 GetPosition()
        {
            return transform.position;
        }

        private void Update()
        {
            if (GetComponent<Health>().isDead) return;
            if (isAggravated() && GetComponent<Fighter>().CanAttack(player))
            {
                AttackBehavior();
                timeSinceSawPlayer = 0;
            }
            else if (timeSinceSawPlayer < suspicionTime)
                SuspicionBehavior();
            else
                GuardBehavior();
            timeSinceSawPlayer += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        private void GuardBehavior()
        {
            Vector3 nextPosition = guardLocation.value;
            if(patrolPath != null)
            {
                if (AtWayPoint())
                    CycleWaypoint();
                nextPosition = GetCurrentWaypoint();
            }
            GetComponent<MovePlayer>().StartMovement(nextPosition, patrolSpeedFraction);
        }

        private void CycleWaypoint()
        {
            timeSinceReachedWaypoint += Time.deltaTime;
            if (timeSinceReachedWaypoint > dwellTime)
                timeSinceReachedWaypoint = 0;
            else
                return;
            waypointNumber++;
            if (waypointNumber == patrolPath.transform.childCount)
                waypointNumber = 0;
        }

        public void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach(RaycastHit hit in hits)
            {
                hit.collider.GetComponent<AIController>()?.Aggravate();
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.transform.GetChild(waypointNumber).position;
        }

        private bool AtWayPoint()
        {
            return Vector3.Distance(transform.position, patrolPath.transform.GetChild(waypointNumber).position) <= 1f;
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehavior()
        {
            GetComponent<Fighter>().Attack(player.GetComponent<Health>(), chaseSpeedFraction);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        public void Aggravate()
        {
            timeSinceAggravated = 0;
        }

        bool isAggravated()
        {
            return Vector3.Distance(player.transform.position, transform.position) < chaseDistance || timeSinceAggravated < 3f;
        }
    }

}