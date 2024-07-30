using System;
using System.Collections;
using UnityEngine;

public class BoxMovement : MonoBehaviour
{
    public int id=1;
    private bool isMoving;//移动中？
    public Vector3Int startPos;
    private Vector3Int targetPos;
    public static event Action<BoxMovement, Vector3Int, Vector3Int> OnPositionChanged;
    
    private void OnEnable()
    {
        BoxMovementManager.OnMoveBoxesToTarget += MoveTo;
        BoxMovementManager.OnMoveBoxToTarget += MoveTo;
        BoxMovementManager.OnMoveBoxIdToTarget += MoveTo;
    }
    private void OnDisable()
    {
        BoxMovementManager.OnMoveBoxesToTarget -= MoveTo;
        BoxMovementManager.OnMoveBoxToTarget -= MoveTo;
        BoxMovementManager.OnMoveBoxIdToTarget += MoveTo;
    }
    
    private void Start()
    {
        startPos = Vector3Int.RoundToInt(transform.position);
        targetPos = startPos;
    }
    private void Update()
    {
        Vector3Int currentPosition = Vector3Int.RoundToInt(transform.position);
        if (!isMoving && currentPosition != startPos)
        {
            OnPositionChanged?.Invoke(this, startPos, currentPosition);
            startPos = currentPosition;
            targetPos = startPos;

        }
    }
    
    private void MoveTo(Vector3Int selfStart, int x, int y, int z, float duration)
    {
        if (startPos != selfStart) return;
        MoveTo(x, y, z, duration);
    }
    private void MoveTo(int i, int x,int y, int z, float duration)
    {
        if (id != i) return;
        MoveTo(x, y, z, duration);
    }
    private void MoveTo(int x,int y, int z, float duration)
    {
        if (isMoving) return; // 防止同时移动
        targetPos.x = x;
        targetPos.y = y;
        targetPos.z = z;
        isMoving = true;
        StartCoroutine(MoveToCoroutine(duration));
    }
    private IEnumerator MoveToCoroutine(float duration)
    {
        
        //BoxMovementManager.MoveBoxToTarget(startPos + Vector3Int.up, Vector3Int.down);    

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPos;
        startPos = targetPos;
        isMoving = false;
    }

    public void PushTo(Vector3Int direction, float speed)
    {
        if (isMoving) return;
        //targetPos = startPos + direction;
        StartCoroutine(PushToCoroutine(direction,speed));
    }

    private IEnumerator PushToCoroutine(Vector3Int direction, float speed)
    {
        
        isMoving = true;
        BoxMovementManager.FallBoxV(startPos, direction,out bool f);
        if (f)
        {
            targetPos = startPos + direction;
            float elapsedTime = 0f;
            while (elapsedTime < speed)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / speed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            targetPos = startPos;
        }
        startPos = targetPos;
        transform.position = targetPos;
        isMoving = false;
    }

    /// <summary>
    /// 申请就位
    /// </summary>
    /// <param name="start">起点位置</param>
    /// <param name="target">目标位置</param>
    /// <param name="isSuccessful">申请结果</param>
    
    private void ApplySeat(Vector3Int start,Vector3Int target,out bool isSuccessful)
    {
        BoxMovementManager.SetMatrixV(target, id,out bool f);
        isSuccessful = f;
        if (f)
        {
            BoxMovementManager.SetMatrixV(start,0,out bool _);
        }
        // if (BoxMovementManager.GetMatrixV(start + Vector3Int.up) < 0) return;//当头上的位置有活动体时不用清空原位置
        // BoxMovementManager.SetMatrixV(start, 0,out bool _);
    }

    private void Fall(Vector3Int direction, float duration)
    {
        if (isMoving) return; // 防止同时移动
        isMoving = true;
        StartCoroutine(MoveToCoroutine(duration));
        BoxMovementManager.FallBoxV(startPos, direction,out _);
    }
}
