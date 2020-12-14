using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonsensicalKit;

public class RaycastManager : MonoBehaviour
{
    public static RaycastManager _Instance { get; private set; }

  [SerializeField]  private Camera camera;


  private  Dictionary<string, RaycastInfo> infos;

    private  void Awake()
    {
        _Instance = this;
    }
    

    public RaycastHit[] GetHits(string mask=null)
    {
        int crtCount = Time.frameCount;
        if (infos.ContainsKey(mask)&& infos[mask].FrameCount==crtCount)
        {
            return infos[mask].RaycastHits;
        }
        else
        {
            if (infos.ContainsKey(mask)==false)
            {
                infos.Add(mask,null);
            }
            infos[mask] = Check(mask);
            return infos[mask].RaycastHits;
        }
    }

    private RaycastInfo Check(string mask)
    {
        RaycastInfo raycastInfo = new RaycastInfo();
        raycastInfo.FrameCount = Time.frameCount;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (mask==null)
        {
            raycastInfo.RaycastHits = Physics.RaycastAll(ray, 100);
        }
        else
        {
            raycastInfo.RaycastHits = Physics.RaycastAll(ray, 100, LayerMask.GetMask(mask));
        }
        return raycastInfo;
    }
}

public class RaycastInfo
{
    public int FrameCount;
    public RaycastHit[] RaycastHits;


}
