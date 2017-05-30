MapView ii

2017 may 29

major rewrite of the original MapView by Ben Ratzlaff (and others)

MapView is a map and routes editor for XCOM UFO/TFTD. It nominally requires the original UFO/TFTD resources, such as PCK/TAB/MCD files. It can edit pre-existing MAP and RMP files, and create new ones from scratch.

A converter utility, ConfigConverter, is included that might/should be able to convert a MapEdit.dat/cfg file, which is used for MapView 1, into a MapView 2 configuration file for tilesets. MapView 2 uses YAML to save most if not all settings; MapView 1 uses a custom parser.

Not all MapEdit.dat/cfg files will convert gracefully. The design of how tilesets load, store, and access their configurations has changed substantially. Instead of loading/storing tilesets under a group and subgroup, the group and category of a tileset is now stored within the tileset configuration itself. A default resource folder is assigned upon installation, for either or both UFO and TFTD, which is the parent-path of the MAPS, ROUTES, and TERRAIN folders of your stock XCOM installation(s). However, if you prefer to use a scratch directory for editing tilesets, tilesets can be created (or manually copied) into an arbitrary folder of your choice, as long as they also have the subfolders MAPS, ROUTES, and TERRAINS.

That would then become a tileset's "basepath".

MAKE BACKUPS OF ALL YOUR STUFF. i mean it. Stuff being your MAPS and ROUTES, mostly. PckView is included but saving PCK/TAB files has been disabled. MCD files cannot be edited directly with MapView 1 or 2 (although there is a setting under Options to open an MCD file in a 3rd party app).

/enjoy


ps. Open MapView at least once before trying to run PckView (the latter needs some settings that only the former creates).