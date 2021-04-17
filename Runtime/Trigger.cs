using UnityEngine;
using UnityEngine.Events;

namespace com.gb.statemachine_toolkit
{
    [RequireComponent(typeof(Collider))]
    public class Trigger : MonoBehaviour
    {
        public UnityEvent OnTrigger;
        public bool disableColliderAfterTrigger = false;

        Collider _collider;

        // Start is called before the first frame update
        void Start()
        {
            _collider = this.GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTrigger?.Invoke();
            _collider.enabled = !disableColliderAfterTrigger;
        }
    }
}
