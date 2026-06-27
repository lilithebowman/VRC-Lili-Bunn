using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.PublicAssets
{
    [PjktBadgeEditor(PjktEventBadgePreset.Fest26)]
    public class Fest26BadgeEditor : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Fest26BadgeEditor> { }
        
        PjktBadgeComponent BadgeComponent; //need to grab this so you can change the values
        PjktEventBadge settings; //set this manually or grab presets form the class ie: PjktEventBadge.Fang26() or pass a settings scriptable object PjktEventBadge.FromSettingsObject 
        
        //just your visual elements for the different fields, feel free to change names
        private VisualElement profilePicture => this.Q<VisualElement>("ProfilePic");
        //private VisualElement editButton => this.Q<VisualElement>("Edit");
        private ObjectField profilePictureField => this.Q<ObjectField>("ProfilePicObjectField");
        private TextField displayNameField => this.Q<TextField>("NameField");
        private TextField catchPhraseField => this.Q<TextField>("TaglineField");
        private VisualElement generateButton => this.Q<VisualElement>("GenerateButton");
        
        //can add fields for text fonts or materials 
        
        //basically ui elements version of OnEnable. constructor
        public Fest26BadgeEditor()
        {
            //grab the badge component, assuming this is loaded when they click the object to view it in the inspector
            BadgeComponent = Selection.activeGameObject.GetComponent<PjktBadgeComponent>();
            if (BadgeComponent == null)
            {
                Debug.LogError("Selected GameObject does not have a PjktBadgeComponent");
                return;
            }
            
            //get our settings, hardcoded in this case
            settings = PjktEventBadge.FromSettingsObject(PjktEventBadgePreset.Fest26);
            
            //create the actual visuals for this ui element
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(settings.EditorVisualElementPath);
            asset.CloneTree(this);
            
            //set the values in the visual element from the stuff saved on the badge component
            profilePictureField.value = BadgeComponent.profilePicture;
            profilePicture.style.backgroundImage = BadgeComponent.profilePicture;
            displayNameField.value = BadgeComponent.personName;
            catchPhraseField.value = BadgeComponent.personDescription;
            
            //callbacks for making changes in those fields. sets the vlaes on the component
            profilePictureField.objectType = typeof(Texture2D);
            profilePictureField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
            {
                Texture2D texture = evt.newValue as Texture2D;
                profilePicture.style.backgroundImage = texture;
                BadgeComponent.profilePicture = texture;
                EditorUtility.SetDirty(BadgeComponent);
            });
            
            displayNameField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                BadgeComponent.personName = evt.newValue;
                EditorUtility.SetDirty(BadgeComponent);
            });
            
            catchPhraseField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                BadgeComponent.personDescription = evt.newValue;
                EditorUtility.SetDirty(BadgeComponent);
            });
            
            generateButton.RegisterCallback<ClickEvent>(evt =>
            {
                GenerateBadge();
                BurstConfetti(generateButton);
            });

            SetupHoverGlow(generateButton);
        }

        //makes the button glow + gently pulse while hovered.
        private void SetupHoverGlow(VisualElement btn)
        {
            Color baseColor = new Color(1f, 0.769f, 0.157f); //the button's normal yellow (255,196,40)
            Color glowColor = new Color(1f, 0.92f, 0.45f);   //hot, brighter yellow when lit

            float glow = 0f;        //current intensity 0..1
            float glowTarget = 0f;  //where it's easing toward (1 hovered, 0 not)
            double startTime = 0;   //phase anchor for the breathing sine
            double lastTime = 0;    //for frame-delta easing
            IVisualElementScheduledItem pulse = null;

            void SetBorder(float width, Color color)
            {
                btn.style.borderTopWidth = width;
                btn.style.borderRightWidth = width;
                btn.style.borderBottomWidth = width;
                btn.style.borderLeftWidth = width;
                btn.style.borderTopColor = color;
                btn.style.borderRightColor = color;
                btn.style.borderBottomColor = color;
                btn.style.borderLeftColor = color;
            }

            //runs every frame: eases glow toward its target, then renders intensity * breathing sine
            void Render()
            {
                double now = EditorApplication.timeSinceStartup;
                float dt = Mathf.Min((float)(now - lastTime), 0.05f);
                lastTime = now;
                glow = Mathf.Lerp(glow, glowTarget, 1f - Mathf.Exp(-dt * 14f));
                if (Mathf.Abs(glow - glowTarget) < 0.001f) glow = glowTarget;

                float s = 0.5f + 0.5f * Mathf.Sin((float)(now - startTime) * 4f);
                btn.style.scale = new Scale(new Vector2(1f + 0.05f * glow, 1f + 0.05f * glow));
                Color lit = Color.Lerp(glowColor, Color.white, s * 0.35f);
                btn.style.backgroundColor = Color.Lerp(baseColor, lit, glow);
                SetBorder((1.5f + s * 2.5f) * glow, new Color(1f, 1f, 0.75f, (0.55f + 0.45f * s) * glow));

                if (glowTarget <= 0f && glow <= 0f) pulse?.Pause(); //fully faded out -> stop ticking
            }

            btn.RegisterCallback<PointerEnterEvent>(evt =>
            {
                glowTarget = 1f;
                startTime = EditorApplication.timeSinceStartup;
                lastTime = startTime;
                if (pulse == null) pulse = btn.schedule.Execute(Render).Every(16);
                pulse.Resume();
            });

            btn.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                glowTarget = 0f;
                lastTime = EditorApplication.timeSinceStartup;
                pulse?.Resume(); //keep ticking so it can ease out
            });
        }

        //confettii!
        private void BurstConfetti(VisualElement anchor)
        {
            VisualElement root = anchor.panel?.visualTree;
            if (root == null) return;

            Vector2 origin = new Vector2(anchor.worldBound.center.x, anchor.worldBound.yMax);
            Color[] colors = { Color.yellow, Color.cyan, Color.magenta, Color.white, new Color(1f, 0.5f, 0f) };
            const int count = 36;
            const int durationMs = 8000;
            const float durationSec = durationMs / 1000f;

            for (int i = 0; i < count; i++)
            {
                float w = Random.Range(5f, 9f);
                float h = Random.Range(8f, 14f);

                VisualElement p = new VisualElement();
                p.pickingMode = PickingMode.Ignore;
                p.style.position = Position.Absolute;
                p.style.width = w;
                p.style.height = h;
                p.style.left = origin.x - w / 2f;
                p.style.top = origin.y - h / 2f;
                p.style.backgroundColor = colors[Random.Range(0, colors.Length)];
                p.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));
                p.style.opacity = 0f; //start invisible so there's no full-opacity flash frame before the anim ticks
                p.style.translate = new Translate(0f, 0f); //start exactly on the button
                root.Add(p);

                //upward cone — velocities in px/sec, gravity in px/sec^2
                float dir = -Mathf.PI / 2f + Random.Range(-0.28f, 0.28f);
                float speed = Random.Range(320f, 520f);
                float vx = Mathf.Cos(dir) * speed;
                float vy = Mathf.Sin(dir) * speed; //sin(-90) = -1 -> upward
                float gravity = Random.Range(150f, 210f); //weak gravity so it hangs then falls over the full 8s
                float spin = Random.Range(-540f, 540f);

                p.experimental.animation.Start(0f, 1f, durationMs, (e, t) =>
                {
                    //real projectile motion: x = vx*time, y = vy*time + 0.5*g*time^2
                    float time = t * durationSec;
                    float x = vx * time;
                    float y = vy * time + 0.5f * gravity * time * time;
                    e.style.translate = new Translate(x, y);
                    e.style.rotate = new UnityEngine.UIElements.Rotate(spin * t);
                    //smooth fade in, then fade out over the last 25%
                    float fadeIn = Mathf.Min(t / 0.05f, 1f);
                    float fadeOut = t > 0.75f ? (1f - t) / 0.25f : 1f;
                    e.style.opacity = fadeIn * fadeOut;
                }).OnCompleted(() => p.RemoveFromHierarchy());
            }
        }

        //the button that actually calls to create the textures
        private void GenerateBadge()
        {
            //re-mint the id if this badge was duplicated, so it writes its own assets instead of clobbering the original's
            BadgeComponent.EnsureUniqueBadgeId();

            //create an arry of customizations with the info the user set.
            PjktBadgeCustomization[] customizations = new PjktBadgeCustomization[]
            {
                new PjktBadgeCustomization //name
                {
                    customizationType = PjktBadgeCustomization.PjktBadgeCustomizationType.Text, //is it a texture or text?
                    FieldName = "Name", //if text change the field name. this is the name of the gameobject were gonna try to grab the TextMesh component off of from the camera thing
                    Text = displayNameField.value //set the text
                    //FontAsset = //if you want to mess with these, they are per textmesh object
                    //TmpMaterial = //load from assets or whatever, can save asset paths or guids for these up above as const 
                },
                new PjktBadgeCustomization //description or blurb whatever
                {
                    customizationType = PjktBadgeCustomization.PjktBadgeCustomizationType.Text,
                    FieldName = "Blurb",
                    Text = catchPhraseField.value
                },
                new PjktBadgeCustomization
                {
                    customizationType = PjktBadgeCustomization.PjktBadgeCustomizationType.Text,
                    FieldName = "Title",
                    Text = BadgeComponent.personTitle
                },
                new PjktBadgeCustomization //e picture the user set
                {
                    customizationType = PjktBadgeCustomization.PjktBadgeCustomizationType.Image, //images a bit diffrent
                    ShaderPropertyName = "_ProfilePic", //change the shader property name instead of the text FieldName
                    Texture = profilePictureField.value as Texture2D //set the texture
                }
            };
            
            //this is where we call the static class to generate the textures.
            //it uses the settings to find all the base textures, prefabs, materials etc
            //we just need to pass our customizations array, and it gives us back 2 textures, main and emission
            Texture2D[] textures = PjktBadgeTextureGen.GenerateBadgeTextures(settings, customizations, BadgeComponent.BadgeId);
            if (textures == null || textures.Length != 2) //well hopefully 2 textures
            {
                Debug.LogError($"Badge: {BadgeComponent.gameObject.name} failed to generate textures.");
                return;
            }


            //reuse this badge's material if it already exists, otherwise clone the source one once
            string path = Path.Combine(settings.GeneratedImagesPath, $"{settings.EventName}_{BadgeComponent.BadgeId}_Generated.mat");
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(BadgeComponent.Materials[0]);
                AssetDatabase.CreateAsset(mat, path);
            }

            //set the textures, thay are already saved in assets
            mat.SetTexture("_MainTex", textures[0]);
            mat.SetTexture("_EmissionMap", textures[1]);
            EditorUtility.SetDirty(mat);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            //assign to whatever mesh
            Material[] mats = BadgeComponent.Meshes[0].sharedMaterials;
            mats[0] = mat;
            BadgeComponent.Meshes[0].sharedMaterials = mats;
            EditorUtility.SetDirty(BadgeComponent.Meshes[0]);
            EditorUtility.SetDirty(BadgeComponent);
        }
    }
}