using UnityEngine;

namespace N.Package.Tests
{
    public class StoryScene : MonoBehaviour
    {
        public string description;
        
        // the scene in string
        [HideInInspector] public string targetScene;

#if UNITY_EDITOR
        
        // the scene in asset
        public UnityEditor.SceneAsset targetSceneAsset;
        

        /// <summary>
        /// The path to this asset
        /// </summary>
        public string AssetPath { get; set; }

        // whenever you modify the scene in the project, this sets the new name in the
        // targetScene variable automatically.
        private void OnValidate()
        {
            targetScene = "";
            if (targetSceneAsset != null)
            {
                targetScene = UnityEditor.AssetDatabase.GetAssetPath(targetSceneAsset);
            }
        }

#endif
    }
}
