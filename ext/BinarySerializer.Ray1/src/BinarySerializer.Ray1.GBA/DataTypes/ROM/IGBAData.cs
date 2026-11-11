namespace BinarySerializer.Ray1.GBA
{
    /// <summary>
    /// Interface for the data of the GBA or DSi versions
    /// </summary>
    public interface IGBAData
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        GBA_LevelMapData LevelMapData { get; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        GBA_LevelEventData LevelEventData { get; }


        /// <summary>
        /// The background vignette data
        /// </summary>
        GBA_BackgroundVignette[] BackgroundVignettes { get; }

        /// <summary>
        /// The intro vignette data
        /// </summary>
        GBA_IntroVignette[] IntroVignettes { get; }

        /// <summary>
        /// The world map vignette
        /// </summary>
        GBA_WorldMapVignette WorldMapVignette { get; }


        /// <summary>
        /// The sprite palette for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        RGBA5551Color[] GetSpritePalettes(Ray1Settings settings);


        /// <summary>
        /// World level index offset table for global level array
        /// </summary>
        byte[] WorldLevelOffsetTable { get; }


        /// <summary>
        /// The strings
        /// </summary>
        string[][] Strings { get; }


        ZDCReference[] TypeZDC { get; } 

        ZDCBox[] ZdcData { get; }

        ObjTypeFlags[] EventFlags { get; }

        byte[] WorldVignetteIndices { get; }
        WorldInfo[] WorldInfos { get; }

        GBA_EventGraphicsData DES_Ray { get; }
        GBA_EventGraphicsData DES_RayLittle { get; }
        GBA_EventGraphicsData DES_Clock { get; }
        GBA_EventGraphicsData DES_Div { get; }
        GBA_EventGraphicsData DES_Map { get; }
        GBA_ETA ETA_Ray { get; }
        GBA_ETA ETA_Clock { get; }
        GBA_ETA ETA_Div { get; }
        GBA_ETA ETA_Map { get; }
    }
}