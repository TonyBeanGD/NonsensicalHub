//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public struct Int3
//{
//    public int X;
//    public int Y;
//    public int Z;

//    public Int3(int _x, int _y, int _z)
//    {
//        X = _x;
//        Y = _y;
//        Z = _z;
//    }

//    public override bool Equals(object obj)
//    {
//        if (!(obj is Int3))
//        {
//            return false;
//        }

//        var @int = (Int3)obj;
//        return X == @int.X &&
//               Y == @int.Y &&
//               Z == @int.Z;
//    }

//    public override int GetHashCode()
//    {
//        var hashCode = -307843816;
//        hashCode = hashCode * -1521134295 + X.GetHashCode();
//        hashCode = hashCode * -1521134295 + Y.GetHashCode();
//        hashCode = hashCode * -1521134295 + Z.GetHashCode();
//        return hashCode;
//    }
//}

//[System.Serializable]
//public class AreaObject
//{
//    public List<Int3> points;
//}

//public class ColliderMountPointGroup : MonoBehaviour
//{
//    private List<Int3> stayPoints;

//    private void Awake()
//    {
//        stayPoints = new List<Int3>();
//    }

//    private void Start()
//    {
//        ColliderMountPoint[] points = transform.GetComponentsInChildren<ColliderMountPoint>();

//        foreach (var item in points)
//        {
//            item.SetGroup(this);
//        }
//    }

//    public void JoinGroup(ColliderMountPoint colliderMountPoint)
//    {
//        colliderMountPoint.SetGroup(this);
//    }

//    public bool GroupCheck(AreaObject areaObject)
//    {
//        foreach (var item in areaObject.points)
//        {
//            if (HasPoint(stayPoints, item) == true)
//            {
//                return false;
//            }

//            if (item.Y > 0)
//            {
//                Int3 temp = new Int3(item.X, item.Y - 1, item.Z);

//                if (HasPoint(stayPoints, temp) == false && HasPoint(areaObject.points, temp) == false)
//                {
//                    return false;
//                }
//            }
//        }

//        return true;
//    }

//    private bool HasPoint(List<Int3> points, Int3 checkPoint)
//    {
//        foreach (var point in points)
//        {
//            if (point.Equals(checkPoint) == true)
//            {
//                return true;
//            }
//        }

//        return false;
//    }

//    public void SetAreaObject(AreaObject areaObject)
//    {
//        foreach (var item in areaObject.points)
//        {
//            stayPoints.Add(item);
//        }
//    }

//    public void GetAreaObject(AreaObject areaObject)
//    {
//        foreach (var item in areaObject.points)
//        {
//            stayPoints.Remove(item);
//        }
//    }
//}
