using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Snake.Online
{
    public class OnlineManager : NetworkView, IRegulator
    {
        public IUiRegulator uiRegulator { get; set; }

        public bool autoJoinLobby = false;
        public bool isConnect;
        public bool inRoom;
        public string Version;
        public GameObject loadParent;
        public Text showMsg;
        
        private bool isConnectMaster = false;
        private bool creatingRoom = false;
        private bool joiningRoom = false;
        
        public OnlineSceneManager sceneManager;

        public void Init()
        {
        }
        
        private void Update()
        {
            inRoom = PhotonNetwork.inRoom;
            isConnect = PhotonNetwork.connected;
            PhotonNetwork.autoJoinLobby = autoJoinLobby;
        }

        public void Connect(Action<bool> callback)
        {
            if (!PhotonNetwork.connected)
            {
                StartCoroutine(ConnectRun(callback));
            }
            else
            {
                callback.Invoke(true);
            }
        }
        public void CreateRoom(string name)
        { 
           StartCoroutine(CreateRomRun(name));
        }
        public void JoinRoom(string name)
        {
            StartCoroutine(JoinRoomRun(name));
        }

        public void StartGame()
        {
            sceneManager = GameObject.Find("Manager").GetComponent<OnlineSceneManager>();
            sceneManager.photonView.RPC("StartGame", PhotonTargets.All);
        }

        private IEnumerator ConnectRun(Action<bool> callback)
        {
            loadParent.SetActive(true);
            PhotonNetwork.ConnectUsingSettings(Version);
            isConnectMaster = false;
            while (!isConnectMaster)
            {
                showMsg.text = PhotonNetwork.connectionStateDetailed.ToString();
                yield return new WaitForSeconds(2);
            }

            if (PhotonNetwork.connectionState == ConnectionState.Connected)
            {
                PhotonNetwork.player.NickName = PlayerPrefs.GetString("playerName");
                callback.Invoke(true);
            }
            else
            {
                callback.Invoke(false);
            }
            
            showMsg.text = "";
            loadParent.SetActive(false);
        }

        private IEnumerator CreateRomRun(string name)
        {
            creatingRoom = false;
            joiningRoom = false;
            loadParent.SetActive(true);
            PhotonNetwork.CreateRoom(name, new RoomOptions(){MaxPlayers = 4}, new TypedLobby());
            showMsg.text = "Creating....";
            while (!creatingRoom && !joiningRoom)
            {
                yield return new WaitForSeconds(2);
            }
            showMsg.text = "";
            loadParent.SetActive(false);
        }
        
        private IEnumerator JoinRoomRun(string name)
        {
            joiningRoom = false;
            loadParent.SetActive(true);
            PhotonNetwork.JoinRoom(name);
            showMsg.text = "Joining....";
            while (!joiningRoom)
            {
                yield return new WaitForSeconds(2);
            }
            showMsg.text = "";
            loadParent.SetActive(false);
        }

        public override void OnConnectedToMaster()
        {
            isConnectMaster = true;
            Debug.Log("连接成功");
        }

        public override void OnJoinedLobby()
        {
            isConnectMaster = true;
            Debug.Log("加入大厅");
        }

        public override void OnCreatedRoom()
        {
            creatingRoom = true;
            Debug.Log("创建房间");
        }
        
        public override void OnJoinedRoom()
        {
            joiningRoom = true;
            Debug.Log("加入房间");
        }

        public override void OnLeftLobby()
        {
            Debug.Log("离开大厅");
        }

        public override void OnLeftRoom()
        {
        }
        public override void OnConnectionFail(DisconnectCause cause)
        {
            Debug.LogError(cause);
            SceneManager.LoadSceneAsync("Menu").completed += operation =>
            {
                if(!operation.isDone) return;
                uiRegulator.Clear();
                uiRegulator.Open("start");
            };
        }
        
        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
            Debug.Log("No random room available, so we create one.");
            SceneManager.LoadSceneAsync("Menu").completed += operation =>
            {
                if(!operation.isDone) return;
                uiRegulator.Clear();
                uiRegulator.Open("start");
            };
        }
        
        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            Debug.LogError("Cause: " + cause);
            SceneManager.LoadSceneAsync("Menu").completed += operation =>
            {
                if(!operation.isDone) return;
                uiRegulator.Clear();
                uiRegulator.Open("start");
            };
        }
    }
}