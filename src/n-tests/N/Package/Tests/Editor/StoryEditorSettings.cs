namespace N.Package.Tests
{
    [System.Serializable]
    class StoryEditorSettings
    {
        public string lastFilter;

        public static StoryEditorSettings Defaults()
        {
            return new StoryEditorSettings()
            {
                lastFilter = ".*"
            };
        }
    }
}