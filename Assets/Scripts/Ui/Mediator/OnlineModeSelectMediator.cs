using Snake;
using Snake.Online;
using Snake.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Ui.Mediator
{
    public class OnlineModeSelectMediator : UiMediator<OnlineModeSelect>
    {
        [Inject]
        public OnlineManager onlineManager { get; set; }
        [Inject]
        public AudioManager audioManager { get; set; }
        
        public override void Init()
        {
            view.create.onClick.AddListener(Create);
            view.join.onClick.AddListener(Join);
            view.back.onClick.AddListener(Back);
        }

        private void Create()
        {
            audioManager.PlayButtonEffect();
            SceneManager.LoadSceneAsync("Online").completed += operation =>
            {
                if (!operation.isDone) return;
                onlineManager.CreateRoom(PlayerPrefs.GetString("playerName"));
                Open(view.createSkip.id);
            };
        }

        private void Join()
        {
            audioManager.PlayButtonEffect();
            Open(view.joinSkip.id);
        }

        public override void Back()
        {
            base.Back();
            audioManager.PlayButtonEffect();
        }
    }
}