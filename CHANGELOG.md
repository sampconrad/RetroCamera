`1.4.3`
- added command wheel (right alt default key), must be enabled from menu and commands set in config file commands.json (first spot for name of command you want showing in wheel, second spot for raw command string)
- can set size scaling of crosshair in menu
- mouse should show when building during action mode again

`1.3.2`
- vertical aim offset appears to now be functioning as expected
- option to hide character info panel at the top of the screen during action mode
- generally more state-aware and shouldn't require as much fiddling with options when changing modes (heavy refactor pending, still too much pasta)
- increased range for shoulder offset values
- crash prevention (I'm sure if you're creative enough can still manage but seems pretty stable now :p) for server pause in singleplayer (if you choose to go in the menu while the server is paused and enable RetroCamera after the mod has disabled itself for safety it will probably crash, so don't)
- localizationKeys set in dictionary every time menu is opened instead of just once

`1.2.1`
- mouse unlocks when exiting first person or action mode without further user input
- added check to prevent memory access issues when escape menu is open

`1.1.0`
- Default mouse wheel buttons (emotes, shapeshifts) will temporarily override mouse lock in first person/action mode while pressed
- Added keybind for completing journal quests (minus default)

`1.0.0`
- Initial release
