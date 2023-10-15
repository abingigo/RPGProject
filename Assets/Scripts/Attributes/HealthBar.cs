using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas canvas = null;

        void Update()
        {
            foreground.localScale = new Vector3(GetComponent<Health>().GetHealthPoints() / GetComponent<BaseStats>().GetStat(Stat.Health), 1, 1);
            if (GetComponent<Health>().isDead)
                canvas.enabled = false;
        }
    }
}

