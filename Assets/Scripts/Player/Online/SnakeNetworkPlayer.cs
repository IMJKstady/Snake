using Snake;
using Snake.Online;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using NetworkView = Snake.Online.NetworkView;

namespace DefaultNamespace
{
    public class SnakeNetworkPlayer : NetworkView, IPunObservable
    {
        [HideInInspector]
        public bool isDie = false;
        public int speed = 2;
        public int step;
        public GameMode gameMode;
        public SpriteRenderer body;
        public Text showName;
        public OnlineMap map;
        public Color tileColor;
        [HideInInspector]
        public Camera main;

        protected override void Start()
        {
            base.Start();
            main = Camera.main;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(JsonUtility.ToJson(tileColor));
                stream.SendNext(transform.position);
            }
            else
            {
                tileColor = JsonUtility.FromJson<Color>((string) stream.ReceiveNext());
                if (main != null)
                {
                    Vector3 pos = (Vector3) stream.ReceiveNext();
                    showName.transform.position = main.WorldToScreenPoint(pos + new Vector3(1f, 1f, 01));
                }
            }
        }
    }
}