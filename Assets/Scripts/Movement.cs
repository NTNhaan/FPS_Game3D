using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 1f;
    [SerializeField] float Speed = 6.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;

    public float jumpHeight = 6f;
    float velocityY;
    bool isGrounded;
    public bool isJumping = true;
    float cameraCap;
    Vector2 currentMouseDelta;
    // private float currentMouseDeltaX;
    // private float currentMouseDeltaY;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;
    //public float weaponSwayAmount = 0.05f;
    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        //UpdateMouse();
        UpdateMove();
    }

    // void UpdateMouse()
    // {
    //     Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));  // giá trị đầu vào của chuột theo trục X và Y
    //     mouseAxis *= mouseSensitivity;
    //     currentMouseDelta.x += mouseAxis.x;
    //     currentMouseDelta.y += mouseAxis.y;
    //     currentMouseDelta.y = Mathf.Clamp(currentMouseDelta.y, -90, 90);  // giới hạn góc quay trục Y để không bị lật ngược
    //     transform.localPosition += (Vector3)mouseAxis * weaponSwayAmount / 1000;
    //     transform.root.localRotation = Quaternion.AngleAxis(currentMouseDelta.x, Vector3.up);
    //     playerCamera.transform.localRotation = Quaternion.AngleAxis(-currentMouseDelta.y, Vector3.right);


    //     // Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

    //     // currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

    //     // cameraCap -= currentMouseDelta.y * mouseSensitivity;

    //     // cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

    //     // playerCamera.localEulerAngles = Vector3.right * cameraCap;

    //     // transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    // }

    void UpdateMove()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Speed = 7.0f;
        }
        else
        {
            Speed = 5.0f;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);   // check if player is standing on the ground

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime); // SmoothDamp is used to smooth the movement of the player

        velocityY += gravity * 2f * Time.deltaTime;    // Increase free fall speed 

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump") && isJumping)
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = false;
        }

        if (isGrounded! && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        int layer = hit.collider.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Ground"))
        {
            if (hit.normal.y >= 0)
            {
                isJumping = true;
            }
        }
    }
}
