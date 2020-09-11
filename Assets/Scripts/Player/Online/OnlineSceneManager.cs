using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Photon;
using Snake;
using Snake.Online;
using Snake.UI;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets._2D;
using Random = UnityEngine.Random;

namespace Snake.Online
{
    public class OnlineSceneManager : NetworkView
    {
        [Inject]
        public IUiRegulator uiRegulator { get; set; }
        [Inject]
        public AudioManager audioManager { get; set; }

        [Inject]
        public BackToMenuSingal backToMenuSingal { get; set; }

        public Signal gameOverSignal = new Signal();

        public GameObject uiRoot;
        public Button roomView;
        public ViewBase waitUI;
        public PhotonView photonView;
        public Camera2DFollow Camera;
        public OnlineMap map;
        public Text[] playerText;

        [Space] 
        public GameObject menuRoot;
        public Text winText;
        public Button backRoom;

        [Space]
        public Vector2 mapSizeX;
        public Vector2 mapSizeY;
        public Color[] allPlayerColor = new Color[4];
        
        private Dictionary<string, bool> playerState = new Dictionary<string, bool>();
        
        protected override void Start()
        {
            base.Start();
            roomView.onClick.AddListener(() =>
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.JoinLobby();
                audioManager.StopVictory();
                backToMenuSingal.Dispatch();
            });
            backRoom.onClick.AddListener(() =>
            {
                uiRoot.gameObject.SetActive(false);
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.JoinLobby();
                audioManager.StopVictory();
                backToMenuSingal.Dispatch();
            });
        }

        [PunRPC]
        public void StartGame()
        {
            uiRegulator.Close(waitUI.id);

            PhotonPlayer[] players = PhotonNetwork.playerList;
            // 初始玩家状态
            for (int i = players.Length - 1; i >= 0; i--)
            {
                playerState.Add(players[i].NickName, false);
            }
            uiRoot.SetActive(true);
            audioManager.PlayBgm();
            CreatePlayerObject();
            
            StartCoroutine(ShowPlayerState());
        }

        [PunRPC]
        public void GameOver(string winName)
        {
            gameOverSignal.Dispatch();
            uiRegulator.Close(waitUI.id);
            menuRoot.SetActive(true);
            audioManager.PlayVictory();

            winText.text = winName + "\t victory";
        }
        
        [PunRPC]
        public void PlayerDie(string palyerName)
        {
            playerState[palyerName] = true;
        }

        public void PlayerDeath(string palyerName)
        {
            photonView.RPC("PlayerDie", PhotonTargets.All, palyerName);
            uiRegulator.Open(waitUI.id);
        }

        public override void OnLeftRoom()
        {
            Debug.Log("离开房间");
        }
        private void CreatePlayerObject()
        {
            // 随机生成位置
            int x = (int)Random.Range(mapSizeX.x + 5, mapSizeX.y - 5);
            int Y = (int)Random.Range(mapSizeY.x + 5, mapSizeY.y - 5);
            Vector3 pos = new Vector3(x - 0.5f, Y - 0.5f, 0);
            
            GameObject newPlayerObject = PhotonNetwork.Instantiate( "OnlinePlayer", pos, Quaternion.identity, 0 );
            
            SnakeNetworkPlayer player = newPlayerObject.GetComponent<SnakeNetworkPlayer>();
            // 随机颜色
            int collor = Random.Range(0, allPlayerColor.Length - 1);
            player.tileColor = allPlayerColor[collor];

            Camera.target = newPlayerObject.transform;
            Camera.gameObject.SetActive(true);
        }
        
        public override void OnConnectionFail(DisconnectCause cause)
        {
            audioManager.StopBgm();
        }
        
        private IEnumerator ShowPlayerState()
        {
            while (true)
            {
                int die = 0;
                int i = 0;
                string savenaem = "";
                foreach (var it in playerState)
                {
                    if (it.Value)
                    {
                        playerText[i].text = it.Key + "\t---  死亡";
                        die++;
                    }
                    else
                    {
                        savenaem = it.Key;
                        playerText[i].text = it.Key + "\t---  存活";
                    }
                    i++;
                }

                if (die == playerState.Count - 1)
                {
                    photonView.RPC("GameOver", PhotonTargets.All, savenaem);
                    yield break;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}