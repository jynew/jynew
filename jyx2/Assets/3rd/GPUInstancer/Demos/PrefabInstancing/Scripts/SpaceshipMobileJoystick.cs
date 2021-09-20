using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GPUInstancer
{
    public class SpaceshipMobileJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [HideInInspector]
        public Vector3 inputDirection;

        private Image joystickBase;
        private Image joystick;
        private Vector2 dragPosition;
        
        private void Start()
        {
            joystickBase = GetComponent<Image>();
            joystick = transform.GetChild(0).GetComponent<Image>();
            inputDirection = Vector3.zero;
        }

        public virtual void OnDrag(PointerEventData data)
        {
            dragPosition = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase.rectTransform, data.position, data.pressEventCamera, out dragPosition))
            {
                inputDirection.x = (dragPosition.x / joystickBase.rectTransform.sizeDelta.x) * 2 + ((joystickBase.rectTransform.pivot.x == 1) ? 1 : -1);
                inputDirection.z = (dragPosition.y / joystickBase.rectTransform.sizeDelta.y) * 2 + ((joystickBase.rectTransform.pivot.y == 1) ? 1 : -1);

                inputDirection = Vector3.ClampMagnitude(inputDirection, 1);
                joystick.rectTransform.anchoredPosition = new Vector3(inputDirection.x * (joystickBase.rectTransform.sizeDelta.x / 3),
                                                                      inputDirection.z * (joystickBase.rectTransform.sizeDelta.y / 3));
            }
        }

        public virtual void OnPointerDown(PointerEventData data)
        {
            OnDrag(data);
        }

        public virtual void OnPointerUp(PointerEventData data)
        {
            inputDirection = Vector3.zero;
            joystick.rectTransform.anchoredPosition = Vector3.zero;
        }
    }
}