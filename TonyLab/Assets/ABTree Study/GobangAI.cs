using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Personality
{
    Experimental,
    Calm,
    Impulsive,
    Careless,
    Conservative,
}


public class GoBangAI
{
    public int level;  //AI等级
    public Personality personality;

    private PlayerColor playerColor;

    private float randomAddDepth = 0;
    private float randomReduceDepth = 0;
    private float randomNotBest = 0;
    private float attackOrDeffend = 0.5f;
    

    public GoBangAI(PlayerColor playerColor, int level, Personality personality)
    {
        this.playerColor = playerColor;
        this.level = level;
        this.personality = personality;

        switch (personality)
        {
            default://未判断枚举则使用默认枚举
            case Personality.Calm:
                {

                }
                break;
            case Personality.Impulsive:
                break;
            case Personality.Careless:
                break;
            case Personality.Conservative:
                break;
        }
    }
}
