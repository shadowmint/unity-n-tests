using System;
using System.Collections.Generic;
using N.Package.Tests.Runtime.Outputs;
using UnityEngine;

namespace N.Package.Tests.Runtime
{
  public class RuntimeTestRunner : MonoBehaviour
  {
    [Tooltip("Set true to run the tests again")]
    public bool RunTests = true;

    [Tooltip("Automatically spawn UI for test runs?")]
    public bool DisplayTestOutput = true;

    [Tooltip("Timeout tests if they take longer than this; set to -ve or zero to never timeout")]
    public float TestTimeout = 5f;

    public List<RuntimeTestGroup> TestCases;

    private readonly Queue<RuntimeTest> _tests = new Queue<RuntimeTest>();

    private bool _runningTests;

    private RuntimeTestGroup _activeTestGroup;

    private IRuntimeTestOutput _output;

    private readonly List<RuntimeTest> _allTests = new List<RuntimeTest>();

    private float _elapsed;

    public void Enqueue(RuntimeTest runtimeTest)
    {
      _allTests.Add(runtimeTest);
    }

    public void Completed(RuntimeTestResult activeTest)
    {
      if (_activeTestGroup == null || _activeTestGroup.Active != activeTest) return;
      _activeTestGroup.CompletedActiveTest();
    }


    public void Completed(RuntimeTestResult activeTest, Exception error)
    {
      if (_activeTestGroup == null || _activeTestGroup.Active != activeTest) return;
      _activeTestGroup.CompletedActiveTest(false, error);
    }

    public void Log(RuntimeTest runtimeTest, string message)
    {
      if (_activeTestGroup == null || _activeTestGroup.TestCase != runtimeTest)
      {
        Debug.Log("Discarding non active log message");
        Debug.Log(message);
        return;
      }

      _activeTestGroup.Log(message);
    }

    public void Start()
    {
      _output = GetOutputTarget();
    }

    private IRuntimeTestOutput GetOutputTarget()
    {
      return DisplayTestOutput ? new RuntimeTestOutput() as IRuntimeTestOutput : new MockRuntimeTestOutput();
    }

    public void Update()
    {
      if (_runningTests)
      {
        StartNextTest();
        AbortExpiredTest();
        return;
      }

      if (RunTests)
      {
        RunTests = false;
        _output.Reset();
        _runningTests = true;
        _tests.Clear();
        _activeTestGroup = null;
        _allTests.ForEach(i => _tests.Enqueue(i));
        TestCases.Clear();
      }
    }

    private void AbortExpiredTest()
    {
      if (_activeTestGroup == null) return;
      _elapsed += Time.deltaTime;
      if (_elapsed > TestTimeout)
      {
        Completed(_activeTestGroup.Active, new Exception("Timeout executing test. Did you forget to call Completed()?"));
      }
    }

    private void StartNextTest()
    {
      // Start next test if group is idle
      if (_activeTestGroup != null && _activeTestGroup.Idle)
      {
        var activeTest = _activeTestGroup.StartNextTest();
        if (activeTest == null)
        {
          _activeTestGroup = null;
        }
        else
        {
          _output.Started(activeTest);
          _elapsed = 0f;
          activeTest.Execute.Invoke();
        }
      }

      // If we have no group, try to start the next group
      if (_activeTestGroup == null)
      {
        var runOutOfTests = !StartNewTestGroup();
        if (runOutOfTests)
        {
          _runningTests = false;
          _output.CompletedAllTests();
        }
      }
    }

    private bool StartNewTestGroup()
    {
      var next = _tests.Count > 0 ? _tests.Dequeue() : null;
      if (next == null) return false;

      var group = new RuntimeTestGroup(this, next, _output);
      _activeTestGroup = group;
      _output.Started(group);
      TestCases.Add(group);
      return true;
    }

    public static RuntimeTestRunner PlayModeInstance
    {
      get
      {
        var runner = FindObjectOfType<RuntimeTestRunner>();
        if (runner != null) return runner;

        var runnerObj = new GameObject();
        runnerObj.transform.name = "RuntimeTestRunner";
        runner = runnerObj.AddComponent<RuntimeTestRunner>();
        return runner;
      }
    }
  }
}