# 🪑 Please Remain Seated

![](https://img.shields.io/badge/UPM-com.inthepocket.pleaseremainseated-purple.svg) ![](https://img.shields.io/badge/Version-1.0.0-orange.svg) ![](https://img.shields.io/badge/Unity-2020.3-blue.svg)

Third-party Unity [XR plugin](https://docs.unity3d.com/Manual/XRPluginArchitecture.html) that simulates AR features in the Unity editor and standalone builds.

This project is the successor to [AR Simulator](https://git.inthepocket.org/team-aurora/itp-arsimulator-unity), which is now deprecated.

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
  "com.inthepocket.pleaseremainseated": "ssh://git@git.inthepocket.org/team-aurora/unity-packages/please-remain-seated.git#1.0.0",
}
```

For a usage example, see the [development project](https://git.inthepocket.org/team-aurora/unity-package-development/please-remain-seated-dev).

## 🤝 Contributing

Please submit a pull request if you have something to contribute. To be elegible for inclusion, your code must

1. solve a generic problem that is applicable to most or all Unity projects (no project-specific or highly domain-specific code);
2. be as cross-platform as possible (and at the very least compile on every platform);
3. be properly structured according to [our development standards](https://confluence.itpservices.be/display/UNITY/Development+Standards);
4. include appropriate unit and/or integration tests.
