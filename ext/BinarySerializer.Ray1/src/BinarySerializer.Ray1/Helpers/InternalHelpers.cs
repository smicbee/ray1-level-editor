using BinarySerializer.PlayStation.PS1;
using BinarySerializer.Ray1;

internal static class InternalHelpers
{
    public static int GetImageBufferLength(Sprite[] sprites, Ray1Settings settings)
    {
        if (sprites == null)
            return 0;

        int length = 0;

        foreach (Sprite spr in sprites)
        {
            bool is4bit;

            if (settings.EngineBranch == Ray1EngineBranch.PS1)
            {
                if (settings.EngineVersion == Ray1EngineVersion.PS1_JP ||
                    settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3 ||
                    settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6 ||
                    settings.EngineVersion == Ray1EngineVersion.Saturn)
                {
                    is4bit = spr.Depth == SpriteDepth.BPP_4;
                }
                else
                {
                    is4bit = spr.TexturePage.TP == TSB.TexturePageTP.CLUT_4Bit;
                }
            }
            else
            {
                is4bit = false;
            }

            if (spr.IsDummySprite())
                continue;

            int curLength = spr.ImageBufferOffset;

            if (is4bit)
                curLength += (spr.Width / 2) * spr.Height;
            else
                curLength += spr.Width * spr.Height;

            if (curLength > length)
                length = curLength;
        }

        return length;
    }
}