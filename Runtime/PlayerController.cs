using UnityEngine;
using UnityEngine.UI;

namespace com.gb.statemachine_toolkit
{
    public class PlayerController : MonoBehaviour
    {
        public bool interactionActive;
        public bool movementActive;
        public bool lookActive;

        [Header("INTERACTION")]
        [Tooltip("The icon animator, to be able to show it when over an InteractiveObject")]
        public Animator iconAnimator;
        [Tooltip("This is the name of the bool parameter used in the IconAnimator")]
        public string iconAnimatorBool = "IsVisible";
        [Tooltip("The cursor that shows where the player is looking at")]
        public GameObject cursor;
        [Tooltip("The button used to click on mobile platforms. It will be activated or not depending on the run platform.")]
        public Button mobileButton;
        [Tooltip("The maximum distance at which an object can be detected")]
        public float maxDistance = 10f;
        [Tooltip("This is the layer that will be tested by the raycast. " +
            "Note that the 'Default' Layer is always tested together with the choosen one. " +
            "This is necessary for example to avoid detecting an object through a wall.")]
        public LayerMask layerMask;
        private Transform selected = null;
        private bool mobileClicked;

        [Header("MOVEMENT")]
        [Tooltip("The component that will manage the movement in mobile devices. Warning, mobile devices have some issues in WebGL, they are not officially supported!")]
        public JoystickHandle movementJoystick;
        [Tooltip("The Character Controller associated with this Player. This is needed to handle the Player movement without using a RigidBody.")]
        public CharacterController controller;
        [Tooltip("The minimum distance from the ground that the player will be placed vertically")]
        public float groundDistance = 0.2f;
        [Tooltip("The gravity force that affects the speed of the vertical movement. It doesn't use Physics but it is possible for the Player to fall until the Player is at \"groundDistance\" from the ground.")]
        public float gravity = -9.81f;
        [Tooltip("A transform placed inside the player that will be used to check the distance to the ground.")]
        public Transform groundChecker;
        [Tooltip("The LayerMask of the ground, to be able to detect when the player is grounded.")]
        public LayerMask groundMask;
        [Tooltip("The movement speed of the player.")]
        public float speed = 6f;
        private bool _isGrounded;
        private Vector3 _velocity;

        [Header("LOOK")]
        [Tooltip("The component that will manage the camera movement in mobile devices. Warning, mobile devices have some issues in WebGL, they are not officially supported!")]
        public JoystickHandle lookJoystick;
        [Tooltip("The sensitivity of the mouse. The higher it is, the more responsive is the camera rotation.")]
        public float mouseSensitivity = 50f;
        [Tooltip("The transform of the camera inside the Player.")]
        public Transform playerCamera;
        private float xRotation = 0f;

        [Header("DEBUG")]
        [Tooltip("Select this to print debug information")]
        public bool debug;

        public bool IsMobile { get { return Application.isMobilePlatform && Input.touchSupported; } }

        // Start is called before the first frame update
        void Start()
        {
            if (IsMobile)
            {
                if (mobileButton)
                {
                    mobileButton.gameObject.SetActive(interactionActive);
                    mobileButton.onClick.AddListener(() =>
                     {
                         if (interactionActive)
                             mobileClicked = true;
                     });
                }
                else
                {
                    Debug.LogWarning("Mobile button not assigned in PlayerController, on mobile platforms clicking objects won't be possible!");
                }
                if (movementJoystick)
                    movementJoystick.Enable(true);
                else Debug.LogWarning("Movement Joystick not assigned in PlayerController, on mobile platforms movement won't be possible!");
                if (lookJoystick) lookJoystick.Enable(true);
                else Debug.LogWarning("Movement Joystick not assigned in PlayerController, on mobile platforms looking around won't be possible!");
            }
            else
            {
                if (mobileButton) mobileButton.gameObject.SetActive(false);
                if (movementJoystick) movementJoystick.Enable(false);
                if (lookJoystick) lookJoystick.Enable(false);
            }
        }

        public void LockView(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            lookActive = locked;
            playerCamera.localRotation = Quaternion.identity;
        }

        // Update is called once per frame
        void Update()
        {
            _isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);
            if (_isGrounded && _velocity.y < 0)
                _velocity.y = 0f;
            _velocity.y += gravity * Time.deltaTime;

            if (interactionActive)
            {
                cursor.SetActive(true);

                if (IsMobile)
                    if (mobileButton)
                        mobileButton.gameObject.SetActive(true);

                RaycastHit hit;
                var mask = LayerMask.GetMask("Default") | LayerMask.GetMask("Interactable");//1 << layerMask;
                var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
                if (Physics.Raycast(ray, out hit, maxDistance, mask))
                {
                    if (debug) Debug.Log("Hitting " + hit.transform.name);

                    InteractiveObject io = null;
                    if (selected != hit.transform)
                    {
                        selected = hit.transform;
                        io = hit.transform.GetComponent<InteractiveObject>();
                        SetIconVisibility(io != null);
                    }

                    if (IsMobile)
                    {
                        if (mobileClicked)
                        {
                            mobileClicked = false;
                            ClickObject(selected);
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            ClickObject(selected);
                        }
                    }
                }
                else
                {
                    if (debug) Debug.Log("Nothing to interact with");
                    if (selected != null)
                        SetIconVisibility(false);
                    selected = null;
                }
            }
            else
            {
                cursor.SetActive(false);
                SetIconVisibility(false);
                if (IsMobile) if (mobileButton) mobileButton.gameObject.SetActive(false);
            }

            // MOVEMENT 
            if (movementActive)
            {
                if (IsMobile) if (movementJoystick) movementJoystick.Enable(true);

                var x = IsMobile ? movementJoystick != null ? movementJoystick.Horizontal : 0f : Input.GetAxis("Horizontal");
                var z = IsMobile ? movementJoystick != null ? movementJoystick.Vertical : 0f : Input.GetAxis("Vertical");
                var move = transform.right * x + transform.forward * z;
                move.y = _velocity.y;
                controller.Move(move * speed * Time.deltaTime);
            }
            else
            {
                if (IsMobile) if (movementJoystick) movementJoystick.Enable(false);
            }

            // LOOK
            if (lookActive)
            {
                if (IsMobile) if (lookJoystick) lookJoystick.Enable(true);

                var inputX = IsMobile ? lookJoystick.Horizontal : Input.GetAxis("Mouse X");
                var inputY = IsMobile ? lookJoystick.Vertical : Input.GetAxis("Mouse Y");

                var x = inputX * mouseSensitivity * Time.deltaTime;
                var y = inputY * mouseSensitivity * Time.deltaTime;

                xRotation -= y;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * x);
            }
            else
            {
                if (IsMobile)
                    if (lookJoystick)
                        lookJoystick.Enable(false);
            }
        }

        private void ClickObject(Transform selected)
        {
            var io = selected.GetComponent<InteractiveObject>();
            if (io != null)
                io.Click();
        }

        private void SetIconVisibility(bool visible)
        {
            iconAnimator.SetBool(iconAnimatorBool, visible);
        }
    }
}
