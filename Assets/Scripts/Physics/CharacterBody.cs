using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class CharacterBody : MonoBehaviour
{
    public float JumpHeight = 1.1f;
    private float JumpVelocity;

    [HideInInspector]
    public Vector3 Velocity;

    [HideInInspector]
    public Vector3 MoveOrder;
    [HideInInspector]
    public float JumpOrder;

    public bool TouchingGround => (chara.collisionFlags & CollisionFlags.Below) == CollisionFlags.Below;
    public bool ApplyGravity = true;

    [SerializeField]
    private LayerMask Ground;
    private CharacterController chara;

    BlockCollider blockCollider;

    private void Awake()
    {
        chara = GetComponent<CharacterController>();
        blockCollider = FindObjectOfType<BlockCollider>();

        JumpVelocity = Mathf.Sqrt(2 * JumpHeight * -Physics.gravity.y);
    }

    private void FixedUpdate()
    {
        if (!ApplyGravity)
        {
            Velocity.y = 0;
        }
        else
        {
            if (!TouchingGround)
                Velocity += Physics.gravity * Time.fixedDeltaTime;
            else
                Velocity.y = -0.1f;
        }

        Velocity *= 1 - 0.0001f * Velocity.sqrMagnitude;

        if (JumpOrder > 0)
            Velocity.y = JumpOrder * JumpVelocity;

        CollisionFlags flags = chara.collisionFlags;
        if ((flags & CollisionFlags.Above) == CollisionFlags.Above)
            Velocity.y = Mathf.Min(0, Velocity.y);

        chara.Move(Velocity * Time.fixedDeltaTime + MoveOrder);
        MoveOrder = Vector3.zero;
        JumpOrder = 0;
    }
}
