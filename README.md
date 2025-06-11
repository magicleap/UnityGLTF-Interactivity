# GLTF Interactivity

Package for supporting the Interactivity extension for GLTFs. Built on top of UnityGLTF.

Focused on playing back interactive GLTFs at runtime. Future work may include runtime authoring/export to allow for editors to be made.

> [!WARNING]
> **This project has been converted into a package and is in the process of being merged into [UnityGLTF](https://github.com/KhronosGroup/UnityGLTF/pull/849). All further development will take place in the UnityGLTF repo, this repo is now a read-only snapshot from before then.**

## Supported Nodes

Currently this project supports all the interactivity nodes from the [KHR_interactivity spec](https://github.com/KhronosGroup/glTF/blob/interactivity/extensions/2.0/Khronos/KHR_interactivity/Specification.adoc) along with extensions [KHR_node_hoverability](https://github.com/KhronosGroup/glTF/blob/355fdb80fe4601eefa687e3380b615a524d4e00a/extensions/2.0/Khronos/KHR_node_hoverability/README.md) and [KHR_node_selectability](https://github.com/KhronosGroup/glTF/blob/d5e03ce9707670b3f6f2b1f26f6dd8e6b43ff096/extensions/2.0/Khronos/KHR_node_selectability/README.md).

## Supported Pointers

Support for all GLTF [Core Pointers](github.com/KhronosGroup/glTF/blob/main/specification/2.0/ObjectModel.adoc#core-pointers), [Interactivity Pointers](github.com/KhronosGroup/glTF/blob/main/specification/2.0/ObjectModel.adoc#core-pointers), and most extension pointers for materials have been added.

## Test Generation

All unit tests in this engine are portable to other implementations. By running this project's tests from the Unity test runner you can generate json files that represent each test. These tests can be consumed by any engine by running them and listening for Completed and Failed events. If a Completed event occurs with no Failed events you may assume the test has passed and your implementation is correct.

## Testing GLBs
To test out an interactive GLB in this repo, place it in the *StreamingAssets* folder and change the **Model Name** you would like to load in the **Test** GameObject.