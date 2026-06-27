using System;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PJKT.PublicAssets
{
    public enum PjktEventBadgePreset //S = supporter version
    {
        Fest25,
        Fest25S,
        Fang26,
        Fang26S,
        Fest26,
        Fest26S,
    }

#if UNITY_EDITOR
    [Serializable]
    public class PjktEventBadge
    {
        public string EventName;
        
        //texture Assets
        public string BadgeBaseTexturePath;
        public string BadgeBaseEmissionPath;
        public string GeneratedImagesPath;
        
        //other important assets
        public string TextRenderSetupPrefabPath; //for the text renderer
        
        //visual elements
        public string EditorVisualElementPath;
        public string BlitMaterialPath;
        
        //presets for events but you can just manually assign values
        public static PjktEventBadge Fang26()
        {
            PjktEventBadge badge = new PjktEventBadge();
            badge.EventName = "Fang26";
            badge.BadgeBaseEmissionPath = "Assets/NewGenerator/Badges/Fang26/Fang26BadgeEmission.png";
            badge.BadgeBaseTexturePath = "Assets/NewGenerator/Badges/Fang26/Fang26BadgeBase.png";
            badge.EditorVisualElementPath = "Assets/PJKT/FANG 2026/Assets/Amulet/Nametag Generator/UI/NametagGeneratorUI2025.uxml";
            return badge;
        }

        public static PjktEventBadge FromSettingsObject(PjktEventBadgePreset preset)
        {
            //try to find in assets
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(PjktEventBadgeSettings));
            PjktEventBadgeSettings settings = null;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path)) continue;
                
                PjktEventBadgeSettings test = AssetDatabase.LoadAssetAtPath<PjktEventBadgeSettings>(path);
                if (test == null) continue;
                
                if  (test.EventPreset == preset)
                {
                    settings = test;
                    break;
                }
            }
            
            if (settings == null)
            {
                Debug.LogError($"Could not find settings for PJKTEventBadgePreset {preset}");
                return null;
            }
            
            PjktEventBadge badge = new PjktEventBadge();
            badge.EventName = settings.EventPreset.ToString();

            badge.BadgeBaseTexturePath = AssetDatabase.GetAssetPath(settings.BaseTexture);
            badge.BadgeBaseEmissionPath = AssetDatabase.GetAssetPath(settings.EmissionTexture);
            badge.GeneratedImagesPath = settings.GeneratedImagesOutputPath;
                
            badge.TextRenderSetupPrefabPath = AssetDatabase.GetAssetPath(settings.TextRenderSetupPrefab);
            
            badge.EditorVisualElementPath = AssetDatabase.GetAssetPath(settings.EditorVisualElement);
            badge.BlitMaterialPath = AssetDatabase.GetAssetPath(settings.BlitMaterial);

            return badge;
        }
    }
#endif
}