using PlayerManagement;
using UnityEngine;

/// <summary>
/// Chinar相机脚本
/// </summary>
public class CameraChange : MonoBehaviour
{
    public Transform pivot;                      // 手动添加：被跟踪的对象：pivot——以什么为轴
    public Vector3 pivotOffset = Vector3.zero; // 与目标的偏移量
    public float distance = 25f;     // 距目标距离(使用变焦)
    public float minDistance = 8f;        //最小距离
    public float maxDistance = 30f;       //最大距离
    public float zoomSpeed = 1f;        //速度倍率
    public float xSpeed = 250.0f;    //x速度
    public float ySpeed = 120.0f;    //y速度
    public bool allowYTilt = true;      //允许Y轴倾斜
    public float yMinLimit = 0f;      //相机向下最大角度
    public float yMaxLimit = 90f;       //相机向上最大角度
    private float x = 0.0f;      //x变量
    private float y = 0.0f;      //y变量
    private float targetX = 0f;        //目标x
    private float targetY = 0f;        //目标y
    private float targetDistance = 0f;        //目标距离
    private float xVelocity = 1f;        //x速度
    private float yVelocity = 1f;        //y速度
    private float zoomVelocity = 1f;        //速度倍率

    public Vector2 touchDeltaPosition = Vector2.zero;
    private Vector2 previousTouchPosition;//存储上一次触摸的位置

    void Start()
    {
        //pivot.GetComponent<PlayerController1>().cameraChange = this;
        var angles = transform.eulerAngles;                          //当前的欧拉角
        targetX = x = angles.x;                                   //给x，与目标x赋值
        targetY = y = ClampAngle(angles.y, yMinLimit, yMaxLimit); //限定相机的向上，与下之间的值，返回给：y与目标y
        targetDistance = distance;                                       //初始距离数据为10；
    }


    void LateUpdate()
    {
        if (pivot) //如果存在设定的目标
        {
            bool aa = false;
            float scroll = 0;

            float axisX = 0;
            float axisY = 0;
#if UNITY_STANDALONE
            scroll = Input.GetAxis("Mouse ScrollWheel"); //获取滚轮轴
            // aa=Input.GetMouseButton(1) || Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
            // axisX = Input.GetAxis("Mouse X");
            // axisY = Input.GetAxis("Mouse Y");
            
            if (scroll > 0.0f) targetDistance -= zoomSpeed;                         //如果大于0，说明滚动了：那么与目标距离，就减少固定距离1。就是向前滚动，就减少值，致使越来越近
            else if (scroll < 0.0f) targetDistance += zoomSpeed;                                                                                                     //距离变远                                              //否则
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);                                                         //目标的距离限定在2-15之间
            if (Input.GetKey(KeyCode.E))
            {
                axisX = -0.08f;
                aa = true;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                axisX = 0.08f;
                aa = true;
            }
            
#endif
#if UNITY_ANDROID
            
            aa = false;
            if (Input.touchCount > 0)
            {
                // if (Input.touchCount >= 2)
                // {
                //     //scroll = Input.GetAxis("Mouse ScrollWheel");
                // }
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    if (touch.position.x > Screen.width / 2)
                    {
                        aa = true;
                        axisX = touch.deltaPosition.x*0.064f;
                        axisY = touch.deltaPosition.y*0.064f;
                    }
                }
            }

            targetDistance = 20;
            
#endif
            
            if (aa)
            {
                
                targetX += axisX * xSpeed * 0.02f; //目标的x随着鼠标x移动*5
                if (allowYTilt)                                       //y轴允许倾斜
                {
                    targetY -= axisY * ySpeed * 0.02f; //目标的y随着鼠标y移动*2.4
                    targetY = ClampAngle(targetY, yMinLimit, yMaxLimit); //限制y的移动范围在0到90之间
                }
            }
            
            x = Mathf.SmoothDampAngle(x, targetX, ref xVelocity, 0.3f);
            if (allowYTilt) y = Mathf.SmoothDampAngle(y, targetY, ref yVelocity, 0.3f);
            else y = targetY;
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            distance = Mathf.SmoothDamp(distance, targetDistance, ref zoomVelocity, 0.5f);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + pivot.position + pivotOffset;
            transform.rotation = rotation;
            transform.position = position;
        }

    
}

    /// <summary>
    /// 限定一个值，在最小和最大数之间，并返回
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}