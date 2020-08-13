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
        
        List<int[]> score_all_arr = 0;

        float my_score = 0;
        foreach (var pt in my_list)
        {
            int m = pt.int1;
            int n = pt.int2;
            my_score += cal_score(m, n, 0, 1, enemy_list, my_list, score_all_arr);
            my_score += cal_score(m, n, 1, 0, enemy_list, my_list, score_all_arr);
            my_score += cal_score(m, n, 1, 1, enemy_list, my_list, score_all_arr);
            my_score += cal_score(m, n, -1, 1, enemy_list, my_list, score_all_arr);
        }

        float score_all_arr = 0;
        enemy_score = 0
    for pt in enemy_list:
        m = pt[0]
        n = pt[1]
        enemy_score += cal_score(m, n, 0, 1, my_list, enemy_list, score_all_arr_enemy)
        enemy_score += cal_score(m, n, 1, 0, my_list, enemy_list, score_all_arr_enemy)
        enemy_score += cal_score(m, n, 1, 1, my_list, enemy_list, score_all_arr_enemy)
        enemy_score += cal_score(m, n, -1, 1, my_list, enemy_list, score_all_arr_enemy)

    total_score = my_score - enemy_score * ratio * 0.1

    return total_score
    }
  

}
