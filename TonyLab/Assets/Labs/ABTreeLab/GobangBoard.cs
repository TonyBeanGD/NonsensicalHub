using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum LatticeType
{
    White,
    Black,
    None,
}

public class GobangBoard
{
    public LatticeType[] board;
    public readonly int width;
    public readonly int height;

    private Dictionary<int[], int> powerTable;

    public GobangBoard() : this(15, 15) { }

    public GobangBoard(int width, int height)
    {
        this.width = width;
        this.height = height;
        board = new LatticeType[width * height];
    }

    public float GetTotalPower()
    {
        float totalPower = 0;

        return totalPower;
    }

    public bool CheckWin(bool isWhite)
    {
        LatticeType checkType = isWhite ? LatticeType.White : LatticeType.Black;
        for (int i = 0; i <= 10; i++)
        {
            for (int j = 0; j <= 10; j++)
            {
                if (GetLatticeType(i, j) == checkType)
                {
                    if (GetLatticeType(i + 1, j) == checkType && GetLatticeType(i + 2, j) == checkType && GetLatticeType(i + 3, j) == checkType && GetLatticeType(i + 4, j) == checkType)//右
                    {
                        return true;
                    }
                    if (GetLatticeType(i, j + 1) == checkType && GetLatticeType(i, j + 2) == checkType && GetLatticeType(i, j + 3) == checkType && GetLatticeType(i, j + 4) == checkType)//下
                    {
                        return true;
                    }
                    if (GetLatticeType(i + 1, j + 1) == checkType && GetLatticeType(i + 2, j + 2) == checkType && GetLatticeType(i + 3, j + 3) == checkType && GetLatticeType(i + 4, j + 4) == checkType)//右下
                    {
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i <= 10; i++)
        {
            for (int j = 4; j <= 14; j++)
            {
                if (GetLatticeType(i, j) == checkType)
                {
                    if (GetLatticeType(i + 1, j - 1) == checkType && GetLatticeType(i + 2, j - 2) == checkType && GetLatticeType(i + 3, j - 3) == checkType && GetLatticeType(i + 4, j - 4) == checkType)//右上
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public GobangBoard Clone()
    {
        GobangBoard newGobangBoard = new GobangBoard(width, height);
        newGobangBoard.board = this.board;
        return newGobangBoard;
    }

    private Tuple<int, int> GetXY(int pos)
    {
        int x = pos / width;
        int y = pos % width;
        return new Tuple<int, int>(x, y);
    }

    private LatticeType GetLatticeType(int x, int y)
    {
        return board[x * width + y];
    }
}
