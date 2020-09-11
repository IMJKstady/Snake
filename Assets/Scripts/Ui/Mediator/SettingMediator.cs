using Snake;
using Snake.UI;

namespace Ui.Mediator
{
    public class SettingMediator : UiMediator<Setting>
    {
        [Inject]
        public AudioManager audioManager { get; set; }

        public override void Init()
        {
            view.bgm.value = audioManager.GetSoundBGM();
            view.effect.value = audioManager.GetSoundEffect();
            
            view.bgm.onValueChanged.AddListener(v =>
            {
                audioManager.SetSoundBGM(v);
            });
            
            view.effect.onValueChanged.AddListener(v =>
            {
                audioManager.SetSoundEffect(v);
            });
            
            view.back.onClick.AddListener(Back);
        }

        public override void Back()
        {
            base.Back();
            audioManager.PlayButtonEffect();
        }
    }
}