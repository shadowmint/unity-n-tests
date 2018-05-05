using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace N.Package.Test.Runtime
{
  [System.Serializable]
  public class RuntimeTestGroup
  {
    private readonly RuntimeTestRunner _parent;
    
    private readonly IRuntimeTestOutput _output;

    private RuntimeTestResult _active;

    public RuntimeTest TestCase;
    
    public Queue<RuntimeTestResult> Pending = new Queue<RuntimeTestResult>();
    
    public List<RuntimeTestResult> Results = new List<RuntimeTestResult>();

    public RuntimeTestResult Active => _active;
    
    public RuntimeTestGroup(RuntimeTestRunner parent, RuntimeTest test, IRuntimeTestOutput output)
    {
      _parent = parent;
      TestCase = test;
      _output = output;
      LoadRunnableTests();
    }

    public bool Idle => _active == null;

    public RuntimeTestResult CompletedActiveTest(bool success = true, Exception error = null)
    {
      if (_active == null) return null;
      var rtn = _active;
      _active.State = success ? RuntimeTestState.Success : RuntimeTestState.Failed;
      _active.Error = error;
      Results.Add(_active);
      _output.Completed(_active);
      _active = null;
      return rtn;
    }

    public RuntimeTestResult StartNextTest()
    {
      if (_active != null)
      {
        CompletedActiveTest(false);
      }

      var next = Pending.Count > 0 ? Pending.Dequeue() : null;
      if (next != null)
      {
        next.State = RuntimeTestState.Running;
        TestCase.Active = next;
      }

      _active = next;
      return next;
    }

    public void Log(string message)
    {
      _output.Log(message);
      _active?.Log.Add(message);
    }

    private void LoadRunnableTests()
    {
      var allMethods = TestCase.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
      foreach (var method in allMethods)
      {
        var attributes = method.GetCustomAttributes<RuntimeTestAttribute>().ToArray();
        if (attributes.Length > 0)
        {
          Pending.Enqueue(MakeTestCaseFrom(method));
        }
      }
    }

    private RuntimeTestResult MakeTestCaseFrom(MethodInfo method)
    {
      Action task = () =>
      {
        try
        {
          method.Invoke(TestCase, new object[] { });
        }
        catch (Exception error)
        {
          _parent.Completed(Active, error);
        }
      };
      return new RuntimeTestResult()
      {
        Error = null,
        Execute = task,
        Log = new List<string>(),
        Name = $"{TestCase.GetType().FullName}::{method.Name}",
        State = RuntimeTestState.Pending
      };
    }
  }
}
