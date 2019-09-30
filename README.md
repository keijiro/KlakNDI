KlakNDI
=======

![gif](https://i.imgur.com/k3Bwcoq.gif)
![photo](https://i.imgur.com/HY1NMYm.jpg)

**KlakNDI** is a Unity plugin that allows sharing video frames between
computers using NDI.

[NDI] (Network Device Interface) is a standard developed by [NewTek] that
enables applications to deliver video streams via a local area network. It
provides a high quality, low latency and performant way to mix multiple video
streams sent from several applications/devices without the need of a complex
video capturing setup but only a wired/wireless network connection.

NDI® is a registered trademark of NewTek, Inc. Please refer to [ndi.tv][NDI]
for further information about the technology.

[NDI]: https://www.ndi.tv/
[NewTek]: https://www.newtek.com/

System requirements
-------------------

- Unity 2018.3 or later
- Windows, macOS and Linux (full feature support)
- iOS (limited feature support)

### Windows

The plugin only supports Direct3D 11.

### macOS

The plugin only supports Metal.

### Linux

The plugin only supports Vulkan.

The NDI 4.0.1 shared library files must be installed on the system to use the
plugin. To install them, download the [NDI SDK] and run
[this script][install-ndi.sh] as root in the extracted SDK directory.

[NDI SDK]: https://www.ndi.tv/sdk/
[install-ndi.sh]: https://gist.github.com/keijiro/0cd095b54e5c2846fb683ad48e8292d2

### iOS

The iOS plugin only supports the sender functionality due to a limitation of
the standard NDI SDK.

The [NDI SDK] 4.0 is required when building a project on Xcode.

Installation
------------

Download and import one of the `.unitypackage` files from [Releases] page.

You can also install the package using the Package Manager with the [scoped
registry] feature. Please add the following sections to the package manifest
file (`Packages/manifest.json`).

To the `scopedRegistries` section:

```
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.klak.ndi": "0.2.3"
```

After changes, the manifest file should look like below:

```
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.klak.ndi": "0.2.3",
    ...
```

[Releases]: https://github.com/keijiro/KlakNDI/releases
[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

NDI Sender component
--------------------

The **NDI Sender component** (`NdiSender`) is used to send frames to
NDI-enabled systems.

There are two modes in NDI Sender:

### Camera capture mode

![inspector](https://i.imgur.com/EH4caKU.png)

The NDI Sender component runs in the **camera capture mode** when attached to a
camera object. It automatically captures frames rendered by the camera and
publish them to a network. The dimensions of the frames are dependent on the
screen/game view size.

Note that the camera capture mode is not compatible with [scriptable render
pipelines]; The render texture mode should be applied in case of using SRP.

[scriptable render pipelines]: https://docs.unity3d.com/Manual/ScriptableRenderPipeline.html

### Render texture mode

![inspector](https://i.imgur.com/BN5RsXl.png)

The NDI Sender component runs in the **render texture mode** when it's
independent from any camera. In this mode, the sender publishes content of a
render texture specified in the **Source Texture** property. This render
texture should be updated in some way -- by attaching to a camera as a target
texture, by [custom render texture], etc.

[render texture]: https://docs.unity3d.com/Manual/class-RenderTexture.html
[custom render texture]: https://docs.unity3d.com/Manual/CustomRenderTextures.html

### Alpha channel support

The **Alpha Support** property controls if the sender includes alpha channel to
published frames. In most use-cases of Unity, alpha channel in rendered frames
is not in use; it only contains garbage data. It's generally recommended to
turn off Alpha Support to prevent causing wrong effects on a receiver side.

NDI Receiver component
----------------------

![inspector](https://i.imgur.com/pKn7mTn.png)

The NDI Receiver component (`NdiReceiver`) is used to receive frames sent from
a NDI-enabled system.

### Source Name property

The NDI Receiver tries to connect to a sender that has a name specified in the
**Source Name** property. It can be manually edited with the text field or
selected from the drop-down list, which shows currently available NDI senders.

### Target Texture property

The NDI Receiver updates a render texture specified in the **Target Texture**
property every frame. Note that the receiver doesn't take the aspect ratio into
account. The dimensions of the render texture should be manually adjusted to
avoid stretching.

### Target Renderer property

When a renderer component is specified in the **Target Renderer** property, the
receiver overrides one of texture properties in the renderer using a [material
property block]. This is a handy way to display received frames when it's only
used in a single instance of renderer.

[material property block]: https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html

### Script interface

Received frames are also accessible via the `receivedTexture` property of the
`NdiReceiver` class. Note that the `receivedTexture` object is
destroyed/recreated when frame settings (e.g. screen dimensions) are modified.
It's recommended updating the texture reference every frame.

Performance considerations
--------------------------

### NDI vs Spout/Syphon: Which one is better?

The answer is simple: If you're going to use multiple apps on a single
computer, and those apps support Spout/Syphon, you should use one of them. NDI
is just overkill for such cases.

Spout/Syphon are superior solutions for local interoperation. They're faster,
low latency, more memory efficient and better quality. It's recommended using
Spout/Syphon unless multiple computers are involved.

### The plugin works slow on MacBook

It was observed that the plugin worked significantly slow on some MacBook
devices with dedicated graphics. It's probably improved by switching to
integrated graphics. From Unity 2018.3, GPU in use can be implicitly selected
in the Preferences panel. Please try changing it when significant slowdown is
observed.
