using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UITemplate
{
    public class RotateImages : MonoBehaviour
    {
        // Start is called before the first frame update
        Vector3 rotationEuler;
        public Image thisImage;
        void Start()
        {
            rotationEuler = new Vector3(0, 0, UIManager.Instance.loadingIconSpeed);
        }

        // Update is called once per frame
        void Update()
        {
            if (thisImage.color.a > 0)
            {
                this.transform.Rotate(rotationEuler * Time.unscaledDeltaTime, Space.Self);
                // rotationEuler += Vector3.forward * UIManager.Instance.loadingIconSpeed * Time.unscaledDeltaTime;
                // transform.rotation = Quaternion.Euler(rotationEuler);
            }
        }
    }
}