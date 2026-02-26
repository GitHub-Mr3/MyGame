using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Mr3
{
    /// <summary>
    /// AssetBundle资源管理器 - 负责AB包的加载、缓存和卸载
    /// 优化点：修复资源泄漏、改进缓存机制、增强异步加载稳定性
    /// </summary>
    public class AssetBundleMgr : Singleton<AssetBundleMgr>
    {
        // AB包主路径
        private string _mainAssetBundlePath = "";
        // AB包文件路径分隔符
        private readonly string _assetBundlePath = "/";
        // 主AB包文件名
        private string _mainAssetBundleName = "";

        // 主AB包引用
        private AssetBundle _mainAB;
        // AB包清单文件
        private AssetBundleManifest _manifest;
        // AB包缓存字典
        private readonly Dictionary<string, AssetBundle> _assetBundleCache = new Dictionary<string, AssetBundle>();

        // 需要缓存的图集名称列表
        private readonly HashSet<string> _cacheAtlasNames = new HashSet<string>
        {
            "green", "red", "blick", "result_black", "result_green", 
            "result_red", "10", "100", "1000", "5000", "defaultHeadIcon"
        };

        // 资源缓存字典 (AB包名 -> 资源名 -> 资源对象)
        private readonly Dictionary<string, Dictionary<string, Object>> _resourceCache = 
            new Dictionary<string, Dictionary<string, Object>>();

        /// <summary>
        /// 初始化AB管理器，加载主AB包和清单文件
        /// </summary>
        public void Init()
        {
            DisposeAllBundles();
            DebugMgr.Instance.Log($"_mainAssetBundlePath: {_mainAssetBundlePath}");
            LoadManifest();
        }

        /// <summary>
        /// 释放所有资源，防止内存泄漏
        /// </summary>
        private void DisposeAllBundles()
        {
            DebugMgr.Instance.Log("开始释放所有AB资源");
            AssetBundle.UnloadAllAssetBundles(true);
            Resources.UnloadUnusedAssets();
            DebugMgr.Instance.Log("完成释放未使用的资源");
        }

        /// <summary>
        /// 加载主AB包和清单文件
        /// </summary>
        private void LoadManifest()
        {
            if (_mainAB != null) return;

            string manifestPath = _mainAssetBundlePath + _mainAssetBundleName;
            if (string.IsNullOrEmpty(manifestPath))
            {
                DebugMgr.Instance.LogError("主AB包路径为空");
                return;
            }

            _mainAB = AssetBundle.LoadFromFile(manifestPath);
            if (_mainAB != null)
            {
                _manifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                _mainAB.Unload(false); // 卸载主AB包但保留清单
            }
            else
            {
                DebugMgr.Instance.LogError($"无法加载主AB包: {manifestPath}");
            }
        }

        /// <summary>
        /// 加载指定的AB包及其依赖
        /// </summary>
        /// <param name="abName">AB包名称</param>
        private void LoadAssetBundle(string abName)
        {
            if (_manifest == null)
            {
                DebugMgr.Instance.LogError("AB清单未加载");
                return;
            }

            if (!_manifest.GetAllAssetBundles().Contains(abName))
            {
                DebugMgr.Instance.Log($"AB包不存在: {abName}");
                return;
            }

            // 加载依赖包
            string[] dependencies = _manifest.GetAllDependencies(abName);
            foreach (string dependency in dependencies)
            {
                if (!_assetBundleCache.ContainsKey(dependency))
                {
                    LoadSingleAssetBundle(dependency);
                }
            }

            // 加载主AB包
            if (!_assetBundleCache.ContainsKey(abName))
            {
                LoadSingleAssetBundle(abName);
            }
        }

        /// <summary>
        /// 加载单个AB包
        /// </summary>
        /// <param name="abName">AB包名称</param>
        private void LoadSingleAssetBundle(string abName)
        {
            string filePath = _mainAssetBundlePath + abName;
            AssetBundle ab = AssetBundle.LoadFromFile(filePath);
            if (ab != null)
            {
                _assetBundleCache[abName] = ab;
                DebugMgr.Instance.Log($"成功加载AB包: {abName}");
            }
            else
            {
                DebugMgr.Instance.LogError($"加载AB包失败: {filePath}");
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <param name="resName">资源名称</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载的资源对象</returns>
        public T LoadRes<T>(string abName, string resName) where T : Object
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(resName))
            {
                DebugMgr.Instance.LogError("AB包名或资源名为空");
                return null;
            }

            LoadAssetBundle(abName);
            
            if (_assetBundleCache.TryGetValue(abName, out AssetBundle ab))
            {
                return ab.LoadAsset<T>(resName);
            }

            return null;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <param name="resName">资源名称</param>
        /// <param name="callBack">加载完成回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(resName))
            {
                DebugMgr.Instance.LogError("AB包名或资源名为空");
                callBack?.Invoke(null);
                return;
            }

            // 检查缓存
            if (TryGetCachedResource(abName, resName, out T cachedResource))
            {
                callBack?.Invoke(cachedResource);
                return;
            }

            CoroutineHelper.Instance.DoFunc($"LoadResAsync_{abName}_{resName}", 
                ReallyLoadResAsync(abName, resName, callBack));
        }

        /// <summary>
        /// 尝试从缓存获取资源
        /// </summary>
        private bool TryGetCachedResource<T>(string abName, string resName, out T result) where T : Object
        {
            result = null;
            
            if (_resourceCache.TryGetValue(abName, out Dictionary<string, Object> abCache) &&
                abCache.TryGetValue(resName, out Object cachedObj))
            {
                result = cachedObj as T;
                return result != null;
            }

            return false;
        }

        /// <summary>
        /// 实际的异步加载协程
        /// </summary>
        private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            LoadAssetBundle(abName);

            if (_assetBundleCache.TryGetValue(abName, out AssetBundle ab))
            {
                AssetBundleRequest request = ab.LoadAssetAsync<T>(resName);
                yield return request;

                T loadedAsset = request.asset as T;
                if (loadedAsset != null)
                {
                    // 缓存资源
                    CacheResource(abName, resName, loadedAsset);
                }

                callBack?.Invoke(loadedAsset);
            }
            else
            {
                callBack?.Invoke(null);
            }
        }

        /// <summary>
        /// 缓存资源到内存中
        /// </summary>
        private void CacheResource(string abName, string resName, Object resource)
        {
            if (_cacheAtlasNames.Contains(resName))
            {
                if (!_resourceCache.TryGetValue(abName, out Dictionary<string, Object> abCache))
                {
                    abCache = new Dictionary<string, Object>();
                    _resourceCache[abName] = abCache;
                }
                abCache[resName] = resource;
            }
        }

        /// <summary>
        /// 卸载指定的AB包
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否卸载所有已加载的对象</param>
        public void UnLoad(string abName, bool unloadAllLoadedObjects = false)
        {
            if (_assetBundleCache.TryGetValue(abName, out AssetBundle ab))
            {
                ab.Unload(unloadAllLoadedObjects);
                _assetBundleCache.Remove(abName);
                
                // 清理资源缓存
                if (_resourceCache.ContainsKey(abName))
                {
                    _resourceCache.Remove(abName);
                }
                
                DebugMgr.Instance.Log($"卸载AB包: {abName}");
            }
        }

        /// <summary>
        /// 卸载所有AB包
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否卸载所有已加载的对象</param>
        public void UnLoadAll(bool unloadAllLoadedObjects = false)
        {
            foreach (var kvp in _assetBundleCache)
            {
                kvp.Value?.Unload(unloadAllLoadedObjects);
            }
            
            _assetBundleCache.Clear();
            _resourceCache.Clear();
            _mainAB = null;
            _manifest = null;
        }

        /// <summary>
        /// 清理并释放所有资源
        /// </summary>
        public void Dispose()
        {
            DebugMgr.Instance.Log("AssetBundleMgr Dispose");
            UnLoadAll(true);
        }
    }
}