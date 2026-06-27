using System;
using UnityEngine;
using VRC.SDKBase;

namespace PJKT.PublicAssets
{
    public class PjktBadgeComponent : MonoBehaviour, IEditorOnly
    {
        //core
        public Material[] Materials;
        public Renderer[] Meshes;
        
        //user set stuff
        public Texture2D profilePicture;
        public string personName = "";
        public string personTitle = "";
        public string personDescription = "PJKT Event Attendee";
        
        //possibly implement at some point
        public Texture2D[] stickers;
        public Vector4[] stickerPositions;
        public Vector3[] stickerPositions3D;
        public float[] stickerRotations;
        public Vector3[] stickerScales;

        
        public PjktEventBadgePreset BadgeEditor = PjktEventBadgePreset.Fest26; //the editor to use

        //stable per-badge id so each badge owns a fixed set of generated assets instead of making new ones every generate
        [SerializeField, HideInInspector] private string badgeId;

        public string BadgeId => string.IsNullOrEmpty(badgeId)
            ? (badgeId = Guid.NewGuid().ToString("N"))
            : badgeId;

        //a scene duplicate copies badgeId verbatim, which would make two badges overwrite each other's assets.
        //the collision only matters when we actually write files, so we re-mint here right before generating.
        public void EnsureUniqueBadgeId()
        {
            foreach (var other in FindObjectsOfType<PjktBadgeComponent>())
            {
                if (other != this && other.badgeId == badgeId)
                {
                    badgeId = "";
                    break;
                }
            }
            _ = BadgeId; //re-mint if we cleared it above
        }
    }
}