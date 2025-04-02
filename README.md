# GLTF Interactivity

Package for supporting the Interactivity extension for GLTFs. Built on top of UnityGLTF.

Focused on playing back interactive GLTFs at runtime. Future work may include runtime authoring/export to allow for editors to be made.

> [!WARNING]
> **This project is a work in progress as we finish adding tests, samples, and modify the loader components to be more streamlined.**

## Supported Nodes

Currently this project supports all the interactivity nodes from the [KHR_interactivity spec](https://github.com/KhronosGroup/glTF/blob/interactivity/extensions/2.0/Khronos/KHR_interactivity/Specification.adoc) along with extensions [KHR_node_hoverability](https://github.com/KhronosGroup/glTF/blob/355fdb80fe4601eefa687e3380b615a524d4e00a/extensions/2.0/Khronos/KHR_node_hoverability/README.md) and [KHR_node_selectability](https://github.com/KhronosGroup/glTF/blob/d5e03ce9707670b3f6f2b1f26f6dd8e6b43ff096/extensions/2.0/Khronos/KHR_node_selectability/README.md).

## Supported Pointers

Support for all GLTF [Core Pointers](github.com/KhronosGroup/glTF/blob/main/specification/2.0/ObjectModel.adoc#core-pointers), [Interactivity Pointers](github.com/KhronosGroup/glTF/blob/main/specification/2.0/ObjectModel.adoc#core-pointers), and most extension pointers for materials have been added.

## Testing GLBs
To test out an interactive GLB in this repo, place it in the *StreamingAssets* folder and change the **Model Name** you would like to load in the **Test** GameObject.