using UnityEngine.UI;
using UnityEngine;
using System;
using RPG.Combat;

namespace RPG.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health health;
        [SerializeField] Text text;

        private void Update()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Fighter>().GetTarget();
            if (health != null)
                text.text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
            else
                text.text = "N/A";
        }
    }
}