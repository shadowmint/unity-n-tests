using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace N.Package.Test
{
  /// <summary>
  /// Unity aware test case helper methods.
  /// </summary>
  public class TestCase
  {
    /// Assert something is true in a test
    public void Assert(bool value)
    {
      if (!value)
      {
        throw new Exception("TestCase failed");
      }
    }

    /// Assert something is true in a test
    public void Assert(bool value, string msg)
    {
      if (!value)
      {
        throw new Exception($"TestCase failed: {msg}");
      }
    }

    /// Assert the current block of code is never reached
    public void Unreachable()
    {
      throw new Exception("TestCase failed: Entered unreachable block");
    }

    /// Assert the current block of code is never reached
    public void Unreachable(string msg)
    {
      throw new Exception($"TestCase failed: {msg}");
    }

    /// Create an empty game object and return it.
    public GameObject SpawnBlank()
    {
      if (TestExtensionsData.Spawned == null)
      {
        TestExtensionsData.Spawned = new List<GameObject>();
      }

      var rtn = new GameObject();
      TestExtensionsData.Spawned.Add(rtn);
      return rtn;
    }

    /// Create an empty game object and return it.
    public GameObject SpawnObjectWithComponent<T>() where T : Component
    {
      var rtn = SpawnBlank();
      rtn.AddComponent<T>();
      return rtn;
    }

    /// Create an empty game object and return it.
    public T SpawnComponent<T>() where T : Component
    {
      var obj = SpawnBlank();
      var rtn = obj.AddComponent<T>();
      return rtn;
    }

    /// Teardown created objects
    public void TearDown()
    {
      if (TestExtensionsData.Spawned == null) return;
      foreach (var obj in TestExtensionsData.Spawned)
      {
#if UNITY_EDITOR
        Object.DestroyImmediate(obj);
#else
        Object.Destroy(obj);
#endif
      }

      TestExtensionsData.Spawned = null;
    }
  }
}