using BinarySerializer.PlayStation.PS1;
using System;

namespace BinarySerializer.Ray1.PS1
{
    // TODO: Rewrite this to match what the game does
    public static class PS1VramHelpers
    {
        public static VRAM PS1_JPDemoVol6_FillVRAM(RGBA5551Color[] pal4, RGBA5551Color[] pal8, RGBA5551Color[] palLettre, byte[] fixGraphics, byte[] wldGraphics, byte[] lvlGraphics)
        {
            VRAM vram = new VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.CurrentXPage = 5;

            // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
            // storing the first bit of sprites we load as 1x2
            byte[] cageSprites = new byte[128 * (256 * 2)];
            Array.Copy(fixGraphics, 0, cageSprites, 0, cageSprites.Length);
            byte[] allFixSprites = new byte[fixGraphics.Length - cageSprites.Length];
            Array.Copy(fixGraphics, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
            /*byte[] unknown = new byte[128 * 8];
            vram.AddData(unknown, 128);*/
            vram.AddData(cageSprites, 128);
            vram.AddData(allFixSprites, 256);

            vram.AddData(wldGraphics, 256);
            vram.AddData(lvlGraphics, 256);

            int paletteY = 256 - 3; // 480 - 1 page height
            vram.AddPalette(palLettre, 1, 1, 0, paletteY++);
            vram.AddPalette(pal4, 1, 1, 0, paletteY++);
            vram.AddPalette(pal8, 1, 1, 0, paletteY++);

            return vram;
        }

        public static VRAM PS1_FillVRAM(VRAMMode mode, AllfixPack allFix, WorldPack world, BigRayPack bigRay, LevelPack lev, byte[] font, bool isUSVersion)
        {
            // TODO: Support BigRay + font for US version
            // TODO: Fill background palettes

            VRAM vram = new VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.CurrentXPage = 5;

            if (mode != VRAMMode.BigRay)
            {
                // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
                // storing the first bit of sprites we load as 1x2
                byte[] cageSprites = new byte[128 * (256 * 2 - 8)];
                Array.Copy(allFix.ImageData, 0, cageSprites, 0, cageSprites.Length);
                byte[] allFixSprites = new byte[allFix.ImageData.Length - cageSprites.Length];
                Array.Copy(allFix.ImageData, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
                byte[] unknown = new byte[128 * 8];
                vram.AddData(unknown, 128);
                vram.AddData(cageSprites, 128);
                vram.AddData(allFixSprites, 256);
            }

            if (mode == VRAMMode.Level)
            {
                vram.AddData(world.ImageData, 256);
                vram.AddData(lev.ImageData, 256);
            }
            else if (mode == VRAMMode.Menu)
            {
                if (isUSVersion)
                    vram.AddDataAt(10, 1, 0, 80, font, 256);
                else
                    vram.AddDataAt(10, 0, 0, 226, font, 256);
            }
            else if (mode == VRAMMode.BigRay)
            {
                vram.AddDataAt(10, 0, 0, 0, bigRay.ImageData, 256);
            }

            // Palettes start at y = 256 + 234 (= 490), so page 1 and y=234
            int paletteY = 234;
            if (mode != VRAMMode.BigRay)
            {
                /*vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette3.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
                vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette4.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);*/
                if (mode == VRAMMode.Level)
                {
                    vram.AddPalette(world.Palette1, 12, 1, 0, paletteY++);
                    vram.AddPalette(world.Palette2, 12, 1, 0, paletteY++);
                }
                else
                {
                    vram.AddPalette(allFix.Palette3, 12, 1, 0, paletteY++);
                    vram.AddPalette(allFix.Palette4, 12, 1, 0, paletteY++);
                }
                vram.AddPalette(allFix.Palette1, 12, 1, 0, paletteY++);
                vram.AddPalette(allFix.Palette5, 12, 1, 0, paletteY++);
                vram.AddPalette(allFix.Palette6, 12, 1, 0, paletteY++);
                vram.AddPalette(allFix.Palette2, 12, 1, 0, paletteY++);

                if (mode == VRAMMode.Level)
                {
                    paletteY += 13 - world.TilePalettes.Length;

                    // Add tile palettes
                    foreach (var p in world.TilePalettes)
                        vram.AddPalette(p, 12, 1, 0, paletteY++);
                }
            }
            else
            {
                // BigRay
                vram.AddPalette(bigRay.Palette1, 12, 1, 0, paletteY++);
                vram.AddPalette(bigRay.Palette2, 12, 1, 0, paletteY++);
            }

            return vram;
        }

        public static VRAM PS1_JP_FillVRAM(VRAMMode mode, AllfixPack allFix, WorldPack world, BigRayPack bigRay, LevelPack lev, byte[] font, int tileColorsCount)
        {
            VRAM vram = new VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.CurrentXPage = 5;

            // Reserve spot for tiles in vram
            if (mode == VRAMMode.Level)
            {
                int tilesetHeight = tileColorsCount / 256;
                const int tilesetWidth = 4 * 128;
                int tilesetPage = (16 - 4); // Max pages - tileset width
                while (tilesetHeight > 0)
                {
                    int thisPageHeight = Math.Min(tilesetHeight, 2 * 256);
                    vram.ReserveBlock(tilesetPage * 128, (2 * 256) - thisPageHeight, tilesetWidth, thisPageHeight);
                    tilesetHeight -= thisPageHeight;
                    tilesetPage -= 4;
                }
            }

            if (mode != VRAMMode.BigRay)
            {
                // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
                // storing the first bit of sprites we load as 1x2
                byte[] cageSprites = new byte[(128 * 3) * (256 * 2)];
                Array.Copy(allFix.ImageData, 0, cageSprites, 0, cageSprites.Length);
                byte[] allFixSprites = new byte[allFix.ImageData.Length - cageSprites.Length];
                Array.Copy(allFix.ImageData, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
                vram.AddData(cageSprites, (128 * 3));
                vram.AddData(allFixSprites, 256);
            }

            if (mode == VRAMMode.Level)
            {
                vram.AddData(world.ImageData, 256);
                vram.AddData(lev.ImageData, 256);
            }
            else if (mode == VRAMMode.Menu)
            {
                vram.AddDataAt(10, 1, 0, 80, font, 256);
            }
            else if (mode == VRAMMode.BigRay)
            {
                vram.AddDataAt(10, 0, 0, 0, bigRay.ImageData, 256);
            }

            int paletteY = 224; // 480 - 1 page height
            if (mode != VRAMMode.BigRay)
            {
                vram.AddPalette(allFix.Palette2, 1, 1, 0, paletteY++);
                vram.AddPalette(allFix.Palette6, 1, 1, 0, paletteY++);
                vram.AddPalette(allFix.Palette5, 1, 1, 0, paletteY++);

                paletteY += 26;
                vram.AddPalette(allFix.Palette1, 1, 1, 0, paletteY++);

                if (mode == VRAMMode.Level)
                {
                    vram.AddPalette(world.Palette2, 1, 1, 0, paletteY++);
                    vram.AddPalette(world.Palette1, 1, 1, 0, paletteY++);
                }
                else
                {
                    vram.AddPalette(allFix.Palette4, 1, 1, 0, paletteY++);
                    vram.AddPalette(allFix.Palette3, 1, 1, 0, paletteY++);
                }
            }
            else
            {
                paletteY += 31;

                // BigRay
                vram.AddPalette(bigRay.Palette1, 1, 1, 0, paletteY++);
                vram.AddPalette(bigRay.Palette2, 1, 1, 0, paletteY++);
            }

            return vram;
        }

        public static VRAM PS1_R2_FillVRAM(byte[] fixGraphics, byte[] lvlGraphics, RGBA5551Color[] spritePalettes, RGBA5551Color[][] tilePalettes)
        {
            VRAM vram = new VRAM();

            // skip loading the backgrounds for now. They take up 320 (=5*64) x 256 per background
            // 2 backgrounds are stored underneath each other vertically, so this takes up 10 pages in total
            vram.CurrentXPage = 5;

            // Since skippedPagesX is uneven, and all other data takes up 2x2 pages, the game corrects this by
            // storing the first bit of sprites we load as 1x2
            byte[] cageSprites = new byte[128 * (256 * 2)];
            Array.Copy(fixGraphics, 0, cageSprites, 0, cageSprites.Length);
            byte[] allFixSprites = new byte[fixGraphics.Length - cageSprites.Length];
            Array.Copy(fixGraphics, cageSprites.Length, allFixSprites, 0, allFixSprites.Length);
            /*byte[] unknown = new byte[128 * 8];
            vram.AddData(unknown, 128);*/
            vram.AddData(cageSprites, 128);
            vram.AddData(allFixSprites, 256);

            vram.AddData(lvlGraphics, 256);

            // Palettes start at y = 256 + 234 (= 490), so page 1 and y=234
            int paletteY = 240;
            vram.AddPalette(spritePalettes, 0, 0, 0, paletteY);

            paletteY = 248;
            vram.AddPalette(tilePalettes[3], 12, 1, 0, paletteY++);
            vram.AddPalette(tilePalettes[2], 12, 1, 0, paletteY++);
            vram.AddPalette(tilePalettes[2], 12, 1, 0, paletteY++);
            vram.AddPalette(tilePalettes[2], 12, 1, 0, paletteY++);
            vram.AddPalette(tilePalettes[2], 12, 1, 0, paletteY++);
            vram.AddPalette(tilePalettes[1], 12, 1, 0, paletteY++);
            vram.AddPalette(tilePalettes[0], 12, 1, 0, paletteY++);
            vram.AddPalette(tilePalettes[0], 12, 1, 0, paletteY++);
            /*vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette3.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette4.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);*/
            /*vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, world.EventPalette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette1.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette5.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette6.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);
            vram.AddDataAt(12, 1, 0, paletteY++, allFix.Palette2.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);

            paletteY += 13 - world.TilePalettes.Length;

            foreach (var p in world.TilePalettes)
                vram.AddDataAt(12, 1, 0, paletteY++, p.SelectMany(c => BitConverter.GetBytes(c.Color1555)).ToArray(), 512);*/

            return vram;
        }

        public enum VRAMMode
        {
            Level,
            Menu,
            BigRay
        }
    }
}