using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using strange.extensions.context.impl;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Snake.UI
{
    /// <summary>
    /// UI控制器
    /// </summary>
    public class UiManager : MonoBehaviour, IUiRegulator
    {
        [SerializeField] 
        private AssetReference uiRoot;
        [SerializeField]
        private ViewBase firstUi;
        [SerializeField]
        private Vector2 m_ReferenceResolution;                    // 目标分辨率
        private Dictionary<string, IView> insUIs;                 // 已实例化的UI
        private Stack<IView> uiStack;                             // UI显示栈
        private UiRoot rootNode = new UiRoot();                   // UI根节点

        public void Init() {
            
            insUIs = new Dictionary<string, IView>();
            uiStack = new Stack<IView>();
            rootNode = new UiRoot();

            // 加载ui根节点
            uiRoot.InstantiateAsync().Completed += handle => {
                if (!handle.IsDone)return;
                if (handle.Result == null) throw new Exception("ui根节点加载失败");
                Transform t = handle.Result.transform;

                rootNode.RootNode = t.Find("RootNode");
                rootNode.NormalNode = t.Find("NormalNode");
                rootNode.FixedNode = t.Find("FixedNode");
                rootNode.PopNode = t.Find("PopNode");

                rootNode.cannasScalers[0] = rootNode.NormalNode.GetComponent<CanvasScaler>();
                rootNode.cannasScalers[1] = rootNode.FixedNode.GetComponent<CanvasScaler>();
                rootNode.cannasScalers[2] = rootNode.PopNode.GetComponent<CanvasScaler>();
                
                // CanvasScaler数据设置
                foreach (var it in rootNode.cannasScalers)
                {
                    switch (it.uiScaleMode)
                    {
                        case CanvasScaler.ScaleMode.ConstantPixelSize:
                            break;
                        case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                            it.referenceResolution = m_ReferenceResolution;
                            break;
                        case CanvasScaler.ScaleMode.ConstantPhysicalSize:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
                t.SetParent(transform);

                if (firstUi != null)
                {
                    Open(firstUi.id);
                }
            };
        }

        /// <summary>
        /// 获取UI实例
        /// </summary>
        public  void GetUi<T>(string uiName, Action<T> callBack)
        {
            if (insUIs.TryGetValue(uiName, out var view))
            {
                callBack((T) view);
                return;
            }
            LoadUi(uiName, view1 => { callBack((T)view1); });
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        public void Open(string uiName) {
            IView uI = GetUi(uiName);
            if (uI == null) {
                LoadUi(uiName, OpenUi);
            } else {
                OpenUi(uI);
            }
        }

        public void Back(string uiName) {
            BackUi(GetUi(uiName));
        }

        public void Close(string uiName)
        {
            CloseUi(GetUi(uiName));
        }

        // 获取ui原型
        private IView GetUi(string uiName)
        {
            return insUIs.TryGetValue(uiName, out var obj) ? obj : null;
        }
        
        // 打开UI
        private void OpenUi(IView ui) {
            if (ui.uiType != UiType.Fixed) {
                if (uiStack.Count > 0) {
                    uiStack.Peek().gameObject.SetActive(false);
                }
                uiStack.Push(ui);
            }

            ui.SetEanble();
            var uiAnimation = ui.gameObject.transform.GetComponent<IUiAnimation>();
            uiAnimation?.ShowUIAnima();
        }
        
        // 返回上UI
        private void BackUi(IView ui) {
            if (ui == null) return;

            if (uiStack.Count > 0 && ui.uiType != UiType.Fixed) {
                if (uiStack.Peek().id != ui.id) return;
                ui = uiStack.Pop();
                uiStack.Peek().SetEanble();
            }

            var uiAnimation = ui.gameObject.GetComponent<IUiAnimation>();
            if (uiAnimation != null) {
                uiAnimation .HideUIAnima(() => {
                    ui.SetDisble();
                });
            } else {
                ui.SetDisble();
            }
        }

        // 关闭Ui
        private void CloseUi(IView ui)
        {
            if (ui == null) return;
            
            var uiAnimation = ui.gameObject.GetComponent<IUiAnimation>();
            if (uiAnimation != null) {
                uiAnimation.HideUIAnima(() => {
                    ui.SetDisble();
                });
            } else {
                ui.SetDisble();
            }
        }

        /// <summary>
        /// 清空ui栈
        /// </summary>
        public void Clear() {
            uiStack.Clear();
        }

        /// <summary>
        /// 清空已加载的ui实例
        /// </summary>
        public void ClearIns() {
            insUIs.Clear();
        }

        /// <summary>
        /// 加载UI
        /// </summary>
        private void LoadUi(string name, Action<IView> callBack = null) {
            Addressables.InstantiateAsync(name, rootNode.RootNode).Completed += operation => {
                if (!operation.IsDone)  return;
                StartCoroutine(Instant(operation.Result, callBack));
            };
        }

        private IEnumerator Instant(GameObject obj, Action<IView> callBack)
        {
            yield return new WaitUntil(() =>
            {
                IView view = obj.GetComponent<IView>();
                if (view == null) return false;
                
                view.SetDisble();
                
                if (insUIs.ContainsKey(view.id))
                {
                    insUIs.Remove(view.id);
                }
                insUIs.Add(view.id, view);
                switch (view.uiType)
                {
                    case UiType.Nornal:
                        view.gameObject.transform.SetParent(rootNode.NormalNode, true);
                        break;

                    case UiType.Fixed:
                        view.gameObject.transform.SetParent(rootNode.FixedNode, true);
                        break;

                    case UiType.Pop:
                        view.gameObject.transform.SetParent(rootNode.PopNode, true);
                        break;
                }

                RectTransform tran = view.gameObject.transform as RectTransform;
                tran.sizeDelta = Vector2.zero;
                tran.position = Vector3.zero;
                tran.anchorMin = Vector2.zero;
                tran.anchorMax = Vector2.one;
                tran.localScale = Vector3.one;
                tran.rotation = new Quaternion(0,0,0,0);

                callBack?.Invoke(view);
                return true;
            });
        }
        
        /// <summary>
        /// 卸载UI
        /// </summary>
        public bool UnloadUI(string name) {
            return insUIs.Remove(name);
        }

        /// <summary>
        /// 移除UI实例
        /// </summary>
        private bool Remove(IView view) {
            if (!insUIs.ContainsValue(view)) return false;
            
            insUIs.Remove(view.id);
            Destroy(view.gameObject);
            return true;
        }

        /// <summary>
        /// 移除UI实例
        /// </summary>
        public bool Remove(string ui)
        {
            return Remove(GetUi(ui));
        }
    }
}