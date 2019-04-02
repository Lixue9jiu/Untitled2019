using UnityEngine;
using System.Collections;

public class FirstPersonController2 : MonoBehaviour
{
    public float MouseSensitivity = 1f;
    public float Speed = 1f;
    public float JumpHeight = 2f;

    [SerializeField]
    Transform cameraTransform;
    [SerializeField]
    private LayerMask Ground;
    private CharacterBody2 chara;
    private float _pitch;
    private bool m_applyGravity = true;

    private Vector3 velocity;

    private DoubleTapListener doubleJumpListener = new DoubleTapListener(0.5f, "Jump");

    private CrossPlatfromInput m_input;

    private void Awake()
    {
        chara = GetComponent<CharacterBody2>();
    }

    private void Start()
    {
        m_input = CrossPlatfromInput.instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            m_applyGravity = !m_applyGravity;

        float pitch = _pitch - m_input.GetAxis("Mouse Y") * MouseSensitivity;
        pitch = Mathf.Clamp(pitch, -85, 85);
        cameraTransform.Rotate(new Vector3(pitch - _pitch, 0, 0), Space.Self);
        _pitch = pitch;

        transform.Rotate(new Vector3(0, m_input.GetAxis("Mouse X") * MouseSensitivity, 0), Space.World);

        doubleJumpListener.Update();
    }

    private void FixedUpdate()
    {
        Vector3 input = (transform.forward * m_input.GetAxis("Vertical") + transform.right * m_input.GetAxis("Horizontal")) * Speed * Time.fixedDeltaTime;

        if (!m_applyGravity)
        {
            velocity.y = 0;
            input.y = m_input.GetAxis("Fly") * Speed * Time.fixedDeltaTime;
            input *= 1.5f;
        }

        var flags = chara.Move(velocity * Time.fixedDeltaTime + input);
        if ((flags & CharacterBody2.HitResult.Ceiling) == CharacterBody2.HitResult.Ceiling)
            velocity.y = 0;

        bool isTouchingGround = (flags & CharacterBody2.HitResult.Floor) == CharacterBody2.HitResult.Floor;

        if (m_applyGravity)
        {
            if (!isTouchingGround)
                velocity += Physics.gravity * Time.fixedDeltaTime;
            else
                velocity.y = -0.1f;
        }

        velocity -= velocity.normalized * velocity.sqrMagnitude * 0.0003f;
        if (m_input.GetAxis("Jump") > 0)
        {
            if (isTouchingGround && (flags & CharacterBody2.HitResult.Ceiling) != CharacterBody2.HitResult.Ceiling)
            {
                velocity.y = JumpHeight;
            }
        }

        if (doubleJumpListener.IsDoubleTapping)
        {
            m_applyGravity = !m_applyGravity;
            doubleJumpListener.IsDoubleTapping = false;
        }
        //rig.MovePosition(rig.position + input * Time.fixedDeltaTime * Speed);
    }
}
