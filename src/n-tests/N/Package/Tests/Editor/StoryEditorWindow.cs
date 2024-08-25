using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using N.Package.Tests.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace N.Package.Tests
{
    class StoryEditorWindow : EditorWindow
    {
        private List<StoryScene> _stories;
        private Label _disableLabel;
        private ListView _listView;
        private TextField _filterField;

        private static readonly Lazy<StoryEditorSettings> Settings = new(GetSettings);

        private string LastFilter
        {
            get => Settings.Value.lastFilter;
            set
            {
                Settings.Value.lastFilter = value;
                SaveSettings();
            }
        }

        [MenuItem("Window/Storybook")]
        public static void ShowWindow()
        {
            GetWindow(typeof(StoryEditorWindow));
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

            _filterField = new TextField();
            _filterField.style.marginBottom = new StyleLength(4f);
            _filterField.style.marginRight = new StyleLength(2f);
            _filterField.value = LastFilter ?? "";
            rootVisualElement.Add(_filterField);

            _disableLabel = new Label("Stories cannot be run in play mode");
            _disableLabel.style.paddingLeft = new StyleLength(6f);
            rootVisualElement.Add(_disableLabel);

            _listView.makeItem = () => new Label();
            _listView.bindItem = (item, index) =>
            {
                var label = item as Label;
                if (label == null) return;

                var story = _stories[index];
                var displayName = DisplayNameFor(story);

                label.text = $"{displayName}";
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
            RefreshStoryList(service);
        }

        private static string DisplayNameFor(StoryScene story)
        {
            var name = story.targetScene;
            if (!string.IsNullOrWhiteSpace(story.description))
            {
                name = story.description;
            }

            return name;
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

            // Filter to subset
            if (!string.IsNullOrWhiteSpace(_filterField.text))
            {
                var filter = _filterField.value.ToLowerInvariant();
                Regex regex;
                try
                {
                    regex = new Regex(filter);
                }
                catch
                {
                    regex = new Regex(".*");
                }

                _stories = _stories.Where(i => regex.IsMatch(DisplayNameFor(i).ToLowerInvariant())).ToList();
                LastFilter = _filterField.value;
            }

            // Sort by name
            _stories = _stories.OrderBy(DisplayNameFor).ToList();

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

        private static StoryEditorSettings GetSettings()
        {
            var defaultSettings = StoryEditorSettings.Defaults();
            var data = EditorPrefs.GetString(nameof(StoryEditorWindow), JsonUtility.ToJson(defaultSettings, false));
            return JsonUtility.FromJson<StoryEditorSettings>(data);
        }

        public void SaveSettings()
        {
            var settings = Settings.Value;
            var data = JsonUtility.ToJson(settings, false);
            EditorPrefs.SetString(nameof(StoryEditorWindow), data);
        }
    }
}