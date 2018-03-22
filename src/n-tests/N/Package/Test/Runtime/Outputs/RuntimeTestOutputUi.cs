using UnityEngine;
using UnityEngine.UI;

namespace N.Package.Test.Runtime
{
  public class RuntimeTestOutputUi : MonoBehaviour
  {
    public Text Output;

    public void Reset()
    {
      if (Output == null) return;
      Output.text = null;
    }

    public void Log(string message, Color color)
    {
      if (Output == null) return;
      Output.text += $"<color=#{HexCodeFor(color)}>{message}</color>\n";
    }

    private string HexCodeFor(Color color)
    {
      return ColorUtility.ToHtmlStringRGBA(color);
    }
  }
}