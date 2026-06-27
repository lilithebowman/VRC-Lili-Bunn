#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace PJKT.PublicAssets
{
    /// <summary>
    /// A disposable object to create the text renderer prefab, set the text and fonts, and render out the text texture.
    /// returns a PjktBadgeCustomization with the texture
    /// </summary>
    public class PjktTextBaker : IDisposable
    {
        private Camera camera;
        private RenderTexture  renderTexture;
        private Material clearBlitMaterial;
        private const string ClearShaderName = "Unlit/PJKT/Clear";
        public byte TextSetCount { get; private set; } = 0;
        private Vector2Int res;
        
        public PjktTextBaker(string TextCameraPrefabPath, Vector2Int resolution)
        {
            res = new Vector2Int(resolution.x, resolution.y);
            
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TextCameraPrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"TextBaker: unable to load asset at path {TextCameraPrefabPath}");
                return;
            }
            
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            camera = instance.GetComponent<Camera>();
            if (camera == null)
            {
                Debug.LogError($"TextBaker: camera prefab {prefab.name} has no camera");
                GameObject.Destroy(instance);
                return;
            }
            
            //render texture setup
            renderTexture = new RenderTexture(res.x, res.y, 0, GraphicsFormat.R8G8B8A8_UNorm); 
            camera.clearFlags = CameraClearFlags.Nothing;
            camera.targetTexture = renderTexture;
            
            instance.transform.position = new Vector3(0, 1000, 0);
            instance.transform.rotation = Quaternion.Euler(90, 0, 0);
            
            //material
            Shader shader = Shader.Find(ClearShaderName);
            if (shader == null)
            {
                Debug.LogError($"TextBaker: shader {ClearShaderName} not found");
                return;
            }
            clearBlitMaterial = new Material(shader);
        }

        public bool TrySetText(string text, string tmpName, TMP_FontAsset font = null, Material material = null)
        {
            Transform textMeshObj = camera.transform.Find(tmpName);
            if (textMeshObj == null)
            {
                Debug.LogError($"TextBaker: textMeshObj {tmpName} not found");
                return false;
            }
            
            TextMeshPro textMesh = textMeshObj.GetComponent<TextMeshPro>();
            if (textMesh == null)
            {
                Debug.LogError($"TextBaker: TextMeshPro component not found on {textMeshObj.name}");
                return false;
            }
            
            if (font) textMesh.font = font;
            if (material) textMesh.material = material;

            textMesh.text = text;
            PopulateGlyphs(textMesh, text); //force the dynamic atlas to rasterize these glyphs now so the bake isn't blank/boxed
            textMesh.ForceMeshUpdate(); //rebuild the mesh against the now-populated atlas (TMP otherwise defers this a frame)
            UseNativeFallbackMaterials(textMesh); //stop fallback glyphs from inheriting the base font's style
            TextSetCount++;
            return true;
        }

        //TMP renders fallback glyphs on child sub-meshes whose material TMP_MaterialManager blends from the BASE font's style
        //(outline, weight, face). That makes a dynamic JP/CJK fallback look wrong. Swap each sub-mesh back to its own font
        //asset's native material so the fallback renders in its own style. Runs after ForceMeshUpdate so the sub-meshes exist,
        //and right before the bake renders so nothing regenerates over it.
        private void UseNativeFallbackMaterials(TextMeshPro textMesh)
        {
            TMP_SubMesh[] subMeshes = textMesh.GetComponentsInChildren<TMP_SubMesh>();
            foreach (TMP_SubMesh sub in subMeshes)
            {
                if (sub.fontAsset != null && sub.fontAsset.material != null)
                    sub.sharedMaterial = sub.fontAsset.material;
            }
        }

        //Dynamic font assets only rasterize a glyph into their atlas when something renders it. Leaning on render-time population
        //is the usual reason dynamic bakes come out blank or boxed. Force every glyph in up front, walking the fallback chain,
        //and warn on anything no font in the chain can provide so a bad bake is loud instead of silent.
        private void PopulateGlyphs(TextMeshPro textMesh, string text)
        {
            TMP_FontAsset font = textMesh.font;
            if (font == null) return;

            string missing = "";
            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c) || char.IsControl(c)) continue;
                //HasCharacter(c, includeFallbacks, tryAddCharacter): the third arg synchronously rasterizes the glyph into the atlas
                if (!font.HasCharacter(c, true, true) && missing.IndexOf(c) < 0) missing += c;
            }

            if (missing.Length > 0)
                Debug.LogWarning($"TextBaker: '{font.name}' and its fallbacks have no glyph for: {missing} — these bake as boxes. Add a fallback font that covers them.");
        }

        public Texture2D BakeText()
        {
            if (!IsValid()) //skipts if anything couldnt be assigned, generated, or there was no text set to render
            {
                Debug.LogError($"TextBaker: Invalid setup, destroy and start over. Also look for other errors in the console.");
                return null;
            }
            
            Graphics.Blit(null, renderTexture, clearBlitMaterial); // Write Alpha 0

            //first-run guard: Unity compiles shaders async and renders a dummy shader meanwhile, so the first bake comes out blank.
            //turning async off forces the shader variant to compile synchronously when camera.Render() asks for it. restore after.
            bool prevAsync = ShaderUtil.allowAsyncCompilation;
            ShaderUtil.allowAsyncCompilation = false;

            camera.Render();
            if (ShaderUtil.anythingCompiling) camera.Render(); //a variant already mid-async-compile from elsewhere may still be warming; one more pass

            ShaderUtil.allowAsyncCompilation = prevAsync;

            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(res.x, res.y, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, res.x, res.y), 0, 0);
            texture.Apply();
            
            RenderTexture.active = null; // Reset active RenderTexture
            camera.targetTexture = null; // Unassign from camera before destroying
           
            return texture;
        }

        /// <summary>
        /// Returns true when the text renderer was setup with no issues
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (camera == null) return false;
            if (renderTexture == null) return false;
            if (clearBlitMaterial == null) return false;
            if (TextSetCount == 0) return false;
            return true;
        }
        
        public void Dispose()
        {
            if (camera != null) GameObject.DestroyImmediate(camera.gameObject);
            if (renderTexture != null) GameObject.DestroyImmediate(renderTexture);
            if (clearBlitMaterial != null) GameObject.DestroyImmediate(clearBlitMaterial);
        }
    }
}
#endif