shader_type spatial;

uniform sampler2D fur_tex;
uniform sampler2D fur_params_tex;
uniform float max_len=0.5;
uniform float cur_layer:hint_range(0,1);
uniform vec3 gravity = vec3(0,-1,0);

void vertex() {
	vec3 forceDirection = vec3(0.0);
	vec4 position = vec4(VERTEX, 1.0);
	
	// Wind
	forceDirection.x = sin(TIME + position.x * 0.05) * 0.2;
	forceDirection.y = cos(TIME * 0.7 + position.y * 0.04) * 0.2;
	forceDirection.z = sin(TIME * 0.7 + position.y * 0.04) * 0.2;
	
	vec3 displacement = forceDirection + gravity;
	
	float displacementFactor = pow(cur_layer, 3.0);
	vec4 aNormal = vec4(NORMAL,1.0);
	aNormal.xyz += displacement * displacementFactor;
	
	vec4 furcolor = texture(fur_tex, UV);
	
	vec4 n = normalize(aNormal) * max_len * cur_layer * furcolor.b * furcolor.g;
	vec4 wpos = vec4(VERTEX.xyz + n.xyz, 1.0);
    VERTEX = wpos.xyz;
    
    vec3 pos = VERTEX.xyz + NORMAL * max_len * cur_layer * (furcolor.b / 300.0);
    VERTEX = pos;

    
    // twp possibilities to offset the vertex:
    // VERTEX.xyz + NORMAL * offset
    // VERTEX + NORMAL * MaxHairLength * CurrentLayer
}


void fragment(){
    vec4 fur_params = texture(fur_params_tex, UV);
	vec3 fur_color = texture(fur_tex, UV).rgb;

   	ALBEDO = fur_color;
	if (fur_params.g > 0.5) {
		ALPHA = fur_params.r * fur_color.g;
	} else {
		ALPHA = 0.0;
	}
}
