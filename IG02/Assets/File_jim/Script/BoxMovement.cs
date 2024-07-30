using System;
using System.Collections;
using UnityEngine;

namespace File_jim.Script
{
    public class BoxMovement : MonoBehaviour
{
    public int id=1;
    private bool isMoving;//�ƶ��У�
    public Vector3Int startPos;
    private Vector3Int targetPos;
    public static event Action<BoxMovement, Vector3Int, Vector3Int> OnPositionChanged;
    
    private void OnEnable()
    {
        BoxMovManager.OnMoveBoxesToTarget += MoveTo;
        BoxMovManager.OnMoveBoxToTarget += MoveTo;
        BoxMovManager.OnMoveBoxIdToTarget += MoveTo;
    }
    private void OnDisable()
    {
        BoxMovManager.OnMoveBoxesToTarget -= MoveTo;
        BoxMovManager.OnMoveBoxToTarget -= MoveTo;
        BoxMovManager.OnMoveBoxIdToTarget += MoveTo;
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
        //if (isMoving) return; // ��ֹͬʱ�ƶ�
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
        StartCoroutine(PushToCoroutine(direction,speed));
    }

    private IEnumerator PushToCoroutine(Vector3Int direction, float speed)
    {
        
        isMoving = true;
        BoxMovManager.FallBoxV(startPos, direction,out bool f);
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
        transform.position = targetPos;
        startPos = targetPos;
        
        isMoving = false;
        //MoveTo(targetPos.x,targetPos.y,targetPos.z,BoxMovManager.MoveDuration);
        //BoxMovManager.Metronome();
    }

    /// <summary>
    /// �����λ
    /// </summary>
    /// <param name="start">���λ��</param>
    /// <param name="target">Ŀ��λ��</param>
    /// <param name="isSuccessful">������</param>
    
    private void ApplySeat(Vector3Int start,Vector3Int target,out bool isSuccessful)
    {
        BoxMovManager.SetMatrixV(target, id,out bool f);
        isSuccessful = f;
        if (f)
        {
            BoxMovManager.SetMatrixV(start,0,out bool _);
        }
        // if (BoxMovementManager.GetMatrixV(start + Vector3Int.up) < 0) return;//��ͷ�ϵ�λ���л��ʱ�������ԭλ��
        // BoxMovementManager.SetMatrixV(start, 0,out bool _);
    }

    // private void Fall(Vector3Int direction, float duration)
    // {
    //     if (isMoving) return; // ��ֹͬʱ�ƶ�
    //     isMoving = true;
    //     StartCoroutine(MoveToCoroutine(duration));
    //     BoxMovManager.FallBoxV(startPos, direction,out _);
    // }
}

}

