using Godot;
using System;
using System.Collections.Generic;
/*
 * 
 * THIS IS AN AUTOLOAD SCRIPT. IT NEEDS TO BE AUTOLOADED IN ORDER TO FUNCTION PROPERLY.
 * 
 */
public partial class G3Debug_Master : Node
{
	private List<DebugCamera> debuggableCameras = new List<DebugCamera>();
	private string debugCameraRoot = "res://Scripts/lib/DebugCamera.cs";
	private Variant oldScript;
	private Camera3D activeCamera;
	DebugCamera dCam;

    private bool enabled = false;
    string path;

    public void RegisterDebugCamera(DebugCamera camera)
	{
		debuggableCameras.Add(camera);
	}

	public DebugCamera GetRegisteredDebugCamera(DebugCamera camera)
	{
		foreach (DebugCamera cam in debuggableCameras)
			if (cam == camera) return cam;

		return null;
	}

	public void EnableFreecam(DebugCamera camera)
	{
		DisableCameraFreecam();
		GetRegisteredDebugCamera(camera).FreecamEnabled = true;
	}

	public void DisableCameraFreecam()
	{
        /*foreach (DebugCamera cam in debuggableCameras)
			cam.FreecamEnabled = false;*/

        dCam = GetNode<DebugCamera>(path);
        dCam.FreecamEnabled = false;
    }


	public override void _Process(double delta)
	{
		if(enabled) { dCam._Process(delta); }
		if (Input.IsPhysicalKeyPressed(Key.F))
		{
			if(!enabled)
			{
                //EnableFreecam(debuggableCameras[0]);
                activeCamera = GetViewport().GetCamera3D();
				path = activeCamera.GetPath();
                oldScript = activeCamera.GetScript();
                activeCamera.SetScript(ResourceLoader.Load<CSharpScript>(debugCameraRoot));
				dCam = GetNode<DebugCamera>(path);
				dCam._Ready();
				dCam.FreecamEnabled = true;
            } else
			{
				activeCamera = GetNode<DebugCamera>(path);
				DisableCameraFreecam();
                activeCamera.SetScript(oldScript);
            }
			enabled = !enabled;
		}

	}

    public override void _Input(InputEvent @event)
    {
		if (enabled)
			dCam._Input(@event);
    }
}
