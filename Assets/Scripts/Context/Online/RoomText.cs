using UnityEngine;
using UnityEngine.UI;

namespace Snake.Online
{
    public class RoomText : MonoBehaviour
    {
        public Toggle toggle;
        public Text text;
        [HideInInspector]
        public RoomInfo info;
    }
}