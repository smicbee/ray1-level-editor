# BinarySerializer.Ray1
BinarySerializer.Ray1 is an extension library to [BinarySerializer](https://github.com/RayCarrot/BinarySerializer). It supports serializing formats from all versions of Rayman 1, including the SNES prototype and the Atari Jaguar versions.

# Usage
Rayman 1 consists of multiple engine versions, set in the `Ray1Settings`. Commonly there are the multi-platform versions and the Atari Jaguar and SNES versions. Each version stores its data differently and will thus have to be read differently. Currently this has to be done manually, although loader classes to simplify this might get created in the future. Below are examples for how to serialize each of the main versions.

## SNES
The SNES version is for the SNES prototype. Serializing the `SNES_ROM` class will automatically include all supported data from the ROM, such as the maps and object (Rayman).

```cs
// Create a context. The base path passed in is the absolute base path for all the files which are to be serialized.
using Context context = new Context(basePath);

// Add the settings specifying the game version.
context.AddSettings(new Ray1Settings(Ray1EngineVersion.SNES));

// Add the ROM file to serialize using its relative path.
context.AddFile(new LinearFile(context, relativeFilePath)
{
    // Don't recreate file since not everything gets read.
    RecreateOnWrite = false,
});

// Use FileFactory for reading/writing.
SNES_ROM rom = FileFactory.Read<SNES_ROM>(context, relativeFilePath);

// Modify the data as a normal C# object. In this example the animation speed of Rayman's first state is changed.
rom.Rayman.States[0].AnimSpeed = 2;

// Use FileFactory to write the file, saving the modifications.
FileFactory.Write<SNES_ROM>(context, relativeFilePath);
```

## Jaguar
The Jaguar versions include the two prototypes as well as the final version. This engine version is very different from the other versions of Rayman 1, using mostly its own classes for the data structs. Some of the data doesn't currently get serialized correctly, mostly due to certain array lengths being unknown and the multi-sprite parameters and relevant structs being different for each verbe which hasn't been fully implemented.

```cs
// Create a context. The base path passed in is the absolute base path for all the files which are to be serialized.
using Context context = new Context(basePath);

// Add the settings specifying the game version, world and level. Only the data for the specified world and level
// will be serialized from the ROM as it's set up now.
context.AddSettings(new Ray1Settings(Ray1EngineVersion.Jaguar, World.Jungle, 1));

// Add pointers for the version being serialized.
context.AddPreDefinedPointers(JAG_DefinedPointers.JAG);

// Add the ROM file to serialize using its relative path. The base address will differ depending on the format of the
// ROM, but usually it will be 0x00800000. The endianness has to be set as big endian for this game.
context.AddFile(new MemoryMappedFile(context, relativeFilePath, 0x00800000, endianness: Endian.Big)
{
    // Don't recreate file since not everything gets read.
    RecreateOnWrite = false,
});

// Use FileFactory for reading/writing.
JAG_ROM rom = FileFactory.Read<JAG_ROM>(context, relativeFilePath);

// Data can be accessed as normal C# objects.
MapData map = rom.MapData;
JAG_EventBlock events = rom.EventData;

// Use FileFactory to write the file, saving any modifications.
FileFactory.Write<JAG_ROM>(context, relativeFilePath);
```

## PS1
The PS1 versions might be the most complicated to serialize due the data being loaded into memory at specific addresses and data accessed between files. Because of this it is recommended to first read the file table in the EXE and then read the files in the order of fix, world and finally level.

```cs
// Create a context. The base path passed in is the absolute base path for all the files which are to be serialized.
using Context context = new Context(basePath);

// Add the settings specifying the game version and optionally the world and level.
context.AddSettings(new Ray1Settings(Ray1EngineVersion.PS1, World.Jungle, 1));

// Add pointers for the version being serialized.
context.AddPreDefinedPointers(PS1_DefinedPointers.PS1_US);

// Add the exe file using its relative path. The base address will differ depending on the regional version. It also
// has to account for the header size (0x800).
context.AddFile(new MemoryMappedFile(context, relativeFilePath, 0x80125000 - 0x800)
{
    // Don't recreate file since not everything gets read.
    RecreateOnWrite = false,
});

// Use FileFactory for reading/writing. When serializing the EXE the config has to be specified.
PS1_ExecutableConfig exeConfig = PS1_ExecutableConfig.PS1_US;
PS1_Executable exe = FileFactory.Read<PS1_Executable>(context, ExeFilePath,
    onPreSerialize: (_, exe) => exe.Pre_PS1_Config = exeConfig);

// Get world and level indexes for the file tables.
int worldIndex = (int)context.GetRequiredSettings<Ray1Settings>().World - 1;
int lvlIndex = context.GetRequiredSettings<Ray1Settings>().Level - 1;

// Get file entries.
PS1_FileTableEntry fileEntryFix = exe.PS1_FileTable[exe.GetFileTypeIndex(exeConfig, PS1_FileType.filefxs)];
PS1_FileTableEntry fileEntryworld = exe.PS1_FileTable[exe.GetFileTypeIndex(exeConfig, PS1_FileType.wld_file) + worldIndex];
PS1_FileTableEntry fileEntrylevel = exe.PS1_FileTable[exe.GetFileTypeIndex(exeConfig, PS1_FileType.map_file) + (worldIndex * 21 + lvlIndex)];

// Helper method for loading files.
void LoadFile(PS1_FileTableEntry file)
{
    context.AddFile(new MemoryMappedPS1File(
        context: context, 
        filePath: file.ProcessedFilePath, 
        baseAddress: file.MemoryAddress, 
        currentInvalidPointerMode: MemoryMappedPS1File.InvalidPointerMode.DevPointerXOR, 
        fileLength: file.File.Size));
}

// Read fixed file pack.
LoadFile(fileEntryFix);
PS1_AllfixPack fix = FileFactory.Read<PS1_AllfixPack>(context, fileEntryFix.ProcessedFilePath);

// Read world file pack.
LoadFile(fileEntryworld);
PS1_WorldPack wld = FileFactory.Read<PS1_WorldPack>(context, fileEntryworld.ProcessedFilePath);

// Read level pack.
LoadFile(fileEntrylevel);
PS1_LevelPack levFile = FileFactory.Read<PS1_LevelPack>(context, fileEntrylevel.ProcessedFilePath);

// Modify the data as a normal C# object. In this example the RAY_POS object has its x position changed.
levFile.LevelData.Objects.First(x => x.Type == ObjType.TYPE_RAY_POS).XPosition = 500;

// Use FileFactory to write the file, saving any modifications.
FileFactory.Write<PS1_LevelPack>(context, fileEntrylevel.ProcessedFilePath);
```

## PC
The PC versions are the most up to date versions of the engine and store their data in the most raw format making it easier to use.

```cs
// Create a context. The base path passed in is the absolute base path for all the files which are to be serialized.
using Context context = new Context(basePath);

// Add the settings specifying the game version and optionally the world and level.
context.AddSettings(new Ray1Settings(Ray1EngineVersion.PC, World.Jungle, 1, pcVersion: Ray1PCVersion.PC_1_21));

// Add the files to serialize using their relative paths.
context.AddFile(new LinearFile(context, relativeFilePath));

// Use FileFactory for reading/writing
PC_LevFile levelData = FileFactory.Read<PC_LevFile>(context, relativeFilePath);

// Modify the data as a normal C# object. In this example the RAY_POS object has its x position changed.
levelData.ObjData.Objects.First(x => x.Type == ObjType.TYPE_RAY_POS).XPosition = 500;

// Use FileFactory to write the file, saving the modifications.
FileFactory.Write<PC_LevFile>(context, relativeFilePath);
```
