using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace N.Package.Tests.Examples
{
    public class UserInteractionStory : Story
    {
        public bool isClicked;

        public float timeout = 5f;

        public TMP_Text box;
        
        public void OnClick()
        {
            isClicked = true;
        }

        protected override IEnumerator Execute()
        {
            var elapsed = 0f;
            while (!isClicked)
            {
                box.text = $"Remaining time: {timeout - elapsed}s";         
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;

                if (elapsed > timeout)
                {
                    throw new Exception("Timeout waiting for user interaction");
                }
            }
        }
    }
}