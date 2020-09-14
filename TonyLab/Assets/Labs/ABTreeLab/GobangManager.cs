using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerColor
{
    White,
    Balck,
}

public struct Int2
{
    public int int1;
    public int int2;
}

public class GobangManager
{

    Dictionary<int, int[]> shape_score = new Dictionary<int, int[]>
    {
        { 1,new int[]{ 0, 1, 1, 0, 0 } },
        { 1,new int[]{ 0, 0, 1, 1, 0 } },
        { 4,new int[]{ 1, 1, 0, 1, 0 } },
       { 4,new int[]{ 0, 1, 0, 1, 1 } },
       { 10,new int[]{ 0, 0, 1, 1, 1 } },
       { 10,new int[]{ 1, 1, 1, 0, 0 } },
       { 100,new int[]{ 0, 1, 1, 1, 0 } },
       { 100,new int[]{ 0, 1, 0, 1, 1, 0 } },
       { 100,new int[]{ 0, 1, 1, 0, 1, 0 } },
       { 100,new int[]{ 1, 1, 1, 0, 1 } },
       { 100,new int[]{ 1, 1, 0, 1, 1 } },
       { 100,new int[]{ 1, 0, 1, 1, 1 } },
       { 100,new int[]{ 1, 1, 1, 1, 0 } },
       { 100,new int[]{ 0, 1, 1, 1, 1 } },
       { 1000,new int[]{ 0, 1, 1, 1, 1, 0 } },
       { 999999,new int[]{ 1, 1, 1, 1, 1 } },
    };

   
}
