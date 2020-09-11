using System;
using System.Collections;
using UnityEngine;

namespace Snake
{
    public class PlayerP1 : PlayerControl
    {
        protected void Start()
        {
            base.Start();
            StartCoroutine(SpeedUp());
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.W) && Math.Abs(targetTowardsY) != step)
            {
                if (targetTowardsX == step)
                {
                    turn = -1;
                }
                
                if (targetTowardsX == -step)
                {
                    turn = 1;
                }
                targetRota = Quaternion.Euler(0, 0, 0);
                targetTowardsX = 0;targetTowardsY = step;
            }
            if (Input.GetKey(KeyCode.S) && Math.Abs(targetTowardsY) != step)
            {
                if (targetTowardsX == step)
                {
                    turn = 1;
                }
                
                if (targetTowardsX == -step)
                {
                    turn = -1;
                }
                targetRota = Quaternion.Euler(0, 0, 180);
                targetTowardsX = 0; targetTowardsY = -step;
            }
            if (Input.GetKey(KeyCode.A) && Math.Abs(targetTowardsX) != step)
            {
                if (targetTowardsY == step)
                {
                    turn = -1;
                }
                
                if (targetTowardsY == -step)
                {
                    turn = 1;
                }
                
                targetRota = Quaternion.Euler(0, 0, 90);
                targetTowardsX = -step; targetTowardsY = 0;
            }
            if (Input.GetKey(KeyCode.D) && Math.Abs(targetTowardsX) != step)
            {
                if (targetTowardsY == step)
                {
                    turn = 1;
                }
                
                if (targetTowardsY == -step)
                {
                    turn = -1;
                }
                targetRota = Quaternion.Euler(0, 0, -90);
                targetTowardsX = step; targetTowardsY = 0;
            }
        }

        private IEnumerator SpeedUp()
        {
            bool isSpeedUp = false;
            bool isRest = false;
            float delay = 0;
            
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space) && !isSpeedUp && !isRest)
                {
                    isSpeedUp = true;
                    speed *= 2;
                }

                if (isSpeedUp)
                {
                    delay += Time.deltaTime;
                    if (delay >= 2)
                    {
                        speed /= 2;
                        delay = 0;
                        isSpeedUp = false;
                        isRest = true;
                    }

                }

                if (isRest)
                {
                    delay += Time.deltaTime;
                    if (delay >= 4)
                    {
                        isRest = false;
                        delay = 0;
                    }
                }
                yield return null;
            }
        }
    }
}