using System;
using UnityEngine;

namespace Snake
{
    public class PlayerControl : MonoBehaviour
    {
        [HideInInspector]
        public int targetTowardsX;
        [HideInInspector]
        public int targetTowardsY;
        [HideInInspector]
        public Quaternion targetRota;
        [HideInInspector]
        public int turn = 0;    // 转向 -1 左转， 1 右转
        
        public int speed = 2;
        public int step;

        protected void Start()
        {
            targetTowardsX = 0;
            targetTowardsY = step;
        }
    }
}