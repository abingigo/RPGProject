using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        public object CaptureState()
        {
            return GetComponent<BoxCollider>().enabled;
        }

        public void RestoreState(object state)
        {
            GetComponent<BoxCollider>().enabled = (bool)state;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
