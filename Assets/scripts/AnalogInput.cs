using UnityEngine;

public struct AnalogInput
{
    public Vector2 LeftAnalog { get; set; }
    public Vector2 RightAnalog { get; set; }

    public float LeftDeadzone
    {
        get
        {
            return 0.1f;
        }
    }

    public float RightDeadzone
    {
        get
        {
            return 0.1f;
        }
    }

    public float LeftLivezone
    {
        get
        {
            return 0.95f;
        }
    }

    public float RightLivezone
    {
        get
        {
            return 0.95f;
        }
    }

    public bool LeftMoved
    {
        get
        {
            return LeftAnalog.magnitude > LeftDeadzone;
        }
    }

    public bool RightMoved
    {
        get
        {
            return RightAnalog.magnitude > RightDeadzone;
        }
    }

    public Vector2 LeftAnalogAdjusted
    {
        get
        {
            return CircularDeadzoneAdjust(LeftAnalog, LeftDeadzone, LeftLivezone);
        }
    }
    
    public Vector2 RightAnalogAdjusted
    {
        get
        {
            return CircularDeadzoneAdjust(RightAnalog, RightDeadzone, RightLivezone);
        }
    }

    private Vector2 CircularDeadzoneAdjust(Vector2 raw, float deadzone = 0, float livezone = 1)
    {
        return raw.normalized * Tools.Map(raw.magnitude, deadzone, livezone, 0, 1, true);
    }

    public static AnalogInput GetCurrentInputs()
    {
        return new AnalogInput
        {
			LeftAnalog = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
			RightAnalog = new Vector2(Input.GetAxisRaw("Look Horizontal"), Input.GetAxisRaw("Look Vertical"))
		};
    }

    public static AnalogInput GetZeroInputs()
    {
        return new AnalogInput
        {
            LeftAnalog = Vector2.zero,
            RightAnalog = Vector2.zero
        };
    }
}
