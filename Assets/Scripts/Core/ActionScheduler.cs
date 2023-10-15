using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction = null;

        public void StartAction(IAction m)
        {
            if (currentAction == m) return;
            if (currentAction != null)
                currentAction.Cancel();
            currentAction = m;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
