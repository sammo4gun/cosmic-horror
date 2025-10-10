shader_type canvas_item;

uniform float flicker_strength : hint_range(0.0, 1.0) = 0.3; // 1.0 = fully visible, 0.0 = fully transparent
uniform float flicker_speed : hint_range(0.0, 200.0) = 100.0; // 1.0 = fully visible, 0.0 = fully transparent

void fragment() {
	vec4 tex_color = texture(TEXTURE, UV);
	tex_color.a *= 1.+(flicker_strength*sin(TIME*flicker_speed));
    tex_color *= COLOR; // COLOR is the modulate value from the node
	COLOR = tex_color;
}