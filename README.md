KlakNDI
=======

![gif](https://i.imgur.com/k3Bwcoq.gif)
![photo](https://i.imgur.com/HY1NMYm.jpg)

**KlakNDI** is a [NewTek NDI] protocol plugin for Unity that allows
sending/receiving video streams between computers via a local area network.
It provides a high quality, low latency and performant way to mix multiple
video streams from several applications and devices without the need of complex
video capturing setups but only a wired/wireless network connection. The plugin
is implemented to fully utilize the [asynchronous GPU readback] and [custom
texture update] features to achieve the optimal performance and the lowest
latency in Unity.

NDI™ is a trademark of NewTek, Inc.

[NewTek NDI]: http://NDI.NewTek.com/
[asynchronous GPU readback]: https://github.com/keijiro/AsyncCaptureTest
[custom texture update]: https://github.com/keijiro/TextureUpdateExample

System requirements
-------------------

- Unity 2018.3 or later
- Windows: Direct3D 11 support
- macOS: 64-bit, Metal support
- iOS: Metal support

The latest version of the plugin (v0.1.x) requires Unity 2018.3 or later for
macOS and iOS support. Consider using v0.0.x when you have to use one of the
previous versions of Unity.

The iOS plugin only supports the sender functionality due to a limitation of
the NDI SDK.

The plugin is presented in a self-contained form for Windows and macOS. The NDI
SDK is only required when building to iOS.

Installation
------------

Download and import one of the .unitypackage files from [Releases] page.

[Releases]: https://github.com/keijiro/KlakNDI/releases

NDI Sender component
--------------------

The **NDI Sender component** (`NdiSender`) is used to send rendered frames to
other NDI supported software/hardware via a network.

There are two modes in NDI Sender:

### Camera capture mode

![inspector](https://i.imgur.com/EH4caKU.png)

The NDI Sender component runs in the **camera capture mode** when attached to a
camera object. It automatically captures frames rendered by the camera and
publish them to the network. The dimensions of the frames are dependent on the
screen/game view size.

Note that the camera capture mode is not compatible with [scriptable render
pipelines].

[scriptable render pipelines]: https://docs.unity3d.com/Manual/ScriptableRenderPipeline.html

### Render texture mode

![inspector](https://i.imgur.com/BN5RsXl.png)

The NDI Sender component runs in the **render texture mode** when it's
independent from any camera. To publish frames in this mode, a [render texture]
should be specified via the **Source Texture** property. The NDI Sender
component publishes the contents of the render texture every frame.

[render texture]: https://docs.unity3d.com/Manual/class-RenderTexture.html

### Alpha channel support

The NDI Sender component uses the UYVY format (4:2:2 subsampled color) by
default, which is the most optimal setting to feed a video stream to the NDI
library. By enabling the **Alpha Support** property, it can be switched to the
UYVA format (4:2:2 color + alpha) that supports alpha channel. It requires an
extra bandwidth and processing resources, so that it's recommended disabling
when alpha channel is not actually needed.

NDI Receiver component
----------------------

![inspector](https://i.imgur.com/hdxALxS.png)

The NDI Receiver component (`NdiReceiver`) is used to receive frames published
by other NDI supported software/hardware via a network.

### Name Filter property

In case that multiple NDI sources (senders) exist in a network, the **Name
Filter** property is used to determine which source should be connected. The NDI
Receiver tries to connect to the first source whom name contains the string
specified in the property. Note that this string matching is case-sensitive.

When nothing is specified in the Name Filter property, the NDI Receiver tries
connecting to the first found source without string matching.

### Target Texture property

The NDI Receiver updates a render texture specified in the **Target Texture**
property every frame. Note that the NDI Receiver doesn't care about aspect
ratio; The dimensions of the render texture should be manually adjusted to
avoid stretching.

### Target Renderer property

When a renderer component (in most cases it may be a mesh renderer) is
specified in the **Target Renderer** property, the NDI Receiver sets the
received frames to one of the texture properties of the material used in the
renderer. This is a convenient way to display received frames when they're only
used in a single instance of renderer.

### Script interface

The received frames are also accessible via the `receivedTexture` property of
the `NdiReceiver` class. Note that the `receivedTexture` object is
destroyed/recreated when the settings (e.g. screen size) are changed. It's
recommended updating the reference every frame.

NDI source list view
--------------------

![window](https://i.imgur.com/gdfF7tO.png)

The NDI source list view is a handy way to check what NDI sources are
currently available in the network. To open the list view, from the
application menu select "Window" - "Klak" - "NDI Source List". The list is
updated even in the edit mode.

License
-------

[MIT](LICENSE)

The NDI dynamic library files (`Processing.NDI.Lib.*.dll`, `libndi.*.dylib`)
contained in the plugin internal directory is provided by NewTek, Inc under the
NDI® SDK License Agreement. Please review the original license when
distributing products with the plugin.
