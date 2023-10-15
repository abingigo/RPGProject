using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public enum CursorType
    {
        None,
        Movement,
        Combat,
        UI,
        Pickup
    }

    public class PlayerController : MonoBehaviour
    {

        [Serializable]
        struct CursorMapping
        {
            public CursorType cursorType;
            public Texture2D texture;
            public Vector2 hotspot;
        };

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float maxPathLength = 40f;
        [SerializeField] float raycastRadius = 1f;

        void Update()
        {
            if (InteractWithUI()) return;
            if (GetComponent<Health>().isDead)
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RayCastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if(raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        public bool InteractWithMovement()
        {
            Vector3 target;
            bool hashit = RaycastNavMesh(out target);
            if (hashit)
            {
                if (!GetComponent<MovePlayer>().canMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<MovePlayer>().StartMovement(target);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hashit = Physics.Raycast(GetMouseRay(), out hit);
            if(hashit)
            {
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas))
                {
                    target = navMeshHit.position;
                    NavMeshPath path = new NavMeshPath();
                    if (NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path))
                        if (path.status == NavMeshPathStatus.PathComplete)
                            return GetPathLength(path) <= maxPathLength;
                }
            }
            return false;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float length = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++)
                length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            return length;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        RaycastHit[] RayCastAllSorted()
        {
            RaycastHit[] raycastHits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[raycastHits.Length];
            for(int i = 0; i < raycastHits.Length; i++)
                distances[i] = raycastHits[i].distance;
            Array.Sort(distances, raycastHits);
            return raycastHits;
        }

        private bool InteractWithUI()
        {
            bool value = EventSystem.current.IsPointerOverGameObject();
            if (value)
                SetCursor(CursorType.UI);
            return value;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping mapping in cursorMappings)
            {
                if (mapping.cursorType == type)
                    return mapping;
            }
            return new CursorMapping();
        }
    }
}