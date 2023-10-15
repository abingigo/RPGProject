using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageText = null;

        public void Spawn(float amount)
        {
            DamageText instance = Instantiate(damageText, this.transform);
            instance.GetComponentInChildren<Text>().text = amount.ToString();
        }
    }
}
