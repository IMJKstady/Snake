
using UnityEngine;

namespace Snake.UI
{
    /**
     * ui统一对外接口
     */
    public interface IView
    {
        string id { get; }
        UiType uiType { get; }
        GameObject gameObject { get; }

        void SetEanble();
        void SetDisble();
    }
}