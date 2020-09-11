using Snake.UI;
using UnityEngine;

namespace Ui.Mediator
{
    public class EnterPlayerNameMediator : UiMediator<EnterPlayerName>
    {
        public override void Init()
        {
            view.inputField.onEndEdit.AddListener(EndEdit);
        }

        private void EndEdit(string name)
        {
            if (name == "") return;
            PlayerPrefs.SetString("playerName", name);
            Open(view.skip.id);
        }
    }
}