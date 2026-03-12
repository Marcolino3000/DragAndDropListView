using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using MouseButton = UnityEngine.UIElements.MouseButton;

namespace Runtime
{
    public class Drag : PointerManipulator
    {
        private bool m_IsDragging;
        private Vector2 m_PointerStartPanel;
        private Vector2 m_ElementStartWorld;
        private VisualElement itemRoot;
        private ListView m_ListView;

        // private int targetIndex; -> wegen bringitemtofront
        private VisualElement m_CurrentlyHoveredItem;
        // private VisualElement m_ghostItem;
        private VisualTreeAsset m_itemTemplate;

        public Drag(VisualElement element, ListView listView, VisualTreeAsset itemTemplate)
        {
            target = element;
            
            itemRoot = element.parent.parent;

            m_ListView = listView;
            
            m_itemTemplate = itemTemplate;
        }
        
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            //todo: mouse button in Settings-SO einstellen
            if(evt.button != (int)MouseButton.LeftMouse)
                return;

            // itemRoot.style.position = Position.Absolute;

            // m_ghostItem = m_itemTemplate.CloneTree();
            //
            // if (m_ghostItem != null)
            // {
            //     m_ghostItem.style.position = Position.Absolute;
            //     var itemLocalPos = itemRoot.parent.WorldToLocal(itemRoot.worldBound.position);
            //     m_ghostItem.style.left = itemLocalPos.x;
            //     m_ghostItem.style.top = itemLocalPos.y;
            //     m_ghostItem.style.opacity = 0.f;
            //     m_ghostItem.pickingMode = PickingMode.Ignore;
            //     itemRoot.parent.Add(m_ghostItem);
            //
            //     // hide original while dragging so layout doesn't collapse visually
            //     itemRoot.style.visibility = Visibility.Hidden;
            // }
            
            m_IsDragging = true;
            
            m_ElementStartWorld = itemRoot.worldBound.position;
            m_PointerStartPanel = evt.position;
            
            // itemRoot.BringToFront();
            target.CapturePointer(evt.pointerId);
            target.RegisterCallback<PointerMoveEvent>(OnCapturedMove);
            // evt.StopPropagation();
        }

        private void OnCapturedMove(PointerMoveEvent evt)
        {
            // Debug.Log("captured move: " + evt.position);
            
            var closestItem = FindClosestOverlappingItem(evt.position);
            if (closestItem == null) return;
            
            if (m_CurrentlyHoveredItem == null)
                m_CurrentlyHoveredItem = closestItem;

            if (closestItem == m_CurrentlyHoveredItem) return;
                
            m_CurrentlyHoveredItem.style.backgroundColor = Color.clear;
            m_CurrentlyHoveredItem = closestItem;
            m_CurrentlyHoveredItem.style.backgroundColor = new StyleColor(Color.aquamarine);
            
            var content = m_ListView.Q("unity-content-container");
            Debug.Log("dragged item index " + content.IndexOf(itemRoot) + "slot index " + content.IndexOf(m_CurrentlyHoveredItem));
        }

        private VisualElement FindClosestOverlappingItem(Vector3 evtPosition)
        {
            var content = m_ListView.Q("unity-content-container");
            
            VisualElement closestItem = null;
            float closestDistance = float.MaxValue;
            
            foreach (var child in content.Children())
            {
                if (child == itemRoot)
                    continue;

                var childBounds = child.worldBound;
                
                if (childBounds.Overlaps(itemRoot.worldBound))
                {
                    var childCenter = childBounds.center;
                    var distance = Vector2.Distance(evtPosition, childCenter);
                    
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestItem = child;
                    }
                }
            }
            
            return closestItem;
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if(!m_IsDragging || !target.HasPointerCapture(evt.pointerId))
                return;
            
            var parent = target.parent;
            if (parent == null)
                return;
            
            var pointerCurrent = (Vector2)evt.position;
            var pointerDelta = pointerCurrent - m_PointerStartPanel;
            var newWorldPosition = m_ElementStartWorld + pointerDelta;
            var newLocalPosition = itemRoot.parent.WorldToLocal(newWorldPosition);
            
            itemRoot.style.left = newLocalPosition.x;
            itemRoot.style.top = newLocalPosition.y;
            
            // evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if(!m_IsDragging || !target.HasPointerCapture(evt.pointerId))
                return;

            target.ReleasePointer(evt.pointerId);
            
            var content = m_ListView.Q("unity-content-container");
            
            int draggedIndex = content.IndexOf(itemRoot);
            int targetIndex = content.IndexOf(m_CurrentlyHoveredItem);
            
            
            m_ListView.viewController.Move(draggedIndex, targetIndex);
            itemRoot.style.position = Position.Relative;
            
            
            evt.StopPropagation();
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            m_IsDragging = false;
        }
    }
}