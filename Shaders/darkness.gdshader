shader_type canvas_item;

uniform vec4 base_color : source_color = vec4(0, 0, 0, 1.0);
uniform float fade_progress : hint_range(0.0, 1.0) = 0.0;
uniform float softness : hint_range(0.0, 1.0) = 0.3;

void fragment() {
    // Centered UV coordinates (0.5, 0.5 = center of screen)
    vec2 centered_uv = abs(UV - vec2(0.5));

    // Distance to the farthest edge (rectangular fade)
	float dist_to_edge = length(centered_uv) * 2.0;

    // Fade in from edges toward center
    float mask = smoothstep(1.0, 1.0 - 3.*softness, 1.*fade_progress + (1.0 - 2.*dist_to_edge));

    COLOR = vec4(base_color.rgb, base_color.a * mask);
}