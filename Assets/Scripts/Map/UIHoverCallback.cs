using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace YuriGameJam2023.Map
{
    public class UIHoverCallback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent EnterCallback { get; } = new();
        public UnityEvent ExitCallback { get; } = new();

        public void OnPointerEnter(PointerEventData eventData)
        {
            EnterCallback.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ExitCallback.Invoke();
        }
    }
}
