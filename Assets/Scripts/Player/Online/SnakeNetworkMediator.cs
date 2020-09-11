using System;
using System.Collections;
using DefaultNamespace;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

namespace Snake.Online
{
    public class SnakeNetworkMediator : NetworkMediator
    {
        [Inject]
        public OnlineManager oblineManager { get; set; }
        
        private int towardsX;
        private int towardsY;
        private SnakeNetworkPlayer view;
        private Vector3Int nowPos;
        private Coroutine run;
        private OnlineSceneManager sceneManager;
        private bool isFirstTile = true;
        
        public Tile tileHorizontal;
        public Tile tileVertical;
        public Tile tileLU;
        public Tile tileRU;
        public Tile tileLD;
        public Tile tileRD;
        public Tile tileTr;
        
        private int targetTowardsX;
        private int targetTowardsY;
        private Quaternion targetRota;
        private int turn = 0;    // 转向 -1 左转， 1 右转

        private bool isInit = false;

        private void Start()
        {
            isInit = false;
        }

        public override void OnRegister()
        {
            if (!photonView.isMine) return;
            view = GetComponent<SnakeNetworkPlayer>();

            targetTowardsX = 0;
            targetTowardsY = view.step;
            view.map = GameObject.Find("onlineMap").GetComponent<OnlineMap>();
            sceneManager =  GameObject.Find("Manager").GetComponent<OnlineSceneManager>();
            StartCoroutine(Init());
            StartCoroutine(SpeedUp());
            
            sceneManager.gameOverSignal.AddListener(DieSignal);
        }

        private IEnumerator SpeedUp()
        {
            bool isSpeedUp = false;
            bool isRest = false;
            float delay = 0;
            
            while (true)
            {
                if (!photonView.isMine) yield return null;
                
                if (Input.GetKeyDown(KeyCode.Space) && !isSpeedUp && !isRest)
                {
                    isSpeedUp = true;
                    view.speed *= 2;
                }

                if (isSpeedUp)
                {
                    delay += Time.deltaTime;
                    if (delay >= 2)
                    {
                        view.speed /= 2;
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

        private IEnumerator Init()
        {
            if (!photonView.isMine) yield return null;
            // 创建瓦片
            tileHorizontal = ScriptableObject.CreateInstance<Tile>();
            tileVertical = ScriptableObject.CreateInstance<Tile>();
            tileLU = ScriptableObject.CreateInstance<Tile>();
            tileRU = ScriptableObject.CreateInstance<Tile>();
            tileLD = ScriptableObject.CreateInstance<Tile>();
            tileRD = ScriptableObject.CreateInstance<Tile>();
            tileTr = ScriptableObject.CreateInstance<Tile>();
            
            // 创建瓦片
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
            
            towardsX  = 0;
            towardsY = view.step;

            Vector3 headPos = gameObject.transform.localPosition;
            int x = (int)headPos.x + (headPos.x < 0 ? -1 : 0);
            int y = (int)headPos.y + (headPos.y < 0 ? -1 : 0);
            // 获取初始位置
            nowPos = new Vector3Int(x, y, 0);
            isInit = true;
        }

        private void Update()
        {
            if (!isInit) return;
            if (!photonView.isMine) return;

            if (Input.GetKey(KeyCode.W) && Math.Abs(targetTowardsY) != view.step)
            {
                if (targetTowardsX == view.step)
                {
                    turn = -1;
                }

                if (targetTowardsX == -view.step)
                {
                    turn = 1;
                }

                targetRota = Quaternion.Euler(0, 0, 0);
                targetTowardsX = 0;
                targetTowardsY = view.step;
            }

            if (Input.GetKey(KeyCode.S) && Math.Abs(targetTowardsY) != view.step)
            {
                if (targetTowardsX == view.step)
                {
                    turn = 1;
                }

                if (targetTowardsX == -view.step)
                {
                    turn = -1;
                }

                targetRota = Quaternion.Euler(0, 0, 180);
                targetTowardsX = 0;
                targetTowardsY = -view.step;
            }

            if (Input.GetKey(KeyCode.A) && Math.Abs(targetTowardsX) != view.step)
            {
                if (targetTowardsY == view.step)
                {
                    turn = -1;
                }

                if (targetTowardsY == -view.step)
                {
                    turn = 1;
                }

                targetRota = Quaternion.Euler(0, 0, 90);
                targetTowardsX = -view.step;
                targetTowardsY = 0;
            }

            if (Input.GetKey(KeyCode.D) && Math.Abs(targetTowardsX) != view.step)
            {
                if (targetTowardsY == view.step)
                {
                    turn = 1;
                }

                if (targetTowardsY == -view.step)
                {
                    turn = -1;
                }

                targetRota = Quaternion.Euler(0, 0, -90);
                targetTowardsX = view.step;
                targetTowardsY = 0;
            }
            
            //保存下来蛇头移动前的位置
            Vector3 headPos = gameObject.transform.localPosition;
            Vector3 targetPos = new Vector3(headPos.x + towardsX, headPos.y + towardsY, headPos.z);
            Vector3 pos = Vector3.MoveTowards(headPos, targetPos, Time.deltaTime * view.speed);
                
            // 计算下一步所在格子的位置
            int x = (int)pos.x + (pos.x < 0 ? -1 : 0);
            int y = (int)pos.y + (pos.y < 0 ? -1 : 0);
            Vector3Int nextPos = new Vector3Int(x, y, 0);
                
            // 在当前格子中的坐标
            Vector2 inPos = new Vector2(Math.Abs(pos.x) * 10 % 10 / 10, Math.Abs(pos.y) * 10 % 10 / 10);
            // 计算蛇在格子中的坐标里到格子中心的距离
            float dis = Vector2.Distance(new Vector2(0.5f, 0.5f),  inPos);

            if (dis < 0.05f && !HasTile(nowPos))
            {
                SetTile();
                if (turn != 0)
                {
                    gameObject.transform.localRotation = targetRota;
                    towardsX = targetTowardsX;
                    towardsY = targetTowardsY;

                    turn = 0;
                }
            }
                
            // 下一步所到的格子不是上一次的格子，进入一个新格子
            if (nowPos.x != nextPos.x || nowPos.y != nextPos.y)
            {
                // 不为空撞到图块
                if (HasTile(nextPos))
                {
                    Die();
                }
                nowPos = new Vector3Int(x, y, 0);
            }

            //蛇头向期望位置移动
            gameObject.transform.localPosition = pos;
            view.showName.transform.position = view.main.WorldToScreenPoint(pos + new Vector3(1f, 1f, 01));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Die();
        }

        private void Die()
        {
            if (run != null)
            {
                StopCoroutine(run);
            }            
            view.isDie = true;
            sceneManager.PlayerDeath(PhotonNetwork.player.NickName);
            Destroy(gameObject);
        }

        private void DieSignal()
        {
            if (run != null)
            {
                StopCoroutine(run);
            }       
            Destroy(gameObject);
        }

        private void SetTile()
        {
            if (isFirstTile)
            { 
                view.map.SetTile(nowPos, tileTr);
                isFirstTile = false;
                return;
            }
            
            int step = view.step;
            // 直行
            if (turn == 0)
            {
                // 直线2 水平
                if (Math.Abs(towardsX) != 0)
                {
                    SetNetworkTile(nowPos, tileHorizontal);
                }
                // 直线1 垂直
                else
                {
                    SetNetworkTile(nowPos, tileVertical);
                }
            }
            // 右转弯
            else if (turn == 1)
            {
                // 左上的地图块
                if (towardsY == step)
                {
                    SetNetworkTile(nowPos, tileLU);
                }
                // 右下的地图块
                else if (towardsY == -step)
                {
                    SetNetworkTile(nowPos, tileRD);
                }
                // 右上的地图块
                else if (towardsX == step)
                {
                    SetNetworkTile(nowPos, tileRU);
                }
                // 左下的地图块
                else if (towardsX == -step)
                {
                    SetNetworkTile(nowPos, tileLD);
                }
            }
            // 左转弯
            else if (turn == -1)
            {
                // 右上的地图块
                if (towardsY == step)
                {
                    SetNetworkTile(nowPos, tileRU);
                }
                // 左下的地图块
                else if (towardsY == -step)
                {
                    SetNetworkTile(nowPos, tileLD);
                }
                // 右下的地图块
                else if (towardsX == step)
                {
                    SetNetworkTile(nowPos, tileRD);
                }
                // 左上的地图块
                else if (towardsX == -step)
                {
                    SetNetworkTile(nowPos, tileLU);
                }
            }
        }

        private void SetNetworkTile(Vector3Int pos, Tile tile)
        {
            view.map.SetTile(pos, tile);
        }

        private bool HasTile(Vector3Int pos)
        {
            return view.map.HasTile(pos);
        }

        private void OnDestroy()
        {
            if (sceneManager != null)
            {
                sceneManager.gameOverSignal.RemoveListener(DieSignal);
            }
        }
    }
}