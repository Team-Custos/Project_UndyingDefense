using Cinemachine;
using UnityEngine;

public class InGame_VirtualCamManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform cameraPivot;
    public Vector3 UnitToLookAt_Offset;

    public bool isCameraMoving = false;


    public float moveSpeed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float zoomSpeed = 2.0f;
    public float zoomMin = 5.0f;
    public float zoomMax = 20.0f;

    public float xMax = 30.0f;
    public float xMin = -40.0f;
    public float zMax = 4.5f;
    public float zMin = -25.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
        CameraZoom();
    }

    void CameraMove()
    {
        //// 카메라 이동
        //Vector3 moveDirection = Vector3.zero;

        //if (Input.GetKey(KeyCode.W))
        //{
        //    // W 키를 누르면 Z 증가, X 증가
        //    moveDirection += new Vector3(1, 0, 1);
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    // S 키를 누르면 Z 감소, X 감소
        //    moveDirection += new Vector3(-1, 0, -1);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    // D 키를 누르면 X 증가, Z 감소
        //    moveDirection += new Vector3(1, 0, -1);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    // A 키를 누르면 X 감소, Z 증가
        //    moveDirection += new Vector3(-1, 0, 1);
        //}

        //// 이동 벡터를 정규화하여 일정한 속도로 이동
        //if (moveDirection != Vector3.zero)
        //{
        //    moveDirection.Normalize();
        //}


        //// 카메라 피벗 이동
        //cameraPivot.position += moveDirection * moveSpeed * Time.deltaTime;

        //// 이동 범위 제한
        //float clampedX = Mathf.Clamp(cameraPivot.position.x, xMin, xMax);
        //float clampedZ = Mathf.Clamp(cameraPivot.position.z, zMin, zMax);
        //cameraPivot.position = new Vector3(clampedX, cameraPivot.position.y, clampedZ);
    }

    void CameraZoom()
    {
        //float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        //if (scrollInput != 0)
        //{
        //    float currentFov = virtualCamera.m_Lens.FieldOfView;
        //    currentFov -= scrollInput * zoomSpeed * 100f * Time.deltaTime;
        //    currentFov = Mathf.Clamp(currentFov, zoomMin, zoomMax);
        //    virtualCamera.m_Lens.FieldOfView = currentFov;
        //}
    }
}