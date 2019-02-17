using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    public float MouseSensitivity = 1f;
    public float Speed = 1f;
    public float JumpHeight = 2f;

    [SerializeField]
    Transform cameraTransform;
    [SerializeField]
    private LayerMask Ground;
    private Rigidbody rig;
    private Vector3 input;
    private float _pitch;
    private bool waitForInAir;
    private bool touchingGround;
    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 120, 40), $"{waitForInAir}, {touchingGround}");
    }

    private void Update()
    {
        bool isTouchingGround = Physics.Raycast(transform.position, Vector3.down, 1f, Ground, QueryTriggerInteraction.Ignore);
        touchingGround = isTouchingGround;
        input += transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        
        if (!isTouchingGround)
        {
            waitForInAir = false;
        }
        if (!waitForInAir && isTouchingGround && Input.GetButton("Jump") && rig.velocity.y == 0)
        {
            rig.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
            waitForInAir = true;
        }

        float pitch = _pitch - Input.GetAxisRaw("Mouse Y") * MouseSensitivity;
        pitch = Mathf.Clamp(pitch, -85, 85);
        cameraTransform.Rotate(new Vector3(pitch - _pitch, 0, 0), Space.Self);
        _pitch = pitch;

        transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * MouseSensitivity, 0), Space.World);
    }

    private void FixedUpdate()
    {
        rig.AddForce(input, ForceMode.Force);
        //rig.MovePosition(rig.position + input * Time.fixedDeltaTime * Speed);
        input = Vector3.zero;
    }
}
