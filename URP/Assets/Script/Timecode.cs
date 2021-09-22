using UnityEngine;
using UI = UnityEngine.UI;

sealed class Timecode : MonoBehaviour
{
    [SerializeField] UI.Text _label = null;
    [SerializeField] Camera _camera = null;

    Color _bgColor;

    void Start()
      => _bgColor = _camera.backgroundColor;

    void Update()
    {
        var t = Time.time;

        var frame = (int)(t * 60 % 60);
        _label.text = $"{(int)(t / 60):00}:{(int)(t % 60):00}:{frame:00}";

        var flash = (int)t != (int)(t - Time.deltaTime);
        _camera.backgroundColor = flash ? Color.white : _bgColor;
    }
}
