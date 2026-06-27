using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.PublicAssets
{
    //for editor editing simpler stuff
    [CreateAssetMenu(fileName = "New badge settings", menuName = "Pjkt/Event Badge Settings")]
    
    public class PjktEventBadgeSettings : ScriptableObject
    {
        public PjktEventBadgePreset EventPreset = PjktEventBadgePreset.Fest26;

        public Texture2D BaseTexture;
        public Texture2D EmissionTexture;
        public string GeneratedImagesOutputPath;
        
        public GameObject TextRenderSetupPrefab;
        
        public VisualTreeAsset EditorVisualElement;
        public Material BlitMaterial;
        
#if UNITY_EDITOR
        [MenuItem("CONTEXT/PjktEventBadgeSettings/Copy code block")]
        public static void ToCodeBlock(MenuCommand command)
        {
            PjktEventBadgeSettings settings = command.context as PjktEventBadgeSettings;
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public static PjktEventBadge {settings.EventPreset.ToString()}()");
            sb.AppendLine("{");
            sb.AppendLine($"    PjktEventBadge badge = new PjktEventBadge();");
            sb.AppendLine($"    ");
            sb.AppendLine($"    badge.BadgeBaseTexturePath = \"{AssetDatabase.GetAssetPath(settings.BaseTexture)}\";");
            sb.AppendLine($"    badge.BadgeBaseEmissionPath = \"{AssetDatabase.GetAssetPath(settings.EmissionTexture)}\";");
            sb.AppendLine($"    badge.GeneratedImagesPath = \"{settings.GeneratedImagesOutputPath}\";");
            sb.AppendLine($"    ");
            sb.AppendLine($"    badge.TextRenderSetupPrefabPath = \"{AssetDatabase.GetAssetPath(settings.TextRenderSetupPrefab)}\";");
            sb.AppendLine($"    ");
            sb.AppendLine($"    badge.EditorVisualElementPath = \"{AssetDatabase.GetAssetPath(settings.EditorVisualElement)}\";");
            sb.AppendLine($"    badge.BlitMaterialPath = \"{AssetDatabase.GetAssetPath(settings.BlitMaterial)}\";");
            sb.AppendLine($"    ");
            sb.AppendLine($"    return badge;");
            sb.AppendLine("}");
            
            //copy to system clipboard
            EditorGUIUtility.systemCopyBuffer = sb.ToString();
        }
#endif
    }
}