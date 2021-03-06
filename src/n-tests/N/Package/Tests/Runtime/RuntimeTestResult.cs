﻿using System;
using System.Collections.Generic;

namespace N.Package.Tests.Runtime
{
  [System.Serializable]
  public class RuntimeTestResult
  {
    public string Name;
    public List<string> Log;
    public Exception Error;
    public RuntimeTestState State;
    public Action Execute;
  }
}