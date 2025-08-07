//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class InputMoveWithFinger : MonoBehaviour, IDragHandler
//{
//    [SerializeField] private Canvas canvas;
//    [SerializeField] private RectTransform movableUIObject;
//    private InputManager IM;
//    private RectTransform rectTransform;
//    private bool isDragging;
//    private Vector2 startPosition;
//    private Vector2 pointerDownPosition;
//    private Vector2 localPoint;
//    private Vector2 delta;
//    private Vector2 movementInput;

//    [SerializeField] private float movementRange = 50f;

//    private bool hasTouchscreen;
//    //private bool joystickMoved;
//    void Awake()
//    {
//        rectTransform = GetComponent<RectTransform>();
//        startPosition = movableUIObject.anchoredPosition;
//        //Debug.Log($"Start Position: {startPosition}");

//        if (Touchscreen.current != null)
//            hasTouchscreen = true;
//    }

//    void Start()
//    {
//        IM = InputManager.Instance;
//    }

//    public void InitializeInputReading(Vector2 _pointerDownPosition)
//    {
//        pointerDownPosition = _pointerDownPosition;
//        isDragging = true;
//    }
//    public void StopInputReading()
//    {
//        isDragging = false;
//        movableUIObject.anchoredPosition = startPosition;
//        IM.SingleJoystickInput = Vector2.zero;
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        if (!isDragging) return;

//        RectTransformUtility.ScreenPointToLocalPointInRectangle(
//            canvas.transform as RectTransform,
//            eventData.position,
//            eventData.pressEventCamera,
//            out localPoint
//        );

//        delta = localPoint - pointerDownPosition;
//        delta = Vector2.ClampMagnitude(delta, movementRange);
//        movableUIObject.anchoredPosition = startPosition + delta;

//        // Calculate the movement vector as a normalized Vector2
//        movementInput = delta / movementRange;
//        movementInput = Vector2.ClampMagnitude(movementInput, 1f);

//        if (IM == null)
//            return;

//        IM.SingleJoystickInput = movementInput;
//    }    
//}
