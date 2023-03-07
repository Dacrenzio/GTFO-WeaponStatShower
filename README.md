# **Weapon Stat Shower**

WeaponStatShower is a mod that allows player to see the stats of a selected weapon and which enemy they can kill with 1 shot and where it does kill. This mod is compatible with WeaponCustomizer and modded weapons (as far as I know).

## Mod Summary

This mod generates the description of the weapons from scratch so even if you have a custom weapon it will be automatically update the UI. The mods calculate the damage of the weapon and show which Sleeper can be killed in 1 shot and from what position.
You can enable which enemy to show from the configuration file located in:

- BepInEx/config/weaponStatShower.txt

and just follow the instructions.

## **Current stats shown:**

![example](https://i.ibb.co/kcjKq8P/Weapon-Show-Stat-GTFO.png)

    - FireMode of the weapon
    - Charge_Up time (show in brakets)
    - Recoil class(not dynamic)
    - Spread Cone for Shotguns

    - DMG: the base Damage of the weapon
    - CLP: the magazine size
    - MAX: the max number of ammo (mag included)
    - PRCSN: the precision multiplier (if invisible PRCSN = 1)
    - DIST: the distance from which the weapon starts dealing less damage
    - RLD: reload time in seconds
    - STRG: stagger damage multiplier
    - PIERC: the number of enemies the bullet can pierce
    - PELTS: the number of pellet fired by a shotgun

    - Enemies selected (DEFAULT: striker, shooter, scout)
        * Each enemies can have 4 values [C,B,H,O]:
            - C: chest kill
            - B: back kill
            - H: head kill (from front)
            - O: occiput kill

        * The math is done considering always the max damage possible (so no distance applied and all pellets hit) and all enemies multiplier and armor when applied.
        * Burst or Auto weapons calculate the damage for just 1 hit.

## **Installation**

### EASY

1. download [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager)
2. search for Weapon Stat Shower and install it
3. launch modded, done!

### MANUAL

1. dowanload [BepInExPack GTFO](https://gtfo.thunderstore.io/package/BepInEx/BepInExPack_GTFO/) and extract it in the GTFO base folder
2. copy and paste the extracted mod zip file inside {GTFO_FOLDER}/BepInEx/plugins
3. done!

# THANKS

This is my first mod so huge thanks to [GTFO_TheArchive](https://github.com/AuriRex/GTFO_TheArchive) which this mod is based on a specific feature that I wanted as a standalone, then modified.
