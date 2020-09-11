using UnityEngine;
using UnityEngine.UI;
using NetworkView = Snake.Online.NetworkView;

namespace Player.Online
{
    public class SnakeName : NetworkView, IPunObservable
    {
        public Text playerName;
        

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                string name = PhotonNetwork.playerName;
                playerName.text = name;
                stream.SendNext(name);
            }
            else
            {
                playerName.text = (string) stream.ReceiveNext();
            }
        }
    }
}