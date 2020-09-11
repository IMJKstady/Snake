using System;
using strange.extensions.mediation.impl;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

namespace Snake
{
    public class SnakePlayerMediator : Mediator
    {
        [Inject]
        public GameOverSignal gameOverSignal { get; set; }
        
        [HideInInspector]
        public int towardsX;
        [HideInInspector]
        public int towardsY;
        private SnakePlayer view;
        private Vector3Int nowPos;
        private Camera main;
        private PlayerControl playerControl;
        private Coroutine run;

        public Tile tileHorizontal;
        public Tile tileVertical;
        public Tile tileLU;
        public Tile tileRU;
        public Tile tileLD;
        public Tile tileRD;
        public Tile tileTr;
        
        public override void OnRegister()
        {
            main = Camera.main;
            view = GetComponent<SnakePlayer>();
            playerControl = GetComponent<PlayerControl>();
            towardsX  = 0;
            towardsY = playerControl.step;
            view.showName.text = view.playerName;
            view.isFirstTile = true;
            
            // 创建瓦片
            tileHorizontal = ScriptableObject.CreateInstance<Tile>();
            tileVertical = ScriptableObject.CreateInstance<Tile>();
            tileLU = ScriptableObject.CreateInstance<Tile>();
            tileRU = ScriptableObject.CreateInstance<Tile>();
            tileLD = ScriptableObject.CreateInstance<Tile>();
            tileRD = ScriptableObject.CreateInstance<Tile>();
            tileTr = ScriptableObject.CreateInstance<Tile>();
            
            StartCoroutine(Init());
        }
        
        private IEnumerator Init()
        {
            AsyncOperationHandle<Sprite> horize = Addressables.LoadAssetAsync<Sprite>("sprite_h");
            AsyncOperationHandle<Sprite> vertical =  Addressables.LoadAssetAsync<Sprite>("sprite_v");
            AsyncOperationHandle<Sprite> lu =  Addressables.LoadAssetAsync<Sprite>("sprite_lu");
            AsyncOperationHandle<Sprite> ld =  Addressables.LoadAssetAsync<Sprite>("sprite_ld");
            AsyncOperationHandle<Sprite> ru =  Addressables.LoadAssetAsync<Sprite>("sprite_ru");
            AsyncOperationHandle<Sprite> rd =  Addressables.LoadAssetAsync<Sprite>("sprite_rd");
            AsyncOperationHandle<Sprite> tr =  Addressables.LoadAssetAsync<Sprite>("sprite_tr");
            yield return horize;
            yield return vertical;
            yield return lu;
            yield return ld;
            yield return ru;
            yield return rd;
            yield return tr;
            
            // 设置图片
            tileHorizontal.sprite = horize.Result;
            tileVertical.sprite = vertical.Result;
            tileLU.sprite = lu.Result;
            tileRU.sprite = ru.Result;
            tileLD.sprite = ld.Result;
            tileRD.sprite = rd.Result;
            tileTr.sprite = tr.Result;
            
            // 设置颜色
            tileHorizontal.color = view.tileColor;
            tileVertical.color = view.tileColor;
            tileLU.color = view.tileColor;
            tileRU.color = view.tileColor;
            tileLD.color = view.tileColor;
            tileRD.color = view.tileColor;
            tileTr.color = view.tileColor;
            
            Vector3 headPos = gameObject.transform.localPosition;
            int x = (int)headPos.x + (headPos.x < 0 ? -1 : 0);
            int y = (int)headPos.y + (headPos.y < 0 ? -1 : 0);
            // 获取初始位置
            nowPos = new Vector3Int(x, y, 0);
            run = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            while (true)
            {
                //保存下来蛇头移动前的位置
                Vector3 headPos = gameObject.transform.localPosition;
                Vector3 targetPos = new Vector3(headPos.x + towardsX, headPos.y + towardsY, headPos.z);
                Vector3 pos = Vector3.MoveTowards(headPos, targetPos, Time.deltaTime * playerControl.speed);
                
                // 计算下一步所在格子的位置
                int x = (int)pos.x + (pos.x < 0 ? -1 : 0);
                int y = (int)pos.y + (pos.y < 0 ? -1 : 0);
                Vector3Int nextPos = new Vector3Int(x, y, 0);
                
                // 在当前格子中的坐标
                Vector2 inPos = new Vector2(Math.Abs(pos.x) * 10 % 10 / 10, Math.Abs(pos.y) * 10 % 10 / 10);
                // 计算蛇在格子中的坐标里到格子中心的距离
                float dis = Vector2.Distance(new Vector2(0.5f, 0.5f),  inPos);

                if (dis < 0.05f && !view.tilemap.HasTile(nowPos))
                {
                    SetTile();
                    if (playerControl.turn != 0)
                    {
                        gameObject.transform.localRotation = playerControl.targetRota;
                        towardsX = playerControl.targetTowardsX;
                        towardsY = playerControl.targetTowardsY;

                        playerControl.turn = 0;
                    }
                }
                
                // 下一步所到的格子不是上一次的格子，进入一个新格子
                if (nowPos.x != nextPos.x || nowPos.y != nextPos.y)
                {
                    // 不为空撞到图块
                   if (view.tilemap.HasTile(nextPos))
                    {
                        Die();
                    }
                    nowPos = new Vector3Int(x, y, 0);
                }

                //蛇头向期望位置移动
                gameObject.transform.localPosition = pos;
                view.showName.transform.position = main.WorldToScreenPoint(pos + new Vector3(1f, 1f, 01));
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Die();
        }

        private void Die()
        {
            view.isDie = true;
            StopCoroutine(run);
            gameOverSignal.Dispatch(view.gameMode);
        }

        private void SetTile()
        {
            if (view.isFirstTile)
            { 
                view.tilemap.SetTile(nowPos, tileTr);
                view.isFirstTile = false;
                return;
            }
            
            int step = playerControl.step;
            // 直行
            if (playerControl.turn == 0)
            {
                // 直线2 水平
                if (Math.Abs(towardsX) != 0)
                {
                    view.tilemap.SetTile(nowPos, tileHorizontal);
                }
                // 直线1 垂直
                else
                {
                    view.tilemap.SetTile(nowPos, tileVertical);
                }
            }
            // 右转弯
            else if (playerControl.turn == 1)
            {
                // 左上的地图块
                if (towardsY == step)
                {
                    view.tilemap.SetTile(nowPos, tileLU);
                }
                // 右下的地图块
                else if (towardsY == -step)
                {
                    view.tilemap.SetTile(nowPos, tileRD);
                }
                // 右上的地图块
                else if (towardsX == step)
                {
                    view.tilemap.SetTile(nowPos, tileRU);
                }
                // 右下的地图块
                else if (towardsX == -step)
                {
                    view.tilemap.SetTile(nowPos, tileLD);
                }
            }
            // 左转弯
            else if (playerControl.turn == -1)
            {
                // 右上的地图块
                if (towardsY == step)
                {
                    view.tilemap.SetTile(nowPos, tileRU);
                }
                // 左下的地图块
                else if (towardsY == -step)
                {
                    view.tilemap.SetTile(nowPos, tileLD);
                }
                // 右下的地图块
                else if (towardsX == step)
                {
                    view.tilemap.SetTile(nowPos, tileRD);
                }
                // 左上的地图块
                else if (towardsX == -step)
                {
                    view.tilemap.SetTile(nowPos, tileLU);
                }
            }
        }
    }
}