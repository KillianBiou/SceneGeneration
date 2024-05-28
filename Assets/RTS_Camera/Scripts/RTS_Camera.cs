using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

namespace RTS_Cam
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RTS Camera")]
    public class RTS_Camera : MonoBehaviour
    {

        #region Foldouts

#if UNITY_EDITOR

        public int lastTab = 0;

        public bool movementSettingsFoldout;
        public bool zoomingSettingsFoldout;
        public bool rotationSettingsFoldout;
        public bool heightSettingsFoldout;
        public bool mapLimitSettingsFoldout;
        public bool targetingSettingsFoldout;
        public bool inputSettingsFoldout;

#endif

        #endregion

        private Transform m_Transform; //camera tranform
        public bool useFixedUpdate = false; //use FixedUpdate() or Update()

        #region Movement

        public float minKeyboardMovementSpeed = 5f; //speed with keyboard movement
        public float maxKeyboardMovementSpeed = 5f; //speed with keyboard movement
        public float screenEdgeMovementSpeed = 3f; //spee with screen edge movement
        public float followingSpeed = 5f; //speed when following a target
        public float rotationSped = 3f;
        public float panningSpeed = 10f;
        public float mouseRotationSpeed = 10f;

        #endregion

        #region Height

        public bool autoHeight = true;
        public LayerMask groundMask = -1; //layermask of ground or other objects that affect height

        public float maxHeight = 10f; //maximal height
        public float minHeight = 15f; //minimnal height
        [Range(0f, 90f)]
        public float maxHeightCameraAngle = 90f;
        [Range(0f, 90f)]
        public float minHeightCameraAngle = 15f;
        public float heightDampening = 5f; 
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 25f;

        private float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

        #endregion

        #region MapLimits

        public bool limitMap = true;
        public float limitX = 50f; //x limit of map
        public float limitY = 50f; //z limit of map

        #endregion

        #region Targeting

        public Transform targetFollow; //target to follow
        public Vector3 targetOffset;

        /// <summary>
        /// are we following target
        /// </summary>
        public bool FollowingTarget
        {
            get
            {
                return targetFollow != null;
            }
        }

        #endregion

        #region Input

        public bool useScreenEdgeInput = true;
        public float screenEdgeBorder = 25f;

        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        public bool usePanning = true;
        public KeyCode panningKey = KeyCode.Mouse2;

        public bool useKeyboardZooming = true;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;

        public bool useScrollwheelZooming = true;
        public string zoomingAxis = "Mouse ScrollWheel";

        public bool useKeyboardRotation = true;
        public KeyCode rotateRightKey = KeyCode.X;
        public KeyCode rotateLeftKey = KeyCode.Z;

        public bool useMouseRotation = true;
        public KeyCode mouseRotationKey = KeyCode.Mouse1;

        public float longClickThreshold = 0.1f;
        public UnityEvent OnShortClickRoom;
        public UnityEvent OnShortClick2D;
        public UnityEvent OnShortClick3D;

        private int cursorLockRequest = 0;
        private bool longClick = false;
        private bool hasClicked = false;
        private bool lockPan = false;
        private float totalDownTime = 0f;
        private bool onUI = false;
        public RectTransform popUp;

        private Vector2 KeyboardInput
        {
            get { return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero; }
        }

        private Vector2 MouseInput
        {
            get { return Input.mousePosition; }
        }

        private float ScrollWheel
        {
            get { return -Input.GetAxis(zoomingAxis); }
        }

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);
                if (zoomIn && zoomOut)
                    return 0;
                else if (!zoomIn && zoomOut)
                    return 1;
                else if (zoomIn && !zoomOut)
                    return -1;
                else 
                    return 0;
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKey(rotateRightKey);
                bool rotateLeft = Input.GetKey(rotateLeftKey);
                if(rotateLeft && rotateRight)
                    return 0;
                else if(rotateLeft && !rotateRight)
                    return -1;
                else if(!rotateLeft && rotateRight)
                    return 1;
                else 
                    return 0;
            }
        }

        #endregion

        #region Unity_Methods

        private void Start()
        {
            m_Transform = transform;
        }

        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }

        #endregion

        #region RTSCamera_Methods

        /// <summary>
        /// update camera movement and rotation
        /// </summary>
        private void CameraUpdate()
        {

            /*if (FollowingTarget)
                FollowTarget();
            else
                Move();*/

            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            onUI = false;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    onUI = true;
                }
            }

            if (onUI)
                return;

            Move();

            HeightCalculation();
            Rotation();
            LimitPosition();

            CheckCursorLock();
            CheckClick();
            ActionUIHandle();
        }

        private void ActionUIHandle()
        {
            RectTransform canvasRectTransform = popUp.GetComponentInParent<Canvas>().transform as RectTransform;
            Vector2 canvasPositionCursor;
            Vector2 canvasPositionUI;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, null, out canvasPositionCursor))
            {
                // Convert UI element position to canvas space
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, popUp.position, null, out canvasPositionUI))
                {
                    // Calculate the distance between cursor and UI element
                    // X Axis
                    if(Mathf.Abs(canvasPositionCursor.x - canvasPositionUI.x) > popUp.rect.width * 1.5f)
                    {
                        popUp.gameObject.SetActive(false);
                    }
                    // Y Axis
                    if (Mathf.Abs(canvasPositionCursor.y - canvasPositionUI.y) > popUp.rect.height * 1.5f)
                    {
                        popUp.gameObject.SetActive(false);
                    }

                }
            }
        }

        private void CheckClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                totalDownTime = 0;
                hasClicked = true;
            }

            if (hasClicked && Input.GetMouseButton(0))
            {
                totalDownTime += Time.deltaTime;

                if (totalDownTime >= longClickThreshold)
                {
                    longClick = true;
                }
            }

            if(hasClicked && Input.GetMouseButtonUp(0))
            {
                if (!longClick)
                {
                    Debug.Log("Short Click");
                    OnShortClickTrigger();
                }
                longClick = false;
            }
        }

        private void OnShortClickTrigger()
        {
            OnShortClick3D.Invoke();
            // DIFFERENCIATE FROM SELECTION MODE
            /*switch (ToolMenu.Instance.currentState)
            {
                case ToolSelectionState.MODE_ROOM:
                    OnShortClickRoom.Invoke();
                    break;
                case ToolSelectionState.MODE_2D:
                    OnShortClick2D.Invoke();
                    break;
                case ToolSelectionState.MODE_3D:
                    OnShortClick3D.Invoke();
                    break;
                default:
                    break;
            }*/
        }

        public void SetPopUp()
        {
            Vector3 mousePosition = Input.mousePosition;

            // Create a ray from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                popUp.gameObject.SetActive(true);
                popUp.GetComponent<GenerationPopUp>().point = hit.point;

                RectTransform canvasRectTransform = popUp.GetComponentInParent<Canvas>().transform as RectTransform;
                Vector2 canvasPosition;

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePosition, null, out canvasPosition))
                {
                    // Set the position of the UI element
                    popUp.localPosition = canvasPosition;
                }
            }
        }

        /// <summary>
        /// move camera with keyboard or with screen edge
        /// </summary>
        private void Move()
        {
            if (useKeyboardInput)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Vector3 desiredMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);

                desiredMove *= Mathf.Lerp(minKeyboardMovementSpeed, maxKeyboardMovementSpeed, (DistanceToGround() - minHeight) / (maxHeight - minHeight));
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (useScreenEdgeInput)
            {
                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }       
        
            if(usePanning && Input.GetKey(panningKey) && longClick && MouseAxis != Vector2.zero)
            {
                Vector3 desiredMove = new Vector3(-MouseAxis.x, 0, -MouseAxis.y);

                desiredMove *= panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }
        }

        /// <summary>
        /// calcualte height
        /// </summary>
        private void HeightCalculation()
        {
            float distanceToGround = DistanceToGround();
            if(useScrollwheelZooming)
                zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            if (useKeyboardZooming)
                zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;

            zoomPos = Mathf.Clamp01(zoomPos);

            float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomPos);
            float difference = 0; 

            if(distanceToGround != targetHeight)
                difference = targetHeight - distanceToGround;

            // Camera angle depending on height
            m_Transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(minHeightCameraAngle, m_Transform.rotation.eulerAngles.y, m_Transform.rotation.eulerAngles.z), 
                                                                 new Vector3(maxHeightCameraAngle, m_Transform.rotation.eulerAngles.y, m_Transform.rotation.eulerAngles.z), 
                                                                 (distanceToGround - minHeight) / (maxHeight - minHeight)));

            m_Transform.position = Vector3.Lerp(m_Transform.position, 
                new Vector3(m_Transform.position.x, targetHeight + difference, m_Transform.position.z), Time.deltaTime * heightDampening);
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        private void Rotation()
        {
            if(useKeyboardRotation)
                transform.Rotate(Vector3.up, RotationDirection * Time.deltaTime * rotationSped, Space.World);

            if (useMouseRotation && Input.GetKey(mouseRotationKey))
            {
                m_Transform.Rotate(Vector3.up, -MouseAxis.x * Time.deltaTime * mouseRotationSpeed, Space.World);
            }
        }

        private void CheckCursorLock()
        {
            // Rotation
            if (Input.GetKeyDown(mouseRotationKey))
                cursorLockRequest++;
            if (Input.GetKeyUp(mouseRotationKey))
                cursorLockRequest--;

            // Panning
            /*if (longClick && !lockPan)
            {
                lockPan = true;
                cursorLockRequest++;
            }
            if (Input.GetKeyUp(panningKey) && lockPan)
            {
                lockPan = false;
                cursorLockRequest--;
            }*/

            Mathf.Clamp(cursorLockRequest,0, Mathf.Infinity);

            Cursor.lockState = cursorLockRequest > 0 ? CursorLockMode.Locked : CursorLockMode.None;
        }

        /// <summary>
        /// follow targetif target != null
        /// </summary>
        private void FollowTarget()
        {
            Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z) + targetOffset;
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
        }

        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            if (!limitMap)
                return;
                
            m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, -limitX, limitX),
                m_Transform.position.y,
                Mathf.Clamp(m_Transform.position.z, -limitY, limitY));
        }

        /// <summary>
        /// set the target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            targetFollow = target;
        }

        /// <summary>
        /// reset the target (target is set to null)
        /// </summary>
        public void ResetTarget()
        {
            targetFollow = null;
        }

        /// <summary>
        /// calculate distance to ground
        /// </summary>
        /// <returns></returns>
        private float DistanceToGround()
        {
            Ray ray = new Ray(m_Transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, groundMask.value))
                return (hit.point - m_Transform.position).magnitude;

            return 0f;
        }

        #endregion
    }
}