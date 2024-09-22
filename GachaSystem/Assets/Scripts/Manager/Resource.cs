using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Manager
{
    public class Resource : Util.Inherited.Singleton<Resource>
    {
        private Dictionary<string, SpriteAtlas> atlasDB;


        private readonly string atlasPath = "Atlas/";

        private void Start()
        {
            Load();
        }

        private void Load()
        {
            var loadAtlas = Resources.LoadAll<SpriteAtlas>(atlasPath);

            atlasDB = new Dictionary<string, SpriteAtlas>(loadAtlas.Length);

            foreach (var item in loadAtlas)
            {
                if (atlasDB.ContainsKey(item.name))
                {
                    Debug.LogError($"{item.name} Atlas가 이미 존재합니다.\n");
                    continue;
                }

                atlasDB.Add(item.name, item);
            }
        }
        public Sprite GetSprite(string atlas, string name)
        {
            if(atlasDB.ContainsKey(atlas))
            {
                return atlasDB[atlas].GetSprite(name);
            }

            return null;
        }
    }
}
