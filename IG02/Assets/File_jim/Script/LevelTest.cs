using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UITemplate
{
    public class LevelTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                UIManager.Instance.completeLevel(true);
            }
        }
    }
}
