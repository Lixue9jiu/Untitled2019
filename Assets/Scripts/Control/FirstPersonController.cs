using UnityEngine;

[RequireComponent(typeof(CharacterBody))]
public class FirstPersonController : MonoBehaviour
{
    public float MouseSensitivity = 1f;
    public float Speed = 1f;
    public float JumpHeight = 2f;

    private CharacterBody body;

    Transform cameraTransform;
    private float _pitch;

    private DoubleTapListener doubleJumpListener = new DoubleTapListener(0.4f, "Jump");

    private CrossPlatfromInput m_input;

    private void Awake()
    {
        body = GetComponent<CharacterBody>();
    }

    private void Start()
    {
        m_input = CrossPlatfromInput.instance;
        cameraTransform = GetComponentInChildren<Camera>().transform;
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
            body.ApplyGravity = !body.ApplyGravity;
        
        float pitch = _pitch - m_input.GetAxis("Mouse Y") * MouseSensitivity;
        pitch = Mathf.Clamp(pitch, -85, 85);
        cameraTransform.Rotate(new Vector3(pitch - _pitch, 0, 0), Space.Self);
        _pitch = pitch;

        transform.Rotate(new Vector3(0, m_input.GetAxis("Mouse X") * MouseSensitivity, 0), Space.World);

        doubleJumpListener.Update();
        if (doubleJumpListener.IsDoubleTapping)
        {
            body.ApplyGravity = !body.ApplyGravity;
            doubleJumpListener.IsDoubleTapping = false;
        }
    }

    private void FixedUpdate()
    {
        Vector3 input = (transform.forward * m_input.GetAxis("Vertical") + transform.right * m_input.GetAxis("Horizontal")) * Speed * Time.fixedDeltaTime;

        if (!body.ApplyGravity)
        {
            input.y = m_input.GetAxis("Fly") * Speed * Time.fixedDeltaTime;
            input *= 2f;
        }

        if (m_input.GetAxis("Jump") > 0)
        {
            if (body.TouchingGround)
            {
                body.JumpOrder = 1;
            }
        }
        body.MoveOrder = input;
    }
}
