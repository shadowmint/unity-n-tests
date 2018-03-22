#if N_TESTS
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace N.Package.Test.Runtime.Examples
{
  public class ExamplePlayModeTest : TestCase
  {
    [Test]
    public void NewPlayModeTestSimplePasses()
    {
      Assert(true);
    }

    [UnityTest]
    public IEnumerator NewPlayModeTestWithEnumeratorPasses()
    {
      var instance = this.SpawnComponent<ExampleComponent>();
      yield return null;
      
      // Start and Update are called on the first frame
      Assert(instance.Values == 2);

      yield return null;
      Assert(instance.Values == 3);

      // Remove allocated instances
      TearDown();
    }
  }
}
#endif