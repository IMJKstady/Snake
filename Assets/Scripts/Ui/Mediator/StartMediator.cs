using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Snake.UI
{
    public class StartMediator : UiMediator<Start>
    {
        [Inject]
        public AudioManager audioManager { get; set; }

        public override void Init()
        {
            Addressables.LoadSceneAsync("Menu");
            view.start.onClick.AddListener(ToStart);   
            view.setting.onClick.AddListener(Setting);   
            view.quit.onClick.AddListener(Quit);
            
            Refresh();
        }

        private void ToStart()
        {
            audioManager.PlayButtonEffect();
            Open(view.skip.id);
        }
        
        private void Setting()
        {
            audioManager.PlayButtonEffect();
            Open(view.settingSkip.id);
        }
        private void Quit()
        {            
            audioManager.PlayButtonEffect();
            Application.Quit();   
        }
    }
}

