# C2M 2.0 beta & Blender 2.8+ plugin 

**NOT RELEASED YET!!**

### Supported games
- BO2

### Blender plugin features
- Models
- Materials
- Lights
- Decals

### Installing the Blender plugin
1. Extract **io_c2m_import.zip**
2. In Blender, go to **Edit > Preferences**
3. Click the Install... button and select the plugin ZIP file

### Usage
1. Load any map (preferrably via private match)
2. Click the circle arrow button to load BSP
3. After the map data appears on the right side of C2M's window, hit the box button to extract map data and wait untill map & textures are exported (might consume a lot of memory and time, be patient).
4. In Blender, click **File > Import > CoDMap (.c2m)** and import our map.

#### Dynamic models
For whatever reason, the dynamic model names displayed in map ents aren't always the actual names of the models, so dynamic models that weren't found will be spawned as arrows in Blender instead.

To deal with that, follow these steps (I'll use one of mp_raid's vehicles as example):
1. In Blender, find and click on any arrow that represents a missing model ![](/images/modelarrow.png)

2. Copy the name of that model (everything before the double colon)

![](/images/modelname.png)


3. Blender will add the missing models to `dynamic_models.cfg` in C2M's main folder
4. Using Greyhound/Wraith, find the models you want to add, and get their actual name (I found out that Blender model `veh_t6_civ_sportscar_static_silver` is actually named `veh_t6_civ_sportscar_whole_silver`)
5. Once you got the name of the model, go back to `dynamic_models.cfg` and replace `xmodelName` with the actual name. **You can delete any model you don't wish to add**
6. Re-export the map, and the models should be there now.

### Known issues
- Decals use the same UV set as the base material below them so some might look weird
- SpotLight direction might be wrong

Feel free to report bugs via [Twitter](https://twitter.com/SHEILANff) or [AGR Discord (#C2M channel)](https://discord.gg/JcEvDBH)


If you wish to support the development of this tool, feel free to donate via [PayPal](https://paypal.me/ksheilan).
