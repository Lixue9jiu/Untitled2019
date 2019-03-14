using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public float MouseSensitivity = 1f;
    public float Speed = 1f;
    public float JumpHeight = 2f;

    [SerializeField]
    Transform cameraTransform;
    [SerializeField]
    private LayerMask Ground;
    private CharacterController chara;
    private float _pitch;
    private bool m_applyGravity = true;

    private Vector3 velocity;

    private void Awake()
    {
        chara = GetComponent<CharacterController>();
    }

#if UNITY_EDITOR || !UNITY_ANDROID
    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
#endif

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            m_applyGravity = !m_applyGravity;

        float pitch = _pitch - CrossPlatfromInput.GetAxis("Mouse Y") * MouseSensitivity;
        pitch = Mathf.Clamp(pitch, -85, 85);
        cameraTransform.Rotate(new Vector3(pitch - _pitch, 0, 0), Space.Self);
        _pitch = pitch;

        transform.Rotate(new Vector3(0, CrossPlatfromInput.GetAxis("Mouse X") * MouseSensitivity, 0), Space.World);
    }

    private void FixedUpdate()
    {
        Vector3 input = (transform.forward * CrossPlatfromInput.GetAxis("Vertical") + transform.right * CrossPlatfromInput.GetAxis("Horizontal")) * Speed * Time.fixedDeltaTime;

        if (!m_applyGravity)
        {
            velocity.y = 0;
            input.y = Input.GetAxis("Fly") * Speed * Time.fixedDeltaTime;
            input *= 4;
        }

        var flags = chara.Move(velocity * Time.fixedDeltaTime + input);
        bool touchingGround = (flags & CollisionFlags.Below) == CollisionFlags.Below;
        if ((flags & CollisionFlags.Above) == CollisionFlags.Above || (flags & CollisionFlags.Below) == CollisionFlags.Below)
            velocity.y = 0;

        if (m_applyGravity)
        {
            if (!touchingGround)
                velocity += Physics.gravity * Time.fixedDeltaTime;
            else
                velocity.y = -0.1f;
        }

        velocity -= velocity.normalized * velocity.sqrMagnitude * 0.0003f;
        if (touchingGround && CrossPlatfromInput.GetAxis("Jump") > 0)
        {
            velocity.y = JumpHeight;
        }
        //rig.MovePosition(rig.position + input * Time.fixedDeltaTime * Speed);
    }
}
