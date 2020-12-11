### Blender plugin installation
1. Extract all files
2. In Blender, go to **Edit > Preferences**
3. Click the `Install...` button and select **C2M_IMPORTER.zip**

### Usage
1. Load any map (preferrably via private match)
2. Click the circle arrow button to load the map and wait till map data appears
3. Click the box button to extract map data and wait untill everything's exported (might take some time).
4. In Blender, click **File > Import > CoDMap (.c2m)** and import our map.

### Export content
 - C2M file - currently only supported by Blender plugin
 - IWMap file - all static & dynamic models listed as entities
 - OBJ File - has all map geometry (no xmodels)

### Supported Games

|                |     Models     |    Textures    |     Lights     |     Decals     |
| -------------  | :-----------:  | :-----------:  | :-----------:  | :-----------:  |
|    **COD4**    |      Yes       |      Yes       |      Yes       |      Yes       |
|    **MW2**     |      Yes       |      Yes       |      Yes       |      Yes       |
|    **MW3**     |      Yes       |      Yes       |      Yes       |      Yes       |
|    **BO2**     |      Yes       |      Yes       |      Yes       |  Experimental  |
|    **BO3**     |      Yes       |      Yes       |      No        |      Yes       |
|    **BO4**     |      Yes       |      Yes       |      No        |      Yes       |
|   **GHOSTS**   |      Yes       |      Yes       |      Yes       |      No        |
|    **AW**      |      Yes       |      Yes       |      Yes       |      No        |
|    **MWR**     |      Yes       |      Yes       |      Yes       |      No        |
|    **IW**      |      Yes       |      Yes       |      Yes       |      No        |
|    **WW2**     |      Yes       |      Yes       |      Yes       |      No        |

### Known issues
- BO2 Decals use the same UV set as the base material below them so some might look weird
- Light power might be wrong, but most of the data should be correct
- Some shaders aren't supported so don't expect 1:1 accuracy with the game
- I recommend disabling lights/models you don't see because they seem to be really heavy on Blender.


* This is a beta version, so bugs are expected. If you encounter any, please report at [Twitter](https://twitter.com/SHEILANff) or [AGR Discord (#C2M channel)](https://discord.gg/JcEvDBH)


If you wish to support the development of this tool, feel free to donate via [PayPal](https://paypal.me/ksheilan).


### Credits

- [**SHEILAN**](https://github.com/sheilan102) - Developer & Game Research.

- [**Scobalula**](https://github.com/Scobalula) - Game Research (Husky, Greyhound) & PhilLibX.

- [**DTZxPorter**](https://github.com/dtzxporter) - Game Research (Wraith) & ExportX (XMODEL_BIN & XMODEL_EXPORT).
