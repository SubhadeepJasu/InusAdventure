extends Node3D

# Called when the node enters the scene tree for the first time.
func _ready():
	var cr = $Simulation/ColorRect
	var water = $Water
	var sim_tex = $Simulation.get_texture()
	var col_tex = $Collision.get_texture()
	var reflection_probe = $ReflectionProbe

	cr.material.set_shader_parameter('sim_tex', sim_tex)
	cr.material.set_shader_parameter('col_tex', col_tex)

	water.mesh.surface_get_material(0).set_shader_parameter('simulation', sim_tex)
	water.mesh.surface_get_material(0).set_shader_parameter('simulation2', sim_tex)
	
	var water_cam = $Collision/TransformReceiver/WaterCam
	water_cam.size = max(scale.x, scale.z) * 8
	
	reflection_probe.size.x = scale.x * 8
	reflection_probe.size.z = scale.z * 8

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass

