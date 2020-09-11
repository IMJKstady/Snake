
using strange.extensions.mediation.impl;

namespace Snake.UI {

    /// <summary>
    /// UI基类
    /// </summary>
    public abstract class ViewBase : View {
        
        public string id;
        public UiType uiType;
        public PopUiModel popUiModel;
        
        public bool isShow => gameObject.activeSelf;

        protected override void Start() {
            base.Start();
            Init();
        }

        public virtual void Init() {
        
        }
    }
}