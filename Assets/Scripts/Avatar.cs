using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Avatar : MonoBehaviour
{
    public string uploadURL = "https://quiz.asra-studios.com/Avatar";
    public string imageURL;

    public void UploadAvatar(Sprite avatar)
    {
        StartCoroutine(UploadSpriteCoroutine(avatar));
    }

    private IEnumerator UploadSpriteCoroutine(Sprite sprite)
    {
        // Convert the sprite to a Texture2D
        Texture2D texture = SpriteToTexture2D(sprite);

        // Encode the texture to PNG
        byte[] imageData = texture.EncodeToPNG();
        Destroy(texture); // Clean up the texture

        // Create a UnityWebRequest
        UnityWebRequest www = new UnityWebRequest(uploadURL, UnityWebRequest.kHttpVerbPOST);
        www.uploadHandler = new UploadHandlerRaw(imageData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "image/png");

        // Send the request and wait for a response
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Image upload failed: " + www.error);
        }
        else
        {
            Debug.Log("Image uploaded successfully!");

            // Assuming the server returns the image URL as a plain text or JSON response
            imageURL = www.downloadHandler.text;

            Debug.Log("Image URL: " + imageURL);
        }
    }

    private Texture2D SpriteToTexture2D(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            // Create a new texture with the exact size of the sprite
            Texture2D newTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newTexture.SetPixels(newColors);
            newTexture.Apply();
            return newTexture;
        }
        else
        {
            return sprite.texture;
        }
    }
}
