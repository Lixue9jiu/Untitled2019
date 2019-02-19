using UnityEngine;

// TODO: fix the jump
// TODO: fix the gravity while on ground
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
    private Vector3 input;
    private float _pitch;
    private bool touchingGround;

    private Vector3 velocity;

    private void Awake()
    {
        chara = GetComponent<CharacterController>();
    }

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

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 120, 40), $"{velocity}, {touchingGround}");
    }

    private void Update()
    {
        input += transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");

        if (touchingGround && Input.GetButton("Jump"))
        {
            velocity.y = JumpHeight;
        }

        float pitch = _pitch - Input.GetAxisRaw("Mouse Y") * MouseSensitivity;
        pitch = Mathf.Clamp(pitch, -85, 85);
        cameraTransform.Rotate(new Vector3(pitch - _pitch, 0, 0), Space.Self);
        _pitch = pitch;

        transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * MouseSensitivity, 0), Space.World);
    }

    private void FixedUpdate()
    {
        var flags = chara.Move((input + velocity) * Time.fixedDeltaTime);
        touchingGround = (flags & CollisionFlags.Below) == CollisionFlags.Below;
        if (!touchingGround)
            velocity += Physics.gravity * Time.fixedDeltaTime;
        else
            velocity.y = -0.1f;
        //rig.MovePosition(rig.position + input * Time.fixedDeltaTime * Speed);
        input = Vector3.zero;
    }
}
