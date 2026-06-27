using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace PJKT.PublicAssets
{
    [CustomEditor(typeof(PjktBadgeComponent))]
    public class PjktBadgeComponentEditor : Editor
    {
        PjktBadgeComponent badgeComponent;

        public override VisualElement CreateInspectorGUI()
        {
            badgeComponent = (PjktBadgeComponent)target;
            
            VisualElement root = new VisualElement();
            var badgeEditor = PjktBadgeEditorFactory.Create<VisualElement>(badgeComponent.BadgeEditor);
            
            if (badgeEditor == null)
            {
                Debug.LogError($"No editor found for event {badgeComponent.BadgeEditor.ToString()}");
                InspectorElement.FillDefaultInspector(root, serializedObject, this); //draw default inspector
                return root;
            }
            
            root.Add(badgeEditor);
            return root;
        }
    }
}