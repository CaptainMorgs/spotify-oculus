using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Converter
{

    public static Sprite ConvertWWWToSprite(WWW www)
    {
        //TODO look at what texture format is best to use
        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        //          Debug.Log("Texture width " + www.texture.width + ", texture height "  + www.texture.height);
        www.LoadImageIntoTexture(texture);

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 1);

        return spriteToUse;
    }

    public static Texture2D ConvertWWWToTexture(WWW www)
    {
        //TODO look at what texture format is best to use
        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGBA32, false);
        www.LoadImageIntoTexture(texture);
        // Debug.Log("Texture width " + www.texture.width + ", texture height " + www.texture.height);    

        return texture;
    }

    public static Sprite ConvertTextureToSprite(Texture2D texture)
    {
        Rect rec = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 1);
    }
}
