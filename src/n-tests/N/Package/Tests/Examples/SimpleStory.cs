using System.Collections;
using UnityEngine;

namespace N.Package.Tests.Examples
{
    public class SimpleStory : Story
    {
        public GameObject template;
        
        protected override IEnumerator Execute()
        {
            var instance = Object.Instantiate(template);
            instance.transform.position = Vector3.zero;
            
            yield return new WaitForSeconds(0.5f);

            for (var i = 0; i < 10; i++)
            {
                instance.transform.position += 0.1f * Vector3.up;
                yield return new WaitForSeconds(0.1f);
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
}