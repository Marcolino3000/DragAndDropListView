using System;
using Data;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime
{
    public class CustomListView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset itemTemplate;
        [SerializeField] private UIDocument listviewDocument;
        [SerializeField] private Data.Data data;
        
        [Header("Settings")]
        [SerializeField] private int itemHeight;
        [SerializeField] private bool reorderable;
        
        private UnityEngine.UIElements.ListView listView;
        private VisualElement root;
        
        private void OnEnable()
        {
            root = listviewDocument.rootVisualElement;
            
            Func<VisualElement> makeItem = () => MakeItem();
            
            listView = new ListView(data.Items, itemHeight, makeItem, BindItems);
            listView.reorderable = reorderable;
            listView.showAddRemoveFooter = true;
            listView.horizontalScrollingEnabled = true;

            listView.itemIndexChanged += OnItemIndexChanged;
            
            root.Add(listView);
        }

        private void OnItemIndexChanged(int arg1, int arg2)
        {
            UnityEngine.Debug.Log("id" + arg1 + "moved to" + arg2);
        }

        private TemplateContainer MakeItem()
        {
            var item = itemTemplate.CloneTree();
            var dragArea = item.Q("dragArea");
            dragArea.AddManipulator(new Drag(dragArea, listView, itemTemplate));
            // dragArea.AddManipulator(new Drop(dragArea));
            listView.AddManipulator(new Drop(listView));
            return item;
        }

        private void BindItems(VisualElement element, int index)
        {
            DataItem taskData = data.Items[index];
            
            if(taskData == null)
            {
                taskData = new DataItem();
                data.Items[index] = taskData;
                Debug.LogWarning("TaskData was null - created new TaskData");
            }

            var label = element.Q<Label>("name");
            label.SetBinding("value", new DataBinding
            {
                dataSource = data.Items[index],
                dataSourcePath = new PropertyPath(nameof(taskData.Name)),
                bindingMode = BindingMode.ToTarget
            });
        }
    }
}