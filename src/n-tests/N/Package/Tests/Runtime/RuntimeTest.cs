using System;
using UnityEngine;

namespace N.Package.Tests.Runtime
{
  [RequireComponent(typeof(RuntimeTestRunner))]
  public class RuntimeTest : MonoBehaviour
  {
    private RuntimeTestRunner _runner;

    private TestCase _testCase = new TestCase();
    
    public RuntimeTestResult Active { get; set; }

    public void Awake()
    {
      _runner = GetComponent<RuntimeTestRunner>();
      if (_runner == null)
      {
        _runner = RuntimeTestRunner.PlayModeInstance;
      }

      _runner.Enqueue(this);
    }

    /// <summary>
    /// Runtime tests are async; you must explicitly terminate a test once you're done with it.
    /// </summary>
    protected void Completed()
    {
      _runner.Completed(Active);
    }

    /// <summary>
    /// Runtime tests are async; you must explicitly terminate a test once you're done with it.
    /// </summary>
    protected void Failed(Exception error)
    {
      _runner.Completed(Active, error);
    }

    /// <summary>
    /// To display test output, log it with this, not Debug.Log
    /// </summary>
    protected void Log(string message)
    {
      _runner.Log(this, message);
    }

    /// Assert something is true in a test
    public void Assert(bool value)
    {
      _testCase.Assert(value);
    }

    /// Assert something is true in a test
    public void Assert(bool value, string msg)
    {
      _testCase.Assert(value, msg);
    }

    /// Assert the current block of code is never reached
    public void Unreachable()
    {
      _testCase.Unreachable();
    }

    /// Assert the current block of code is never reached
    public void Unreachable(string msg)
    {
      _testCase.Unreachable(msg);
    }

    /// Create an empty game object and return it.
    public GameObject SpawnBlank()
    {
      return _testCase.SpawnBlank();
    }

    /// Create an empty game object and return it.
    public GameObject SpawnObjectWithComponent<T>() where T : Component
    {
      return _testCase.SpawnObjectWithComponent<T>();
    }

    /// Create an empty game object and return it.
    public T SpawnComponent<T>() where T : Component
    {
      return _testCase.SpawnComponent<T>();
    }

    /// Teardown created objects
    public void TearDown()
    {
      _testCase.TearDown();
    }
  }
}