using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Control;
using RPG.Core;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                StartCoroutine(Transition());
        }

        IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);
            GameObject player = GameObject.FindWithTag("Player");
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
            yield return StartCoroutine(FindObjectOfType<Fader>().Fadeout(2f));
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerController>().enabled = false;
            wrapper.Load();
            Portal otherPortal = GetOtherPortal();
            if (otherPortal)
            {
                player.GetComponent<NavMeshAgent>().enabled = false;
                player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
                player.transform.rotation = otherPortal.spawnPoint.rotation;
                player.GetComponent<NavMeshAgent>().enabled = true ;
            }
            wrapper.Save();
            yield return StartCoroutine(FindObjectOfType<Fader>().Fadein(2f));
            player.GetComponent<PlayerController>().enabled = true;
            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = GameObject.FindObjectsOfType<Portal>();
            foreach(Portal p in portals)
            {
                if (p != this && p.destination == this.destination)
                    return p;
            }
            return null;
        }
    }
}

