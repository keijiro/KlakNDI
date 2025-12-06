using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using Klak.Ndi;

public sealed class SourceSelector : MonoBehaviour
{
    [SerializeField] NdiReceiver _receiver = null;

    [CreateProperty]
    public List<string> SourceList => NdiFinder.sourceNames.ToList();

    VisualElement UIRoot
      => GetComponent<UIDocument>().rootVisualElement;

    DropdownField UISelector
      => UIRoot.Q<DropdownField>("source-selector");

    void Start()
    {
        UISelector.dataSource = this;
        UISelector.RegisterValueChangedCallback
          (evt => _receiver.ndiName = evt.newValue);
    }
}
