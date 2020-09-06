KlakNDI
=======

![gif](https://i.imgur.com/I1ZMSY8.gif)

**KlakNDI** is a Unity plugin that allows sending/receiving video frames
between computers using [NDI].

[NDI]® (Network Device Interface) is a standard developed by [NewTek], Inc that
enables applications to deliver video streams via a local area network. Please
refer to [ndi.tv][NDI] for further information about the technology.

[NDI]: https://www.ndi.tv/
[NewTek]: https://www.newtek.com/

System Requirements
-------------------

- Unity 2019.4
- Windows: D3D11 and D3D12 are supported
- macOS: Metal required
- Linux: Vulkan required
- iOS: Metal required

KlakNDI supports all the standard render pipelines (built-in, URP, and HDRP).

Limitations On Mobile Platforms
-------------------------------

At the moment, **KlakNDI only supports the sender functionality on iOS**. This
limitation came from the NDI SDK, so it's not possible to add receiver
functionality support.

KlakNDI requires the NDI SDK v4.5 to build to iOS. Please download and install
the SDK before compilation.

At the moment, **KlakNDI doesn't support Android**.

How To Install
--------------

The KlakNDI package uses the [scoped registry] feature to import dependent
packages. Please add the following sections to the package manifest file
(`Packages/manifest.json`).

To the `scopedRegistries` section:

```
{
  "name": "Unity NuGet",
  "url": "https://unitynuget-registry.azurewebsites.net",
  "scopes": [ "org.nuget" ]
},
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.klak.ndi": "1.0.11"
```

After changes, the manifest file should look like below:

```
{
  "scopedRegistries": [
    {
      "name": "Unity NuGet",
      "url": "https://unitynuget-registry.azurewebsites.net",
      "scopes": [ "org.nuget" ]
    },
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.klak.ndi": "1.0.11",
...
```

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

NDI Sender Component
--------------------

![screenshot](https://i.imgur.com/kUnWqeZ.png)

The **NDI Sender** component (`NdiSender`) sends a video stream from a given
video source.

**NDI Name** - Specify the name of the NDI endpoint.

**Enable Alpha** - Enable this checkbox to make the stream contain the alpha
channel. You can disable it to reduce the bandwidth.

**Capture Method** - Specify how to capture the video source from the following
options.

  - Game View - The sender captures frames from the Game View.
  - Camera - The sender captures frames from a given camera. Note: This option
    only supports URP and HDRP.
  - Texture - The sender captures frames from a texture asset. You can also use
    a render texture with this option.

You can attach metadata using `.metadata` property.

NDI Receiver Component
----------------------

![screenshot](https://i.imgur.com/UmCvOK6.png)

The **NDI Receiver** component (`NdiReceiver`) receives a video stream and
feeds it to a renderer object or a render texture asset.

**NDI Name** - Specify the name of the NDI source. You can edit the text field
or use the selector to choose a name from currently available NDI sources.

**Target Texture** - The receiver copies the received frames into this render
texture asset.

**Target Renderer** - The receiver overrides a texture property of the given
renderer.

You can extract metadata using `.metadata` property.

Tips for Scripting
------------------

You can enumerate currently available NDI sources using the NDI Finder class
(`NdiFinder`). See the [Source Selector] example for usage.

[Source Selector]: Assets/Test/SourceSelector.cs

You can instantiate the NDI Sender/Receiver component from a script but have
to specify an NDI Resources asset (`NdiResources.asset`) right after the
instantiation. See the [Sender Benchmark]/[Receiver Benchmark] examples for
details.

[Sender Benchmark]:
https://github.com/keijiro/KlakNDI/blob/master/Assets/Test/SenderBenchmark.cs
[Receiver Benchmark]:
https://github.com/keijiro/KlakNDI/blob/master/Assets/Test/ReceiverBenchmark.cs
