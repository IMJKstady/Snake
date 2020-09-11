using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Snake
{
    public class SnakePlayer : View
    {
        [HideInInspector]
        public bool isDie = false;
        public string playerName;
        [HideInInspector]
        public bool isFirstTile = true;
        
        public GameMode gameMode;
        public SpriteRenderer body;
        public Text showName;
        public Tilemap tilemap;
        public Color tileColor;
    }
}