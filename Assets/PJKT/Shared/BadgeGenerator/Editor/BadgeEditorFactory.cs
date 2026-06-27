using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.PublicAssets
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PjktBadgeEditor : Attribute
    {
        public PjktEventBadgePreset BadgeEditorPreset;
        public PjktBadgeEditor(PjktEventBadgePreset badgeEditor) => BadgeEditorPreset = badgeEditor;
    }
    
    public static class PjktBadgeEditorFactory
    {
        private static Dictionary<PjktEventBadgePreset, Type> editorTypes = new Dictionary<PjktEventBadgePreset, Type>();

        [InitializeOnLoadMethod]
        private static void Init()
        {
            editorTypes.Clear();
            foreach (var type in TypeCache.GetTypesWithAttribute<PjktBadgeEditor>())
            {
                var attr = type.GetCustomAttributes(typeof(PjktBadgeEditor), true)[0] as PjktBadgeEditor;
                if (attr == null) continue;
                
                if (editorTypes.ContainsKey(attr.BadgeEditorPreset))
                {
                    Debug.LogError($"multiple  editor types for {attr.BadgeEditorPreset} found, using a random one");
                    continue;
                }
                editorTypes.Add(attr.BadgeEditorPreset, type);
            }
        }

        public static T Create<T>(PjktEventBadgePreset badgeEditor)  where T : VisualElement
        {
            if  (!editorTypes.ContainsKey(badgeEditor))  return null;
            var editor = editorTypes[badgeEditor];
            return Activator.CreateInstance(editor) as T;
        }
    }
}