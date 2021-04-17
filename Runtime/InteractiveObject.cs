using UnityEngine;
using UnityEngine.Events;

namespace com.gb.statemachine_toolkit
{
    [RequireComponent(typeof(Collider))]
    public class InteractiveObject : MonoBehaviour
    {
        public UnityEvent onClick;
        public UnityEvent onAnimationEnded;

        Collider _collider;

        /// <summary>
        /// Enable/Disables the collider on this object, so that it can/cannot be clicked.
        /// If also when disabled it has to block other objects, add another collider as a child, 
        /// without the InteractiveObject component
        /// </summary>
        /// <param name="enable"></param>
        public void Enable(bool enable)
        {
            _collider = GetComponent<Collider>();
            if (!_collider) return;
            _collider.enabled = enable;
        }

        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<Collider>();
        }

        public void Click()
        {
            Debug.Log("Clicked " + this.name);
            onClick?.Invoke();
            _collider.enabled = false;
        }

        public void AnimationEnded()
        {
            onAnimationEnded?.Invoke();
        }
    }
}
