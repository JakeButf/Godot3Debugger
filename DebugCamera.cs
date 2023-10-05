using Godot;
using System;

public partial class DebugCamera : Camera3D
{
    [Export] public string G3DebugMasterScriptLocation = "/root/G3DebugMaster";
    [Export(PropertyHint.Range, "0, 1")] public float Sensitivity { get; set; } = 0.25f;
    [Export] public bool FreecamEnabled = false;

    Vector2 mousePosition = new Vector2(0.0f, 0.0f);
    float totalPitch = 0;

    Vector3 direction = new Vector3(0, 0, 0);
    Vector3 velocity = new Vector3(0, 0, 0);
    float accel = 30;
    float decel = -10;
    float velocityMultiplier = 4;

    const float shiftMultiplier = 2.5f;
    const float altMultiplier = 1.0f / shiftMultiplier;

    struct kbStateStructure
    {
        public bool w;
        public bool s;
        public bool a;
        public bool d;
        public bool q;
        public bool e;
        public bool shift;
        public bool alt;
    }

    kbStateStructure kbState = new kbStateStructure();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<G3Debug_Master>(G3DebugMasterScriptLocation).RegisterDebugCamera(this);
        UnsetKBState(kbState);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (FreecamEnabled)
        {
            UpdateCamLook();
            UpdateCamMovement(delta);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion iemm)
            mousePosition = iemm.Relative;

        if (@event is InputEventMouseButton e_iemb)
        {
            switch (e_iemb.ButtonIndex)
            {
                case MouseButton.Right:
                    if (e_iemb.Pressed)
                        Input.MouseMode = Input.MouseModeEnum.Captured;
                    else
                        Input.MouseMode = Input.MouseModeEnum.Visible;
                    break;

                case MouseButton.WheelUp:
                    velocityMultiplier = Mathf.Clamp(velocityMultiplier * 1.1f, .2f, 20f);
                    break;

                case MouseButton.WheelDown:
                    velocityMultiplier = Mathf.Clamp(velocityMultiplier / 1.1f, .2f, 20f);
                    break;
            }
        }

        if (@event is InputEventKey e_iek)
        {
            switch (e_iek.Keycode)
            {
                case Key.W:
                    kbState.w = e_iek.Pressed;
                    break;

                case Key.S:
                    kbState.s = e_iek.Pressed;
                    break;

                case Key.D:
                    kbState.d = e_iek.Pressed;
                    break;

                case Key.A:
                    kbState.a = e_iek.Pressed;
                    break;

                case Key.Q:
                    kbState.q = e_iek.Pressed;
                    break;

                case Key.E:
                    kbState.e = e_iek.Pressed;
                    break;

                case Key.Shift:
                    kbState.shift = e_iek.Pressed;
                    break;

                case Key.Alt:
                    kbState.alt = e_iek.Pressed;
                    break;
            }
        }
    }

    void UnsetKBState(kbStateStructure kb)
    {
        kb.w = false;
        kb.s = false;
        kb.a = false;
        kb.d = false;
        kb.q = false;
        kb.e = false;
        kb.shift = false;
        kb.alt = false;
    }

    void UpdateCamMovement(double delta)
    {
        direction = new Vector3(
            (kbState.d ? 1.0f : 0.0f) - (kbState.a ? 1.0f : 0.0f),
            (kbState.e ? 1.0f : 0.0f) - (kbState.q ? 1.0f : 0.0f),
            (kbState.s ? 1.0f : 0.0f) - (kbState.w ? 1.0f : 0.0f));

        Vector3 offset = direction.Normalized() * accel * velocityMultiplier * (float)delta + velocity.Normalized() * decel * velocityMultiplier * (float)delta;

        float speedMultiplier = 1;

        if (kbState.shift)
            speedMultiplier *= shiftMultiplier;
        if (kbState.alt)
            speedMultiplier *= altMultiplier;

        if (direction == Vector3.Zero && offset.LengthSquared() > velocity.LengthSquared())
            velocity = Vector3.Zero;
        else
        {
            velocity.X = Mathf.Clamp(velocity.X + offset.X, -velocityMultiplier, velocityMultiplier);
            velocity.Y = Mathf.Clamp(velocity.Y + offset.Y, -velocityMultiplier, velocityMultiplier);
            velocity.Z = Mathf.Clamp(velocity.Z + offset.Z, -velocityMultiplier, velocityMultiplier);

            this.Translate(velocity * (float)delta * speedMultiplier);
        }
    }

    void UpdateCamLook()
    {
        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            mousePosition *= Sensitivity;
            float yaw = mousePosition.X;
            float pitch = mousePosition.Y;
            mousePosition = new Vector2(0, 0);

            //pitch = Mathf.Clamp(pitch, -90 - totalPitch, 90 - totalPitch);
            totalPitch += pitch;

            RotateY(Mathf.DegToRad(-yaw));
            RotateObjectLocal(new Vector3(1, 0, 0), Mathf.DegToRad(-pitch));
        }
    }
}
