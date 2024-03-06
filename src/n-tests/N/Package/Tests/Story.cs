using System;
using System.Collections;
using System.Threading.Tasks;
using N.Package.Promises;
using N.Package.Workflows;
using UnityEngine;

namespace N.Package.Tests
{
    [RequireComponent(typeof(StoryScene))]
    public abstract class Story : MonoBehaviour
    {
        public void Start()
        {
            StartAsync().Dispatch();
        }

        private async Task StartAsync()
        {
            var workflow = new StoryWorkflow(this);
            try
            {
                await workflow.Run();
                Debug.Log($"<color=#00ff00>Test passed</color>");
                
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            catch (Exception error)
            {
                Debug.LogError("Test failed");
                Debug.LogException(error);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }

        /// <summary>
        /// Implement this to run the story.
        /// </summary>
        protected abstract IEnumerator Execute();

        public class StoryWorkflow : UnityWorkflow
        {
            private readonly Story _parent;

            public StoryWorkflow(Story parent)
            {
                _parent = parent;
            }

            public override string Id => _parent.GetType().Name;

            protected override void Validate()
            {
            }

            protected override IEnumerator ExecuteAsync()
            {
                yield return new WaitForEndOfFrame();
                var wrappedEnumerable = _parent.Execute();
                while (wrappedEnumerable.MoveNext())
                {
                    yield return wrappedEnumerable.Current;
                }
                Resolve();
            }
        }
    }
}