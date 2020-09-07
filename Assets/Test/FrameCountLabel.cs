using UnityEngine;
using UnityEngine.UI;

sealed class FrameCountLabel : MonoBehaviour
{
    [SerializeField] Text _label = null;

    void Update()
      => _label.text = Time.frameCount.ToString();
}
