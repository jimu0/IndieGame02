
using UnityEngine;

namespace File_jim.Script.ViewAngle
{
    public class CmSwitchHeight : MonoBehaviour
    {
        public GameObject player;//玩家单位
        private Vector3 playerPos = Vector3.zero;//玩家单位位置
        private Vector3 pos = Vector3.zero;//自身位置
        public Vector3 maximumGap = new(9,2,9);//每个轴响应更新位置的距离
        public float followSpeed = 2f;    // 跟随速度
        void Start()
        {
            if (player)
            {
                playerPos = player.transform.position;
            }
            else
            {
                playerPos = Vector3.zero;
            }
            pos = transform.position;

        }
        
        void Update()
        {
            Transform tsf = transform;
            if (player)
            {
                playerPos = player.transform.position;
                pos = tsf.position;
                
                if (Mathf.Abs(playerPos.x - tsf.position.x) > maximumGap.x)
                    pos.x = Mathf.Lerp(tsf.position.x, playerPos.x, followSpeed * Time.deltaTime);
                if (Mathf.Abs(playerPos.y - tsf.position.y) > maximumGap.y)
                    pos.y = Mathf.Lerp(tsf.position.y, playerPos.y, followSpeed * Time.deltaTime);
                if (Mathf.Abs(playerPos.z - tsf.position.z) > maximumGap.z)
                    pos.z = Mathf.Lerp(tsf.position.z, playerPos.z, followSpeed * Time.deltaTime);
                if (playerPos.y == 0)
                {
                    pos.y = Mathf.Lerp(tsf.position.y, playerPos.y, followSpeed * Time.deltaTime);
                }
                tsf.position = pos;
            }
            else
            {
                playerPos = Vector3.zero;
                pos = Vector3.zero;
                tsf.position = pos;
            }
        }
    }
}
