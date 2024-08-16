
using UnityEngine;

namespace File_jim.Script.ViewAngle
{
    public class CmSwitchHeight : MonoBehaviour
    {
        [Header("锁定镜头到谁？")]
        [SerializeField] public Transform lookAtXAndZ;
        public GameObject player;//玩家单位
        

        private Vector3 newPos = Vector3.zero;//新目标位置
        private Vector3 pos = Vector3.zero;//当前位置
        public Vector3 maximumGap = new(9,2,9);//每个轴响应更新位置的距离
        public float followSpeed = 2f;    // 跟随速度
        void Start()
        {
            if (player)
            {
                newPos = player.transform.position;
            }
            else
            {
                newPos = Vector3.zero;
            }
            pos = transform.position;

        }
        
        void Update()
        {
            Transform tsf = transform;
            if (lookAtXAndZ)
            {
                newPos = lookAtXAndZ.position;
                //pos = tsf.position;
                if (Mathf.Abs(newPos.x - tsf.position.x) > maximumGap.x)
                    pos.x = Mathf.Lerp(tsf.position.x, newPos.x, followSpeed * Time.deltaTime);
                if (Mathf.Abs(newPos.z - tsf.position.z) > maximumGap.z)
                    pos.z = Mathf.Lerp(tsf.position.z, newPos.z, followSpeed * Time.deltaTime);
            }
            else
            {
                if (player)
                {
                    newPos = player.transform.position;
                    pos.x = newPos.x;
                    pos.z = newPos.z;
                }
                else
                {
                    pos.x = 0;
                    pos.z = 0;
                }
            }

            if (player)
            {
                newPos.y = player.transform.position.y;
                //pos = tsf.position;
                if (Mathf.Abs(newPos.y - tsf.position.y) > maximumGap.y)
                    pos.y = Mathf.Lerp(tsf.position.y, newPos.y, followSpeed * Time.deltaTime);
                if (newPos.y == 0)
                {
                    pos.y = Mathf.Lerp(tsf.position.y, newPos.y, followSpeed * Time.deltaTime);
                }
            }
            else
            {
                pos.y = 0;
            }
            tsf.position = pos;
        }
    }
}
