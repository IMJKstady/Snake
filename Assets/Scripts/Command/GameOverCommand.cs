using System;
using Snake.Online;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Snake
{
    public class GameOverSignal : Signal<GameMode>
    {
    }

    public class GameOverCommand : Command
    {


        public override void Execute()
        {
            // 获取游戏模式
            GameMode mode = (GameMode)((object[]) data)[0];

            switch (mode)
            {
                case GameMode.PvB:
                case GameMode.PvP:
                    PvPAndPvBSceneManager andPvBSceneManager = GameObject.Find("SceneManager").GetComponent<PvPAndPvBSceneManager>();
                    andPvBSceneManager.GameOver();
                    break;
                case GameMode.Online:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}