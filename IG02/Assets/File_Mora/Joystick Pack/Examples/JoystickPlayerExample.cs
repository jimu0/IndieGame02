using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickPlayerExample : MonoBehaviour
{
    public float JumpForce;
    public float speed;
    public VariableJoystick variableJoystick;
    public Button jumpBtn;
    public Rigidbody rb;

    private void Start()
    {
        jumpBtn.onClick.AddListener(() =>
        {
            Vector3 direction = Vector3.up * JumpForce;
            rb.AddForce(direction * Time.fixedDeltaTime, ForceMode.VelocityChange);
        });
    }

    public void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}