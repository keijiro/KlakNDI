using UnityEngine;

namespace TestCards
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class TestOverlay : MonoBehaviour
    {
        #region Editable properties

        public enum Mode { Fill, Spectrum, Checker, Pattern, Shutter, Frequency }

        [SerializeField] Mode _mode = Mode.Pattern;

        public Mode mode {
            get { return _mode; }
            set { _mode = value; }
        }

        [SerializeField] Color _color = Color.gray;

        public Color color {
            get { return _color; }
            set { _color = value; }
        }

        [SerializeField, Range(1, 8)] int _scale = 4;

        public int scale {
            get { return _scale; }
            set { _scale = value; }
        }

        #endregion

        #region Private members

        [SerializeField, HideInInspector] Shader _shader = null;
        Material _material;

        #endregion

        #region MonoBehaviour functions

        void OnDestroy()
        {
            if (_material != null)
                if (Application.isPlaying)
                    Destroy(_material);
                else
                    DestroyImmediate(_material);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            _material.color = _color;
            _material.SetFloat("_Scale", 1.0f / Mathf.Pow(2, _scale));

            Graphics.Blit(source, destination, _material, (int)_mode);
        }

        #endregion
    }
}
