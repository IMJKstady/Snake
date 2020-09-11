using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine.AddressableAssets;

namespace Snake
{
    public class BackToMenuSingal : Signal
    {
    }
    
    public class BackToMenuCommand : Command
    {
        [Inject]
        public IUiRegulator uiRegulator { get; set; }
        public override void Execute()
        {
            Addressables.LoadSceneAsync("Menu").Completed += handle =>
            {
                if (!handle.IsDone) return;

                uiRegulator.Open("start");
            };
        }
    }
}