# Unity Project: HTC Vive Tracker Without Headset

This Unity project enables you to use HTC Vive Trackers without an HMD (headset), by leveraging the SteamVR Null Driver.
It allows you to assign trackers directly to objects in Unity, manage their configuration, and smooth their motion for stable tracking.

## Prerequisites

- OS: Windows 10/11 (Linux supported with SteamVR)
- Unity: 2022.3 or newer
- Steam: Download [here](https://store.steampowered.com/about/)
- Steam VR: Install [here](https://store.steampowered.com/app/250820/SteamVR/)
- Unity SteamVR Plugin: Download [here](https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647) and follow [here](https://valvesoftware.github.io/steamvr_unity_plugin/articles/Quickstart.html)
- Unity SteamVR-Utils package: Download [here](https://github.com/stex2005/SteamVR-Utils) if not available through git submodules.
- Hardware: 
	- 2+ HTC Vive Base Station
	- 2+ HTC Vive VR Tracker
- Null Driver for SteamVR (to bypass the headset requirement)

## Features

- Direct tracker-to-object binding: Assign HTC Vive trackers to Unity GameObjects in your scene.

- Tracker identification in Unity: Manage trackers by name and serial number directly in the Inspector.

- Relative pose tracking: Compute and apply tracker poses relative to a configurable reference tracker.

- Motion smoothing: Use exponential smoothing to reduce jitter in both position and rotation.

- Headset-free VR tracking: Run SteamVR with no HMD attached by using the Null Driver.

## Additional Features

- Tracker management: Assign logical roles (e.g., “Wrist”, “Hand” or “Reference”) to trackers and swap them dynamically.
- Visualization: Debug view to show live tracker positions, orientations, and connection status in the Scene view.
- Multi-tracker support: Supports multiple trackers active simultaneously.

## Usage Guide

### TrackerConfigurationLoader

The `TrackerConfigurationLoader` ensures trackers are consistently assigned based on their serial IDs rather than connection order.

**Setup:**
1. Add the `TrackerConfigurationLoader` component to any GameObject that should be tracked by a VIVE tracker
2. Set the `Configured Name` field to match a name from `tracker_config.txt` (e.g., "VR-17", "VR-19", etc.)
3. When the scene starts, the tracker will automatically assign based on its ID from the config file

**Configuration File Format:**
The `StreamingAssets/tracker_config.txt` file uses the format:
```
TrackerName;TrackerSerialID
VR-17;LHR-F806079D
VR-19;LHR-481B26F5
```

### ViveTrackersUI

Interactive UI system for managing tracker configurations in real-time.

**Setup:**
1. Drag the `ViveTrackersUI` prefab into your scene
2. Unity will prompt to import TMP Essentials - click "Import" (you don't need examples & extras)
3. If your scene doesn't have an Event System:
   - Right-click in Hierarchy
   - Go to `UI > Event System`
   - Add the Event System (required for UI button functionality)

**Usage:**
1. Start the scene - the UI will automatically detect trackers with `TrackerConfigurationLoader` components
2. **Set New Tracker IDs:** Change the text field next to any tracker name and click "Submit" to assign a new tracker ID
3. **Save Configuration:** Submitting changes automatically saves the configuration to a CSV file in StreamingAssets
4. **Load from File:** Click "Load from File" to restore previously saved configurations
5. **Get from Scene:** Click "Get from Scene" to refresh the UI with current TrackerConfigurationLoader settings

## Installation of Null Driver

To run without an HMD, enable the Null Driver in SteamVR. You will need to edit two files, the `default.vrsettings` of the null driver, and the `default.vrsettings` of SteamVR.

### Locate Config Files

Both files are contained in your steam directory. Where that directory is depends on your operating system.

- Windows: `C:\Program Files (x86)\Steam\` (unless Steam is installed elsewhere)

- Linux: If installed from the homepage, at least on Ubuntu, it should be in `~/.local/share/Steam`, while the official package manger places it in `~/.steam`.

### Null Driver Config

The null driver file can be found in `*Steam Directory*/steamapps/common/SteamVR/drivers/null/resources/settings/default.vrsettings`.

Open the null driver file and change `"enable": false,` to `"enable": true,`.

### SteamVR Config 

The second file, the SteamVR config file, can be found in `*Steam Directory*/steamapps/common/SteamVR/resources/settings/default.vrsettings`

Change:
- `"requireHmd": true,` to `"requireHmd": false,` 
- `"forcedDriver": "",` to `"forcedDriver": "null",` 
- `"activateMultipleDrivers": false,` to `"activateMultipleDrivers": true,`

The new config should be:
```
"requireHmd": false,
"forcedDriver": "null",
"activateMultipleDrivers": true,
```
Notice that, like it says at the top of the file, this file will be replaced when SteamVR updates. If you want, you can place the settings in your `steamvr.vrsettings` file located somewhere in the Steam directory. Make sure you place them under the `steamvr` header.

### Troubleshooting

- SteamVR updates: Make sure you modify `/resources/settings/default.vrsettings` every time you update.
Re-apply your Null Driver settings after updates, unless you’ve copied them to ``steamvr.vrsettings``.
- Tracker not detected: Confirm base stations are powered and visible, and that SteamVR is running with Null Driver.
- Jittery motion: Adjust the smoothing parameters in the Unity Inspector (positionSmoothSpeed, rotationSmoothSpeed).
