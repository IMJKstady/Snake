using System;
using strange.extensions.signal.impl;

namespace Snake.UI
{
    public interface IToolTip : IView
    {
        /// <summary>
        /// 设置确认按钮监听
        /// </summary>
        IToolTip SetConfirmListener(Action callBack);

        /// <summary>
        /// 设置取消按钮监听
        /// </summary>
        IToolTip SetCancelListener(Action callBack);

        /// <summary>
        /// 设置提示信息
        /// </summary>
        /// <param name="info">信息</param>
        IToolTip SetInfo(string info);
    }
}