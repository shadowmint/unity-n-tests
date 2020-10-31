#if N_TESTS

namespace N.Package.Test.Runtime.Examples
{
  public class ExampleComponentRuntimeTest : RuntimeTest
  {
    private bool _running;
    private ExampleComponent _component;

    [RuntimeTest]
    public void TestComponentInstance()
    {
      _component = gameObject.AddComponent<ExampleComponent>();
      _running = true;
    }

    public void Update()
    {
      if (!_running) return;
      Assert(_component.Values > 0);
      
      if (_component.Values != 10) return;
      
      _running = false;
      Completed();
    }
  }
}

#endif