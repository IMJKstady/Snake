using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Snake
{
    public class PlayerAi : PlayerControl
    {
        private Tilemap drawMap;        // 地
        private Tilemap wallMap;        // 墙
        private bool speedUpKey;
        private bool wKey;
        private bool sKey;
        private bool aKey;
        private bool dKey;
        private int dir = 1;    // 方向 1 上, 2 下， 3 左, 4 右
        private int targetDir = 1;
        private SnakePlayer snakePlayer;

        [HideInInspector] public int level; // 难度
        
        protected void Start()
        {
            base.Start();
            snakePlayer = GetComponent<SnakePlayer>();
            
            drawMap = snakePlayer.tilemap;
            StartCoroutine(SpeedUp());
            switch (level)
            {
                case 1:
                    StartCoroutine(AiRunLevel_1());
                    break;
                case 2:
                    StartCoroutine(AiRunLevel_2());
                    break;
                case 3:
                    wallMap = drawMap.transform.parent.Find("wall").GetComponent<Tilemap>();
                    StartCoroutine(AiRunLevel_3());
                    break;
            }
        }

        private void SetDir()
        {
             if (wKey)
            {
                wKey = false;
                if (Math.Abs(targetTowardsY) != step)
                {
                    dir = 1;
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
            }

            if (sKey)
            {
                sKey = false;
                if (Math.Abs(targetTowardsY) != step)
                {
                    dir = 2;
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
            }

            if (aKey)
            {
                aKey = false;
                if (Math.Abs(targetTowardsX) != step)
                {
                    dir = 3;
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
            }

            if (dKey)
            {
                dKey = false;
                if (Math.Abs(targetTowardsX) != step)
                {

                    dir = 4;
                    if (targetTowardsY == step)
                    {
                        Debug.Log("d");
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
        }

        private IEnumerator SpeedUp()
        {
            bool isSpeedUp = false;
            bool isRest = false;
            float delay = 0;
            
            while (true)
            {
                if (speedUpKey && !isSpeedUp && !isRest)
                {
                    speedUpKey = false;
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

        // ai 简单
        private IEnumerator AiRunLevel_1()
        {
            while (true)
            {                
                yield return new WaitForSeconds(0.9f);
                if (snakePlayer.isFirstTile) yield return null;
                
                yield return new WaitWhile(() => {
                    targetDir = new System.Random(Guid.NewGuid().GetHashCode()).Next(1, 4);
                    return targetDir == dir;
                });

                switch (targetDir)
                {
                    case 1:
                        wKey = true;
                        break;
                    case 2:
                        sKey = true;
                        break;
                    case 3:
                        aKey = true;
                        break;
                    case 4:
                        dKey = true;
                        break;
                }

                SetDir();

            }
        }
        
        // ai 中等
        private IEnumerator AiRunLevel_2()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.6f);
                if (snakePlayer.isFirstTile) yield return null;
                
                yield return new WaitUntil(() =>
                {
                    targetDir = new System.Random(Guid.NewGuid().GetHashCode()).Next(1, 4);
                    if (targetDir == dir)
                    {
                        return false;
                    } 
                    
                    Vector3Int nowPos = getPos();
                    Vector3Int nextPos = Vector3Int.up;
                    switch (targetDir)
                    {
                        case 1:
                            nextPos = nowPos + new Vector3Int(0, 1, 0);
                            break;
                        case 2:
                            nextPos = nowPos + new Vector3Int(0, -1, 0);
                            break;
                        case 3:
                            nextPos = nowPos + new Vector3Int(-1, 0, 0);
                            break;
                        case 4:
                            nextPos = nowPos + new Vector3Int(1, 0, 0);
                            break;
                    }
                    
                    if (drawMap.HasTile(nextPos)){
                        return false;
                    }
                    return true;
                });
                
                switch (targetDir)
                {
                    case 1:
                        wKey = true;
                        break;
                    case 2:
                        sKey = true;
                        break;
                    case 3:
                        aKey = true;
                        break;
                    case 4:
                        dKey = true;
                        break;
                }

                SetDir();
            }
        }
        
        // ai 困难
        private IEnumerator AiRunLevel_3()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.45f);
                if (snakePlayer.isFirstTile) yield return null;
                
                yield return new WaitUntil(() =>
                {
                    targetDir = new System.Random(Guid.NewGuid().GetHashCode()).Next(1, 4);
                    if (targetDir == dir)
                    {
                        return false;
                    } 

                    Vector3Int nextPos = Vector3Int.up;
                    Vector3Int nowPos = getPos();
                    switch (targetDir)
                    {
                        case 1:
                            nextPos = nowPos + new Vector3Int(0, 1, 0);
                            break;
                        case 2:
                            nextPos = nowPos + new Vector3Int(0, -1, 0);
                            break;
                        case 3:
                            nextPos = nowPos + new Vector3Int(-1, 0, 0);
                            break;
                        case 4:
                            nextPos = nowPos + new Vector3Int(1, 0, 0);
                            break;
                    }

                    if (drawMap.HasTile(nextPos))
                    {
                        return false;
                    }
                    if (wallMap.HasTile(nextPos))
                    {
                        return false;
                    }
                    
                    return true;
                });
                
                switch (targetDir)
                {
                    case 1:
                        wKey = true;
                        break;
                    case 2:
                        sKey = true;
                        break;
                    case 3:
                        aKey = true;
                        break;
                    case 4:
                        dKey = true;
                        break;
                }

                SetDir();
            }
        }

        private Vector3Int getPos()
        {
            Vector3 headPos = gameObject.transform.localPosition;
            int x = (int)headPos.x + (headPos.x < 0 ? -1 : 0);
            int y = (int)headPos.y + (headPos.y < 0 ? -1 : 0);
            // 获取初始位置
            return new Vector3Int(x, y, 0);
        }
    }
}