namespace N.Package.Tests.Runtime
{
  public interface IRuntimeTestOutput
  {
    void Completed(RuntimeTestResult activeTest);
    void Started(RuntimeTestResult activeTest);
    void Log(string message);
    void Reset();
    void CompletedAllTests();
    void Started(RuntimeTestGroup activeTest);
  }
}