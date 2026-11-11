namespace BinarySerializer.Ray1.GBA
{
    public enum GBA_DefinedPointer
    {
        LevelMaps,
        BackgroundVignette,
        IntroVignette,
        SpritePalettes,
        
        EventGraphicsPointers,
        EventDataPointers,
        EventGraphicsGroupCountTablePointers,
        LevelEventGraphicsGroupCounts,
        
        WorldLevelOffsetTable,

        WorldInfo,
        WorldMapVignetteImageData,
        WorldMapVignetteBlockIndices,
        WorldMapVignettePaletteIndices,
        WorldMapVignettePalettes,

        StringPointers,

        TypeZDC,
        ZdcData,
        EventFlags,

        WorldVignetteIndices,

        DES_Ray,
        DES_RayLittle,
        DES_Clock,
        DES_Div,
        DES_Map,
        DES_Alpha,
        DES_Alpha2,

        ETA_Ray,
        ETA_Clock,
        ETA_Div,
        ETA_Map,
        
        // Graphics not referenced from events
        DES_DrumWalkerGraphics,
        DES_InkGraphics,
        DES_PinsGraphics,

        ExtFontImgBuffers,
        MultiplayerImgBuffers,

        MusyxFile
    }
}