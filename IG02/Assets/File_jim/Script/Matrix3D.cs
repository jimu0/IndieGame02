using System;
using UnityEngine;

public class Matrix3D
{
    private int[,,] matrix;

    public Matrix3D(int width, int height, int depth)
    {
        matrix = new int[width, height, depth];
    }

    public bool IsCellOccupied(int x, int y, int z)
    {
        return matrix[x, y, z] != 0;
    }

    public void SetCell(int x, int y, int z, int value)
    {
        matrix[x, y, z] = value;
    }

    public int GetWidth() => matrix.GetLength(0);
    public int GetHeight() => matrix.GetLength(1);
    public int GetDepth() => matrix.GetLength(2);
}



