using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum eCameraMode { Free, Locked };

	[SerializeField] private Transform m_Target;
    [SerializeField] private Transform m_LockObject;

    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private float m_Pitch = 2f;
    [SerializeField] private float m_Zoom = 5f;
    [SerializeField] private float m_YawSpeed = 100f;
    //[SerializeField] private float m_LockRotationSpeed = 3f;
    [SerializeField] private float m_LockedViewDistance = 5f;
    [SerializeField] private float m_MinCameraY = 0.3f;

    private float m_CurrentYaw = 0f;
    private eCameraMode m_CameraMode = eCameraMode.Free;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameplayEvents.OnLevelLoadedEvent>(OnLevelLoaded);
    }

    private void OnLevelLoaded(GameplayEvents.OnLevelLoadedEvent e)
    {
        if (e.SceneID == 2)
        {
            // just look at the dish I guess when the level loads
            transform.position = new Vector3(49.75f, 0.85f, 40);
            transform.rotation = Quaternion.identity;

            m_CurrentYaw = -90;
        }
    }

    public void SetFollowTarget(Transform target)
    {
        m_Target = target;
    }

    public void SetLockTarget(Transform lockObj)
    {
        m_LockObject = lockObj;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Camera Toggle"))
        {
            if (m_CameraMode == eCameraMode.Free)
            {
                ChangeMode(eCameraMode.Locked);
            }
            else
            {
                ChangeMode(eCameraMode.Free);
            }
        }

        if (m_CameraMode == eCameraMode.Free)
        {
            m_CurrentYaw += Input.GetAxis("Camera_Horizontal") * m_YawSpeed * Time.deltaTime;
        }
    }

    public void ChangeMode(eCameraMode mode)
    {
        m_CameraMode = mode;
    }

    private void LateUpdate()
    {
        if (m_Target != null)
        {
            if (m_CameraMode == eCameraMode.Free)
            {
                transform.position = m_Target.position - m_Offset * m_Zoom;
                transform.LookAt(m_Target.position + Vector3.up * m_Pitch);

                transform.RotateAround(m_Target.position, Vector3.up, m_CurrentYaw);
            }
            else if (m_CameraMode == eCameraMode.Locked)
            {

                Vector3 lockToTarget = (m_LockObject.position - m_Target.position).normalized;

                // don't adjust the camera's height, just rotation
                lockToTarget.y = 0f;

                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lockToTarget), m_LockRotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(lockToTarget);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                //transform.position = Vector3.Slerp(transform.position, (m_Target.position + Vector3.up) - lockToTarget * m_LockedViewDistance, Time.deltaTime);

                Vector3 cameraPos = (m_Target.position + Vector3.up) - lockToTarget * m_LockedViewDistance;
                if (cameraPos.y < m_MinCameraY)
                {
                    cameraPos.y = m_MinCameraY;
                }
                transform.position = cameraPos;
            }
        }
    }
}
