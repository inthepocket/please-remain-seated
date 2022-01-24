# 🪑 Please Remain Seated

![](https://img.shields.io/badge/UPM-com.inthepocket.pleaseremainseated-purple.svg) ![](https://img.shields.io/badge/Version-1.0.0-orange.svg) ![](https://img.shields.io/badge/Unity-2020.3-blue.svg)

Third-party Unity [XR plugin](https://docs.unity3d.com/Manual/XRPluginArchitecture.html) that simulates AR features in the Unity editor and standalone builds.

## 🏔 Overview

The goal of this plugin is to reduce the need for on-device deployment and testing by simulating the features and behaviour of mobile AR frameworks like ARKit and ARCore inside the Unity editor. It also dramatically reduces the need for AR developers to get up from their chairs during development, hence the name.

These AR Foundation features are currently supported:

- `ARSession` management
- Passthrough rendering with a custom `ARBackgroundRenderer`
- Horizontal and vertical `ARPlane` detection based on scene `BoxCollider`s
- Raycasting against `ARPlane`s
- `ARAnchor` creation on planes or at arbitrary positions

This version has been tested and validated to work properly with:

- Unity 2020.3 LTS
- AR Foundation 4.1.7
- Universal Render Pipeline 10.7

## 🧑🏻‍💻 Usage

Add this library to your Unity project's `Packages/manifest.json`:

```json
{
  "com.inthepocket.pleaseremainseated": "ssh://git@github.com:inthepocket/please-remain-seated.git",
}
```