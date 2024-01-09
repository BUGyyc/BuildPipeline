using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniFramework.Event;

public class PatchWindow : MonoBehaviour
{

    public static PatchWindow Instance;

    /// <summary>
    /// 对话框封装类
    /// </summary>
    private class MessageBox
    {
        private GameObject _cloneObject;
        private Text _content;
        private Button _btnOK;
        private System.Action _clickOK;
        private System.Action _clickCancel;

        private Button _btnCancel;

        public bool ActiveSelf
        {
            get
            {
                return _cloneObject.activeSelf;
            }
        }

        

        public void Create(GameObject cloneObject)
        {
            _cloneObject = cloneObject;
            _content = cloneObject.transform.Find("txt_content").GetComponent<Text>();
            _btnOK = cloneObject.transform.Find("btn_ok").GetComponent<Button>();
            _btnOK.onClick.AddListener(OnClickYes);
            _btnCancel = cloneObject.transform.Find("btn_cancel").GetComponent<Button>();
            _btnCancel.onClick.AddListener(OnClickCancel);
        }
        public void Show(string content, System.Action clickOK, System.Action cancel)
        {
            _content.text = content;
            _clickOK = clickOK;
            _clickCancel = cancel;
            _cloneObject.SetActive(true);
            _cloneObject.transform.SetAsLastSibling();
        }
        public void Hide()
        {
            _content.text = string.Empty;
            _clickOK = null;
            _cloneObject.SetActive(false);
        }
        private void OnClickYes()
        {
            _clickOK?.Invoke();
            Hide();
        }

        private void OnClickCancel()
        {
            _clickCancel?.Invoke();
            Hide();
        }
    }


    private readonly EventGroup _eventGroup = new EventGroup();
    private readonly List<MessageBox> _msgBoxList = new List<MessageBox>();

    // UGUI相关
    private GameObject _messageBoxObj;
    private Slider _slider;
    private Text _tips;

    private Text _cdn;


    void Awake()
    {
        Instance = this;

        _slider = transform.Find("UIWindow/Slider").GetComponent<Slider>();
        _tips = transform.Find("UIWindow/Slider/txt_tips").GetComponent<Text>();
        _tips.text = "Initializing the game world !";
        _cdn = transform.Find("UIWindow/Slider/TxtCdn").GetComponent<Text>();
        _messageBoxObj = transform.Find("UIWindow/MessgeBox").gameObject;
        _messageBoxObj.SetActive(false);

        _eventGroup.AddListener<PatchEventDefine.InitializeFailed>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.PatchStatesChange>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.FoundUpdateFiles>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.DownloadProgressUpdate>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.PackageVersionUpdateFailed>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.PatchManifestUpdateFailed>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.WebFileDownloadFailed>(OnHandleEventMessage);
    }

    private void Update()
    {
        //Debug.Log("  VersionSettings.CDN_URL " + VersionSettings.CDN_URL);
        _cdn.text = VersionSettings.CDN_URL;
    }

    void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is PatchEventDefine.InitializeFailed)
        {
            System.Action callback = () =>
            {
                UserEventDefine.UserTryInitialize.SendEventMessage();
            };

            System.Action cancelCall = () =>
            {
                UserEventDefine.UserCancelUpgrade.SendEventMessage();
            };

            ShowMessageBox($"资源包初始化失败 !", callback, cancelCall);
        }
        else if (message is PatchEventDefine.PatchStatesChange)
        {
            var msg = message as PatchEventDefine.PatchStatesChange;
            _tips.text = msg.Tips;
        }
        else if (message is PatchEventDefine.FoundUpdateFiles)
        {
            var msg = message as PatchEventDefine.FoundUpdateFiles;
            System.Action callback = () =>
            {
                UserEventDefine.UserBeginDownloadWebFiles.SendEventMessage();
            };

            System.Action cancelCall = () =>
            {
                UserEventDefine.UserCancelUpgrade.SendEventMessage();
            };

            float sizeMB = msg.TotalSizeBytes / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            ShowMessageBox($"发现需要热更的文件, 数量：{msg.TotalCount} 大小： {totalSizeMB}MB", callback, cancelCall);
        }
        else if (message is PatchEventDefine.DownloadProgressUpdate)
        {
            var msg = message as PatchEventDefine.DownloadProgressUpdate;
            _slider.value = (float)msg.CurrentDownloadCount / msg.TotalDownloadCount;
            string currentSizeMB = (msg.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (msg.TotalDownloadSizeBytes / 1048576f).ToString("f1");
            _tips.text = $"{msg.CurrentDownloadCount}/{msg.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
        }
        else if (message is PatchEventDefine.PackageVersionUpdateFailed)
        {
            System.Action callback = () =>
            {
                UserEventDefine.UserTryUpdatePackageVersion.SendEventMessage();
            };

            System.Action cancelCall = () =>
            {
                UserEventDefine.UserCancelUpgrade.SendEventMessage();
            };

            ShowMessageBox($"热更失败, 请检测网络状况.", callback, cancelCall);
        }
        else if (message is PatchEventDefine.PatchManifestUpdateFailed)
        {
            System.Action callback = () =>
            {
                UserEventDefine.UserTryUpdatePatchManifest.SendEventMessage();
            };

            System.Action cancelCall = () =>
            {
                UserEventDefine.UserCancelUpgrade.SendEventMessage();
            };

            ShowMessageBox($"无法热更清单文件, 请检测网络状况.", callback, cancelCall);
        }
        else if (message is PatchEventDefine.WebFileDownloadFailed)
        {
            var msg = message as PatchEventDefine.WebFileDownloadFailed;
            System.Action callback = () =>
            {
                UserEventDefine.UserTryDownloadWebFiles.SendEventMessage();
            };

            System.Action cancelCall = () =>
            {
                UserEventDefine.UserCancelUpgrade.SendEventMessage();
            };

            ShowMessageBox($"下载失败，文件名称 : {msg.FileName}", callback, cancelCall);
        }
        else
        {
            throw new System.NotImplementedException($"{message.GetType()}");
        }
    }

    /// <summary>
    /// 显示对话框
    /// </summary>
    private void ShowMessageBox(string content, System.Action ok, System.Action cancel)
    {
        // 尝试获取一个可用的对话框
        MessageBox msgBox = null;
        for (int i = 0; i < _msgBoxList.Count; i++)
        {
            var item = _msgBoxList[i];
            if (item.ActiveSelf == false)
            {
                msgBox = item;
                break;
            }
        }

        // 如果没有可用的对话框，则创建一个新的对话框
        if (msgBox == null)
        {
            msgBox = new MessageBox();
            var cloneObject = GameObject.Instantiate(_messageBoxObj, _messageBoxObj.transform.parent);
            msgBox.Create(cloneObject);
            _msgBoxList.Add(msgBox);
        }

        // 显示对话框
        msgBox.Show(content, ok, cancel);
    }
}