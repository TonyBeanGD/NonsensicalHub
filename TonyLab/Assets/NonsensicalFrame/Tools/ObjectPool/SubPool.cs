using UnityEngine;
using System.Collections.Generic;

namespace NonsensicalKit
{
    public class SubPool
    {
        private List<GameObject> _objects = new List<GameObject>();

        private GameObject _prefab;

        private Transform _parentT;

        public string name
        {
            get
            {
                return _prefab.name;
            }
        }

        public SubPool(Transform parent, GameObject go)
        {
            _prefab = go;
            _parentT = parent;
        }

        public GameObject Spawn()
        {
            GameObject go = null;
            foreach (var item in _objects)
            {
                if (item.activeSelf != true)
                {
                    go = item;
                    break;
                }
            }
            if (go == null)
            {
                go = GameObject.Instantiate(_prefab, _parentT);
                _objects.Add(go);
            }

            go.SetActive(true);

            go.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
            return go;
        }

        public void UnSpawn(GameObject go)
        {
            if (ContainGo(go))
            {
                go.SetActive(false);
                go.SendMessage("OnUnSpawn", SendMessageOptions.DontRequireReceiver);
            }
        }

        public bool ContainGo(GameObject go)
        {
            return _objects.Contains(go);
        }

        public void UnSpawnAll()
        {
            foreach (var item in _objects)
            {
                if (item && item.activeSelf)
                {
                    item.SetActive(false);
                    item.SendMessage("OnUnSpawn", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

}
