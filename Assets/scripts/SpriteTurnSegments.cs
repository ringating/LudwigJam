using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTurnSegments : MonoBehaviour
{
    public Transform actuallyRotates;

    // the first segment in the array is the front, the last sprite is the back. the in-betweens are mirrored for the other side of the object.
    public WobbleSprite[] segments;
    public bool reverseLeftAndRight;

    private int segmentIndex;

    private void Start()
    {
		if (segments.Length < 1) { Debug.LogError("missing turn segments!"); }

        segmentIndex = 0;
        segments[0].enabled = true;

        for (int i = 1; i < segments.Length; ++i)
        {
            segments[i].enabled = false;
        }
    }

    private void Update()
    {
        UpdateActiveSegment();
        RotateToFaceCamera();  
    }

	private void UpdateActiveSegment()
    {
        int totalSegments = (segments.Length - 1) * 2;
        float degreesPerSegment = 360f / totalSegments;
        Vector3 forward = actuallyRotates.forward;
        float rotationRelativeToCamera = Tools.PositiveModulo(GlobalObjects.cameraScriptStatic.yaw - Tools.Vec2ToDegrees(new Vector2(forward.x, forward.z)), 360f);

        int newSegmentIndex = Mathf.RoundToInt( rotationRelativeToCamera  / degreesPerSegment ) % (totalSegments);

        //print(newSegmentIndex + " " + Mathf.RoundToInt(GlobalObjects.cameraScriptStatic.yaw));

        setActiveSegment(newSegmentIndex);
    }

    private void setActiveSegment(int newSegmentIndex)
    {
        if (_visible && newSegmentIndex != segmentIndex)
        {
            int actualIndex = getActualIndex(newSegmentIndex);
            float xScalar;
            if (newSegmentIndex >= segments.Length)
            {
                xScalar = reverseLeftAndRight ? 1f : -1f;
            }
			else
            {
                xScalar = reverseLeftAndRight ? -1f : 1f;
            }


            segments[ getActualIndex(segmentIndex) ].enabled = false;
            segments[ actualIndex ].enabled = true;

            Vector3 localScale = segments[actualIndex].transform.localScale;
            segments[actualIndex].transform.localScale = new Vector3(Mathf.Abs(localScale.x) * xScalar, localScale.y, localScale.z);

            segmentIndex = newSegmentIndex;
        }
    }

    private void RotateToFaceCamera()
    {
        segments[getActualIndex(segmentIndex)].transform.eulerAngles = new Vector3(0, 90 - GlobalObjects.cameraScriptStatic.yaw, 0);
    }

    private int getActualIndex(int potentiallyBigIndex)
    {
        if (potentiallyBigIndex >= segments.Length)
        {
            return (segments.Length - 2) - (potentiallyBigIndex - segments.Length);
        }
        else
        {
            return potentiallyBigIndex;
        }
    }

    private bool _visible = true;
    public bool visible
    {
        set
        {
            if (value && !_visible)
            {
                segments[getActualIndex(segmentIndex)].enabled = true;
                _visible = true;
            }
            else if (!value && _visible)
            {
                segments[getActualIndex(segmentIndex)].enabled = false;
                _visible = false;
            }

			if (GlobalObjects.cameraScriptStatic)
            {
                UpdateActiveSegment();
                RotateToFaceCamera();
            }
        }

        get { return _visible; }
    }

    private void OnEnable()
    {
        visible = true;
    }

    private void OnDisable()
    {
        visible = false;
    }

    public void forceHideAndDisable()
    {
        for (int i = 0; i < segments.Length; ++i)
        {
            segments[i].forceHideAndDisable();
        }
        enabled = false;
    }
}
