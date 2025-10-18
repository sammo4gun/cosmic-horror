shader_type canvas_item;

uniform float radius = .3;
uniform int cell_amount = 10;
uniform float rotation_speed = 60.0;
uniform vec3 world_period = vec3(9.5, 9.5, 9.5);

// color palette to skin the noise map
uniform vec4 tangerine : source_color = vec4(0.40, 0.38, 0.37, 1.); // darker highlight
uniform vec4 sunset : source_color = vec4(0.36, 0.35, 0.34, 1.); // darker pale gray
uniform vec4 moonstone : source_color = vec4(0.30, 0.30, 0.31, 1.); // darker moonstone gray
uniform vec4 shadow : source_color = vec4(0.24, 0.25, 0.26, 1.); // darker shadow
uniform vec4 emerald : source_color = vec4(0.18, 0.19, 0.20, 1.); // darker gray
uniform vec4 deep_shadow : source_color = vec4(0.10, 0.11, 0.12, 1.); // deepest darker shadow

uniform int polygon_sides = 9;          // number of sides
uniform float jag_amplitude = 0.02;     // how far the edge can protrude
uniform float jag_freq1 = 5.2;
uniform float jag_freq2 = 5.3;
uniform float jag_time_speed = 0.01;
uniform bool use_jagged = true;
uniform bool antialias_edge = false;


// modulo function that returns positive wrapped values. (13 % 5 = 3, but -13 % 5 = 2)
vec3 modulo(vec3 divident, vec3 divisor){
	vec3 positiveDivident = mod(divident, divisor) + divisor;
	return mod(positiveDivident, divisor);
}

// 3 dimensional pseudo random number generator
vec3 random(vec3 value){
	vec3 return_value = vec3( dot(value, vec3(127.1,311.7, 201.9) ),
				  dot(value, vec3(269.5,183.3, 367.7) ),
				  dot(value, vec3(245.1,367.7, 105.6) ) );
	return -1.0 + 2.0 * fract(sin(return_value) * 43758.5453123);
}

// seamless noise function adapted from Godot Shaders seamless Perlin Noise function
float seamless_noise(vec2 uv, vec3 _period, mat3 rot_p) {
	// equation of a sphere at coordinates (0.5, 0.5)
	// (x-.5)^2 + (y-.5)^2 + z^2 = radius^2
	float w = 0.0; // flat: no spherical depth, square mapping
	vec3 uvw = rot_p * vec3(uv, w); // multiply by rotation vector to spin world
	uvw = vec3(uvw * float(cell_amount)); // multiply by cell amount and then round to create discrete cells
	vec3 cellsMinimum = floor(uvw);
	vec3 cellsMaximum = ceil(uvw);
	vec3 uvw_fract = fract(uvw);
	
	// wrap every period
	cellsMinimum = modulo(cellsMinimum, _period);
	cellsMaximum = modulo(cellsMaximum, _period);
	
	// calc lerp
	vec3 blur = smoothstep(0.0, 1.0, uvw_fract);
	
	// generate cube of pseudo-random values for every pixel
	vec3 p_000 = random(vec3(cellsMinimum.x, cellsMinimum.y, cellsMinimum.z));
	vec3 p_100 = random(vec3(cellsMaximum.x, cellsMinimum.y, cellsMinimum.z));
	vec3 p_010 = random(vec3(cellsMinimum.x, cellsMaximum.y, cellsMinimum.z));
	vec3 p_110 = random(vec3(cellsMaximum.x, cellsMaximum.y, cellsMinimum.z));
	vec3 p_001 = random(vec3(cellsMinimum.x, cellsMinimum.y, cellsMaximum.z));
	vec3 p_101 = random(vec3(cellsMaximum.x, cellsMinimum.y, cellsMaximum.z));
	vec3 p_011 = random(vec3(cellsMinimum.x, cellsMaximum.y, cellsMaximum.z));
	vec3 p_111 = random(vec3(cellsMaximum.x, cellsMaximum.y, cellsMaximum.z));
	
	vec3 fraction = fract(uvw);
	
	// return a smoothed version of the noise
	return mix(mix( mix( dot( p_000, fraction - vec3(0, 0, 0) ),
                     dot( p_100, fraction - vec3(1, 0, 0) ), blur.x),
                mix( dot( p_010, fraction - vec3(0, 1, 0) ),
                     dot( p_110, fraction - vec3(1, 1, 0) ), blur.x), blur.y),
			mix( mix( dot( p_001, fraction - vec3(0, 0, 1) ),
                     dot( p_101, fraction - vec3(1, 0, 1) ), blur.x),
                mix( dot( p_011, fraction - vec3(0, 1, 1) ),
                     dot( p_111, fraction - vec3(1, 1, 1) ), blur.x), blur.y), blur.z) * 0.8 + 0.5;
}

// map perlin noise to color pallette
// fluxuate moonstone color by sin(time) to create waves
vec4 pixel_color_noise(vec2 uv, vec3 _period, mat3 rot_p){
	vec2 grid_uv = round(uv * float(300)) / float(300);
	float perlin = seamless_noise(grid_uv, _period, rot_p);
	vec4 color = vec4(0);
	
	if (perlin < 0.10){
		color = shadow;
	}else if (perlin < 0.35){
		color = moonstone;
	}else if (perlin < 0.45){
		color = sunset;
	}else if (perlin < 0.80){
		color = tangerine;
	}else if (perlin < .85){
		color = emerald;
	}else if (perlin < 1.){
		color = deep_shadow;
	};
	return color;
}

void fragment() {
	float theta = float(-TIME/rotation_speed);
	vec3 rot_1 = vec3(.5*(1.-cos(theta)) + cos(theta), .5*(1.-cos(theta)), -(1./sqrt(2))*sin(theta));
	vec3 rot_2 = vec3(.5*(1.-cos(theta)), .5*(1.-cos(theta)) + cos(theta),(1./sqrt(2))*sin(theta));
	vec3 rot_3 = vec3((1./sqrt(2))*sin(theta), -(1./sqrt(2))*sin(theta), cos(theta));
	mat3 rot = mat3(rot_1, rot_2, rot_3);

	vec2 p = UV - vec2(0.5);
	float dist = length(p);
	float angle = atan(p.y, p.x); // -pi..pi

	// Regular polygon radius at this angle
	float sides_f = max(3.0, float(polygon_sides));
	float sector = 2.0 * PI / sides_f;
	float local = abs(mod(angle + sector * 0.5, sector) - sector * 0.5);
	float base_poly_r = radius * cos(sector * 0.5) / cos(local);

	// Jagged variation
	float jag = 0.0;
	if (use_jagged) {
		// Layer more frequency and sharpen via power & max
		float a = sin(angle * jag_freq1 + TIME * jag_time_speed);
		float b = sin(angle * jag_freq2 - TIME * jag_time_speed * 0.7);
		float c = sin(angle * (jag_freq1 + jag_freq2) * 0.5 + TIME * jag_time_speed * 0.35);

		// Combine then sharpen peaks (pow >1 narrows peaks); renormalize
		float combined = max(a, max(b, c));
		float sharp = sign(combined) * pow(abs(combined), 3.0); // sharper spikes

		// Add a crisp triangle component
		float tri_phase = fract(angle * jag_freq1 / (2.0 * PI));
		float tri = 1.0 - abs(2.0 * tri_phase - 1.0); // 0..1
		tri = tri * 2.0 - 1.0; // -1..1

		jag = jag_amplitude * (sharp + 0.4 * tri);
	}
	float poly_r = base_poly_r + jag;

	// World color (still uses spherical w internally; acceptable for mask)
	vec4 color = pixel_color_noise(UV, world_period, rot);

	// Edge mask
	float edge = poly_r - dist;
	float mask;
	if (antialias_edge) {
		float aa = fwidth(dist);
		mask = smoothstep(0.0, aa, edge);
	} else {
		mask = edge > 0.0 ? 1.0 : 0.0;
	}

	COLOR = vec4(color.rgb * mask, mask);
}
