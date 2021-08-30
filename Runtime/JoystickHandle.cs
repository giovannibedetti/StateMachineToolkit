using UnityEngine;
using UnityEngine.EventSystems;

namespace com.gb.statemachine_toolkit
{
    /// <summary>
    /// A simple Joystick handle. 
    /// Put this script in a gameObject with a graphic element with Raycast target enabled
    /// </summary>
    public class JoystickHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float speedMul = 75.0f;
        public GameObject joystickContainer;
        public GameObject handle;
        public bool active = true;

        public float Horizontal { get { return _dir.x; } }
        public float Vertical { get { return _dir.y; } }
        public Vector2 Direction { get { return _dir; } }

        /// <summary>
        /// Subscribe to this event to receive a clamped (-1, 1) Vector2 based on the handle position.
        /// </summary>
        public event Moved MovedEvent;
        public delegate void Moved(Vector2 dir);

        private bool _pressed;
        private int _touchId;
        private Vector2 _defaultPos;
        private Vector2 _startPos;
        private Vector2 _dir;

        private void Start()
        {
            this._defaultPos = this.handle.transform.position;
        }

        public void Enable(bool enable)
        {
            this.active = enable;
            joystickContainer.SetActive(enable);
        }

        private void Update()
        {
            if (active)
            {
                if (this._pressed)
                {
                    ///Debug.Log($"Application.isMobilePlatform {Application.isMobilePlatform} Input.touchSupported: {Input.touchSupported} Application.isEditor {Application.isEditor}");
                    if (Application.isMobilePlatform && Input.touchSupported && !Application.isEditor)
                    {
                        if (this._touchId < 0) return;
                        var touch = GetTouch(Input.touches, this._touchId);
                        if (touch.fingerId < 0) return;
                        this.handle.transform.position = touch.position; //Input.touches[this._touchId].position;
                    }
                    else
                    {
                        this.handle.transform.position = Input.mousePosition;
                    }

                    //_dir = new Vector2(Mathf.Clamp((this.Handle.transform.position.x - this._defaultPos.x) / speedMul, -1.0f, 1.0f),
                    //                      Mathf.Clamp((this.Handle.transform.position.y - this._defaultPos.y) / speedMul, -1.0f, 1.0f));
                    // Mathf.Clamp throws exceptions on WebGL mobile, rewrite the above without using it

                    var xVal = (this.handle.transform.position.x - this._defaultPos.x) / speedMul;
                    xVal = xVal < -1 ? -1 : xVal;
                    xVal = xVal > 1 ? 1 : xVal;
                    var yVal = (this.handle.transform.position.y - this._defaultPos.y) / speedMul;
                    yVal = yVal < -1 ? -1 : yVal;
                    yVal = yVal > 1 ? 1 : yVal;
                    _dir = new Vector2(xVal, yVal);

                    //invoke the event when moved
                    MovedEvent?.Invoke(_dir);
                    //Debug.Log(_dir);
                }
                else
                {
                    //restore the default position
                    //this.handle.transform.position = this._defaultPos;
                    this.handle.transform.position = this._defaultPos;
                    this._dir = Vector2.zero;
                    //invoke the moved event with a zero Vector2
                    MovedEvent?.Invoke(Vector2.zero);
                }

               // Debug.Log($"[{this.name}] " +
               //$"touches: {Input.touches.Length}," +
               //$"active {this.active} " +
               //$"_pressed {_pressed}, " +
               //$"_touchId {_touchId}, " +
               //$"_startPos {_startPos}, " +
               //$"_dir {_dir}");
            }

            // hack to avoid pointer remaining stucked in wrong positions
            if (Input.touches.Length == 0)
            {
                //restore the default position
                this._pressed = false;
                this._touchId = -1;
                this._startPos = Vector2.zero;
                this.handle.transform.position = this._defaultPos;
                this._dir = Vector2.zero;

                //invoke the moved event with a zero Vector2
                MovedEvent?.Invoke(Vector2.zero);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!this.active) return;
            Debug.Log($"{this.name} OnPointerDown, pointerId: {eventData.pointerId}");
            if (this._pressed) return;

            this._pressed = true;
            this._touchId = eventData.pointerId;
            this._startPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!this.active) return;
            Debug.Log($"{this.name} OnPointerUp, pointerId: {eventData.pointerId}");
            if (eventData.pointerId != this._touchId) return;

            this._pressed = false;
            this._touchId = -1;
            this._startPos = Vector2.zero;
        }

        /// <summary>
        /// Hack to restore the position to zero when resolution changes. 
        /// Can cause an unwanted position when the starting position is not Vector2.zero
        /// Basically it assumes the handle is centered in the container, with no margins
        /// </summary>
        public void OnResolutionChanged()
        {
            this.handle.transform.position = joystickContainer.transform.position; //Vector2.zero;
            this._defaultPos = this.handle.transform.position;
            Debug.Log($"OnResolutionChanged: defaultPos: {_defaultPos}, handle localPos: {this.handle.GetComponent<RectTransform>().localPosition}, handle pos: {this.handle.transform.position} joystickContainer.transform.position: {joystickContainer.transform.position}");
        }

        private Touch GetTouch(Touch[] touches, int id)
        {
            Touch t = new Touch() { fingerId = -1, position = Vector2.zero };
            foreach (var touch in touches)
            {
                if (!touch.fingerId.Equals(id))
                    continue;
                Debug.Log($"Found touch id: {id}");
                t = touch;
                return t;
            }
            return t;
        }
    }
}
