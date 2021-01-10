using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(NetworkTransform))]
public class CharacterControls : NetworkBehaviour
{
    static int numPlayers = 0;

    public GameObject cameraPrefab;

    public int id { get; private set; } = 0;

    float jumpForceCoefficient = 250f;
    float moveForceCoefficient = 15f;
    float brakeForceCoefficient = 2f;

    float maximumSpeed = 15f;

    float moveForceThisFrame = 0f;

    float groundAngleBoundary = Mathf.Cos(Mathf.PI/4);
    bool grounded = false;
    int extraJumps = 1;

    new Rigidbody rigidbody;
    PlayerInput playerInput;
    InputAction moveInputAction;
    InputAction jumpInputAction;

    void Awake() {
        id = numPlayers++;
        Debug.Log(id);
        rigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        moveInputAction = playerInput.actions["Move"];
        jumpInputAction = playerInput.actions["Jump"];
        switch(id) {
            case 0:
                GetComponent<Renderer>().material = Resources.Load("Materials/Red") as Material;
                break;
            case 1:
                GetComponent<Renderer>().material = Resources.Load("Materials/Blue") as Material;
                break;
            case 2:
                GetComponent<Renderer>().material = Resources.Load("Materials/Green") as Material;
                break;
            case 3:
                GetComponent<Renderer>().material = Resources.Load("Materials/Yellow") as Material;
                break;
            default:
                break;
        }
    }

    void Start() {
        if (isLocalPlayer) {
            Instantiate(cameraPrefab);
        }
    }

    private void Update() {
        if (isLocalPlayer) {
            moveForceThisFrame = HandleMoveInput();
            Jump();
        }
    }

    void FixedUpdate() {
        if (isLocalPlayer) {
            Move(moveForceThisFrame);
            
        }
        //JumpInfo();
    }

    void Move(float moveForce) {
        if (rigidbody.velocity.x > maximumSpeed && moveForce > 0) {
            rigidbody.velocity = new Vector3(maximumSpeed, rigidbody.velocity.y, rigidbody.velocity.z);
        } else if (rigidbody.velocity.x < -maximumSpeed && moveForce < 0) {
            rigidbody.velocity = new Vector3(-maximumSpeed, rigidbody.velocity.y, rigidbody.velocity.z);
        } else {
            rigidbody.AddForce(moveForce * Vector3.right);
        }
    }

    float HandleMoveInput() {
        float deltaX = moveInputAction.ReadValue<Vector2>().x;
        float moveForce = deltaX * moveForceCoefficient;
        if ((moveForce > 0 && rigidbody.velocity.x < 0) || (moveForce < 0 && rigidbody.velocity.x > 0)) {
            moveForce *= brakeForceCoefficient;
        }
        return moveForce;
    }

    void Jump() {
        if (jumpInputAction.triggered && (grounded || extraJumps > 0)) { // One frame
            if (rigidbody.velocity.y < 0) {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
            }
            if (grounded) {
                grounded = false;
            } else {
                extraJumps--;
            }
            float jumping = jumpInputAction.ReadValue<float>();
            rigidbody.AddForce(jumping * jumpForceCoefficient * Vector3.up);
        }
    }

    private void OnCollisionStay(Collision collision) {
        grounded = false;
        foreach (ContactPoint cp in collision.contacts) {
            if (cp.normal.y > groundAngleBoundary) {
                grounded = true;
                extraJumps = 1;
                break;
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        grounded = false;
    }

    void JumpInfo() {
        if (grounded) {
            GetComponent<MeshRenderer>().material.color = Color.green;
        } else if (extraJumps > 0) {
            GetComponent<MeshRenderer>().material.color = Color.yellow;
        } else {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
