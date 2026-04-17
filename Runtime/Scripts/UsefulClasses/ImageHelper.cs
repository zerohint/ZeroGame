using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Downloading and caching image
/// TODO
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// URL: Image cache
    /// </summary>
    private static readonly Dictionary<string, Sprite> imageCache = new();

    private static Dictionary<string, string> imageUrlCache = null; // Lazy-load
    private static readonly Dictionary<string, Sprite> spriteRamCache = new(); // RAM cache

    private static string CacheFilePath => Path.Combine(Application.persistentDataPath, "imageHelperCache.json");


    /// <summary>
    /// Load image at url and apply to image
    /// </summary>
    /// <param name="url"></param>
    /// <param name="image"></param>
    public static async Task<Sprite> LoadImage(string url, Image image, bool ignoreCache = false)
    {
        if (url.IsNullOrEmpty()) return null;
        var sprite = await LoadImage(url, ignoreCache);
        image.sprite = sprite;
        return sprite;
    }


    /// <summary>
    /// Get the sprite from url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="ignoreCache"></param>
    /// <returns></returns>
    public static async Task<Sprite> LoadImage(string url, bool ignoreCache = false)
    {
        if (url.IsNullOrEmpty()) return null;

        if (!ignoreCache)
        {
            // First check in RAM cache
            if (imageCache.TryGetValue(url, out var cachedSprite) && cachedSprite != null)
            {
                return cachedSprite;
            }

            // Then check in disk cache
            //string fileName = Path.GetFileName(UrlToFilename(url));
            //string localPath = Path.Combine(Application.persistentDataPath, fileName);
            //File.Exists(localPath);
            //var sprite = LoadLocalImage(localPath);
            //AddToRamCache(fileName, sprite);
        }

        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        var operation = request.SendWebRequest();
        while (!operation.isDone) await Task.Yield();

        if (request.responseCode == 404)
        {
            Debug.LogWarning($"404 Error: {request.url})");
            return null;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Request error ({request.url}): {request.error}");
            return null;
        }

        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        if (sprite != null)
        {
            imageCache[url] = sprite;
            //SaveTextureToFile(texture, localPath);
        }
        else
            Debug.LogError("Sprite generation failed.");

        return sprite;
    }



    /// <summary>
    /// Deprecated, use LoadImage(string url, Image image, bool ignoreCache = false)
    /// </summary>
    /// <param name="fileNameValue"></param>
    /// <param name="imageUrl"></param>
    /// <returns></returns>
    public static async Task<Sprite> LoadImage(string fileNameValue, string imageUrl)
    {
        EnsureCacheLoaded();

        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogError("HATA: Boş URL!");
            return null;
        }

        string fileName = Path.GetFileName(fileNameValue);

        // RAM cache kontrolü
        var ramSprite = GetFromRamCache(fileName);
        if (ramSprite != null)
            return ramSprite;

        string localPath = Path.Combine(Application.persistentDataPath, fileName);

        if (imageUrlCache.TryGetValue(fileName, out string cachedUrl) && cachedUrl == imageUrl && File.Exists(localPath))
        {
            Debug.Log("Önbellekten yükleniyor...");
            var sprite = LoadLocalImage(localPath);
            AddToRamCache(fileName, sprite);
            return sprite;

            static Sprite LoadLocalImage(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new (2, 2);
        texture.LoadImage(fileData);
        Vector4 border = new (18, 18, 18, 18);
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect,
            border
        );
    }
        }

        if (File.Exists(localPath))
        {
            Debug.Log("Eski dosya siliniyor...");
            File.Delete(localPath);
        }

        Debug.Log("İnternetten indiriliyor...");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            var operation = request.SendWebRequest();
            Debug.Log("request " + request);
            while (!operation.isDone) await Task.Yield();
            Debug.Log("operation " + operation);
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"İndirme hatası ({request.url}): {request.error}");
                return null;
            }

            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            if (sprite != null)
            {
                SaveTextureToFile(texture, localPath);
                imageUrlCache[fileName] = imageUrl;
                SaveCache();
                AddToRamCache(fileName, sprite);
                static void SaveTextureToFile(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }
            }

            return sprite;
        }
    }


    public static async Task<Sprite> LoadImageFromFirebase(string fileNameValue, string firebaseStoragePath, int selection = 99)
    {
        throw new NotImplementedException();
        // EnsureCacheLoaded();

        // if (string.IsNullOrEmpty(firebaseStoragePath))
        // {
        //     Debug.LogError("HATA: Firebase Storage yolu boş!");
        //     return null;
        // }

        // if (selection != 99)
        //     fileNameValue += selection.ToString();

        // string fileName = Path.GetFileName(fileNameValue);

        // // RAM cache kontrolü
        // var ramSprite = GetFromRamCache(fileName);
        // if (ramSprite != null)
        //     return ramSprite;

        // string localPath = Path.Combine(Application.persistentDataPath, fileName);
        // var storageRef = FirebaseStorage.DefaultInstance.GetReference(firebaseStoragePath);

        // if (imageUrlCache.TryGetValue(fileName, out string cachedUrl) && cachedUrl == firebaseStoragePath.ToString() && File.Exists(localPath))
        // {
        //     Debug.Log(fileName + "    cachedUrl:     " + cachedUrl + "      firebaseStoragePath.ToString() :   " + firebaseStoragePath.ToString());
        //     Debug.Log("Önbellekten yükleniyor...");
        //     var sprite = LoadLocalImage(localPath);
        //     AddToRamCache(fileName, sprite);
        //     return sprite;
        // }

        // if (File.Exists(localPath))
        // {
        //     Debug.Log("Eski dosya siliniyor...");
        //     File.Delete(localPath);
        // }

        // Debug.Log("Firebase'den indiriliyor...");
        // Uri uri = await storageRef.GetDownloadUrlAsync();
        // string downloadUrl = uri.ToString();

        // using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(downloadUrl))
        // {
        //     var operation = request.SendWebRequest();
        //     while (!operation.isDone) await Task.Yield();

        //     if (request.result != UnityWebRequest.Result.Success)
        //     {
        //         Debug.LogError($"İndirme hatası: {request.error}");
        //         return null;
        //     }

        //     Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        //     Vector4 border = new Vector4(10, 10, 10, 10);
        //     var sprite = Sprite.Create(
        //         texture,
        //         new Rect(0, 0, texture.width, texture.height),
        //         new Vector2(0.5f, 0.5f),
        //         100f,
        //         0,
        //         SpriteMeshType.FullRect,
        //         border
        //     );

        //     if (sprite != null)
        //     {
        //         SaveTextureToFile(texture, localPath);
        //         imageUrlCache[fileName] = firebaseStoragePath.ToString();
        //         SaveCache();
        //         AddToRamCache(fileName, sprite);
        //     }

        //     return sprite;
        // }
    }
    

    // TO STORE
    public static async Task<Sprite> LoadInGameAssset(int category, string itemID, int index = 0)
    {
        throw new NotImplementedException();
        // if (index == -1)
        //     index = 0;
        // var sprite = await FirebaseStorageHelper.GetStorageGameAsset(category, itemID, index);
        // return sprite;
    }




    /// <summary>
    /// Generate deterministic filename from any string using SHA256 hash
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static string StringToHash(string url)
    {
        using var sha = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(url);
        var hashBytes = sha.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString().ToLower();
    }


    // TODO

    
    private static void EnsureCacheLoaded()
    {
        if (imageUrlCache != null) return;

        if (!File.Exists(CacheFilePath))
        {
            imageUrlCache = new Dictionary<string, string>();
            return;
        }

        try
        {
            string json = File.ReadAllText(CacheFilePath);
            imageUrlCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            Debug.LogError("ImageHelper cache okunamadı: " + ex.Message);
            imageUrlCache = new Dictionary<string, string>();
        }
    }

    private static Sprite GetFromRamCache(string key)
    {
        if (spriteRamCache.TryGetValue(key, out var sprite) && sprite != null)
            return sprite;
        return null;
    }

    private static void AddToRamCache(string key, Sprite sprite)
    {
        if (!spriteRamCache.ContainsKey(key) && sprite != null)
            spriteRamCache[key] = sprite;
    }

    private static void SaveCache()
    {
        try
        {
            string json = JsonConvert.SerializeObject(imageUrlCache, Formatting.Indented);
            File.WriteAllText(CacheFilePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError("ImageHelper cache error: " + ex.Message);
        }
    }

}
