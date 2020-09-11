using DefaultNamespace;
using Snake.Online;
using Snake.UI;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using Ui.Mediator;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Snake {

    public class RootContext : SignalsContext {

        public RootContext(MonoBehaviour view) : base(view) {
        }


        public RootContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) {
        }

        protected override void mapBindings() {
            base.mapBindings();

            MannagerBind();
            CommandBind();
            ViewBind();
            
            injectionBinder.Bind<PvPAndPvBSceneManager>().ToSingleton();
        }

        // 控制器绑定
        private void MannagerBind()
        {
            var leader = contextView as GameObject;
            if (leader != null)
            {
                injectionBinder.Bind<Leader>().ToSingleton().CrossContext()
                    .ToValue(leader.GetComponent<Leader>());
                injectionBinder.Bind<IUiRegulator>().ToSingleton().CrossContext()
                    .ToValue(leader.GetComponentInChildren<UiManager>());
                injectionBinder.Bind<OnlineManager>().ToSingleton().CrossContext()
                    .ToValue(leader.GetComponentInChildren<OnlineManager>());
                injectionBinder.Bind<AudioManager>().ToSingleton().CrossContext()
                    .ToValue(leader.GetComponentInChildren<AudioManager>());
            }
            
        }

        // 命令绑定
        private void CommandBind()
        {
            commandBinder.Bind<StartGameSignal>().To<StartGameCommand>();
            commandBinder.Bind<GameOverSignal>().To<GameOverCommand>();
            commandBinder.Bind<BackToMenuSingal>().To<BackToMenuCommand>();
        }

        // 视图绑定
        private void ViewBind()
        {
            mediationBinder.Bind<SnakePlayer>().To<SnakePlayerMediator>();
            mediationBinder.Bind<SnakeNetworkPlayer>().To<SnakeNetworkMediator>();
            
            mediationBinder.Bind<EnterPlayerName>().To<EnterPlayerNameMediator>();
            mediationBinder.Bind<Start>().To<StartMediator>();
            mediationBinder.Bind<Setting>().To<SettingMediator>();
            mediationBinder.Bind<SelectGame>().To<SelectGameMediator>();
            mediationBinder.Bind<PlayWithBot>().To<PlayWithBotMediator>();
            mediationBinder.Bind<OnlineModeSelect>().To<OnlineModeSelectMediator>();
            mediationBinder.Bind<OnlineHome>().To<OnlineHomeMediator>();
            mediationBinder.Bind<OnlineHomeSelect>().To<OnlineHomeSelectMediator>();
        }
    }
}