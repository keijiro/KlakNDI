KlakNDI
=======

![gif](https://i.imgur.com/I1ZMSY8.gif)

**KlakNDI** is a Unity plugin that allows sending/receiving video streams
between several devices using [NDI]®.

[NDI]® (Network Device Interface) is a standard developed by [NewTek], Inc that
enables applications to deliver video streams via a local area network. Please
refer to [ndi.tv][NDI] for further information about the technology.

[NDI]: https://www.ndi.tv/
[NewTek]: https://www.newtek.com/

System Requirements
-------------------

- Unity 2019.4
- .NET Standard 2.0 or 2.1

Desktop platforms:

- Windows: x64, D3D11/D3D12
- macOS: x64 or arm64 (M1), Metal
- Linux: x64, Vulkan

Mobile platforms:

- iOS: arm64, Metal
- Android: arm64, Vulkan

KlakNDI runs without the NDI SDK on most supported platforms, but only the iOS
platform requires the SDK to build on Xcode. Please download and install the NDI
Advanced SDK for iOS in advance of building.

Known Issues and Limitations
----------------------------

- At the moment, KlakNDI doesn't run on Unity 2021.2/2022.1 with HDRP due to the
  [Searcher package issue]. Wait for the fix to the Searcher package.

[Searcher package issue]:
  https://forum.unity.com/threads/2021-2-0b6-and-system-memory-readonlyspan-under-net-4-8.1152104/

- Dimensions of frame images should be multiples of 16x8. This limitation causes
  glitches on several mobile devices when using the Game View capture method.

- The camera capture method with HDRP doesn't work on release builds. This is
  a known issue with HDRP at the moment.

- KlakNDI doesn't support audio streaming. There are several technical
  difficulties to implement without perceptible noise or delay, so there is no
  plan to implement it.

How To Install
--------------

This package uses the [scoped registry] feature to resolve package
dependencies. Add the following lines to the manifest file
(`Packages/manifest.json`).

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

<details>
<summary>.NET Standard 2.0 (Unity 2021.1 or earlier)</summary>

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
"org.nuget.system.memory": "4.5.3",
"jp.keijiro.klak.ndi": "2.0.0"
```

After the changes, the manifest file should look like:

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
    "org.nuget.system.memory": "4.5.3",
    "jp.keijiro.klak.ndi": "2.0.0",
    ...
```
</details>

<details>
<summary>.NET Standard 2.1 (Unity 2021.2 or later)</summary>

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
"jp.keijiro.klak.ndi": "2.0.0"
```

After the changes, the manifest file should look like:

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
    "jp.keijiro.klak.ndi": "2.0.0",
    ...
```
</details>

NDI Sender Component
--------------------

![send](https://user-images.githubusercontent.com/343936/134309035-aa5be91f-098b-4352-a49f-0c2d4f49f5b0.png)

The **NDI Sender** component (`NdiSender`) sends a video stream from a given
video source.

**NDI Name** - Specify the name of the NDI endpoint (only available in the
Camera/Texture capture method).

**Keep Alpha** - Enable this checkbox to make the stream contain the alpha
channel. You can disable it to reduce the bandwidth.

**Capture Method** - Specify how to capture the video source from the following
options:

  - Game View - The sender captures frames from the Game View.
  - Camera - The sender captures frames from a given camera. This method only
    supports URP and HDRP.
  - Texture - The sender captures frames from a texture asset. You can also use
    a render texture with this option.

You can attach metadata using the C# `.metadata` property.

NDI Receiver Component
----------------------

![recv](https://user-images.githubusercontent.com/343936/134309054-8c25ed46-263c-4041-b331-aefc3e0e6107.png)

The **NDI Receiver** component (`NdiReceiver`) receives a video stream and
feeds it to a renderer object or a render texture asset.

**NDI Name** - Specify the name of the NDI source. You can edit the text field
or use the selector to choose a name from currently available NDI sources.

**Target Texture** - The receiver copies the received frames into this render
texture asset.

**Target Renderer** - The receiver overrides a texture property of the given
renderer.

You can extract metadata using the C# `.metadata` property.

Tips for Scripting
------------------

You can enumerate currently available NDI sources using the NDI Finder class
(`NdiFinder`). See the [Source Selector] example for usage.

[Source Selector]: URP/Assets/Script/SourceSelector.cs

You can instantiate the NDI Sender/Receiver component from a script but at
the same time, you have to set an NDI Resources asset (`NdiResources.asset`).
See the [Sender Benchmark]/[Receiver Benchmark] examples for details.

[Sender Benchmark]: URP/Assets/Script/SenderBenchmark.cs
[Receiver Benchmark]: URP/Assets/Script/ReceiverBenchmark.cs
