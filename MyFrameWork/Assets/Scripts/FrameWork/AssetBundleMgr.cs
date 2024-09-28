using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Mr3
{
    public class AssetBundleMgr : Singleton<AssetBundleMgr>
    {
        //主AB包路径
        private string _mainAssetBundlePath = "";
        //AB包文件路径名
        private string _assetBundlePath = "/";
        //主AB包文件名
        private string _mainAssetBundleName = "";// Constant.ASSETBUNDLES;



        //主AB包
        private AssetBundle _mainAB = null;
        //主AB包清单文件
        private AssetBundleManifest _manifest = null;
        //AB包缓存字典
        private Dictionary<string, AssetBundle> _assetBundleCache = new Dictionary<string, AssetBundle>();



        //缓存需要重复使用的图集
        List<string> cacheAtlasName = new List<string>() { "green", "red", "blick", "result_black", "result_green", "result_red", "10", "100", "1000", "5000", "defaultHeadIcon" };
        //缓存AB资源字典
        Dictionary<string, Dictionary<string, Object>> resAssetByABName = new Dictionary<string, Dictionary<string, Object>>();

        /// <summary>
        /// 初始化获取主AB包路径，加载主AB包和清单文件
        /// </summary>
        public void Init()
        {
            DisposeAllBundles();

            StringBuilder path = new StringBuilder();
            _mainAssetBundlePath = "";// path.Append(FileUtils.GetInstance().GetUpdatePath()).Append(_assetBundlePath).ToString();
            DebugMgr.Instance.Log($"_mainAssetBundlePath: {_mainAssetBundlePath}");
            LoadManifest();
        }

        /// <summary>
        /// 释放所有资源，防止Sud在安卓平台没有调用框架的Dispose方法时的保底
        /// </summary>
        private void DisposeAllBundles()
        {
            DebugMgr.Instance.Log($"启动时释放所有资源");
            AssetBundle.UnloadAllAssetBundles(true);
            DebugMgr.Instance.Log($"卸载所有的AB包");
            Resources.UnloadUnusedAssets();
            DebugMgr.Instance.Log($"释放所有没有引用的资源");
        }



        /// <summary>
        /// 加载主AB包的清单文件
        /// </summary>
        private void LoadManifest()
        {
            if (_mainAB == null)
            {
                _mainAB = AssetBundle.LoadFromFile(_mainAssetBundlePath + _mainAssetBundleName);
                _manifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                _mainAB.Unload(false);
            }
        }

        /// <summary>
        /// 使用名字加载ab包
        /// </summary>
        /// <param name="abName"></param>
        private void LoadAssetBundle(string abName)
        {
            DebugMgr.Instance.Log($"加载AB包:{abName}");

            if (!_manifest.GetAllAssetBundles().Contains(abName))
            {
                DebugMgr.Instance.Log($"加载AB包:{abName} assetBundle文件不存在");
                return;
            }
            var filePath = _mainAssetBundlePath;

            string[] dependencies = _manifest.GetAllDependencies(abName);

            for (int i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];

                if (!_assetBundleCache.ContainsKey(dependency))
                {
                    AssetBundle temp = AssetBundle.LoadFromFile(filePath + dependency);
                    if (temp != null)
                    {
                        _assetBundleCache.Add(dependency, temp);
                    }
                }
            }

            if (!_assetBundleCache.ContainsKey(abName))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(filePath + abName);
                if (ab != null)
                {
                    _assetBundleCache.Add(abName, ab);
                }
            }
        }

        /// <summary>
        /// 使用泛型加载 AB 包内的资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadRes<T>(string abName, string resName) where T : Object
        {
            DebugMgr.Instance.Log($"LoadRes:{abName},{resName}");
            // 加载 AB 包
            LoadAssetBundle(abName);

            return _assetBundleCache[abName].LoadAsset(resName) as T;
        }

        /// <summary>
        /// 加载 AB 包内的资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, Type type)
        {
            // 加载 AB 包
            LoadAssetBundle(abName);

            return _assetBundleCache[abName].LoadAsset(resName, type);
        }

        /// <summary>
        /// 异步加载 AB 包资源（加载 AB 包同步，加载资源异步）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <param name="callBack"></param>
        public void LoadResAsync(string abName, string resName, Type type, UnityAction<Object> callBack)
        {
            // 启动协程
            CoroutineHelper.Instance.DoFunc("ReallyLoadResAsync", ReallyLoadResAsync(abName, resName, type, callBack));
        }

        /// <summary>
        /// 异步加载协程（加载 AB 包同步，加载资源异步）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private IEnumerator ReallyLoadResAsync(string abName, string resName, Type type, UnityAction<Object> callBack)
        {
            // 加载 AB 包
            LoadAssetBundle(abName);

            // 异步加载资源
            AssetBundleRequest abr = _assetBundleCache[abName].LoadAssetAsync(resName, type);
            yield return abr;

            // 执行回调委托
            callBack(abr.asset);
        }

        /// <summary>
        /// 异步加载 AB 包资源（加载 AB 包同步，加载资源异步）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="callBack"></param>
        public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            var obj = GetObj(abName, resName);
            if (obj != null)
            {
                callBack(obj as T);
                return;
            }
            // 启动协程
            CoroutineHelper.Instance.DoFunc("ReallyLoadResAsync", ReallyLoadResAsync(abName, resName, callBack));
        }
        private Object GetObj(string abName, string resName)
        {
            if (resAssetByABName.ContainsKey(abName))
            {
                var resAssetDic = resAssetByABName[abName];
                if (resAssetDic.ContainsKey(resName))
                {
                    var obj = resAssetDic[resName];
                    return obj;
                }
            }
            return null;
        }

        /// <summary>
        /// 异步加载协程（加载 AB 包同步，加载资源异步）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            // 加载 AB 包
            LoadAssetBundle(abName);
            if (_assetBundleCache.ContainsKey(abName))
            {
                var ab = _assetBundleCache[abName];
                // 异步加载资源
                AssetBundleRequest abrcur = ab.LoadAssetAsync<T>(resName);
                yield return abrcur;
                var obj = GetObj(abName, resName);
                if (obj != null)
                {
                    callBack(obj as T);
                    yield break;
                }
                // 执行回调委托
                callBack(abrcur.asset as T);
                Dictionary<string, Object> AllRes = new Dictionary<string, Object>();

                string[] assetNames = ab.GetAllAssetNames();

                foreach (string assetName in assetNames)
                {
                    var loadResName = Path.GetFileNameWithoutExtension(assetName);

                    AssetBundleRequest abr = ab.LoadAssetAsync<T>(loadResName);
                    yield return abr;

                    var assetObj = abr.asset;
                    AllRes[assetObj.name] = assetObj;
                }

                resAssetByABName[abName] = AllRes;
                UnLoad(abName);
            }
            else
            {
                callBack(null);
            }
        }



        public void LoadAllResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {

            if (resAssetByABName.ContainsKey(abName))
            {
                var resAssetDic = resAssetByABName[abName];
                if (resAssetDic.ContainsKey(resName))
                {
                    var obj = resAssetDic[resName];
                    callBack(obj as T);
                    return;
                }
            }

            CoroutineHelper.Instance.DoFunc("ReallyAllLoadResAsync", ReallyAllLoadResAsync(abName, resName, callBack));
        }

        private IEnumerator ReallyAllLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            // 加载 AB 包
            LoadAssetBundle(abName);

            if (_assetBundleCache.ContainsKey(abName))
            {
                AssetBundle ab = _assetBundleCache[abName];
                AssetBundleRequest abr = ab.LoadAllAssetsAsync<T>();
                yield return abr;
                var obj = GetObj(abName, resName);
                if (obj != null)
                {
                    callBack(obj as T);
                    yield break;
                }
                Object[] allAssets = abr.allAssets;
                Dictionary<string, Object> AllRes = new Dictionary<string, Object>();
                foreach (var item in allAssets)
                {

                    if (cacheAtlasName.Contains(item.name))
                    {
                        AllRes[item.name] = item;
                    }
                    if (string.Compare(item.name, resName) == 0)
                    {
                        callBack(item as T);
                    }
                }
                resAssetByABName[abName] = AllRes;
                UnLoad(abName);

            }
            else
            {
                callBack(null);
            }
            /*
            // 加载 AB 包
            LoadAssetBundle(abName);

            // 异步加载资源
            if (_assetBundleCache.ContainsKey(abName))
            {
                Dictionary<string, Object> AllRes = new Dictionary<string, Object>();
                AssetBundleRequest abr = _assetBundleCache[abName].LoadAllAssetsAsync<T>();
                yield return abr;
                DebugMgr.Instance.Log($"abr.allAssets.Length :  {abr.allAssets.Length}");
                foreach (var item in abr.allAssets)
                {
                    DebugMgr.Instance.Log($"ReallyAllLoadResAsync  {item.name}");
                    AllRes[item.name] = item;
                    if (string.Compare(item.name, resName) == 0)
                    {
                        callBack(item as T);
                    }
                }
                UnLoad(abName);
                ResAssetByABName[abName] = AllRes;
            }
            else
            {
                callBack(null);
            }
            */
        }

        public AssetBundle GetAssetBundleCache(string abName)
        {
            if (_assetBundleCache.ContainsKey(abName))
            {
                return _assetBundleCache[abName];
            }

            return null;
        }

        public void RemoveCache(string abName)
        {
            _assetBundleCache.Remove(abName);
        }

        public void UpdateAssetBundleCache(string abName, AssetBundle ab)
        {
            if (!_assetBundleCache.ContainsKey(abName))
            {
                DebugMgr.Instance.Log($"把{abName}加入到缓存中");
                _assetBundleCache.Add(abName, ab);
            }
        }

        public string[] GetAssetBundleAllDependencies(string abName)
        {
            return _manifest.GetAllDependencies(abName);
        }

        public void SetNewManifeset(AssetBundleManifest newManifest)
        {
            DebugMgr.Instance.Log($"SetNewManifeset");
            _manifest = newManifest;
        }


        public void ReloadAssetBundle(string abName)
        {
            LoadAssetBundle(abName);
        }

        /// <summary>
        /// 卸载AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="unloadAllLoadedObjects"></param>
        public void UnLoad(string abName, bool unloadAllLoadedObjects = false)
        {
            DebugMgr.Instance.Log($"UnLoad  1 abName: {abName}");
            if (_assetBundleCache.ContainsKey(abName))
            {
                DebugMgr.Instance.Log($"UnLoad 2  abName: {abName}");
                _assetBundleCache[abName].Unload(unloadAllLoadedObjects);
                _assetBundleCache.Remove(abName);
                DebugMgr.Instance.Log($"UnLoad 3  abName: {abName}");
            }
        }

        /// <summary>
        /// 卸载所有AB包
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        public void UnLoadAll(bool unloadAllLoadedObjects = false)
        {
            AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);

            foreach (var item in _assetBundleCache)
            {
                Debug.Log($"UnLoad:{item.Value}");
                if (item.Value != null)
                {
                    item.Value.Unload(true);
                }
            }

            _assetBundleCache.Clear();
            _mainAB = null;
            _manifest = null;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            DebugMgr.Instance.Log("AssetBundleMgr Dispose");
            UnLoadAll();
        }

    }
}
