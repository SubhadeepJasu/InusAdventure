@tool
extends Node3D

@export var snap_step: int = 20
@onready var terrain
@onready var camera
var camera_pos: Vector3
var timer = Timer.new()
@export var terrain_scale: float = 1
@export var terrain_height: float = 20
@export var terrain_x_offset: float = 0
@export var terrain_y_offset: float = 0
@export var height_map: Image
@export var color_map: ImageTexture
@export var control_map: ImageTexture
@export var surface_albedo_array: Array[ImageTexture]
@export var surface_normal_array: Array[ImageTexture]


# Called when the node enters the scene tree for the first time.
func _ready():
	terrain = $TerrainPlane
	if not Engine.is_editor_hint():
		camera = get_tree().get_root().get_node("World/MainCamera")

	add_child(timer)
	timer.connect("timeout", Callable(self, "snap"))
	timer.set_wait_time(1)
	snap()

	terrain.get_surface_override_material(0).set("shader_parameter/scale", (terrain_scale / height_map.get_width()))
	terrain.get_surface_override_material(0).set("shader_parameter/terrain_height", terrain_height)
	terrain.get_surface_override_material(0).set("shader_parameter/heightmap", ImageTexture.create_from_image(height_map))
	terrain.get_surface_override_material(0).set("shader_parameter/controlmap", control_map)
	terrain.get_surface_override_material(0).set("shader_parameter/x_offset", terrain_x_offset / height_map.get_width())
	terrain.get_surface_override_material(0).set("shader_parameter/y_offset", terrain_y_offset / height_map.get_height())

func snap():
	var div = 96 / (terrain_scale / height_map.get_width())
	if not Engine.is_editor_hint():
		camera_pos = camera.global_transform.origin
		terrain.global_transform.origin.x = camera_pos.x
		terrain.get_surface_override_material(0).set("shader_parameter/uvx", camera_pos.x/ div)
	else:
		pass
		terrain.get_surface_override_material(0).set("shader_parameter/scale", (terrain_scale / height_map.get_width()))
		terrain.get_surface_override_material(0).set("shader_parameter/terrain_height", terrain_height)
		terrain.get_surface_override_material(0).set("shader_parameter/heightmap", ImageTexture.create_from_image(height_map))
#		terrain.get_surface_override_material(0).set("shader_parameter/albedomap", albedo_map)
		terrain.get_surface_override_material(0).set("shader_parameter/x_offset", terrain_x_offset / height_map.get_width())
		terrain.get_surface_override_material(0).set("shader_parameter/y_offset", terrain_y_offset / height_map.get_height())

	timer.start()
