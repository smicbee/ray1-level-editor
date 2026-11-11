using System.Collections.Generic;

namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_ROMConfig
    {
        #region Static Properties and Methods
            
        /*
        World 7:
        0 = World Map
        1 = Breakout
        2 = ? (crashes)
        3 = Player Select
        4 = Game Over
        5 = Big Ray
        6 = Credits
        7 = Intro
        8 = Demo
        9 = Ubi Logo
        */

        public static JAG_ROMConfig JAG => new JAG_ROMConfig(
            numLevels: new KeyValuePair<World, int>[]
            {
                new KeyValuePair<World, int>(World.Jungle, 21),
                new KeyValuePair<World, int>(World.Mountain, 14),
                new KeyValuePair<World, int>(World.Cave, 13),
                new KeyValuePair<World, int>(World.Music, 19),
                new KeyValuePair<World, int>(World.Image, 14),
                new KeyValuePair<World, int>(World.Cake, 4)
            },
            extraMapCommands: new int[]
            {
                0, 1, 3, 4, 5, 6, 7, 9
            },
            vignette: new KeyValuePair<uint, int>[]
            {
                // Vignette
                new KeyValuePair<uint, int>(0x00800000 + 43680, 384),
                new KeyValuePair<uint, int>(0x00800000 + 127930, 160),
                new KeyValuePair<uint, int>(0x00800000 + 140541, 136),
                new KeyValuePair<uint, int>(0x00800000 + 150788, 160),
                new KeyValuePair<uint, int>(0x00800000 + 162259, 80),
                new KeyValuePair<uint, int>(0x00800000 + 169031, 320),
                new KeyValuePair<uint, int>(0x00800000 + 246393, 320),
                new KeyValuePair<uint, int>(0x00800000 + 300827, 320),
                new KeyValuePair<uint, int>(0x00800000 + 329569, 320),
                new KeyValuePair<uint, int>(0x00800000 + 351048, 320),
                new KeyValuePair<uint, int>(0x00800000 + 372555, 320),
                new KeyValuePair<uint, int>(0x00800000 + 391386, 320),
                new KeyValuePair<uint, int>(0x00800000 + 409555, 320),
                new KeyValuePair<uint, int>(0x00800000 + 423273, 320),
                new KeyValuePair<uint, int>(0x00800000 + 429878, 320),
                new KeyValuePair<uint, int>(0x00800000 + 450942, 320),

                // Background/foreground
                new KeyValuePair<uint, int>(0x00800000 + 1353130, 192),
                new KeyValuePair<uint, int>(0x00800000 + 1395878, 384),
                new KeyValuePair<uint, int>(0x00800000 + 1462294, 384),
                new KeyValuePair<uint, int>(0x00800000 + 1553686, 320),
                new KeyValuePair<uint, int>(0x00800000 + 1743668, 144),
                new KeyValuePair<uint, int>(0x00800000 + 1750880, 48),

                new KeyValuePair<uint, int>(0x00800000 + 1809526, 192),
                new KeyValuePair<uint, int>(0x00800000 + 1845684, 384),
                new KeyValuePair<uint, int>(0x00800000 + 1928746, 192),
                new KeyValuePair<uint, int>(0x00800000 + 1971368, 192),

                new KeyValuePair<uint, int>(0x00800000 + 2205640, 384),
                new KeyValuePair<uint, int>(0x00800000 + 2269442, 384),
                new KeyValuePair<uint, int>(0x00800000 + 2355852, 160),

                new KeyValuePair<uint, int>(0x00800000 + 2702140, 384),
                new KeyValuePair<uint, int>(0x00800000 + 2803818, 192),
                new KeyValuePair<uint, int>(0x00800000 + 2824590, 320),
                new KeyValuePair<uint, int>(0x00800000 + 2916108, 192),

                new KeyValuePair<uint, int>(0x00800000 + 3078442, 192),
                new KeyValuePair<uint, int>(0x00800000 + 3118496, 384),

                new KeyValuePair<uint, int>(0x00800000 + 3276778, 384),
                new KeyValuePair<uint, int>(0x00800000 + 3323878, 320),
            },
            eventCount: 0x1C4,
            additionalEventDefinitionPointers: new uint[] 
            {
                0x00BDBFDC,
                0x00B6018C,

                0x00B617EE,
                0x00B61816,
                0x00B6183E,
                0x00B61866,
                0x00B6188E,

                0x00B5DF54,
                0x00B5DF7C,

                0x00BF8B90,
                0x00BF8CA8,
                0x00BF8D20,
                0x00BF8E38,
                0x00BF8EB8,
                0x00BF8F9C,
                0x00BF9094,
                0x00BF90BC,
                0x00BF90FC,
                0x00BF9124,
                0x00BF9164,
            },
            worldInfoCount: 23
        );

        public static JAG_ROMConfig JAG_Demo => new JAG_ROMConfig(
            numLevels: new KeyValuePair<World, int>[]
            {
                new KeyValuePair<World, int>(World.Jungle, 14),
                new KeyValuePair<World, int>(World.Mountain, 4),
                new KeyValuePair<World, int>(World.Cave, 2),
                new KeyValuePair<World, int>(World.Music, 5),
                new KeyValuePair<World, int>(World.Image, 2),
                new KeyValuePair<World, int>(World.Cake, 2)
            },
            extraMapCommands: new int[]
            {
                0, 1, 2
            },
            vignette: new KeyValuePair<uint, int>[]
            {
                // World map
                new KeyValuePair<uint, int>(0x875BC0, 320), 
            
                // Breakout
                new KeyValuePair<uint, int>(0x8855F8, 320), 

                // Jungle
                new KeyValuePair<uint, int>(0x8B083E, 192),
                new KeyValuePair<uint, int>(0x8D735E, 144),
                new KeyValuePair<uint, int>(0x8D8F8A, 48), 

                // Music
                new KeyValuePair<uint, int>(0x8F64E6, 384),
            },
            eventCount: 0xB5,
            additionalEventDefinitionPointers: new uint[] 
            {
                0x0086da20,
                0x0091fcc8,

                0x0086940E,
                0x0091FD24,
                0x0091FD4C,
                0x0091FE4A,
                0x0091FE72,

                0x00872194,
                0x00872202,
                0x008722DE,
                0x00872426,
                0x0087244e,
                0x00872476,
                0x0087249e,
                0x008724c6,
                0x008724ee,
                0x00872516,

                0x008695DA,
                0x00869602,
                0x0086962A,
                0x00869652,
                0x0086967A,
                0x008696A2,
                0x008696CA,
                0x008696F2,
                0x0086971A,

                0x00873B52,
                0x00873B7A,
                0x00873CF2,
                0x00873D1A,
                0x00873e1e,
                0x00873e46,
                0x00873e6e,
                0x00873e96,
                0x00873ebe,
                0x00873ee6,
                0x00873f0e,
                0x00873f8e,
                0x00873fb6,
                0x00874134,
                0x0087415c,
                0x008741a8,

                0x0086DA20,

                0x00866598,
                0x0086869E,
                0x00868C22,

                0x0084E772,
                0x00850CEE,
                0x00851332,

                0x00857D96,

                0x0085A2D0,
                0x00860E2E,
                0x00860ED6,

                0x0085F88C,
                0x0086080C,

                0x008726FE,

                0x00862C00,
                0x00863182,

                0x0086DAE6,

                0x0086F23A,
            },
            worldInfoCount: 8
        );

        public static JAG_ROMConfig JAG_Proto => new JAG_ROMConfig(
            numLevels: null,
            extraMapCommands: null,
            vignette: null,
            eventCount: 0x1C,
            additionalEventDefinitionPointers: null,
            worldInfoCount: 0
        );

        public static JAG_ROMConfig FromEngineVersion(Ray1EngineVersion e) => e switch
        {
            Ray1EngineVersion.Jaguar => JAG,
            Ray1EngineVersion.Jaguar_Demo => JAG_Demo,
            Ray1EngineVersion.Jaguar_Proto => JAG_Proto,
            _ => null
        };

        #endregion

        #region Constructor

        public JAG_ROMConfig(KeyValuePair<World, int>[] numLevels, int[] extraMapCommands, KeyValuePair<uint, int>[] vignette, uint eventCount, uint[] additionalEventDefinitionPointers, int worldInfoCount)
        {
            NumLevels = numLevels;
            ExtraMapCommands = extraMapCommands;
            Vignette = vignette;
            EventCount = eventCount;
            AdditionalEventDefinitionPointers = additionalEventDefinitionPointers;
            WorldInfoCount = worldInfoCount;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the available levels ordered based on the global level array
        /// </summary>
        public KeyValuePair<World, int>[] NumLevels { get; }

        public int[] ExtraMapCommands { get; }

        /// <summary>
        /// Gets the vignette addresses and widths
        /// </summary>
        public KeyValuePair<uint, int>[] Vignette { get; }

        public uint EventCount { get; }

        public uint[] AdditionalEventDefinitionPointers { get; }

        public int WorldInfoCount { get; }

        #endregion
    }
}
