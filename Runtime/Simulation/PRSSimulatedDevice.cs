using UnityEngine;

namespace PleaseRemainSeated.Simulation
{
  /// <summary>
  /// Virtual AR device.
  /// </summary>
  [RequireComponent(typeof(Camera))]
  public class PRSSimulatedDevice : MonoBehaviour
  {
    [Header("Input Configuration")]
    public Vector2 MouseSensitivity = new Vector2(3f, 3f);
    public float MouseVerticalLimit = 90f;
    public float MovementSpeed = 8f;
    public float GroundAcceleration = 12f;
    public float GroundDeceleration = 10f;
    public float GroundFriction = 6f;
    public float VerticalSpeed = 2;

    [SerializeField] private Transform arCamera;
    [SerializeField] private Transform arSessionOrigin;

    public RenderTexture targetTexture
    {
      get
      {
        if (_targetTexture == null)
        {
          _targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
          Camera.targetTexture = _targetTexture;
        }

        return _targetTexture;
      }
    }

    public Matrix4x4 projectionMatrix => Camera.projectionMatrix;
    
    internal Camera Camera
    {
      get
      {
        if (_camera == null)
        {
          _camera = GetComponent<Camera>();
        }

        return _camera;
      }
    }

    private Camera _camera; // Own camera component

    Vector3 _previousPosition; // Position in previous frame
    Quaternion _previousRotation; // Rotation in previous frame

    Vector2 _mouseRotation = Vector2.zero;
    Vector3 _velocity = Vector3.zero;

    private RenderTexture _targetTexture;

    Pose _pose;

    void Awake()
    {
      _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
      ProcessInput();
      
      arCamera.SetPositionAndRotation(transform.position, transform.rotation);
    }

    private void ProcessInput()
    {
      if (!Input.GetKey(KeyCode.LeftShift))
      {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        return;
      }

      _previousPosition = transform.position;
      _previousRotation = transform.rotation;

      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
      
      var mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

      // Update rotation.
      _mouseRotation.x -= mouseInput.y * MouseSensitivity.x;
      _mouseRotation.x = Mathf.Clamp(_mouseRotation.x, -MouseVerticalLimit, MouseVerticalLimit);
      _mouseRotation.y += mouseInput.x * MouseSensitivity.y;
      transform.rotation = Quaternion.Euler(_mouseRotation.x, _mouseRotation.y, 0f);

      // Apply movement.
      var currentSpeed = _velocity.magnitude;

      var control = Mathf.Max(currentSpeed, GroundDeceleration);
      var deceleration = control * GroundFriction * Time.deltaTime;

      var newSpeed = Mathf.Max(0, currentSpeed - deceleration);
      if (currentSpeed > 0) newSpeed /= currentSpeed;
      _velocity *= newSpeed;

      var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
      var moveDirection = transform.TransformDirection(new Vector3(input.x, 0f, input.y)).normalized;
      var speedIncrease = MovementSpeed - Vector3.Dot(_velocity, moveDirection);
      var acceleration = Mathf.Min(GroundAcceleration * Time.deltaTime * MovementSpeed, speedIncrease);

      _velocity.x += acceleration * moveDirection.x;
      _velocity.z += acceleration * moveDirection.z;

      transform.position += _velocity;

      if (Input.GetKey(KeyCode.Space))
      {
        transform.position += Vector3.up * (VerticalSpeed * Time.deltaTime);
      }
      else if (Input.GetKey(KeyCode.C))
      {
        transform.position -= Vector3.up * (VerticalSpeed * Time.deltaTime);
      }
    }
  }
}