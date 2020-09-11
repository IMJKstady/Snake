using System;

namespace Snake.UI {

    public interface IUiAnimation {

        void ShowUIAnima(Action action = null);

        void HideUIAnima(Action action = null);

        void ResetUIAnima(Action action = null);
    }
}