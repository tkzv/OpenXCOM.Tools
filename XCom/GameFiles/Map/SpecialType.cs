namespace XCom
{
	// TODO: give these generic names (eg, 'Type00'..'Type14') and use the
	// Option-tip via Settings to describe the special properties, ala UFO or
	// TFTD.
	// Note that the Help screen already uses a radio-button to switch the
	// descriptions between the two.
	//
	// WARNING: Do not assign the same value to multiple enum-keys. The Help
	// screen uses Enum.GetName() to evaluate special-property colors.
	public enum SpecialType
	{
		Standard = 0,		// 0	// default tile value
		EntryPoint,			// 1	// EntryTile
		PowerSource,		// 2	// UfoPowerSource
		Navigation,			// 3	// UfoNavigation
		Construction,		// 4	// UfoConstruction
		Food,				// 5	// AlienFood
		Reproduction,		// 6	// AlienReproduction
		Entertainment,		// 7	// AlienEntertainment
		Surgery,			// 8	// AlienSurgery
		ExaminationRoom,	// 9	// AlienExaminationRoom
		Alloys,				// 10	// AlienAlloys
		Habitat,			// 11	// AlienHabitat
		DeadTile,			// 12	// DeadTile
		ExitPoint,			// 13	// ExitTile
		MustDestroy			// 14	// AlienBrain
	};
}

// http://ufopaedia.org/index.php/MCD
//
//  UFO                        TFTD                         TFTD (per MapView)
//  0: No Special Properties   0: No Special Properties     0: No Special Properties
//  1: Entry Point             1: Entry Point               1: Entry Point
//  2: UFO Power Source        2: Ion-Beam Accelerators     2: Ion-Beam Accelerators
//  3: UFO Navigation          3: Magnetic Navigation       3: "Destroy Objective"
//  4: UFO Construction        4: Alien Sub Construction    4: Magnetic Navigation
//  5: Alien Food              5: Alien Cryogenics          5: Alien Cryogenics
//  6: Alien Reproduction      6: Alien Cloning             6: Alien Cloning
//  7: Alien Entertainment     7: Alien Learning Arrays     7: Alien Learning Arrays
//  8: Alien Surgery           8: Alien Implanter           8: Alien Implanter
//  9: Examination Room        9: Examination Room          9: "Unknown9"
// 10: Alien Alloys           10: Aqua Plastics            10: Aqua Plastics
// 11: Alien Habitat          11: Alien Re-animation Zone  11: Examination Room
// 12: Dead Tile              12: Dead Tile                12: Dead Tile
// 13: Exit Point             13: Exit Point               13: Exit Point
// 14: Alien Brain            14: T'leth Power Cylinders   14: T'leth Power Cylinders
//
// Notes:
//
// The images in the TFTD in-game UFOpedia do NOT match the tile-types you'll
// recover from the battlescape. For example, the Dreadnought has tiles which
// match the UFOpedia entries for the Magnetic Navigation systems - but they're
// really Alien Sub Construction units, or so your Alien Sub recovery team will
// tell you! Daishiva's MapView presents tiles according to the UFOpedia, hence
// the additional column.
//
// The "entry points" are where your troopers spawn at the start of the map. If
// you wish to safely abort, your solders must return to those tiles first.
//
// The "exit points" are where soldiers must be if you wish to proceed to the
// next stage of a two-part mission without killing all aliens (by aborting
// instead).
//
// The Brain and Power Cylinder tiles must be destroyed in the final mission, in
// order to win UFO:EU and TFTD respectively.
//
// In UFO:EU, Alien Reproduction and Alien Habitat tiles never appear. Alien
// Reproduction tiles do have a UFOpedia entry that can be accessed by hacking
// those units into your stores, but Alien Habitat tiles do not.
//
// In TFTD, Examination Room tiles never appear. They do have a UFOpedia entry
// that can be accessed by hacking those units into your stores, however. Alien
// Re-animation Zone tiles can be found naturally, but have no research entry.
