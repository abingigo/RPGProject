using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class Target : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<Health>().isDead)
            {
                if (Input.GetMouseButton(0))
                    callingController.GetComponent<Fighter>().Attack(GetComponent<Health>());
                return true;
            }
            return false;
        }
    }
}
