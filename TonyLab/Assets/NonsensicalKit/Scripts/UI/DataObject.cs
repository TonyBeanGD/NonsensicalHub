using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class DataObject<T> :IData where T:IData,new ()
{

    public List<IData> Childs {  get; private set; } =new List<IData>();

    public int Count { get { return Childs.Count; } }
    public List<IData> GetChilds() 
    {
        return Childs;
    }

    public void AddNew()
    {
        Childs.Add(new T());
    }

    public void RemoveLast()
    {
        if (Count>0)
        {
            Childs.RemoveAt(Count - 1);
        }
    }

    public void Top(IData data)
    {
        Childs.Remove(data);

        Childs.Insert(0, data);
    }

    public IData this[int index]
    {
        get
        {
            if (index< Childs.Count)
            {
                return Childs[index];
            }
            else
            {
                return null;
            }
        }
    }
}
