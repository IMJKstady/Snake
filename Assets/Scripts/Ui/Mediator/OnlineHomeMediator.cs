
using System.Collections;
using Snake;
using Snake.Online;
using Snake.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Ui.Mediator
{
    // 局域网房间
    public class OnlineHomeMediator : UiMediator<OnlineHome>
    {
        [Inject]
        public OnlineManager onlineManager { get; set; }
        [Inject]
        public AudioManager audioManager { get; set; }
        
        public override void Init()
        {
            view.back.onClick.AddListener(Back);
            view.start.onClick.AddListener(OnStart);

            Refresh();
        }

        public override void Refresh()
        {
            StartCoroutine(Run());
        }

        private void OnStart()
        {
            if (PhotonNetwork.playerList.Length < 2) return;
            
            audioManager.PlayButtonEffect();
            onlineManager.StartGame();
        }

        public override void Back()
        {
            base.Back();
            audioManager.PlayButtonEffect();
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadSceneAsync("Menu");
            iuiregulator.Remove(id);
        }
        
        private IEnumerator Run()
        {
            while (true)
            {
                view.start.gameObject.SetActive(PhotonNetwork.isMasterClient);
                
                PhotonPlayer[] players = PhotonNetwork.playerList;
                int len = players.Length > 4 ? 4 : players.Length;
                
                for (int i = 0; i < len; i++)
                {
                    PhotonPlayer player = players[i];
                    view.playerText[i].text = player.NickName;
                }
                yield return new WaitForSeconds(2f);
            }
        }
    }
}