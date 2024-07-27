using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UITemplate
{
    public class ChangeCamera : MonoBehaviour
    {
        private bool canTurn = true;
        public Camera CameraToMove;
        [Header("0的位置需要放Y摄像机")]
        public List<GameObject> Cameras;
        int index = 0;
        [Header("安卓")]
        public Button Qbtn;
        public Button Ebtn;

        // Start is called before the first frame update
        void Start()
        {
            canTurn = true;
#if UNITY_ANDROID
            Qbtn.onClick.AddListener(() => {
                if (canTurn)
                {
                    canTurn = false;
                    Qpress();
                }
            });

            Ebtn.onClick.AddListener(() => {
                if (canTurn)
                {
                    canTurn = false;
                    Epress();
                }
            });
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q) && canTurn)
            {
                canTurn = false;
                Qpress();
            }
            if (Input.GetKeyDown(KeyCode.E) && canTurn)
            {
                canTurn = false;
                Epress();
            }
        }

        void Qpress()
        {
            var tgRot = CameraToMove.transform.rotation;
            index++;
            if (index == 3)
            {
                index = 0;
            }

            foreach (GameObject cam in Cameras)
            {
                if (Cameras.IndexOf(cam) == index)
                {
                    CameraToMove.transform.DOMove(cam.transform.position, 0.5f).SetEase(Ease.InOutCirc);
                    //if (index != 0)
                        CameraToMove.transform.DORotateQuaternion(cam.transform.rotation
                        , 0.5f).SetEase(Ease.InOutCirc).OnComplete(() => { canTurn = true; });
                    //else
                    //{
                    //    var q = index - 1;
                    //    if (q == -1)
                    //    {
                    //        q = 2;
                    //    }
                    //    tgRot.ToAngleAxis(out var angle, out var axis);
                    //    CameraToMove.transform.DORotate(new Vector3(90, angle * axis.y, angle * axis.z)
                    //    , 0.5f).SetEase(Ease.InOutCirc).OnComplete(() => { canTurn = true; });
                    //}

                }
            }
        }

        void Epress()
        {
            var tgRot = CameraToMove.transform.rotation;
            index--;
            if (index == -1)
            {
                index = 2;
            }

            foreach (GameObject cam in Cameras)
            {
                if (Cameras.IndexOf(cam) == index)
                {
                    CameraToMove.transform.DOMove(cam.transform.position, 0.5f).SetEase(Ease.InOutCirc);
                    //if (index != 0)
                        CameraToMove.transform.DORotateQuaternion(cam.transform.rotation
                        , 0.5f).SetEase(Ease.InOutCirc).OnComplete(() => { canTurn = true; });
                    //else
                    //{
                    //    var q = index + 1;
                    //    if (q == 3)
                    //    {
                    //        q = 0;
                    //    }
                    //    tgRot.ToAngleAxis(out var angle, out var axis);
                    //    CameraToMove.transform.DORotate(new Vector3(90, angle * axis.y, angle * axis.z)
                    //    , 0.5f).SetEase(Ease.InOutCirc).OnComplete(() => { canTurn = true; });
                    //}
                }
            }
        }
    }
}
