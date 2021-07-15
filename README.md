# 🪑 Please Remain Seated

![](https://img.shields.io/badge/Unity-2020.3-red.svg) ![](https://img.shields.io/badge/AR_Foundation-4.1.7-green.svg) ![](https://img.shields.io/badge/Universal_Render_Pipeline-10.x-blue.svg)

Third-party Unity [XR plugin](https://docs.unity3d.com/Manual/XRPluginArchitecture.html) that simulates AR features in the Unity editor and standalone builds. Developed at [In The Pocket](https://inthepocket.com) in beautiful Ghent, Belgium.

## 🏔 Overview

(insert video here)

The goal of this plugin is to reduce the need for on-device deployment and testing by simulating the features and behaviour of mobile AR frameworks like ARKit and ARCore inside the Unity editor. It also dramatically reduces the need for AR developers to get up from their chairs during development, hence the name.

Currently the plugin does not put in a lot of effort to be compatible with different versions of Unity or AR Foundation, and it only supports the Universal Render Pipeline. This version has been tested and validated to work properly with:

- Unity 2020.3
- AR Foundation 4.1.7
- Universal Render Pipeline 10.x

## 👷🏻‍♀️ Features

These AR Foundation features are currently supported:

- `ARSession` management
- Passthrough rendering with `ARBackgroundRenderer`

These are planned:

- Horizontal and vertical `ARPlane` detection
- `ARAnchor` creation on planes or in arbitrary positions
- Raycasting against planes

## 👋 Alternatives

If you were looking for a way to simulate AR features in the Unity editor but find that this package doesn't match your needs, consider these alternatives:

- [AR Simulation](https://github.com/needle-tools/ar-simulation): third-party commercial plugin that has uses a similar approach to Please Remain Seated, but with better compatibility and support.
- [Unity MARS](https://unity.com/products/unity-mars): first-party Unity framework and toolset that includes advanced simulation & remoting tools.
