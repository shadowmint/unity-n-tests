#if N_TESTS

namespace N.Package.Test.Runtime.Examples
{
  public class ExampleStatefulRuntimeTest : RuntimeTest
  {
    private bool _busy;
    private int _counter;

    [RuntimeTest]
    public void TestStatefulRuntimeTest()
    {
      _busy = true;
      _counter = 0;
    }

    [RuntimeTest]
    public void TestStatefulRuntimeTest2()
    {
      _busy = true;
      _counter = -60;
    }

    public void Update()
    {
      if (!_busy) return;

      _counter += 1;
      if (_counter <= 10) return;

      _busy = false;
      Completed();
    }
  }
}

#endif