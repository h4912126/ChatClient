
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;
using HybridCLR;
using System.Reflection;
using System.Linq;
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
    public interface IEntry
    {
        void Start();
    }
    ResourcePackage package;
    TcpLoginClient TcpLogin= new();
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    public static List<string> AOTMeatAssemblyNames { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
        "Newtonsoft.Json.dll",
    };
    private static Dictionary<string, byte[]> s_assetDatas = new();
    void Start()
    {
        StartCoroutine(DownLoadAssetsByYooAssets(this.StartGame));

    }
    void StartGame() {
        AssetHandle handle = package.LoadAssetSync<TextAsset>("HotUpdate.dll");
        TextAsset textAsset = handle.AssetObject as TextAsset;
        Assembly hotUpdateAss = Assembly.Load(textAsset.bytes);
        Type entryType = hotUpdateAss.GetType("HotUpdateEntry");
        MethodInfo method = entryType.GetMethod("Main");
        Debug.Log("准备执行热更函数");
        method.Invoke(null, null);
        //TcpLogin.Login();

    }
    void OnLoginResultHandler(bool success, string message)
    {

        if (success)
        {
            //loginMainWin.Close();
        }
        else
        {
            Debug.Log(message);
        }

    }
    IEnumerator DownLoadAssetsByYooAssets(Action onDownlodComplete)

    {
        //在没有拉到资源的时候 用本地资源
        YooAssets.Initialize();
        var package1 = YooAssets.CreatePackage("DefaultPackage");
        //设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
        YooAssets.SetDefaultPackage(package1);
        var initParameters1 = new EditorSimulateModeParameters();
        var simulateManifestFilePath1 = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
        initParameters1.SimulateManifestFilePath = simulateManifestFilePath1;
        yield return package1.InitializeAsync(initParameters1);
        TcpLogin.OnLoginResult += OnLoginResultHandler;
        //loginMainWin = new();
        //loginMainWin.Start();
        //loginMainWin.SetPage(0);

        YooAssets.DestroyPackage("DefaultPackage");
        //1.初始化资源系统

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

            string defaultHostServer = "http://[2409:8a62:e42:5a70:b993:170:1eb7:d32f]:8000/Android/v1.0";
            string fallbackHostServer = "http://[2409:8a62:e42:5a70:b993:170:1eb7:d32f]:8000/Android/v1.0";
            var initParameters = new HostPlayModeParameters();
            initParameters.BuildinQueryServices = new GameQueryServices();
            initParameters.DecryptionServices = new FileOffsetDecryption();
            initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
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
        
        //2.获取资源版本
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

            //loginMainWin.SetPage(2);
    }
        var assets = new List<String> {
            "Assembly-CSharp"
        }.Concat(AOTMeatAssemblyNames);
        foreach (var asset in assets)
        {
            AssetHandle handle = package.LoadAssetSync<TextAsset>("HotUpdate.dll");
            TextAsset textAsset = handle.AssetObject as TextAsset;
            Assembly fileData = Assembly.Load(textAsset.bytes);
            //RawFileHandle handle = package.LoadRawFileAsync(asset);
            //yield return handle;
            //byte[] fileData = handle.GetRawFileData();
            s_assetDatas[asset] = textAsset.bytes;
            Debug.Log($"dll:{asset} size:{textAsset.bytes.Length}");
        }
        onDownlodComplete();

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
        //loginMainWin.SetPage(1);
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
        //loginMainWin.SetProgress(value);
        Debug.Log(string.Format("文件总数：{0}，已下载文件数：{1}，下载总大小：{2}，已下载大小{3}", totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes));
    }
    private void OnDownloadOverFunction(bool isSucceed) {
        Debug.Log("下载" + (isSucceed ? "成功" : "失败"));
    }
    private void OnStartDownloadFileFunction(string fileName,long sizeBytes) {

        Debug.Log(string.Format("开始下载：文件名：{0}，文件大小：{1}", fileName, sizeBytes));
    }

}

