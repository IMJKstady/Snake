using System.Collections;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Snake {

    /// <summary>
    /// 管理者
    /// </summary>
    public class Leader : ContextView {
        private static Leader m_leader;

        /// <summary>
        /// 管理者实例
        /// </summary>
        public static Leader ins {
            get {
                if (m_leader == null) {
                    m_leader = FindObjectOfType<Leader>();
                }
                return m_leader;
            }
        }

        /// <summary>
        /// Fps 游戏帧率
        /// </summary>
        [SerializeField, Tooltip("平台目标帧率")]
        private int targetFps = 30;

        [SerializeField]
        private int practicalFps;

        /// <summary>
        /// 实时帧率
        /// </summary>
        public int PracticalFps => practicalFps;

        /// <summary>
        /// 设置目标帧率
        /// </summary>
        public void SetTargetFps(int fps) {
            Application.targetFrameRate = targetFps;
        }

        private void Awake() {
            m_leader = this;
            DontDestroyOnLoad(this);

            context = new RootContext(this, ContextStartupFlags.MANUAL_MAPPING);
        }

        private void Start() {
            SetTargetFps(targetFps);
            
            foreach (var item in GetComponentsInChildren<IRegulator>()) {
                item.Init();
            }
            // 等待其他context初始化后再初始化
            context.Start();
            StartCoroutine(CalculateFps());
        }

        // 计算帧率
        private IEnumerator CalculateFps() {
            int upeadteCount = 0;  // 进入updeate次数
            float passedtime = 0;  // 经过的时间
            while (true) {
                upeadteCount++;
                passedtime += Time.deltaTime;
                yield return null;
                if (upeadteCount >= 1) {
                    practicalFps = System.Convert.ToInt32(upeadteCount / passedtime);

                    upeadteCount = 0;
                    passedtime = 0;
                }
            }
        }
    }
}