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
    List<Int2> list1 = new List<Int2>();
    List<Int2> list2 = new List<Int2>();
    List<Int2> list3 = new List<Int2>();

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
    
    private float evaluation(List<Int2> my_list, List<Int2> enemy_list)
    {
        float total_score = 0;

        return total_score;
    }
}
