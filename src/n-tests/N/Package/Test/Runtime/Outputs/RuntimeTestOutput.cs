using UnityEngine;

namespace N.Package.Test.Runtime
{
  public class RuntimeTestOutput : IRuntimeTestOutput
  {
    private GameObject _ui;
    private RuntimeTestOutputUi _api;
    private const string RuntimeTestOutputAsset = "N/Package/Test/RuntimeTestOutput";
    private int _passed;
    private int _failed;

    public RuntimeTestOutput()
    {
      var template = Resources.Load(RuntimeTestOutputAsset, typeof(GameObject));
      _ui = Object.Instantiate(template) as GameObject;
      _api = _ui.GetComponentInChildren<RuntimeTestOutputUi>();
    }

    public void Completed(RuntimeTestResult activeTest)
    {
      if (activeTest.State == RuntimeTestState.Success)
      {
        _passed += 1;
        _api.Log($"Test End: {activeTest.Name} OK", Color.green);
      }
      else
      {
        _failed += 1;
        _api.Log($"Test End: {activeTest.Name} FAILED", Color.red);
        if (activeTest.Error != null)
        {
          _api.Log(activeTest.Error.ToString(), Color.red);
        }
      }
    }

    public void Started(RuntimeTestResult activeTest)
    {
      _api.Log($"\nTest Start: {activeTest.Name}", Color.gray);
    }

    public void Log(string message)
    {
      _api.Log($"Debug: {message}", Color.black);
    }

    public void Reset()
    {
      _api.Reset();
      _passed = 0;
      _failed = 0;
      _api.Log("Starting new test run...", Color.gray);
    }

    public void CompletedAllTests()
    {
      _api.Log("\nFinished all tests", Color.black);
      _api.Log($"\n{_passed} tests PASSED", Color.green);
      _api.Log($"{_failed} tests FAILED", Color.red);
    }

    public void Started(RuntimeTestGroup activeTest)
    {
      _api.Log($"\nTestGroup: {activeTest.TestCase.GetType().FullName}", Color.gray);
    }
  }
}