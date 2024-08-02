
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;
using HybridCLR;
using System.Reflection;
using System.Linq;
using UnityEditor;
/// <summary>
/// 脚本工作流程
/// 1.下载资源，用YooAsset资源框架进行下载
///     1）资源文件，ab包等
///     2）热更新dll
///     3）AOT泛型补充元数据dll
/// 2.给AOT dll补充元数据，通过RuntimeApi.LoadMetadataForAOTAssembly()
/// 3.通过实例化一个prefab，运行热更新代码
/// </summary>

public class LoadDll : MonoBehaviour
{
    LoginMainWin loginMainWin;
    ResourcePackage package;
    //TcpLoginClient TcpLogin ;
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    public static List<string> AOTMeatAssemblyNames { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
        "Newtonsoft.Json.dll",
        "System.dll",
        "UnityEngine.AndroidJNIModule.dll",
        /*"Mono.Security.dll",
        "System.Configuration.dll",
        "System.Data.dll",
        "System.Drawing.dll",
        "System.Numerics.dll",
        "System.Runtime.Serialization.dll",
        "System.Xml.dll",
        "System.Xml.Linq.dll",
        "UniFramework.Event.dll",
        "UniFramework.Machine.dll",
        "UniFramework.Utility.dll",
        "Unity.VisualScripting.Antlr3.Runtime.dll",
        "Unity.VisualScripting.Core.dll",
        "Unity.VisualScripting.Flow.dll",
        "Unity.VisualScripting.State.dll",
        "UnityEngine.AIModule.dll",
        "UnityEngine.AnimationModule.dll",
        "UnityEngine.AssetBundleModule.dll",
        "UnityEngine.AudioModule.dll",
        "UnityEngine.CoreModule.dll",
        "UnityEngine.dll",
        "UnityEngine.GridModule.dll",
        "UnityEngine.IMGUIModule.dll",
        "UnityEngine.InputLegacyModule.dll",
        "UnityEngine.JSONSerializeModule.dll",
        "UnityEngine.ParticleSystemModule.dll",
        "UnityEngine.Physics2DModule.dll",
        "UnityEngine.PhysicsModule.dll",
        "UnityEngine.PropertiesModule.dll",
        "UnityEngine.SharedInternalsModule.dll",
        "UnityEngine.SpriteShapeModule.dll",
        "UnityEngine.TextCoreFontEngineModule.dll",
        "UnityEngine.TextCoreTextEngineModule.dll",
        "UnityEngine.TextRenderingModule.dll",
        "UnityEngine.TilemapModule.dll",
        "UnityEngine.UI.dll",
        "UnityEngine.UIElementsModule.dll",
        "UnityEngine.UIModule.dll",
        "UnityEngine.UnityAnalyticsModule.dll",
        "UnityEngine.UnityWebRequestAssetBundleModule.dll",
        "UnityEngine.UnityWebRequestModule.dll",
        "YooAsset.dll",*/
    };
    private readonly static Dictionary<string, byte[]> s_assetDatas = new();
    void Start()
    {
        Debug.Log("执行LoadDLL");
        //TcpLogin = ScriptableObject.CreateInstance<TcpLoginClient>();
        StartCoroutine(DownLoadAssetsByYooAssets(StartGame()));

    }
    IEnumerator StartGame() {
        LoadMetadataForAOTAssemblies();
        AssetHandle handle = package.LoadAssetSync<TextAsset>("HotUpdate.dll");
        TextAsset textAsset = handle.AssetObject as TextAsset;
        Assembly hotUpdateAss = Assembly.Load(textAsset.bytes);
        AssetHandle handle2 = package.LoadAssetAsync<GameObject>("startPerfab");
        yield return handle2;
        GameObject go2 = handle2.InstantiateSync();
        Debug.Log($"Prefab name is {go2.name}");
        //loginMainWin.Close();
        //TcpLogin.Login();

    }
    /*void OnLoginResultHandler(bool success, string message)
    {

        if (success)
        {
            Debug.Log("登录成功");
            //loginMainWin.Close();
        }
        else
        {
            Debug.Log(message);
        }

    }*/
    IEnumerator DownLoadAssetsByYooAssets(IEnumerator onDownlodComplete)

    {
        //1.初始化资源系统
        YooAssets.Initialize();
        //创建默认的包
        package = YooAssets.CreatePackage("DefaultPackage");
        //设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
        YooAssets.SetDefaultPackage(package);

        if (PlayMode == EPlayMode.EditorSimulateMode)
        {
            var initParameters = new EditorSimulateModeParameters();
            var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
            initParameters.SimulateManifestFilePath = simulateManifestFilePath;
            yield return package.InitializeAsync(initParameters);
        }
        else if (PlayMode == EPlayMode.OfflinePlayMode)
        {
            var initParameters = new OfflinePlayModeParameters();
            yield return package.InitializeAsync(initParameters);

        }
        else if (PlayMode == EPlayMode.HostPlayMode)
        {
            Debug.Log("资源包初始化");
            string defaultHostServer = "http://[2409:8a62:e42:5a70:b993:170:1eb7:d32f]:8000/Android/v1.0";
            string fallbackHostServer = "http://[2409:8a62:e42:5a70:b993:170:1eb7:d32f]:8000/Android/v1.0";
            var initParameters = new HostPlayModeParameters
            {
                BuildinQueryServices = new GameQueryServices(),
                DecryptionServices = new FileOffsetDecryption(),
                RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer)
            };
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("资源包初始化成功！");
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            }
            /*var assets2 = new List<String> {
            "Assembly-CSharp.dll"
        }.Concat(AOTMeatAssemblyNames);
            foreach (var asset in assets2)
            {
                AssetHandle handle2 = package.LoadAssetSync<TextAsset>(asset);
                TextAsset textAsset2 = handle2.AssetObject as TextAsset;
                s_assetDatas[asset] = textAsset2.bytes;
            }
            LoadMetadataForAOTAssemblies();
            AssetHandle handle = package.LoadAssetSync<TextAsset>("loginUpdate.dll");
            TextAsset textAsset = handle.AssetObject as TextAsset;*/
            //Assembly hotUpdateAss = Assembly.Load(textAsset.bytes);
            loginMainWin = ScriptableObject.CreateInstance<LoginMainWin>();
            loginMainWin.Start();
            //2.获取资源版本
            Debug.Log("获取资源版本");
            var operation = package.UpdatePackageVersionAsync();
            yield return operation;
            if (operation.Status != EOperationStatus.Succeed)
            {
                Debug.LogError(operation.Error);
                yield break;
            }
            string PackageVersion = operation.PackageVersion;
        //3.更新补丁清单
        var operation2 = package.UpdatePackageManifestAsync(PackageVersion);
        yield return operation2;
        if (operation2.Status != EOperationStatus.Succeed)
        {

            Debug.LogError(operation2.Error);

            yield break;
        }
        yield return Download();

            loginMainWin.SetPage(2);
    }
        var assets = new List<String> {
            "Assembly-CSharp.dll"
        }.Concat(AOTMeatAssemblyNames);
        foreach (var asset in assets)
        {
            //Debug.Log($"dll:{asset}");
            AssetHandle handle = package.LoadAssetSync<TextAsset>(asset);
            TextAsset textAsset = handle.AssetObject as TextAsset;
            //Assembly fileData = Assembly.Load(textAsset.bytes);
            //RawFileHandle handle = package.LoadRawFileAsync(asset);
            //yield return handle;
            //byte[] fileData = handle.GetRawFileData();
            s_assetDatas[asset] = textAsset.bytes;
            //Debug.Log($"dll:{asset} size:{textAsset.bytes.Length}");
        }
        StartCoroutine(onDownlodComplete);

    }
    private static void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMeatAssemblyNames)
        {
            byte[] dllBytes = GetAssetData(aotDllName);
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            //Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }
    public static byte[] GetAssetData(string dllName)
    {
        return s_assetDatas[dllName];
    }
    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
    private class FileOffsetDecryption : IDecryptionServices
    {
        /// <summary>
        /// 同步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
        }

        /// <summary>
        /// 异步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
        }

        private static ulong GetFileOffset()
        {
            return 32;
        }
    }
    IEnumerator Download() {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var package = YooAssets.GetPackage("DefaultPackage");
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        //没有需要下载的资源
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("没有要下载的资源");
            yield break;
        }
        loginMainWin.SetPage(1);
        //需要下载的文件总数和总大小
        int totalDownloadCount = downloader.TotalDownloadCount;
        long totalDownloadBytes = downloader.TotalDownloadBytes;

        //注册回调方法
        downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
        downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
        downloader.OnDownloadOverCallback = OnDownloadOverFunction;
        downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

        //开启下载
        downloader.BeginDownload();
        yield return downloader;

        //检测下载结果
        if (downloader.Status == EOperationStatus.Succeed)
        {
            //下载成功
        }
        else
        {
            //下载失败
        }
    }
    private void OnDownloadErrorFunction(string fileName, string error) {
        Debug.LogError(string.Format("下载出错：文件名：{0}，错误信息：{1}", fileName, error));
    }
    private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes) {
        int value = currentDownloadCount / totalDownloadCount * 100;
        loginMainWin.SetProgress(value);
        Debug.Log(string.Format("文件总数：{0}，已下载文件数：{1}，下载总大小：{2}，已下载大小{3}", totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes));
    }
    private void OnDownloadOverFunction(bool isSucceed) {
        Debug.Log("下载" + (isSucceed ? "成功" : "失败"));
    }
    private void OnStartDownloadFileFunction(string fileName,long sizeBytes) {

        Debug.Log(string.Format("开始下载：文件名：{0}，文件大小：{1}", fileName, sizeBytes));
    }

}

