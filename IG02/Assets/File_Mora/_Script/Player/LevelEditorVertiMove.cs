using EditorPlus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement
{
    public class LevelEditorVertiMove : MonoBehaviour
    {
        [ReadOnly] public int index;
        // Start is called before the first frame update
        void Start()
        {
            index = (int)transform.position.y;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<CharacterController>().enabled = false;
                index++;
                transform.position = new Vector3(transform.position.x, index, transform.position.z);
                GetComponent<CharacterController>().enabled = true;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (index == 0)
                    return;
                GetComponent<CharacterController>().enabled = false;
                index--;
                transform.position = new Vector3(transform.position.x, index, transform.position.z);
                GetComponent<CharacterController>().enabled = true;
            }
        }
    }
}
