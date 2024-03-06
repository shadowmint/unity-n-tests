using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace N.Package.Tests.Editor
{
    public class StoryService
    {
        public string CurrentFilePath()
        {
            var editorFolder = Path.GetDirectoryName(GetCurrentFileName());
            if (editorFolder == null)
            {
                throw new Exception("Unable to find folder to scan for assets!");
            }

            return editorFolder;
        }

        public List<StoryScene> FindStories(bool debug)
        {
            if (EditorApplication.isPlaying)
            {
                return new List<StoryScene>();
            }
            
            var servicePath = CurrentFilePath();
            var assetsPath = GetAssetsFolder(servicePath);
            Trace($"Scanning for stories in assets folder: {assetsPath}", debug);

            var queue = new Queue<ScanTarget>();
            var resolved = new List<ScanTarget>();

            queue.Enqueue(new ScanTarget() { FolderPath = assetsPath });

            while (queue.Any())
            {
                ScanFolder(queue.Dequeue(), queue, assetsPath, resolved, debug);
            }

            var uniqueScenePaths = resolved
                .SelectMany(i => i.Stories)
                .GroupBy(i => i.targetScene)
                .Select(i => i.First())
                .ToList();

            return uniqueScenePaths.OrderBy(i => i.AssetPath).ToList();
        }

        public void Trace(string message, bool debug)
        {
            if (!debug) return;
            Debug.Log(message);
        }

        private void ScanFolder(ScanTarget target, Queue<ScanTarget> pending, string assetRootFolder, List<ScanTarget> resolved, bool debug)
        {
            var files = Directory.GetFiles(target.FolderPath);
            var folders = Directory.GetDirectories(target.FolderPath);

            foreach (var file in files)
            {
                if (Path.GetExtension(file) == ".prefab")
                {
                    var assetPath = GetAssetPath(file, assetRootFolder);
                    var objectLike = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (objectLike != null)
                    {
                        var storySceneRef = objectLike.GetComponent<StoryScene>();
                        if (storySceneRef != null)
                        {
                            Trace($"Found story scene ref: {assetPath}", debug);
                            storySceneRef.AssetPath = assetPath;
                            target.Stories.Add(storySceneRef);
                        }
                    }
                }
            }

            foreach (var folder in folders)
            {
                pending.Enqueue(new ScanTarget() { FolderPath = folder });
            }

            resolved.Add(target);
        }

        private string GetAssetPath(string path, string assetRootFolder)
        {
            return Path.Combine("Assets", path.Substring(assetRootFolder.Length + 1)).Replace("\\", "/");
        }

        private string GetAssetsFolder(string arbitraryPath)
        {
            var here = arbitraryPath;
            while (true)
            {
                var folderName = Path.GetFileName(here);
                if (folderName == "Assets")
                {
                    return here;
                }

                var nextPath = Path.GetFullPath(Path.Join(here, ".."));
                if (here == nextPath)
                {
                    throw new Exception($"Invalid path; not assets folder in hierarchy for: {arbitraryPath}");
                }

                here = nextPath;
            }
        }

        public string GetCurrentFileName([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
        {
            return fileName;
        }

        public void PlayStory(StoryScene sceneLike)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Can't start a story  while the editor is playing. Stop the editor first");
                EditorApplication.isPlaying = false;
                return;
            }

            EditorSceneManager.OpenScene(sceneLike.targetScene, OpenSceneMode.Single);
            EditorApplication.isPlaying = true;
        }

        class ScanTarget
        {
            public string FolderPath { get; set; }
            public List<StoryScene> Stories { get; } = new();
        }
    }
}