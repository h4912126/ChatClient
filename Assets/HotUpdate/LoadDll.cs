
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
        Debug.Log("׼��ִ���ȸ�����");
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
        //��û��������Դ��ʱ�� �ñ�����Դ
        YooAssets.Initialize();
        var package1 = YooAssets.CreatePackage("DefaultPackage");
        //���ø���Դ��ΪĬ�ϵ���Դ��������ʹ��YooAssets��ؼ��ؽӿڼ��ظ���Դ�����ݡ�
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
        //1.��ʼ����Դϵͳ

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
                Debug.Log("��Դ����ʼ���ɹ���");
            }
            else
            {
                Debug.LogError($"��Դ����ʼ��ʧ�ܣ�{initOperation.Error}");
            }
        
        //2.��ȡ��Դ�汾
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
        //loginMainWin.SetPage(1);
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
        //loginMainWin.SetProgress(value);
        Debug.Log(string.Format("�ļ�������{0}���������ļ�����{1}�������ܴ�С��{2}�������ش�С{3}", totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes));
    }
    private void OnDownloadOverFunction(bool isSucceed) {
        Debug.Log("����" + (isSucceed ? "�ɹ�" : "ʧ��"));
    }
    private void OnStartDownloadFileFunction(string fileName,long sizeBytes) {

        Debug.Log(string.Format("��ʼ���أ��ļ�����{0}���ļ���С��{1}", fileName, sizeBytes));
    }

}

