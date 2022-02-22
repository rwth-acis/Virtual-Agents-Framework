# Virtual Agents Framework

![Virtual Agents Framework](https://raw.githubusercontent.com/rwth-acis/Virtual-Agents-Framework/master/Logos/Logo_wide.png)

A Unity package for creating virtual agents.

![1.0.0](https://img.shields.io/badge/version-1.0.0-blue)

[![openupm](https://img.shields.io/npm/v/com.i5.virtualagents?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.i5.virtualagents/)

This framework provides the architecture, assets and samples for creating own virtual agents, also called non-player characters (NPCs), in Unity.

## Setup

Minimum Unity version: 2020.3.

There are different ways to add the package to a project.

### Alternative 1: OpenUPM using the Package Manager UI

The first option to install the package is to include it via Unity's package manager.
Here, we first need to add a scoped registry which tells Unity that the package is hosted on [OpenUPM](https://openupm.com/packages/com.i5.virtualagents/?subPage=readme#).
After that, we can add the package in Unity's package manager window.

1. In Unity, go to Window > Package Manager to open the package manager UI.
2. In the right corner of the opened windows, click on the small cogwheel and select "Advanced Project Settings".
3. Add a new scoped registry with the following values:

| Field | Value(s) |
| --- | --- |
| Name: | `package.openupm.com` |
| URL: | `https://package.openupm.com` |
| Scope(s): | `com.i5.toolkit.core` <br/> `com.i5.virtualagents` |

4. Next, click the Save/Apply button.
5. Close the settings window and return to the package manager window.
6. Click the small plus button at the left top of the window.
   Select ""Add package by name..." or "Add package from git URL..." depending on your Unity version.
7. Enter `com.i5.virtualagents` in the text field.
8. If the package manager asks for a version number, enter the latest one, so `1.0.0`.
9. Click the add button and wait for the download to finish.

Alternatively to steps 7 - 8, the package should also be available in the list if you switch the dropdown next to the plus button from "Packages: Unity Registry" to "Packages: My Registries".
You can then select the entry and click the "Install" button.

### Alternative 2: OpenUPM with manifest.json file

A second option is to directly edit the manifest.json file of your project.
In alternative 1, Unity's UI does this in the background but you can also just copy-paste the necessary scoped registry definitions and add the package directly in the manifest.json file.

1. Open a file explorer and navigation into your project's root folder (the folder which contains sub-directories like "Assets" or "Library").
   Then, navigate into the "Packages" folder and open the "manifest.json" file.

Integrate the following json string into the manifest.json file:

```
{
    "scopedRegistries": [
        {
            "name": "package.openupm.com",
            "url": "https://package.openupm.com",
            "scopes": [
                "com.i5.toolkit.core",
                "com.i5.virtualagents"
            ]
        }
    ],
    "dependencies": {
        "com.i5.virtualagents": "1.0.0"
    }
}
```

If your manifest.json file already contains a scopedRegistries array, make sure to add the entry to the existing one.
If your scopedRegistries array already contains an entry with the name "package.openupm.com", just add the given scopes to that list.
In that case, there is no need to create a second "package.openupm.com" entry.

The entry `"com.i5.virtualagents": "1.0.0"` can be inserted anywhere in the dependencies object with the other installed packages that are already listed there.

### Alternative 3: Git Package

You can also directly include the package as a Git repository.

First, you need to install the [i5 Toolkit for Unity](https://github.com/rwth-acis/i5-Toolkit-for-Unity) by following one of its [installation instructions in the readme file](https://github.com/rwth-acis/i5-Toolkit-for-Unity).
This step needs to happen first - otherwise there is an error message as Unity cannot resolve the dependency.

1. In Unity, go to "Window > Package Manager" to open the package manager window.
2. Click the plus button in the top left corner and select "Add package from git URL...".
3. Enter https://github.com/rwth-acis/Virtual-Agents-Framework.git#[version] into the text field where [version] is replaced with "v", followed by the release number, e.g. "v1.0.0" or upm for the latest version.
   Confirm the download by clicking on the "add" button.

If you specify "upm" to get the latest version, be aware that the package is not automatically updated and you will not be notified about updates automatically.
This command just pulls the latest version which is available at that time.
To update to the newest current version, remove the package again and re-download it.

## Documentation

Check out the [documentation pages](https://rwth-acis.github.io/Virtual-Agents-Framework/) and select the version that you are using.

You can also find practical examples of features in the package's samples.
To access these follow these steps:

1. In Unity, go to "Window > Package Manager".
2. Make sure that the dropdown menu in the top left of the package manager window is set to "Packages: In Project".
3. Click on the entry for the Virtual Agents Framework in the list.
4. On the right side, expand the samples section.
   Here, you can find a list of available examples.
5. Click the import button to download the samples.
   They are saved in your Assets folder in a folder "Samples/Virtual Agents Framework/1.0.0".

Usually, each sample contains a sample scene as the starting point.
Within the scene, there is a documentation object, incated by an information icon.
If you click on it, there are further descriptions about the scene in the inspector.

## Licensing

The package's code is distributed under the [MIT license](https://github.com/rwth-acis/Virtual-Agents-Framework/blob/master/LICENSE).

All art assets (e.g. files located in the folders "3D Models" and "Animations" in the package's root folder) are distributed under the [Creative Commons Attribution 4.0 International](http://creativecommons.org/licenses/by/4.0/) license and are attributed to Benedikt Hensen unless stated otherwise.

## Contributors

### Code & Documentation:

- Benedikt Hensen
- Danylo Bekhter
- Sebastian Meinberger

### 3D Models, Animations:
- Benedikt Hensen

## Related Projects

This framework uses the [i5 Toolkit for Unity](https://github.com/rwth-acis/i5-Toolkit-for-Unity) which provides building blocks and modules for general development with the Unity 3D engine.
If you want to use the virtual agents in mixed reality, check out our [i5 Toolkit for Mixed Reality](https://github.com/rwth-acis/i5-Toolkit-for-Mixed-Reality).
It is an extension package that builds upon the functionality of the i5 Toolkit for Unity and is optimized for mixed reality.