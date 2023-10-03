using Godot;
using System;

[GlobalClass]
[Tool]
public partial class J_Debug : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public MeshInstance3D LineRender(Vector3 startPoint,  Vector3 endPoint, Color color)
	{
		MeshInstance3D m_instance = new MeshInstance3D();
		ImmediateMesh im = new ImmediateMesh();
        OrmMaterial3D mat = new OrmMaterial3D();

		m_instance.Mesh = im;
		m_instance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

		im.SurfaceBegin(Mesh.PrimitiveType.Lines, mat);
		im.SurfaceAddVertex(startPoint);
		im.SurfaceAddVertex(endPoint);
		im.SurfaceEnd();

		mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
		mat.AlbedoColor = color;

		this.GetTree().Root.AddChild(m_instance);
		return m_instance;
	}

	public MeshInstance3D PointRenderer(Vector3 position, float radius, Color color)
	{
        MeshInstance3D m_instance = new MeshInstance3D();
		SphereMesh sm = new SphereMesh();
		OrmMaterial3D mat = new OrmMaterial3D();

		m_instance.Mesh = sm;
		m_instance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
		m_instance.Position = position;

		sm.Radius = radius;
		sm.Height = radius * 2;
		sm.Material = mat;

		mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
		mat.AlbedoColor = color;

        this.GetTree().Root.AddChild(m_instance);
        return m_instance;
    }
}
