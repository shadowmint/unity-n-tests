#if N_TESTS

namespace N.Package.Test.Runtime.Examples
{
  public class ExampleSimpleRuntimeTest : RuntimeTest
  {
    [RuntimeTest]
    public void TestSimpleThing()
    {
      Log("Starting simple test");
      Assert(true);
      Log("All done with simple test!");
      Completed();
    }

    [RuntimeTest]
    public void TestSimpleThingThatFails()
    {
      Log("Starting simple test that fails");
      Assert(false);
      Completed();
    }

    [RuntimeTest]
    public void TestSimpleThingThatTimesOut()
    {
      Log("Starting simple test that fails");
    }
  }
}
#endif