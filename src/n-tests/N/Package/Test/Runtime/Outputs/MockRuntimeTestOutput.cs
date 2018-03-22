using UnityEngine;

namespace N.Package.Test.Runtime
{
  public class MockRuntimeTestOutput : IRuntimeTestOutput
  {
    public void Completed(RuntimeTestResult activeTest)
    {
      Debug.Log($"Completed: {activeTest.Name}: {activeTest.State}");
      if (activeTest.Error != null)
      {
        Debug.LogError(activeTest.Error);
      }
    }

    public void Started(RuntimeTestResult activeTest)
    {
      Debug.Log($"Started: {activeTest.Name}");
    }

    public void Log(string message)
    {
      Debug.Log(message);
    }

    public void Reset()
    {
      Debug.Log("Started test run");
    }

    public void CompletedAllTests()
    {
      Debug.Log("Completed test run");
    }

    public void Started(RuntimeTestGroup activeTest)
    {
      Debug.Log($"Started test group: {activeTest.TestCase}");
    }
  }
}