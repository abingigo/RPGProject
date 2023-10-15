using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                int j = i + 1;
                if (j == transform.childCount) j = 0;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.GetChild(i).position, 0.3f);
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(j).position);
            }
        }
    }
}
