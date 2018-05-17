/*
------------------------------
  Dialogue System for Unity  
        Menu Framework
        
  Copyright © Pixel Crushers
------------------------------


See Documentation.pdf for instructions.


RELEASE HISTORY:
2017-04-30
- Menu Framework no longer needs to be child of Dialogue Manager.
- Music Manager now plays title music automatically in title scene.
- Added MusicManager.Stop() method.

2017-01-09
- Added: Graphics settings to options menu: fullscreen, resolution, quality levels.
- Added: Credits scene.
- Added: More customization options for saved game summary info.
- Fixed: Bug fixes with persistent data components and loading scenes.

2016-09-29
- Fixed: LoadingSceneTo() sequencer command wasn't saving persistent data first.
- Fixed: When using loading scenes, wasn't telling persistent data components that level was
  about to be unloaded, causing IncrementOnDestroy etc. to fire when they shouldn't.
- Added: Option to show loading scene for a minimum duration.

2016-09-16
- CONTAINS BREAKING CHANGES! Import a clean copy of this version; don't overwrite old versions.
- No longer uses Unity's Game Jam Menu Template.
- Improved cursor, focus, and open/close animation handling.
- Added quicksave and quickload.

2016-06-19
- Improved: Refinements to joystick/mouse input switching.
- Improved: Refinements to fading when restarting and loading games.

2016-06-03
- Added: Optional animation when opening/closing Pause menu.
- Added: Optional CheckInputDevice to toggle autofocus (joystick) mode based on
  whether player uses joystick or mouse.
- Changed: Reverted Quests button by popular demand; once again closes Pause menu.

2016-05-28
- Changed: Quests button now opens quest menu on top of Pause menu without closing Pause.
- Added: Restart confirmation panel.
- Added: Option to use a loading scene.
- Added: Component to save games to disk files instead of PlayerPrefs.

2016-05-14:
- Added: Subtitles toggle in Options menu.
- Changed: In Pause menu, replaced volume sliders with Options button.
- Improved: Better autofocusing.
- Fixed: If UI is in non-start scene and MenuPanel is inactive, sets startOptions.inMainMenu=false.

2016-04-29: 
- Added: HideCursor method.
- Added: Save Helper > On Return To Menu event.
- Fixed: Return To Menu sets StartOptions.inMainMenu true.

2016-03-05:
- Fixed: Misc. small bugs.

2015-10-24:
- Initial release.

*/