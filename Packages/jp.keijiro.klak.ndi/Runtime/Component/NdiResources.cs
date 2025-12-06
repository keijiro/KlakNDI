// We hide the create menu item because we don't expect a user to create an
// instance manually -- They should use the NdiResources.asset file provided
// within this package. To enable the menu item, uncomment the line bellow.
// #define SHOW_MENU_ITEM

using UnityEngine;

namespace Klak.Ndi {

#if SHOW_MENU_ITEM
[CreateAssetMenu(fileName = "NdiResources",
                 menuName = "ScriptableObjects/KlakNDI/NDI Resources")]
#endif
public sealed class NdiResources : ScriptableObject
{
    public ComputeShader encoderCompute;
    public ComputeShader decoderCompute;
}

} // namespace Klak.Ndi
