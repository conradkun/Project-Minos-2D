using UnityEngine;
using System.Collections;

public class Maze
{

    int width;
    int height;


    int[,] grid;


    int startX;
    int startY;

    public int[,] Grid
    {
        get { return grid; }
    }

    public Maze(int width, int height)
    {
        this.width = width;
        this.height = height;

    }

    public void Generate(int[,] grid)
    {
        this.grid = grid;
    }

}