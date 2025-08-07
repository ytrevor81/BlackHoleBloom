//using UnityEngine;
//using UnityEngine.EventSystems;

//public class MoveJoystickContainerOnTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
//{
//    [SerializeField] private RectTransform joystickContainer;
//    [SerializeField] private Canvas canvas;
//    private Vector2 startingPos;
//    private InputMoveWithFinger customJoystick;
//    private Vector2 pointerDownPosition;
//    private void Awake()
//    {
//        startingPos = joystickContainer.anchoredPosition;
//        customJoystick = joystickContainer.GetComponent<InputMoveWithFinger>();
//    }
    
//    public void OnPointerDown(PointerEventData eventData)
//    {
//        if (customJoystick == null)
//            return;

//        RectTransformUtility.ScreenPointToLocalPointInRectangle(
//            canvas.transform as RectTransform,
//            eventData.position,
//            eventData.pressEventCamera,
//            out pointerDownPosition
//        );

//        //joystickContainer.anchoredPosition = pointerDownPosition;
//        customJoystick.InitializeInputReading(pointerDownPosition);
//    }

//    public void OnPointerUp(PointerEventData eventData)
//    {
//        joystickContainer.anchoredPosition = startingPos;
//        customJoystick.StopInputReading();
//    }
//}
