using UnityEngine;

class TargetFrameRate : MonoBehaviour
{
    [SerializeField] int fps = 60;

    void Start() => Application.targetFrameRate = fps;
}
