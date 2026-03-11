using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime
{
    public class Drop : PointerManipulator
    {
        public Drop(VisualElement element)
        {
            target = element;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            // target.RegisterCallback<MouseOverEvent>(OnMouseOver);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        private void OnMouseOver(MouseOverEvent evt)
        {
            UnityEngine.Debug.Log(evt.target + " " + evt.mousePosition);
        }

        private void OnPointerEnter(PointerEnterEvent evt)
        {
            Debug.Log(evt.position + " " + target.name);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            // UnityEngine.Debug.Log(evt.position + target.name);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            // target.UnregisterCallback<MouseOverEvent>(OnMouseOver);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        }
    }
}