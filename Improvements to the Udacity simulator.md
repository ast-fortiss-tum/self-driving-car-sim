# Improvements to the Udacity simulator

### Unity version
_IMPORTANT_: Moved the project from Unity 5.5.1 to Unity 5.5.6f 

### Fixes 

* Made the steering during manual mode smoother (Steering.cs)
* Fixed a problem with the camera when no audio listener is not provided (CarAudio.cs)

### Additions (ICSE 2020)

* Changed the body of water in LakeTrack using a reflective asset
* Added waypoints to the LakeTrack and JungleTrack
* Added a third track (MountainTrack) [link](https://assetstore.unity.com/packages/3d/environments/roadways/mountain-race-track-53775)
* Added waypoints to the MountainTrack
* Created a script to manage the day/night cycle (DayNightCycle.cs)
* Added script to automatically detect out of bound episodes (OBEs) and collisions with scene objects (CarCollider.cs)
* Added automated restart mechanism in case of collision/OBE. The car is automatically moved to the next waypoint (WayPointUpdate.cs)
* Added night/day version of each track for training
* Updated server script to return new data (OBEs, etc) to the Python client script (CommandServer.cs)
* Modified the main UI of the car to display the lap/waypoint number, and confidence value (UISystem.cs)
* Added the menu options to select the new scenes (MenuOptions.cs)
* Added new scenes to the project (File -> Build Settings...)
