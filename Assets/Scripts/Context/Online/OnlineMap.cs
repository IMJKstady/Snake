
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

namespace Snake.Online
{
    public class OnlineMap : NetworkView
    {
        private Sprite sprite_h;
        private Sprite sprite_v;
        private Sprite sprite_lu;
        private Sprite sprite_ld;
        private Sprite sprite_ru;
        private Sprite sprite_rd;
        private Sprite sprite_tr;
        
        [Serializable]
        class TileData
        {
            public Vector3Int pos;
            public string spriteName;
            public Color spriteColor;
        }
        
        public Tilemap draw;
        private Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile> ();

        private void Start()
        {
            draw = transform.Find("draw").GetComponent<Tilemap>();
            StartCoroutine(Init());
        }
        
        private IEnumerator Init()
        {
            AsyncOperationHandle<Sprite> horize = Addressables.LoadAssetAsync<Sprite>("sprite_h");
            AsyncOperationHandle<Sprite> vertical = Addressables.LoadAssetAsync<Sprite>("sprite_v");
            AsyncOperationHandle<Sprite> lu = Addressables.LoadAssetAsync<Sprite>("sprite_lu");
            AsyncOperationHandle<Sprite> ld = Addressables.LoadAssetAsync<Sprite>("sprite_ld");
            AsyncOperationHandle<Sprite> ru = Addressables.LoadAssetAsync<Sprite>("sprite_ru");
            AsyncOperationHandle<Sprite> rd = Addressables.LoadAssetAsync<Sprite>("sprite_rd");
            AsyncOperationHandle<Sprite> tr = Addressables.LoadAssetAsync<Sprite>("sprite_tr");
            
            yield return horize;
            yield return vertical;
            yield return lu;
            yield return ld;
            yield return ru;
            yield return rd;
            yield return tr;
            
            // 设置图片
            sprite_h = horize.Result;
            sprite_v = vertical.Result;
            sprite_lu = lu.Result;
            sprite_ld = ld.Result;
            sprite_ru = ru.Result;
            sprite_rd = rd.Result;
            sprite_tr = tr.Result;
        }

        public void ClearTile()
        {
            tiles.Clear();
            draw.ClearAllTiles();
        }

        public void SetTile(Vector3Int pos, Tile tile)
        {
            if (HasTile(pos)) return;
            TileData data = new TileData();
            data.pos = pos;
            data.spriteName = tile.sprite.name;
            data.spriteColor = tile.color;
            photonView.RPC("SetOnlineTile", PhotonTargets.All, JsonUtility.ToJson(data));
        }
        
        public bool HasTile(Vector3Int pos)
        {
            return tiles.ContainsKey(pos);
        }
        
        [PunRPC]
        public void SetOnlineTile(string data)
        {
            TileData tileData = JsonUtility.FromJson<TileData>(data);
            Tile tile = CreteTile(tileData);
            if (!tiles.ContainsKey(tileData.pos))
            {
                tiles.Add(tileData.pos, tile);
            }
            draw.SetTile(tileData.pos, tile);
        }

        /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                string[] datas  = new string[tiles.Count];
                int i = 0;
                foreach (var it in tiles)
                {
                    TileData data = new TileData();
                    data.pos = it.Key;
                    data.spriteName = it.Value.sprite.name;
                    data.spriteColor = it.Value.color;
                    
                    datas[i] = JsonUtility.ToJson(data);
                    i++;
                }
                stream.SendNext(datas);
            }
            else
            {
                string[] data = (string[]) stream.ReceiveNext();
                foreach (var it in data)
                {
                    TileData tileData = JsonUtility.FromJson<TileData>(it);
                    if(HasTile(tileData.pos)) continue;
                    CreteTile(tileData);
                }
            }
        }*/

        private Tile CreteTile(TileData data)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            switch (data.spriteName)
            {
                case "h":
                    tile.sprite = sprite_h;
                    break;
                case "v":
                    tile.sprite = sprite_v;
                    break;
                case "lu":
                    tile.sprite = sprite_lu;
                    break;
                case "ld":
                    tile.sprite = sprite_ld;
                    break;
                case "ru":
                    tile.sprite = sprite_ru;
                    break;
                case "rd":
                    tile.sprite = sprite_rd;
                    break;
                case "tr":
                    tile.sprite = sprite_tr;
                    break;
            }
            tile.color = data.spriteColor;
            return tile;
        }
    }
}