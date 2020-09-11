using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.UI {

    /// <summary>
    ///  UI类型
    /// </summary>
    public enum UiType {
        
        /// <summary>
        /// 弹出UI
        /// </summary>
        Nornal,

        /// <summary>
        ///  固定UI
        /// </summary>
        Fixed,

        /// <summary>
        /// 模态UI
        /// </summary>
        Pop
    }

    /// <summary>
    ///  模态UI类型
    /// </summary>
    public enum PopUiModel {

        /// <summary>
        /// 不透明没有碰撞
        /// </summary>
        None,

        /// <summary>
        /// 透明碰撞
        /// </summary>
        Translucence,

        /// <summary>
        /// 半透明碰撞
        /// </summary>
        HalfTranslucence,
    }

    public enum UiState {

        /// <summary>
        /// 隐藏
        /// </summary>
        Hide,

        /// <summary>
        /// 显示
        /// </summary>
        Show,

        /// <summary>
        /// 冻结 显示但不能交互
        /// </summary>
        Freeze
    }

    /// <summary>
    /// UI透明度值
    /// </summary>
    public struct Diaphaneity {

        /// <summary>
        /// 透明
        /// </summary>
        public static readonly float Translucence = 0f;

        /// <summary>
        /// 半透明
        /// </summary>
        public static readonly float HalfTranslucence = 0.5f;

        /// <summary>
        /// 不透明
        /// </summary>
        public static readonly float NotTranslucence = 1f;
    }

}