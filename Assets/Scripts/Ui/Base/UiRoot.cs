using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snake.UI {

    public class UiRoot {
        public Transform RootNode { get; set; }      // 整个UI根节点
        public Transform NormalNode { get; set; }    // 普通UI根节点
        public Transform FixedNode { get; set; }     // 固定UI根节点
        public Transform PopNode { get; set; }       // 模态UI根节点

        public CanvasScaler[] cannasScalers { get; set; } = new CanvasScaler[3];

        private const int NormalSortOrder = 1;       // ui显示层级
        private const int PopSortOrder = 5;
        private const int FixedSortOrder = 10;
    }
}