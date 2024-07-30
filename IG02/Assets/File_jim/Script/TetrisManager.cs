using System.Collections;
using UnityEngine;


public class TetrisManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public float fallSpeed = 1f;
    public int matrixWidth = 10;
    public int matrixHeight = 20;
    public int matrixDepth = 10;

    private Matrix3D matrix;
    private TetrisBlock currentBlock;

    private GameObject aObj;
    
    void Start()
    {
        //matrix = new Matrix3D(matrixWidth, matrixHeight, matrixDepth);
        //SpawnBlock(); 
        //StartCoroutine(SpawnAndDropBlocks());
    }
    
    IEnumerator SpawnAndDropBlocks()
    {
        while (true)
        {
            
            if (!MoveBlock(Vector3Int.down))
            {
                FixBlock();
                //break;
                    
            }
            yield return new WaitForSeconds(fallSpeed);
            

            // while (true)
            // {
            //     if (!MoveBlock(Vector3Int.down))
            //     {
            //         Debug.Log("1");
            //         //FixBlock();
            //         break;
            //     }
            //     yield return new WaitForSeconds(fallSpeed);
            // }
        }
    }
    
    void SpawnBlock()
    {
        // Example of an L-shaped block
        Vector3Int[] cells = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 2, 0)
        };
        currentBlock = new TetrisBlock(cells, new Vector3Int(matrixWidth / 2, matrixHeight - 4, matrixDepth / 2));
        InstantiateBlock();
    }
    
    void InstantiateBlock()
    {
        foreach (Vector3Int cell in currentBlock.Cells)
        {
            Vector3Int pos = cell + currentBlock.Position;
            aObj = Instantiate(cubePrefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity, transform);
        }
    }

    bool MoveBlock(Vector3Int direction)
    {
        
        // 临时移动方块位置
        currentBlock.Position += direction;
        foreach (Vector3Int pos in currentBlock.GetGlobalPositions())
        {
            // 检查是否超出矩阵边界
            if (pos.x < 0 || pos.x >= matrix.GetWidth() ||
                pos.y < 0 || pos.y >= matrix.GetHeight() ||
                pos.z < 0 || pos.z >= matrix.GetDepth() ||
                matrix.IsCellOccupied(pos.x, pos.y, pos.z))
            {
                // 如果超出边界或碰撞，恢复方块位置并返回false
                currentBlock.Position -= direction;
                return false;
            }
        }
        return true;
    }

    void FixBlock()
    {
        foreach (Vector3Int pos in currentBlock.GetGlobalPositions())
        {
            if (pos.x >= 0 && pos.x < matrix.GetWidth() &&
                pos.y >= 0 && pos.y < matrix.GetHeight() &&
                pos.z >= 0 && pos.z < matrix.GetDepth())
            {
                matrix.SetCell(pos.x, pos.y, pos.z, 1);
            }
            else
            {
                Debug.LogWarning($"Block position {pos} is out of bounds!");
            }
        }
    }
}

