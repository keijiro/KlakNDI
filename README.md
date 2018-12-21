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

[NDI]: http://ndi.newtek.com/
[NewTek]: http://www.newtek.com/

System requirements
-------------------

- Unity 2018.3 or later
- **Windows**: Direct3D 11 support
- **macOS**: 64-bit, Metal support
- **iOS**: Metal support

The iOS plugin only supports the sender functionality due to a limitation of
the NDI SDK.

The plugin is presented in a self-contained form on Windows and macOS. The
[NDI SDK] is required when building to iOS.

[NDI SDK]: https://www.newtek.com/ndi/sdk/

Installation
------------

Download and import one of the `.unitypackage` files from [Releases] page.

You can also use [Git support on Package Manager] to import the package. Add
the following line to the `dependencies` section in the package manifest file
(`Packages/manifest.json`). Note that this feature is only available from
Unity 2018.3. See [the forum thread][Git support on Package Manager] for
futher details.

```
"jp.keijiro.klak.ndi": "https://github.com/keijiro/KlakNDI.git#upm"
```

[Releases]: https://github.com/keijiro/KlakNDI/releases
[Git support on Package Manager]:
    https://forum.unity.com/threads/git-support-on-package-manager.573673/

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

License
-------

[MIT](LICENSE)

The NDI dynamic library files (`Processing.NDI.Lib.*.dll`, `libndi.*.dylib`)
contained in the plugin internal directory is provided by NewTek, Inc under the
NDI® SDK License Agreement. Please review the original license when
distributing products with the plugin.
