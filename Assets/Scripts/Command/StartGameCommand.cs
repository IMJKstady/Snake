using System;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Snake
{
    public class StartGameSignal : Signal<GameMode>
    {
    }

    public class StartGameCommand :Command
    {
        [Inject]
        public IEventDispatcher dispatcher { get; set; }

        public override void Execute()
        {
            // 获取游戏模式
            GameMode mode = (GameMode)((object[]) data)[0];

            switch (mode)
            {
                case GameMode.PvB:
                case GameMode.PvP:
                    PlayerPrefs.SetString("GameMode", mode.ToString());
                    SceneManager.LoadSceneAsync("PvP_PvB");
                    break;
                case GameMode.Online:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}