using UnityEngine;
using UI = UnityEngine.UI;

public sealed class FrameCount : MonoBehaviour
{
    void Update()
      => GetComponent<UI.Text>().text = $"{Time.frameCount}";
}
