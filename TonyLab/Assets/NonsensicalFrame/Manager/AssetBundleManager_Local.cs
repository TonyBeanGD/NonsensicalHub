﻿using NonsensicalKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace NonsensicalKit
{
    /// <summary>
    /// 适用于所有ab包都在本地同一个文件夹内的情况
    /// </summary>
    public class AssetBundleManager_Local : MonoSingleton<AssetBundleManager_Local>
    {
        [SerializeField]
        private bool _autoClear_; //自动卸载未被使用的ab包

        private string assetBundlePath;

        private Dictionary<string, AssetBundleInfo> assstBundleDic;

        private AssetBundleManifest assetBundleManifest;

        protected override void Awake()
        {
            base.Awake();

            assetBundlePath = Path.Combine(Application.dataPath,"Editor", "AssetBundles");

            InitAssetBundleManager(Path.Combine(assetBundlePath, "AssetBundles"));
        }

        /// <summary>
        /// 初始化管理类
        /// </summary>
        /// <param name="assetBundleManifestBundlePath"></param>
        public void InitAssetBundleManager(string assetBundleManifestBundlePath)
        {
            assstBundleDic = new Dictionary<string, AssetBundleInfo>();

            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundleManifestBundlePath);
            assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] bundles = assetBundleManifest.GetAllAssetBundlesWithVariant();

            foreach (var item in bundles)
            {
                assstBundleDic.Add(item, new AssetBundleInfo(item, assetBundleManifest.GetDirectDependencies(item)));
            }
        }

        /// <summary>
        /// 加载AB包
        /// </summary>
        /// <param name="_bundleName"></param>
        /// <param name="_onComplete"></param>
        /// <param name="_onLoading"></param>
        public void LoadAssetBundle(string _bundleName,Action _onComplete, Action<float> _onLoading = null)
        {
            if (assstBundleDic.ContainsKey(_bundleName) == false)
            {
                Debug.LogWarning($"错误的包名{_bundleName}");
                return;
            }
            if (assstBundleDic[_bundleName].AssetBundlePack == true)
            {
                _onComplete?.Invoke();
                return;
            }
            if (assstBundleDic[_bundleName].Loading == false)
            {
                Debug.Log("加载ab包" + _bundleName);
                StartCoroutine(LoadAssetBundleCoroutine(_bundleName, _onComplete, _onLoading));
            }
        }

        /// <summary>
        /// 加载Ab包协程
        /// </summary>
        /// <param name="_bundleName"></param>
        /// <param name="_onComplete"></param>
        /// <param name="_onLoading"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetBundleCoroutine(string _bundleName,Action _onComplete, Action<float> _onLoading = null)
        {
            assstBundleDic[_bundleName].Loading = true;
            string[] dependencies = assstBundleDic[_bundleName].Dependencies;
            int completeCount = 0;
            foreach (var item in dependencies)
            {
                assstBundleDic[item].DependencieCount++;
                LoadAssetBundle(item,()=> { completeCount++; });
            }
            while (completeCount< dependencies.Length)
            {
                yield return null;
            }

            string bundlePath = Path.Combine(assetBundlePath, _bundleName);
            
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
            
            do
            {
                yield return null;

                _onLoading?.Invoke(request.progress);
            }
            while (request.progress < 1);

            
            if (request.assetBundle != null)
            {
                assstBundleDic[_bundleName].AssetBundlePack = request.assetBundle;
                assstBundleDic[_bundleName].Loading = false;
                assstBundleDic[_bundleName].OnLoadComplete?.Invoke();
                assstBundleDic[_bundleName].OnLoadComplete = null;
            }
            else
            {
                Debug.LogError($"AB包加载失败，路径：{bundlePath}");
            }
            _onComplete?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_resourcesName">资源名</param>
        /// <param name="_bundleName">包名</param>
        /// <param name="_onComplete">完成回调</param>
        public void LoadResource<T>(string _resourcesName, string _bundleName, Action<T> _onComplete) where T : UnityEngine.Object
        {
            if (assstBundleDic.ContainsKey(_bundleName) == false)
            {
                Debug.LogWarning($"错误的包名{_bundleName}");
                return;
            }

            if (assstBundleDic[_bundleName].AssetBundlePack != null)
            {
                assstBundleDic[_bundleName].LoadCount++;
                StartCoroutine(LoadResourceCoroutine<T>(_resourcesName, assstBundleDic[_bundleName].AssetBundlePack, _onComplete));
            }
            else
            {
                assstBundleDic[_bundleName].OnLoadComplete += () =>
                {
                    assstBundleDic[_bundleName].LoadCount++;
                    StartCoroutine(LoadResourceCoroutine<T>(_resourcesName, assstBundleDic[_bundleName].AssetBundlePack, _onComplete));
                };
                LoadAssetBundle(_bundleName,null);
            }
        }

        private IEnumerator LoadResourceCoroutine<T>(string _resourcesNameOrPath, AssetBundle _assetBundle, Action<T> _onComplete) where T : UnityEngine.Object
        {
            AssetBundleRequest assetBundleRequest = _assetBundle.LoadAssetAsync<T>(_resourcesNameOrPath);

            yield return assetBundleRequest;

            if (assetBundleRequest.asset != null)
            {
                T Object = assetBundleRequest.asset as T;
                _onComplete(Object);
            }
            else
            {
                Debug.LogError($"未加载到对象:{_resourcesNameOrPath}");
            }
        }

        public void ReleaseAsset(string _bundleName)
        {
            assstBundleDic[_bundleName].LoadCount--;
        }
        
        private class AssetBundleInfo
        {
            public string BundleName;   //包的名称
            public string[] Dependencies;       //直接依赖的包名称

            public AssetBundle AssetBundlePack; //ab包
            public int LoadCount = 0;           //包内对象加载的次数
            public int DependencieCount = 0;    //被依赖加载的次数
            public bool Loading;                //是否正在进行加载
            public Action OnLoadComplete;

            public AssetBundleInfo(string _bundleName, string[] _dependencies)
            {
                this.BundleName = _bundleName;
                this.Dependencies = _dependencies;
            }
        }
    }
}
