using System;

namespace N.Package.Tests.Runtime
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class RuntimeTestAttribute : Attribute
  {
  }
}