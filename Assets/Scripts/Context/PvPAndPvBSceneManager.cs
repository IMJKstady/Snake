using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Snake
{
    public class PvPAndPvBSceneManager : View
    {
        [Serializable]
        class StartPoint
        {
            public Vector2 point;
            [HideInInspector]
            public bool isUse;
        }
        
        [Inject]
        public BackToMenuSingal backToMenuSingal { get; set; }
        [Inject]
        public AudioManager audioManager { get; set; }
        
        [SerializeField] 
        private AssetReference map;
    
        [SerializeField]
        private List<StartPoint> startPoint;
        
        // 玩家Prefab
        [SerializeField]
        private AssetReference p1;
        [SerializeField]
        private AssetReference p2;
        [SerializeField]
        private AssetReference ai;
        
        // 结束面板
        public Transform Tootle;
        public Text winName;
        public Button again;
        public Button back;
        
        // 玩家实例
        private SnakePlayer[] players;
        private Tilemap drawMap;    // 填图的地图
        private GameMode gameMode;  // 游戏模式
        private int aiLevel;        // 人机难度
        
        private void Start()
        {
            base.Start();
            map.InstantiateAsync().Completed += handle =>
            {
                if (!handle.IsDone) return;
                drawMap = handle.Result.transform.Find("draw").GetComponent<Tilemap>();
                gameMode = (GameMode)Enum.Parse(typeof(GameMode), PlayerPrefs.GetString("GameMode"));
                aiLevel = PlayerPrefs.GetInt("aiLevel");
                GameStart();
            };
            again.onClick.AddListener(PlayerAgain);
            back.onClick.AddListener(Back);
        }

        private void CreatePlayer()
        {
            players = new SnakePlayer[2];
            p1.InstantiateAsync().Completed += handle =>
            {
                if (!handle.IsDone) return;
                SnakePlayer player = handle.Result.GetComponent<SnakePlayer>();
                // 初始数据
                player.transform.position = GetStartPoint();
                player.tilemap = drawMap;
                player.gameMode = gameMode;
                player.gameObject.SetActive(true);
                players[0] = player;
            };
            
            switch (gameMode)
            {
                case GameMode.PvB:
                    ai.InstantiateAsync().Completed += handle =>
                    {
                        if (!handle.IsDone) return;
                        SnakePlayer player = handle.Result.GetComponent<SnakePlayer>();
                        // 设置难度
                        PlayerAi playerAi = handle.Result.GetComponent<PlayerAi>();
                        playerAi.level = aiLevel;

                        // 初始数据
                        player.transform.position = new Vector3(0.5f, 0.5f, 0);
                        player.tilemap = drawMap;
                        player.gameMode = gameMode;
                        player.gameObject.SetActive(true);
                        players[1] = player;
                    };
                    break;
                case GameMode.PvP:
                    p2.InstantiateAsync().Completed += handle =>
                    {
                        if (!handle.IsDone) return;
                        SnakePlayer player = handle.Result.GetComponent<SnakePlayer>();
                        // 初始数据
                        player.transform.position = GetStartPoint();
                        player.tilemap = drawMap;
                        player.gameMode = gameMode;
                        player.gameObject.SetActive(true);
                        players[1] = player;
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }

        // 开始游戏
        public void GameStart()
        {
            CreatePlayer();
            audioManager.PlayBgm();
        }

        // 游戏结束
        public void GameOver()
        {
            audioManager.StopBgm();
            audioManager.PlayVictory();
            
            Tootle.gameObject.SetActive(true);
            winName.text = players[0].isDie ? players[1].playerName + "  victory" : players[0].playerName + "  victory";
            ResetStartPoint(); // 重置出生点 
            for (int i = 0; i < players .Length; i++)
            {
                Addressables.ReleaseInstance(players[i].gameObject);
            }
        }

        private void PlayerAgain()
        {
            audioManager.PlayButtonEffect();
            drawMap.ClearAllTiles();
            GameStart();
            winName.text = "";
            Tootle.gameObject.SetActive(false);
        }

        // 获取出生点
        private Vector3 GetStartPoint()
        {
            for (int i = startPoint.Count - 1; i >= 0; i--)
            {
                if (!startPoint[i].isUse)
                {
                    startPoint[i].isUse = true;
                    return startPoint[i].point;
                }
            }

            return Vector3.zero;
        }
        
        private void ResetStartPoint()
        {
            for (int i = startPoint.Count - 1; i >= 0; i--)
            {
                startPoint[i].isUse = false;
            }
        }

        private void Back()
        {
            audioManager.StopVictory();
            Tootle.gameObject.SetActive(false);
            backToMenuSingal.Dispatch();
            audioManager.PlayButtonEffect();
        }
    }
}