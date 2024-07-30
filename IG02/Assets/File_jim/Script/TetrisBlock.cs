using UnityEngine;

public class TetrisBlock
{
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; set; }

    public TetrisBlock(Vector3Int[] cells, Vector3Int startPosition)
    {
        Cells = cells;
        Position = startPosition;
    }
    
    public Vector3Int[] GetGlobalPositions()
    {
        Vector3Int[] globalPositions = new Vector3Int[Cells.Length];
        for (int i = 0; i < Cells.Length; i++)
        {
            globalPositions[i] = Cells[i] + Position;
        }
        return globalPositions;
    }
}
