using System;
using System.Collections;
using System.Collections.Generic;
using Snake;
using Snake.Online;
using Snake.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MonoBehaviour = Photon.MonoBehaviour;

namespace Ui.Mediator
{
    
    // 选择局域网房间加入
    public class OnlineHomeSelectMediator : UiMediator<OnlineHomeSelect>
    {
        [Inject]
        public OnlineManager onlineManager { get; set; }
        [Inject]
        public AudioManager audioManager { get; set; }

        private Dictionary<string, RoomText> roomTexts = new Dictionary<string, RoomText>();
        private ToggleGroup toggleGroup;
        private RoomInfo select;

        public override void Init()
        {
            toggleGroup = view.scrollRect.content.GetComponent<ToggleGroup>();
            view.back.onClick.AddListener(Back);
            view.join.onClick.AddListener(Join);
            Refresh();
        }

        public override void Refresh()
        {
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (true)
            {
                // 获取房间
                RoomInfo[] newRoom = PhotonNetwork.GetRoomList();
                
                foreach (var rom in newRoom)
                {
                    Debug.Log(rom.Name);
                }
                
                Transform context = view.scrollRect.content;
                // 删除没有的
                foreach (var lodroom in roomTexts)
                {
                    Destroy(lodroom.Value.gameObject);
                }
                roomTexts = new Dictionary<string, RoomText>();
                
                // 加入新的
                foreach (var roon in newRoom)
                {
                    if (!roomTexts.ContainsKey(roon.Name))
                    {
                        GameObject toggle = Instantiate(view.severToggle.gameObject, context);
                        RoomText roomText = toggle.GetComponent<RoomText>();
                        roomText.info = roon;
                        roomText.text.text = roon.Name;
                        roomText.toggle.onValueChanged.AddListener(OnChange);
                        roomTexts.Add(roon.name, roomText);
                        toggle.SetActive(true);
                    }
                }
                yield return new WaitForSeconds(4f);
            }
        }
        
        private void Join()
        {
            if (select == null) return;

            audioManager.PlayButtonEffect();
            SceneManager.LoadSceneAsync("Online").completed += operation =>
            {
                if (!operation.isDone) return;
                onlineManager.JoinRoom(@select.Name);
                Open(view.skipView.id);
            };
        }

        public override void Back()
        {
            base.Back();
            audioManager.PlayButtonEffect();
            iuiregulator.Remove(id);
        }

        private void OnChange(bool b)
        {
            if (!b) return;
            foreach (var it2 in  toggleGroup.ActiveToggles())
            {
                if (it2.isOn)
                {
                    select = it2.GetComponent<RoomText>().info;
                }
            }
        }
    }
}