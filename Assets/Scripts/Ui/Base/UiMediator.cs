
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

namespace Snake.UI {

    /**
     * 视图中介
     */
    public abstract class UiMediator<T> : Mediator, IView
    {
        public string id
        {
            get
            {
                ViewBase viewBase = view as ViewBase;
                return viewBase == null ? "null" : viewBase.id;
            }
        }
        public UiType uiType {
            get
            {
                ViewBase viewBase = view as ViewBase;
                return viewBase == null ? default : viewBase.uiType;
            }
        }

        [Inject]
        public IUiRegulator iuiregulator { get; set; }          // ui控制器
        [Inject]
        public T view { get; set; }                             // 视图
        [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
        public IEventDispatcher dispatcher { get; set; }        // 跨容器事件

        public override void OnRegister() {
            base.OnRegister();
            Init();
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init() {
        }
        
        /// <summary>
        /// 刷新
        /// </summary>
        public virtual void Refresh() {
        }

        public void SetEanble()
        {
            gameObject.SetActive(true);
            Refresh();
        }

        public void SetDisble()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="name"></param>
        public virtual void Open(string id) {
            iuiregulator.Open(id);
        }

        
        /// <summary>
        ///  返回
        /// </summary>
        public virtual void Back() {
            iuiregulator.Back(id);
        }
        
        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Close() {
            iuiregulator.Close(id);
        }
        
    }
}