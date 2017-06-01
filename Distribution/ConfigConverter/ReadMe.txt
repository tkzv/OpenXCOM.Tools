ConfigConverter v.1
2017 jun 1


ConfigConverter is a standalone utility that converts MapEdit.dat/cfg files to a YAML file. It's not perfect/robust but it might/should output the basics of a configuration file that can be used by MapView 2.

The intention is that persons who are using MapView 1 and want to try MapView 2, but their tileset-configuration for the former is ~1000 lines and they understandably don't want to redo all that tileset-configuring, may run their MapEdit.dat/cfg through ConfigConverter to hopefully get a YAML config-file that's workable with MapView 2.

usage:
[optional: Copy your MapEdit.dat/cfg to a directory along with ConfigConverter.exe.] Run the converter, it's straightforward. (no, i don't know how to make a virus, trojan, or whatever they are) The output should be a logfile (deletable) and MapTilesets.yml - copy/move the latter to your MapView 2 "settings" folder and try running MapView 2.

The definition of MapTilesets.yml is written at the top of the file itself, in case you want to review/edit it by hand. Note: YAML is editable as plaintext, but never use a tab character in YAML.

Other than that, this folder is deletable in toto.

- kevL
