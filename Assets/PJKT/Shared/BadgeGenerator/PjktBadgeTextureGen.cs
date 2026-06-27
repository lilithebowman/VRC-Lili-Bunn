#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace PJKT.PublicAssets
{
    /// <summary>
    /// just handles the actual badge texture generation with the users cusomizations
    /// </summary>
    public static class PjktBadgeTextureGen
    {
        /// <summary>
        /// Assumes there is alwyas goint to be a main[0] and emission[1] texture
        /// returns the generated textures after saving and laoding them from the asset database.
        /// you will need to assign them in your material
        /// </summary>
        /// <param name="badgeData">Data for the event including the file paths</param>
        /// <param name="customizations">An array of customizations to place on the badge texture. ignores text if there is a Texture passed</param>
        /// <returns></returns>
        public static Texture2D[] GenerateBadgeTextures(PjktEventBadge badgeData, PjktBadgeCustomization[] customizations, string badgeId)
        {
            //TMP Essential Resources missing -> no default font/SDF shader -> text bakes blank. warn and bail before doing any work.
            if (TMP_Settings.instance == null || TMP_Settings.defaultFontAsset == null)
            {
                const string msg = "TextMeshPro isn't set up in this project. Open Window > TextMeshPro > Import TMP Essential Resources, then generate the badge again.";
                Debug.LogError($"Badge generation aborted: {msg}");
                EditorUtility.DisplayDialog("TextMeshPro required", msg, "OK");
                return null;
            }

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(badgeData.BlitMaterialPath);
            if (mat == null)
            {
                Debug.LogError($"Failed to load blit material at path {badgeData.BlitMaterialPath}");
                return null;
            }
            
            Texture2D mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(badgeData.BadgeBaseTexturePath);
            if (mainTexture == null)
            {
                Debug.LogError($"Failed to load main texture at path {badgeData.BadgeBaseTexturePath}");
                return null;
            }
            
            Texture2D emissionTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(badgeData.BadgeBaseEmissionPath);
            if (emissionTexture == null)
            {
                Debug.LogError($"Failed to load emission texture at path {badgeData.BadgeBaseEmissionPath}");
                return null;
            }

            //setup the text renderer
            PjktTextBaker textBaker = new PjktTextBaker(badgeData.TextRenderSetupPrefabPath, new Vector2Int(1024, 1024));
            
            //go through the customizations
            for (int i = 0; i < customizations.Length; i++)
            {
                var customization = customizations[i];
                switch (customization.customizationType)
                {
                    case PjktBadgeCustomization.PjktBadgeCustomizationType.Text:
                        textBaker.TrySetText(customization.Text, customization.FieldName, customization.FontAsset,
                            customization.TmpMaterial);
                        break;
                    case PjktBadgeCustomization.PjktBadgeCustomizationType.Image:
                        mat.SetTexture(customization.ShaderPropertyID, customization.Texture);
                        break;
                }
            }
            
            //deal with any text that might have been set
            if (textBaker.TextSetCount > 0)
            {
                Texture2D texture = textBaker.BakeText();
                if (texture != null) mat.SetTexture(Shader.PropertyToID("_NamePic"),  texture);
                
                // SaveFile(texture, "Assets/DEBUGTEX.png");
            }
            textBaker.Dispose();

            //blit the textures
            Texture2D finalMain = BlitTextures(mainTexture, mat);
            Texture2D finalEmission = BlitTextures(emissionTexture, mat);

            string generatedBasePath = Path.Combine(badgeData.GeneratedImagesPath, $"{badgeData.EventName}_{badgeId}_Main.png");
            string generatedEmissionPath = Path.Combine(badgeData.GeneratedImagesPath, $"{badgeData.EventName}_{badgeId}_Emission.png");
            
            SaveFile(finalMain, generatedBasePath);
            SaveFile(finalEmission, generatedEmissionPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            //reload them
            return new []
            {
                AssetDatabase.LoadAssetAtPath<Texture2D>(generatedBasePath),
                AssetDatabase.LoadAssetAtPath<Texture2D>(generatedEmissionPath)
            };
        }

        private static Texture2D BlitTextures(Texture2D sampleTexture, Material mat)
        {
            RenderTexture rt = new RenderTexture(sampleTexture.width, sampleTexture.height, 0);
            Graphics.Blit(sampleTexture, rt,  mat);
            
            // Convert the RenderTexture to a Texture2D
            var finalTexture = new Texture2D(sampleTexture.width, sampleTexture.height);
            RenderTexture.active = rt;
            finalTexture.ReadPixels(new Rect(0, 0, sampleTexture.width, sampleTexture.height), 0, 0);
            finalTexture.Apply();
            RenderTexture.active = null;

            return finalTexture;
        }
        
        private static void SaveFile(Texture2D texture, string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath))) Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
        }
    }
    
    public struct PjktBadgeCustomization
    {
        public PjktBadgeCustomizationType customizationType; //used for logic to set textures or bake text
        
        public Texture2D Texture; //profile pic, sticker, etc
        public string ShaderPropertyName; //shader property to set the texture on for the blit material
        
        public string Text;
        public string FieldName; //hate this but works for now. used to set the text to the correct field in the tmp camera.
        public TMP_FontAsset FontAsset; //for diffrent fonts. can be null
        public Material TmpMaterial; //if you wanna use a funny material for text effects. can be null
        
        public Vector2 Position; //possibly use for stickers later or generic text after we ditch the camera text thing

        public int ShaderPropertyID
        {
            get { return Shader.PropertyToID(ShaderPropertyName); }
        }

        public enum PjktBadgeCustomizationType
        {
            Text,
            Image
        }
    }
}
#endif