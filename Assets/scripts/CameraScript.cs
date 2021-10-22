using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform rotatingFocalPointParent;
    public Camera cam;

    public bool useController { get { return StaticValues.useController; } }

    public bool analogInvertX;
    public bool analogInvertY;
    public float analogSensitivityX;
    public float analogSensitivityY;
    
    public bool mouseInvertX;
    public bool mouseInvertY;
    public float mouseSensitivityX;
    public float mouseSensitivityY;

    public float maxDistance;
    public bool smoothCameraPos = false;          // smooth camera snapping at the cost of often clipping through models briefly
    public float cameraPosSmoothing = 0.00000001f; // the distance that remains after 1 sec of movement towards target pos

    public float yaw { get; private set; } // 0-360 degrees, where 0 is +X and 90 is +Z
    public float pitch { get; private set; }

    private Transform cameraTransform;

    private float nearClippingPlaneHalfDiagonal;

    private void Start()
    {
        yaw = 90f;

        cameraTransform = cam.transform;

        Rect nearPlane = GetNearClippingPlaneDimensions();
        nearClippingPlaneHalfDiagonal = Tools.HypotenuseLength(nearPlane.width, nearPlane.height) / 2;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        if (useController)
        {
            AnalogMoveCamera();
        }
        else
        {
            if (!GlobalObjects.pauseMenuStatic.paused) MouseMoveCamera();
        }

        if (!GlobalObjects.pauseMenuStatic.paused) rotatingFocalPointParent.localRotation = GetCameraRotation();
        SetCameraDistance();
    }

    private Quaternion GetCameraRotation()
    {
        Vector2 yawVec2 = Tools.DegreesToVec2(yaw);
        Vector3 yawVector3 = new Vector3(yawVec2.x, 0, yawVec2.y);
        Vector3 lookVector = Vector3.RotateTowards(yawVector3, Vector3.down, pitch * Mathf.Deg2Rad, 0);
        return Quaternion.LookRotation(lookVector);
    }

    private void MouseMoveCamera()
    {
        float xInvert = mouseInvertX ? 1 : -1;
        float yInvert = mouseInvertY ? 1 : -1;

        pitch = Mathf.Clamp(pitch + Input.GetAxisRaw("Mouse Y") * yInvert * mouseSensitivityY, -89f, 89f);
        yaw = Tools.PositiveModulo(yaw + Input.GetAxisRaw("Mouse X") * xInvert * mouseSensitivityX, 360f);
    }

    private void AnalogMoveCamera()
    {
        AnalogInput currInput = AnalogInput.GetCurrentInputs();

        if (!analogInvertX)
            currInput.RightAnalog = new Vector2(currInput.RightAnalog.x * -1f, currInput.RightAnalog.y);
        if (analogInvertY) // y axis on xbone is inverted by default, so that's why this is opposite
            currInput.RightAnalog = new Vector2(currInput.RightAnalog.x, currInput.RightAnalog.y * -1f);

        Vector2 adjustedForDeadzones = currInput.RightAnalogAdjusted;

        pitch = Mathf.Clamp(pitch + adjustedForDeadzones.y * analogSensitivityY * Time.deltaTime, -89f, 89f);
        yaw = Tools.PositiveModulo(yaw + adjustedForDeadzones.x * analogSensitivityX * Time.deltaTime, 360f);
    }

    private void SetCameraDistance()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            rotatingFocalPointParent.position,
            nearClippingPlaneHalfDiagonal, 
            cameraTransform.position - rotatingFocalPointParent.position,
            maxDistance, 
            LayerMask.GetMask("terrain"), 
            QueryTriggerInteraction.Ignore
        );

        float closestHitDistance = maxDistance;
        foreach (RaycastHit hit in hits)
        {
            closestHitDistance = Mathf.Min(closestHitDistance, hit.distance);
        }

        if (smoothCameraPos)
        {
            cameraTransform.localPosition = new Vector3(0, 0, Tools.Damp(cameraTransform.localPosition.z, -closestHitDistance, cameraPosSmoothing, Time.unscaledDeltaTime) );
        }
        else
        {
            cameraTransform.localPosition = new Vector3(0, 0, -closestHitDistance);
        }
    }

    private Rect GetNearClippingPlaneDimensions()
    {
        // https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-the-near-clipping-plane.72384/#post-463646

        Rect r = new Rect();
        float a = cam.nearClipPlane;                        //get length
        float A = cam.fieldOfView * 0.5f;                   //get angle
        A = A * Mathf.Deg2Rad;                              //convert to radians
        float h = Mathf.Tan(A) * a;                         //calc height
        float w = (h / cam.pixelHeight) * cam.pixelWidth;   //deduct width

        r.xMin = -w;
        r.xMax = w;
        r.yMin = -h;
        r.yMax = h;
        return r;
    }

    public Vector2 RelativeToCameraXZ(Vector2 original)
    {
        float degrees = Tools.Vec2ToDegrees(original);
        return Tools.DegreesToVec2(degrees + yaw + 270) * original.magnitude;
    }
}
