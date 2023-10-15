using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using System.Collections.Generic;
using RPG.Attributes;

namespace RPG.Movement
{
    public class MovePlayer : MonoBehaviour, IAction, ISaveable
    {
        NavMeshAgent meshAgent;
        Health health;
        [SerializeField] float maximumSpeed = 5.66f;
        [SerializeField] float maxPathLength = 40f;

        private void Awake()
        {
            health = GetComponent<Health>();
            meshAgent = GetComponent<NavMeshAgent>();
        }

        public void StartMovement(Vector3 dest, float speedFraction = 1f)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(dest, speedFraction);
        }

        public void MoveTo(Vector3 dest, float speedFraction = 1f)
        {
            meshAgent.speed = maximumSpeed * Mathf.Clamp01(speedFraction);
            meshAgent.isStopped = false;
            meshAgent.destination = dest;
        }

        public void Cancel()
        {
            meshAgent.isStopped = true;
        }

        private void Update()
        {
            meshAgent.enabled = !health.isDead;
            Vector3 velocity = meshAgent.velocity;
            GetComponent<Animator>().SetFloat("Speed", transform.InverseTransformDirection(velocity).z);
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }

        public bool canMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path))
                if (path.status == NavMeshPathStatus.PathComplete)
                    return GetPathLength(path) <= maxPathLength;
            return false;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float length = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++)
                length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            return length;
        }
    }
}