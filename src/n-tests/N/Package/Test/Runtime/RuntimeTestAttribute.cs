using System;

namespace N.Package.Test.Runtime
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class RuntimeTestAttribute : Attribute
  {
  }
}