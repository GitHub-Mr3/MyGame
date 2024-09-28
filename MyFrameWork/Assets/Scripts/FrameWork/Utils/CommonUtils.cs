using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CommonUtils : SingletonBase<CommonUtils>
{
    private CommonUtils() { }

    /// <summary>
    /// 将方形坐标转换成圆形坐标
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }

    /// <summary>
    ///复制文本到剪切板
    /// </summary>
    /// <param name="str"></param>
    public static void CopyText(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }

    #region 设置头像相关的临时变量
    Texture2D bgTexture = null;
    Sprite spr = null;
    Dictionary<string, Texture> ImaTextureDic = new Dictionary<string, Texture>();
    Dictionary<string, Sprite> ImgSpriteDic = new Dictionary<string, Sprite>();
    Rect newRect = new Rect(0, 0, 0, 0);
    Vector2 v2Size = new Vector2(0.5f, 0.5f);
    #endregion
    /// <summary>
    /// 通过URL地址设置头像
    /// </summary>
    /// <param name="url">url地址</param>
    /// <param name="Image">要设置的组件</param>
    /// <param name="act">设置完成的回调</param>
    /// <param name="key">唯一的键值，用于清除缓存</param>
    /// <returns></returns>
    IEnumerator IESetImageByUrl(string url, Image Image, Action act, string key)
    {

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                bgTexture = DownloadHandlerTexture.GetContent(uwr);
                newRect.width = bgTexture.width;
                newRect.height = bgTexture.height;
                spr = Sprite.Create(bgTexture, newRect, v2Size);

                if (ImaTextureDic.ContainsKey(key))
                {
                    UnityEngine.Object.Destroy(ImaTextureDic[key]);

                }
                if (ImgSpriteDic.ContainsKey(key))
                {
                    UnityEngine.Object.Destroy(ImgSpriteDic[key]);

                }
                ImaTextureDic[key] = bgTexture;
                ImgSpriteDic[key] = spr;

                if (Image != null)
                {
                    Image.sprite = spr;
                    act?.Invoke();
                }
            }
            else
            {
                DebugMgr.Instance.Log($"SetImageByUrl Load error: {uwr.error}, load path: {url}");
            }
            uwr.Dispose();
        }
    }

    private void ClearImgByUrlCache()
    {
        List<string> vs = new List<string>();
        foreach (var item in ImaTextureDic)
        {
            vs.Add(item.Key);
        }
        for (int i = 0; i < vs.Count; i++)
        {
            UnityEngine.Object.Destroy(ImaTextureDic[vs[i]]);
            UnityEngine.Object.Destroy(ImgSpriteDic[vs[i]]);
        }
        vs.Clear();
        ImaTextureDic.Clear();
        ImgSpriteDic.Clear();
    }

}
