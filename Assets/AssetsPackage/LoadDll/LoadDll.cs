
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
/// �ű���������
/// 1.������Դ����YooAsset��Դ��ܽ�������
///     1����Դ�ļ���ab����
///     2���ȸ���dll
///     3��AOT���Ͳ���Ԫ����dll
/// 2.��AOT dll����Ԫ���ݣ�ͨ��RuntimeApi.LoadMetadataForAOTAssembly()
/// 3.ͨ��ʵ����һ��prefab�������ȸ��´���
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
        Debug.Log("ִ��LoadDLL");
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
            Debug.Log("��¼�ɹ�");
            //loginMainWin.Close();
        }
        else
        {
            Debug.Log(message);
        }

    }*/
    IEnumerator DownLoadAssetsByYooAssets(IEnumerator onDownlodComplete)

    {
        //1.��ʼ����Դϵͳ
        YooAssets.Initialize();
        //����Ĭ�ϵİ�
        package = YooAssets.CreatePackage("DefaultPackage");
        //���ø���Դ��ΪĬ�ϵ���Դ��������ʹ��YooAssets��ؼ��ؽӿڼ��ظ���Դ�����ݡ�
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
            Debug.Log("��Դ����ʼ��");
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
                Debug.Log("��Դ����ʼ���ɹ���");
            }
            else
            {
                Debug.LogError($"��Դ����ʼ��ʧ�ܣ�{initOperation.Error}");
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
            //2.��ȡ��Դ�汾
            Debug.Log("��ȡ��Դ�汾");
            var operation = package.UpdatePackageVersionAsync();
            yield return operation;
            if (operation.Status != EOperationStatus.Succeed)
            {
                Debug.LogError(operation.Error);
                yield break;
            }
            string PackageVersion = operation.PackageVersion;
        //3.���²����嵥
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
        /// ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�
        /// �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش���
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMeatAssemblyNames)
        {
            byte[] dllBytes = GetAssetData(aotDllName);
            // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
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
        /// ͬ����ʽ��ȡ���ܵ���Դ������
        /// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
        /// </summary>
        AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
        }

        /// <summary>
        /// �첽��ʽ��ȡ���ܵ���Դ������
        /// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
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

        //û����Ҫ���ص���Դ
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("û��Ҫ���ص���Դ");
            yield break;
        }
        loginMainWin.SetPage(1);
        //��Ҫ���ص��ļ��������ܴ�С
        int totalDownloadCount = downloader.TotalDownloadCount;
        long totalDownloadBytes = downloader.TotalDownloadBytes;

        //ע��ص�����
        downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
        downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
        downloader.OnDownloadOverCallback = OnDownloadOverFunction;
        downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

        //��������
        downloader.BeginDownload();
        yield return downloader;

        //������ؽ��
        if (downloader.Status == EOperationStatus.Succeed)
        {
            //���سɹ�
        }
        else
        {
            //����ʧ��
        }
    }
    private void OnDownloadErrorFunction(string fileName, string error) {
        Debug.LogError(string.Format("���س����ļ�����{0}��������Ϣ��{1}", fileName, error));
    }
    private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes) {
        int value = currentDownloadCount / totalDownloadCount * 100;
        loginMainWin.SetProgress(value);
        Debug.Log(string.Format("�ļ�������{0}���������ļ�����{1}�������ܴ�С��{2}�������ش�С{3}", totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes));
    }
    private void OnDownloadOverFunction(bool isSucceed) {
        Debug.Log("����" + (isSucceed ? "�ɹ�" : "ʧ��"));
    }
    private void OnStartDownloadFileFunction(string fileName,long sizeBytes) {

        Debug.Log(string.Format("��ʼ���أ��ļ�����{0}���ļ���С��{1}", fileName, sizeBytes));
    }

}

