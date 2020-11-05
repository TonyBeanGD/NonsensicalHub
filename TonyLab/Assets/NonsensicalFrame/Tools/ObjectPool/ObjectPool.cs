using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace NonsensicalKit
{
    public class ObjectPool : MonoSingleton<ObjectPool>
    {
        //对象资源文件夹路径
        public string objectResourceDir = "Prefab";
        //对象池字典,保存所有子对象池
        private Dictionary<string, SubPool> _pools = new Dictionary<string, SubPool>();

        //取出对象
        public GameObject Spawn(string objName, Transform parent)
        {
            //SubPool pool = null;
            if (!_pools.ContainsKey(objName))
            {
                RegisterPool(objName, parent);
            }
            return _pools[objName].Spawn();
        }
        //注册一个新的子对象池
        private void RegisterPool(string objName, Transform parent)
        {
            //获取预制物资源
            string path = objectResourceDir + "/" + objName;
            GameObject prefab = Resources.Load<GameObject>(path);
            //创建子对象池
            SubPool pool = new SubPool(parent, prefab);
            //注册进字典
            _pools.Add(objName, pool);
        }
        //回收物体
        public void UnSpawn(GameObject go)
        {
            foreach (var item in _pools.Values)
            {
                if (item.ContainGo(go))
                {
                    item.UnSpawn(go);
                    break;
                }
            }
        }
        //回收所有对象
        public void UnSpawnAll()
        {
            foreach (var item in _pools.Values)
            {
                item.UnSpawnAll();
            }
        }
        //清空对象池
        public void Clear()
        {
            UnSpawnAll();
            _pools.Clear();
        }
    }

}
