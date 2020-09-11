
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Snake.UI
{
    public class OnlineHome : ViewBase
    {
        public Button start;
        public Button back;
        public ScrollRect scrollView;
        public Text[] playerText;
    }
}