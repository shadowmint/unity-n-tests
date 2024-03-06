using System.Collections.Generic;
using System.Linq;
using N.Package.Tests.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace N.Package.Tests
{
    class StoryEditorWindow : EditorWindow
    {
        private List<StoryScene> _stories;
        private Label _disableLabel;
        private ListView _listView;

        [MenuItem("Window/Storybook")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(StoryEditorWindow));
        }

        public void CreateGUI()
        {
            titleContent = new GUIContent("Storybook");
            var service = new StoryService();

            _listView = new ListView();
            _stories ??= service.FindStories(false);

            var refreshButton = new Button();
            refreshButton.text = "Refresh";
            refreshButton.style.marginBottom = new StyleLength(4f);
            refreshButton.style.paddingTop = new StyleLength(4f);
            refreshButton.style.paddingBottom = new StyleLength(4f);
            rootVisualElement.Add(refreshButton);

            _disableLabel = new Label("Stories cannot be run in play mode");
            _disableLabel.style.paddingLeft = new StyleLength(6f);
            rootVisualElement.Add(_disableLabel);
         
            _listView.makeItem = () => new Label();
            _listView.bindItem = (item, index) =>
            {
                var label = item as Label;
                if (label == null) return;

                var story = _stories[index];
                var name = story.targetScene;
                if (!string.IsNullOrWhiteSpace(story.description))
                {
                    name = story.description;
                }
                
                label.text = $"{name}";
                label.style.paddingBottom = new StyleLength(6f);
                label.style.paddingLeft = new StyleLength(6f);
            };
            _listView.itemsSource = _stories;
            _listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            _listView.itemsChosen += OnChooseItem;
            _listView.selectionChanged += OnSelectItem;

            refreshButton.clicked += () => { RefreshStoryList(service); };
            
            rootVisualElement.Add(_listView);
            UpdateStyles();

            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged; 
        }

        private void OnSelectItem(IEnumerable<object> obj)
        {
            var firstItem = obj.FirstOrDefault();
            if (firstItem == null) return;
            
            var sceneLike = firstItem as StoryScene;
            if (sceneLike == null)
            {
                Debug.LogWarning($"Click on unknown item: {firstItem}");
                return;
            }
            
            var windowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            EditorWindow window = GetWindow(windowType);
            Selection.activeObject = sceneLike;
        }

        private void RefreshStoryList(StoryService service)
        {
            _stories = service.FindStories(false);
            _listView.itemsSource = _stories;
            _listView.RefreshItems();
            Repaint();
        }

        private void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj)
        {
            var service = new StoryService();
            RefreshStoryList(service);
            UpdateStyles();
        }

        private void UpdateStyles()
        {
            if (!EditorApplication.isPlaying)
            {
                _disableLabel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _listView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            }
            else
            {
                _disableLabel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                _listView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }

        private void OnChooseItem(IEnumerable<object> obj)
        {
            var firstItem = obj.FirstOrDefault();
            if (firstItem == null) return;
            
            var sceneLike = firstItem as StoryScene;
            if (sceneLike == null)
            {
                Debug.LogWarning($"Click on unknown item: {firstItem}");
                return;
            }
            
            var service = new StoryService();
            service.PlayStory(sceneLike);
        }
    }
}