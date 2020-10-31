#if N_TESTS
using NUnit.Framework;
using N.Package.Test.Runtime.Examples;
using UnityEngine;

namespace N.Package.Test.Editor
{
  public class ExampleGameObjectTest : TestCase
  {
    [Test]
    public void TestComponentLifecycle()
    {
      var instance = SpawnComponent<ExampleComponent>();

      instance.Start();
      Assert(instance.Values == 1);

      instance.Update();
      Assert(instance.Values == 2);

      // Remove allocated instances
      TearDown();
    }

    [Test]
    public void TestComponentCount()
    {
      SpawnComponent<ExampleComponent>();
      var instances = Object.FindObjectsOfType<ExampleComponent>();

      Assert(instances.Length == 1);

      // Remove allocated instances
      TearDown();
    }
  }
}
#endif