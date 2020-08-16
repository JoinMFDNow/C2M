# C2M 2.0 BETA & Blender 2.8+ intergation

## [**DOWNLOAD**](https://github.com/sheilan102/C2M/raw/gh-pages/C2M_BETA.rar)

### Supported games
- BO2

### Blender plugin features
- Models
- Materials
- Lights
- Decals

### Installation
1. Extract all files
2. In Blender, go to **Edit > Preferences**
3. Click the `Install...` button and select **C2M_IMPORTER.zip**

### Usage
1. Load any map (preferrably via private match)
2. Click the circle arrow button to load the map and wait till map data appears
3. Click the box button to extract map data and wait untill everything's exported (might take some time).
4. In Blender, click **File > Import > CoDMap (.c2m)** and import our map.

### Dynamic models
For whatever reason, the dynamic model names displayed in map ents aren't always the actual names of the models, so dynamic models that weren't found will be spawned as arrows in Blender instead and added to `dynamic_models.cfg` in C2M's main folder.

To deal with that, follow these steps (I'll use one of mp_raid's vehicles as example):
1. In Blender, find and click on any arrow that represents a missing model
2. Find that model in `dynamic_models.cfg`
3. Using Greyhound/Wraith, find the models you wish to add, and get their actual name (What appeared in Blender as `veh_t6_civ_sportscar_static_silver` was found in Greyhound as `veh_t6_civ_sportscar_whole_silver`)
4. Once you got the name of the model, go back to `dynamic_models.cfg` and replace `xmodelName` with the name you found in Greyhound. **You can delete any model you don't wish to add**
5. Those models will be added the next time you export the map.

* The file already comes with a few models I added from mp_raid.

### Known issues
- Decals use the same UV set as the base material below them so some might look weird
- SpotLight power might be wrong, but most of the data should be correct
- Some shaders aren't supported yet
- I recommend disabling lights you don't see because they seem to be really heavy on Blender.


Feel free to report bugs via [Twitter](https://twitter.com/SHEILANff) or [AGR Discord (#C2M channel)](https://discord.gg/JcEvDBH)


If you wish to support the development of this tool, feel free to donate via [PayPal](https://paypal.me/ksheilan).
