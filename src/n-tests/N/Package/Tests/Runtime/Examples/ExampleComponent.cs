using UnityEngine;

namespace N.Package.Tests.Runtime.Examples
{
  public class ExampleComponent : MonoBehaviour
  {
    public int Values;

    public void Start()
    {
      Values = 1;
    }

    public void Update()
    {
      Values += 1;
    }
  }
}