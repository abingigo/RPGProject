using UnityEngine.UI;
using UnityEngine;
using System;

namespace RPG.Stats
{
    public class ExpDisplay : MonoBehaviour
    {
        Experience exp;
        [SerializeField] Text text;

        private void Awake()
        {
            exp = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            text.text = exp.experiencePoints.ToString();
        }
    }
}