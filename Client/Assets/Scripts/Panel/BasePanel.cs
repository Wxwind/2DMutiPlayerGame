
using UnityEngine;

namespace Panel
{
    class BasePanel:MonoBehaviour
    {
        public virtual void OnHideAndFreeze()
        {
            gameObject.SetActive(false);
            Debug.Log($"{GetType().Name} is Hide");
        }

        public virtual void OnRecovery()
        {
            gameObject.SetActive(true);
            Debug.Log($"{GetType().Name} is Recovery");
        }

        public virtual void OnEnter()
        {
            gameObject.SetActive(true);
            Debug.Log($"{GetType().Name} is Enter/Init");
        }

        public virtual void OnExit()
        {
            gameObject.SetActive(false);
            Debug.Log($"{GetType().Name} is Exit");
        }
    }
}