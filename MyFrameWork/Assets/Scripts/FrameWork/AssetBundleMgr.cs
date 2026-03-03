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

        // 内存使用监控
        private long _cachedResourceSize = 0;
        private const long MAX_CACHE_SIZE = 100 * 1024 * 1024; // 100MB

        /// <summary>
        /// 设置AB包主路径
        /// </summary>
        /// <param name="path">AB包路径</param>
        /// <param name="mainBundleName">主AB包文件名</param>
        public void SetAssetBundlePath(string path, string mainBundleName)
        {
            _mainAssetBundlePath = path?.TrimEnd('/', '\\') + "/";
            _mainAssetBundleName = mainBundleName;
        }

        /// <summary>
        /// 初始化AB管理器，加载主AB包和清单文件
        /// </summary>
        public void Init()
        {
            if (string.IsNullOrEmpty(_mainAssetBundlePath) || string.IsNullOrEmpty(_mainAssetBundleName))
            {
                Debug.LogError("[AssetBundleMgr] AB包路径或主包名未设置");
                return;
            }

            DisposeAllBundles();
            Debug.Log($"[AssetBundleMgr] 初始化路径: {_mainAssetBundlePath}, 主包: {_mainAssetBundleName}");
            LoadManifest();
        }

        /// <summary>
        /// 释放所有资源，防止内存泄漏
        /// </summary>
        private void DisposeAllBundles()
        {
            Debug.Log("[AssetBundleMgr] 开始释放所有AB资源");
            
            // 清理缓存
            _resourceCache.Clear();
            _cachedResourceSize = 0;
            
            // 卸载所有AB包
            foreach (var kvp in _assetBundleCache)
            {
                kvp.Value?.Unload(false);
            }
            _assetBundleCache.Clear();
            
            // 卸载主AB包
            _mainAB?.Unload(false);
            _mainAB = null;
            _manifest = null;
            
            // 强制垃圾回收
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            
            Debug.Log("[AssetBundleMgr] 完成释放所有资源");
        }

        /// <summary>
        /// 加载主AB包和清单文件
        /// </summary>
        private void LoadManifest()
        {
            if (_mainAB != null) return;

            string manifestPath = Path.Combine(_mainAssetBundlePath, _mainAssetBundleName);
            if (!File.Exists(manifestPath))
            {
                Debug.LogError($"[AssetBundleMgr] 主AB包不存在: {manifestPath}");
                return;
            }

            try
            {
                _mainAB = AssetBundle.LoadFromFile(manifestPath);
                if (_mainAB != null)
                {
                    _manifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    Debug.Log($"[AssetBundleMgr] 成功加载清单文件，包含 {_manifest?.GetAllAssetBundles().Length ?? 0} 个AB包");
                }
                else
                {
                    Debug.LogError($"[AssetBundleMgr] 无法加载主AB包: {manifestPath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AssetBundleMgr] 加载主AB包异常: {ex}");
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
                Debug.LogError("[AssetBundleMgr] AB清单未加载");
                return;
            }

            if (string.IsNullOrEmpty(abName))
            {
                Debug.LogError("[AssetBundleMgr] AB包名称为空");
                return;
            }

            // 检查AB包是否存在
            if (!_manifest.GetAllAssetBundles().Contains(abName))
            {
                Debug.LogWarning($"[AssetBundleMgr] AB包不存在: {abName}");
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
            string filePath = Path.Combine(_mainAssetBundlePath, abName);
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[AssetBundleMgr] AB包文件不存在: {filePath}");
                return;
            }

            try
            {
                AssetBundle ab = AssetBundle.LoadFromFile(filePath);
                if (ab != null)
                {
                    _assetBundleCache[abName] = ab;
                    Debug.Log($"[AssetBundleMgr] 成功加载AB包: {abName}");
                }
                else
                {
                    Debug.LogError($"[AssetBundleMgr] 加载AB包失败: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AssetBundleMgr] 加载AB包异常 {abName}: {ex}");
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
                Debug.LogError("[AssetBundleMgr] AB包名或资源名为空");
                return null;
            }

            LoadAssetBundle(abName);
            
            if (_assetBundleCache.TryGetValue(abName, out AssetBundle ab))
            {
                try
                {
                    T resource = ab.LoadAsset<T>(resName);
                    if (resource == null)
                    {
                        Debug.LogWarning($"[AssetBundleMgr] 资源未找到: {abName}/{resName}");
                    }
                    return resource;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[AssetBundleMgr] 加载资源异常 {abName}/{resName}: {ex}");
                    return null;
                }
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
                Debug.LogError("[AssetBundleMgr] AB包名或资源名为空");
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
                try
                {
                    AssetBundleRequest request = ab.LoadAssetAsync<T>(resName);
                    yield return request;

                    T loadedAsset = request.asset as T;
                    if (loadedAsset != null)
                    {
                        // 缓存资源
                        CacheResource(abName, resName, loadedAsset);
                    }
                    else
                    {
                        Debug.LogWarning($"[AssetBundleMgr] 异步加载资源为空: {abName}/{resName}");
                    }

                    callBack?.Invoke(loadedAsset);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[AssetBundleMgr] 异步加载资源异常 {abName}/{resName}: {ex}");
                    callBack?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError($"[AssetBundleMgr] AB包未加载: {abName}");
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
                
                // 估算资源大小（简化版）
                _cachedResourceSize += 1024; // 假设每个资源约1KB
                
                abCache[resName] = resource;
                
                // 内存清理检查
                CheckMemoryUsage();
            }
        }

        /// <summary>
        /// 检查内存使用情况并清理
        /// </summary>
        private void CheckMemoryUsage()
        {
            if (_cachedResourceSize > MAX_CACHE_SIZE)
            {
                Debug.LogWarning($"[AssetBundleMgr] 缓存内存超限 ({_cachedResourceSize / 1024 / 1024}MB)，执行清理");
                ClearResourceCache();
            }
        }

        /// <summary>
        /// 清理资源缓存
        /// </summary>
        private void ClearResourceCache()
        {
            _resourceCache.Clear();
            _cachedResourceSize = 0;
            Resources.UnloadUnusedAssets();
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
                
                Debug.Log($"[AssetBundleMgr] 卸载AB包: {abName}");
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
            _cachedResourceSize = 0;
            _mainAB = null;
            _manifest = null;
            
            Resources.UnloadUnusedAssets();
            Debug.Log("[AssetBundleMgr] 卸载所有AB包完成");
        }

        /// <summary>
        /// 获取当前缓存的AB包数量
        /// </summary>
        /// <returns>AB包数量</returns>
        public int GetCachedBundleCount()
        {
            return _assetBundleCache.Count;
        }

        /// <summary>
        /// 获取当前缓存的资源数量
        /// </summary>
        /// <returns>资源数量</returns>
        public int GetCachedResourceCount()
        {
            int count = 0;
            foreach (var cache in _resourceCache.Values)
            {
                count += cache.Count;
            }
            return count;
        }

        /// <summary>
        /// 清理并释放所有资源
        /// </summary>
        public void Dispose()
        {
            Debug.Log("[AssetBundleMgr] Dispose");
            UnLoadAll(true);
        }
    }
}