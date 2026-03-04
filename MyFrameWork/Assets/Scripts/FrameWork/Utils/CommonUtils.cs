using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 通用工具类 - 提供常用的游戏开发辅助功能
/// </summary>
public class CommonUtils : SingletonBase<CommonUtils>
{
    private CommonUtils() { }

    /// <summary>
    /// 将方形坐标转换为圆形坐标（常用于虚拟摇杆控制）
    /// 解决方形输入区域导致的对角线移动速度过快问题
    /// </summary>
    /// <param name="input">输入的方形坐标 (-1 to 1)</param>
    /// <returns>转换后的圆形坐标</returns>
    public Vector2 SquareToCircle(Vector2 input)
    {
        // 防止输入超出范围
        input = Vector2.ClampMagnitude(input, 1f);
        
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1f - (input.y * input.y) / 2f);
        output.y = input.y * Mathf.Sqrt(1f - (input.x * input.x) / 2f);
        return output;
    }

    /// <summary>
    /// 复制文本到系统剪贴板
    /// </summary>
    /// <param name="text">要复制的文本内容</param>
    public static void CopyText(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            GUIUtility.systemCopyBuffer = text;
        }
    }

    #region 图片加载相关字段
    private readonly Dictionary<string, Texture2D> _imageTextureCache = new Dictionary<string, Texture2D>();
    private readonly Dictionary<string, Sprite> _imageSpriteCache = new Dictionary<string, Sprite>();
    private static readonly Vector2 _pivotCenter = new Vector2(0.5f, 0.5f);
    #endregion

    /// <summary>
    /// 通过URL异步加载图片并设置到指定的Image组件
    /// </summary>
    /// <param name="url">图片URL地址</param>
    /// <param name="targetImage">目标Image组件</param>
    /// <param name="onComplete">加载完成回调</param>
    /// <param name="cacheKey">缓存键值（建议使用URL本身）</param>
    /// <returns>协程IEnumerator</returns>
    public Coroutine LoadImageFromUrl(string url, Image targetImage, Action onComplete = null, string cacheKey = null)
    {
        if (string.IsNullOrEmpty(url) || targetImage == null)
        {
            Debug.LogWarning("LoadImageFromUrl: Invalid parameters");
            onComplete?.Invoke();
            return null;
        }

        // 使用URL作为默认缓存键
        cacheKey ??= url;

        // 检查缓存
        if (_imageSpriteCache.TryGetValue(cacheKey, out Sprite cachedSprite))
        {
            targetImage.sprite = cachedSprite;
            onComplete?.Invoke();
            return null;
        }

        return StartCoroutine(LoadImageCoroutine(url, targetImage, onComplete, cacheKey));
    }

    private IEnumerator LoadImageCoroutine(string url, Image targetImage, Action onComplete, string cacheKey)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                
                if (texture != null)
                {
                    // 创建Sprite
                    Rect rect = new Rect(0, 0, texture.width, texture.height);
                    Sprite sprite = Sprite.Create(texture, rect, _pivotCenter);

                    // 清理旧缓存
                    ClearCacheEntry(cacheKey);

                    // 添加到缓存
                    _imageTextureCache[cacheKey] = texture;
                    _imageSpriteCache[cacheKey] = sprite;

                    // 应用到Image
                    if (targetImage != null)
                    {
                        targetImage.sprite = sprite;
                    }

                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"Failed to create texture from URL: {url}");
                    onComplete?.Invoke();
                }
            }
            else
            {
                Debug.LogError($"Failed to load image from URL: {url}, Error: {request.error}");
                onComplete?.Invoke();
            }
        }
    }

    /// <summary>
    /// 清理指定键值的缓存条目
    /// </summary>
    /// <param name="cacheKey">缓存键值</param>
    private void ClearCacheEntry(string cacheKey)
    {
        if (_imageTextureCache.ContainsKey(cacheKey))
        {
            if (_imageTextureCache[cacheKey] != null)
            {
                UnityEngine.Object.Destroy(_imageTextureCache[cacheKey]);
            }
            _imageTextureCache.Remove(cacheKey);
        }

        if (_imageSpriteCache.ContainsKey(cacheKey))
        {
            if (_imageSpriteCache[cacheKey] != null)
            {
                UnityEngine.Object.Destroy(_imageSpriteCache[cacheKey]);
            }
            _imageSpriteCache.Remove(cacheKey);
        }
    }

    /// <summary>
    /// 清理所有图片缓存（释放内存）
    /// 建议在场景切换或内存紧张时调用
    /// </summary>
    public void ClearAllImageCache()
    {
        foreach (var texture in _imageTextureCache.Values)
        {
            if (texture != null)
            {
                UnityEngine.Object.Destroy(texture);
            }
        }
        
        foreach (var sprite in _imageSpriteCache.Values)
        {
            if (sprite != null)
            {
                UnityEngine.Object.Destroy(sprite);
            }
        }

        _imageTextureCache.Clear();
        _imageSpriteCache.Clear();
        
        Debug.Log("All image cache cleared");
    }

    /// <summary>
    /// 获取当前缓存的图片数量
    /// </summary>
    /// <returns>缓存中的图片数量</returns>
    public int GetCacheCount()
    {
        return _imageSpriteCache.Count;
    }
}