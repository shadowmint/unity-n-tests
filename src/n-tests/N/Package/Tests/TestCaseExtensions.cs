using System;
using System.Threading.Tasks;

namespace N.Package.Tests
{
  public static class TestCaseExtensions
  {
    public static void RunAsyncTest(this TestCase self, Func<Task> task)
    {
      task().Wait();
    }
  }
}