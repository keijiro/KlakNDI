// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Klak.Ndi;

public class SourceSelector : MonoBehaviour
{
    [SerializeField] Dropdown _dropdown;

    NdiReceiver _receiver;

    List<string> _sourceNames = new List<string>();
    bool _disableCallback;

    void Start()
    {
        _receiver = GetComponent<NdiReceiver>();
    }

    void Update()
    {
        // HACK: Assuming that the dropdown would have more than three child
        // objects while the menu is opened. Stop updating it while visible.
        if (_dropdown.transform.childCount > 3) return;

        // Retrieve the NDI source names.
        NdiManager.GetSourceNames(_sourceNames);

        // Update the current selection.
        var index = _sourceNames.IndexOf(_receiver.sourceName);
        if (index < 0)
        {
            // Append the current name to the list when it's not found.
            index = _sourceNames.Count;
            _sourceNames.Add(_receiver.sourceName);
        }

        // We don't like to receive callback while editing options.
        _disableCallback = true;

        // Update the menu options.
        _dropdown.ClearOptions();
        _dropdown.AddOptions(_sourceNames);
        _dropdown.value = index;
        _dropdown.RefreshShownValue();

        // Resume receiving callback.
        _disableCallback = false;
    }

    public void OnChangeValue(int value)
    {
        if (_disableCallback) return;
        _receiver.sourceName = _sourceNames[value];
    }
}
