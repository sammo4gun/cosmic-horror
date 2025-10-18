shader_type canvas_item;

uniform float radius = .1;
uniform int cell_amount = 5;
uniform float rotation_speed = 8.0;
uniform vec3 world_period = vec3(9.5, 9.5, 9.5);
// Offset the cloud periods by adding a displacement vector to each
uniform vec3 cloud_period = vec3(1.0, 5.0, 2.0);   
uniform vec3 cloud_period_2 = vec3(3.5, 6.5, 5.0);

// color palette to skin the noise map
uniform vec4 base_level      : source_color = vec4(0.50, 0.42, 0.30, 1.0); // muted brown
uniform vec4 mid_low_level   : source_color = vec4(0.64, 0.55, 0.40, 1.0); // tan
uniform vec4 mid_level       : source_color = vec4(0.75, 0.66, 0.48, 1.0); // warm beige
uniform vec4 mid_high_level  : source_color = vec4(0.85, 0.76, 0.58, 1.0); // pale sand
uniform vec4 high_level      : source_color = vec4(0.95, 0.90, 0.74, 1.0); // creamy yellow
uniform vec4 accent_level    : source_color = vec4(0.70, 0.60, 0.38, 1.0); // slightly stronger band

uniform float ring_inner = 0.12;        // just outside planet radius
uniform float ring_outer = 0.20;        // outer edge
uniform float ring_feather = 0.06;      // edge softness
uniform float ring_tilt = 0.6;         // tilt angle in radians
uniform float ring_band_scale = 35.0;   // number of subtle bands
uniform float ring_time_scroll = 0.02;  // slow rotation
uniform vec4 ring_col_a : source_color = vec4(0.85, 0.70, 0.50, 0.15);
uniform vec4 ring_col_b : source_color = vec4(0.70, 0.55, 0.40, 0.05);
uniform vec4 ring_col_c : source_color = vec4(0.95, 0.85, 0.70, 0.45);

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

// Stripe noise function to create lines across the sphere instead of Perlin splodges
float seamless_noise(vec2 uv, vec3 _period, mat3 rot_p) {
	// equation of a sphere at coordinates (0.5, 0.5)
	// (x-.5)^2 + (y-.5)^2 + z^2 = radius^2
	float w = sqrt(pow(radius*1.18, 2.) - pow(uv.x - .5, 2.) - pow(uv.y - .5, 2.));
	float theta = TIME / 500.;
	// rotate about 0,0 on UV coords and with the rot_p matrix (see below)
	// Rotate clouds to make them more horizontal (stripes along X axis)
	mat3 rot_0 = mat3(
		vec3(-1, 0, 0),
		vec3(cos(theta), 0, -sin(theta)),
		vec3(sin(theta),0, cos(theta))
	);
	vec3 uvw = rot_0*(rot_p * vec3(uv, w));
	uvw = vec3(uvw * float(cell_amount) * 3.5);
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


vec4 pixel_color_noise(vec2 uv, vec3 _period, mat3 rot_p){
	vec2 grid_uv = round(uv * float(300)) / float(300);
	float perlin = seamless_noise(grid_uv, _period, rot_p);
	vec4 color = vec4(0);
	
	if (perlin < 0.20+sin(TIME)/60.){
		color = base_level;
	}else if (perlin < 0.40+sin(TIME)/30.){
		color = mid_low_level;
	}else if (perlin < 0.65+cos(TIME)/20.){
		color = mid_level;
	}else if (perlin < 0.85){
		color = mid_high_level;
	}else if (perlin < 0.95){
		color = accent_level;
	}else{
		color = high_level;
	};
	return color;
}

vec4 clouds(vec2 uv, vec3 _period, mat3 rot_p){
	uv = round(uv * float(300)) / float(300);
	
	// equation of a sphere at coordinates (0.5, 0.5)
	// (x-.5)^2 + (y-.5)^2 + z^2 = radius^2
	float w = sqrt(pow(radius*1.18, 2.) - pow(uv.x - .5, 2.) - pow(uv.y - .5, 2.));
	float theta = TIME / 100.;
	// rotate about 0,0 on UV coords and with the rot_p matrix (see below)
	// Rotate clouds to make them more horizontal (stripes along X axis)
	mat3 rot_0 = mat3(
		vec3(-1, 0, -1),
		vec3(cos(theta), 0, -sin(theta)),
		vec3(sin(theta),0, cos(theta))
	);
	vec3 uvw = rot_0*(rot_p * vec3(uv, w));
	uvw = vec3(uvw * float(cell_amount) * 0.8);
	vec3 cellsMinimum = floor(uvw);
	vec3 cellsMaximum = ceil(uvw);
	vec3 uvw_fract = fract(uvw);
	
	vec3 blur = smoothstep(0.0, 1.0, uvw_fract);
	
	cellsMinimum = modulo(cellsMinimum, _period);
	cellsMaximum = modulo(cellsMaximum, _period);
	
	// see smooth_noise function for comments on this part
	vec3 p_000 = random(random(vec3(cellsMinimum.x, cellsMinimum.y, cellsMinimum.z)));
	vec3 p_100 = random(random(vec3(cellsMaximum.x, cellsMinimum.y, cellsMinimum.z)));
	vec3 p_010 = random(random(vec3(cellsMinimum.x, cellsMaximum.y, cellsMinimum.z)));
	vec3 p_110 = random(random(vec3(cellsMaximum.x, cellsMaximum.y, cellsMinimum.z)));
	vec3 p_001 = random(random(vec3(cellsMinimum.x, cellsMinimum.y, cellsMaximum.z)));
	vec3 p_101 = random(random(vec3(cellsMaximum.x, cellsMinimum.y, cellsMaximum.z)));
	vec3 p_011 = random(random(vec3(cellsMinimum.x, cellsMaximum.y, cellsMaximum.z)));
	vec3 p_111 = random(random(vec3(cellsMaximum.x, cellsMaximum.y, cellsMaximum.z)));
	
	float perlin =  mix(mix( mix( dot( p_000, uvw_fract - vec3(0, 0, 0) ),
                     dot( p_100, uvw_fract - vec3(1, 0, 0) ), blur.x),
                mix( dot( p_010, uvw_fract - vec3(0, 1, 0) ),
                     dot( p_110, uvw_fract - vec3(1, 1, 0) ), blur.x), blur.y),
					mix( mix( dot( p_001, uvw_fract - vec3(0, 0, 1) ),
                     dot( p_101, uvw_fract - vec3(1, 0, 1) ), blur.x),
                mix( dot( p_011, uvw_fract - vec3(0, 1, 1) ),
                     dot( p_111, uvw_fract - vec3(1, 1, 1) ), blur.x), blur.y), blur.z) * 0.8 + 0.5;
	
    vec4 color = vec4(0.);
    // Increase the lower threshold to reduce cloud coverage
    if (perlin < 0.55){
        color = vec4(0.0, 0.0, 0.0, 0.0);
    }else if (perlin < 0.80){
        color = vec4(0.1, 0.1, 0.1, 0.01);
    }else if (perlin < 0.85){
        color = vec4(0.2, 0.2, 0.2, 0.1);
    }else if (perlin < 1.){
        color = vec4(0.3, 0.3, 0.3, 0.2);
    };
    return color;
}

vec4 clouds_2(vec2 uv, vec3 _period, mat3 rot_p){
	uv = round(uv * float(300)) / float(300);
	
	// equation of a sphere at coordinates (0.5, 0.5)
	// (x-.5)^2 + (y-.5)^2 + z^2 = radius^2
	float w = sqrt(pow(radius*1.18, 2.) - pow(uv.x - .5, 2.) - pow(uv.y - .5, 2.));
	float theta = TIME / 100.;
	// rotate about 0,0 on UV coords and with the rot_p matrix (see below)
	// Rotate clouds to make them more horizontal (stripes along X axis)
	mat3 rot_0 = mat3(
		vec3(-1, 0, -1),
		vec3(cos(theta), 0, -sin(theta)),
		vec3(sin(theta),0, cos(theta))
	);
	vec3 uvw = rot_0*(rot_p * vec3(uv, w));
	uvw = vec3(uvw * float(cell_amount) * 0.8);
	vec3 cellsMinimum = floor(uvw);
	vec3 cellsMaximum = ceil(uvw);
	vec3 uvw_fract = fract(uvw);
	
	vec3 blur = smoothstep(0.0, 1.0, uvw_fract);
	
	cellsMinimum = modulo(cellsMinimum, _period);
	cellsMaximum = modulo(cellsMaximum, _period);
	
	// see smooth_noise function for comments on this part
	vec3 p_000 = random(random(vec3(cellsMinimum.x, cellsMinimum.y, cellsMinimum.z)));
	vec3 p_100 = random(random(vec3(cellsMaximum.x, cellsMinimum.y, cellsMinimum.z)));
	vec3 p_010 = random(random(vec3(cellsMinimum.x, cellsMaximum.y, cellsMinimum.z)));
	vec3 p_110 = random(random(vec3(cellsMaximum.x, cellsMaximum.y, cellsMinimum.z)));
	vec3 p_001 = random(random(vec3(cellsMinimum.x, cellsMinimum.y, cellsMaximum.z)));
	vec3 p_101 = random(random(vec3(cellsMaximum.x, cellsMinimum.y, cellsMaximum.z)));
	vec3 p_011 = random(random(vec3(cellsMinimum.x, cellsMaximum.y, cellsMaximum.z)));
	vec3 p_111 = random(random(vec3(cellsMaximum.x, cellsMaximum.y, cellsMaximum.z)));
	
	float perlin =  mix(mix( mix( dot( p_000, uvw_fract - vec3(0, 0, 0) ),
                     dot( p_100, uvw_fract - vec3(1, 0, 0) ), blur.x),
                mix( dot( p_010, uvw_fract - vec3(0, 1, 0) ),
                     dot( p_110, uvw_fract - vec3(1, 1, 0) ), blur.x), blur.y),
					mix( mix( dot( p_001, uvw_fract - vec3(0, 0, 1) ),
                     dot( p_101, uvw_fract - vec3(1, 0, 1) ), blur.x),
                mix( dot( p_011, uvw_fract - vec3(0, 1, 1) ),
                     dot( p_111, uvw_fract - vec3(1, 1, 1) ), blur.x), blur.y), blur.z) * 0.8 + 0.5;
	
    vec4 color = vec4(0.);

	if (perlin < 0.55){
		color = vec4(1.00, 0.60, 0.20, 0.0); // deep orange, transparent
	}else if (perlin < 0.80){
		color = vec4(0.98, 0.65, 0.28, 0.1); // medium orange, semi-transparent
	}else if (perlin < 0.85){
		color = vec4(0.95, 0.55, 0.18, 0.2); // bright orange, more opaque
	}else if (perlin < 1.){
		color = vec4(0.85, 0.40, 0.10, 0.3); // vivid orange accent, semi-transparent
    };
    return color;
}


float ring_hash(vec2 p){
	return fract(sin(dot(p, vec2(127.1,311.7)))*43758.5453);
}
float ring_noise(vec2 p){
	float t = TIME * 0.02; // evolution speed
	float t0 = floor(t);
	float ft = fract(t);

	vec2 i = floor(p);
	vec2 f = fract(p);
	vec2 u = f*f*(3.0 - 2.0*f);

	// deterministic offsets per time slice
	vec2 off0 = vec2(t0 * 17.0, t0 * 23.0);
	vec2 off1 = vec2((t0 + 1.0) * 17.0, (t0 + 1.0) * 23.0);

	// noise at time slice t0
	float a0 = ring_hash(i + off0);
	float b0 = ring_hash(i + vec2(1.0,0.0) + off0);
	float c0 = ring_hash(i + vec2(0.0,1.0) + off0);
	float d0 = ring_hash(i + vec2(1.0,1.0) + off0);
	float n0 = mix(mix(a0,b0,u.x), mix(c0,d0,u.x), u.y);

	// noise at time slice t1
	float a1 = ring_hash(i + off1);
	float b1 = ring_hash(i + vec2(1.0,0.0) + off1);
	float c1 = ring_hash(i + vec2(0.0,1.0) + off1);
	float d1 = ring_hash(i + vec2(1.0,1.0) + off1);
	float n1 = mix(mix(a1,b1,u.x), mix(c1,d1,u.x), u.y);

	// smooth morph between slices
	return mix(n0, n1, ft);
}

vec4 saturn_rings(vec2 uv){
	// center
	vec2 p = uv - vec2(0.5);

	// apply tilt: rotate then squash
	float ct = cos(ring_tilt);
	float st = sin(ring_tilt);
	vec2 pr = vec2(p.x*ct - p.y*st, p.x*st + p.y*ct);
	pr.y *= 0.35;

	float r = length(pr);
	float usable_outer = min(ring_outer, 0.5);

	// hard reject outside ring
	if (r < ring_inner || r > usable_outer)
		return vec4(0.0);

	// feather only inside bounds
	float innerFade = smoothstep(ring_inner, ring_inner + ring_feather, r);
	float outerFade = 1.0 - smoothstep(usable_outer - ring_feather, usable_outer, r);
	float mask = innerFade * outerFade;

	// band pattern
	float angle = atan(pr.y, pr.x);
	float scroll = TIME * ring_time_scroll;

	// base band seed
	float band_seed = r * ring_band_scale + angle * 2.0 + scroll;

	// add perlin-like noise (radial + angular)
	float n = ring_noise(vec2(r * 55.0, angle * 9.0 + TIME * 0.03));
	float n2 = ring_noise(vec2(r * 18.0 + n*4.0, angle * 3.0 - TIME * 0.02));

	// perturb band seed with noise for irregular spacing
	band_seed += n * 3.0 + n2 * 1.5;

	float bands = fract(band_seed);
	float sharp = smoothstep(0.15, 0.85, bands);

	// micro detail with noise mixed in
	float micro = fract(sin(band_seed * 57.13) * 43758.23);
	micro = mix(micro, n, 0.35);
	micro = mix(micro, n2, 0.25);

	vec4 c1 = mix(ring_col_a, ring_col_b, sharp);
	vec4 c2 = mix(c1, ring_col_c, micro * 0.6);
	vec4 col = c2;

	// color variation driven by noise
	float tonal = mix(0.82, 1.18, n);
	float warm_shift = mix(-0.04, 0.05, n2);
	col.rgb *= tonal;
	col.rgb += warm_shift * ring_col_c.rgb * 0.25;

	// subtle dark gaps
	float gap = smoothstep(0.32, 0.50, bands) - smoothstep(0.50, 0.68, bands);
	col.rgb *= mix(1.0, 0.85, gap * (0.4 + n*0.3));

	// depth fade
	float depthFade = mix(1.0, 0.55, smoothstep(-0.05, 0.25, pr.y));
	col.rgb *= depthFade;

	// alpha modulation with noise
	col.a = mask;
	col.a *= mix(0.75, 1.0, n);
	col.a *= mix(0.70, 1.0, sharp);

	if (col.a < 0.4) {
		col.a = 0.0;
	}

	// do not cover planet body
	if (length(p) < radius){
		return vec4(0.0);
	}

	return col;
}

void fragment() {
	float theta = float(-TIME/rotation_speed);
	// construct rotation matrix about 1/sqrt(2)i, 1/sqrt(2)j, 0k unit vector
	// because UV coordinates start at the top left and rotating the noise
	// rotates all the noise, not just the sphere so if you rotate about any
	// axis of the coordinate system the noise moves through the sphere
	// and it doesn't look like it's spinning (duh)
	// it's also why it rotates on a tilted axis
	// because this was the easiest solution I found
	vec3 rot_1 = vec3(.5*(1.-cos(theta)) + cos(theta), .5*(1.-cos(theta)), -(1./sqrt(2))*sin(theta));
	vec3 rot_2 = vec3(.5*(1.-cos(theta)), .5*(1.-cos(theta)) + cos(theta),(1./sqrt(2))*sin(theta));
	vec3 rot_3 = vec3((1./sqrt(2))*sin(theta), -(1./sqrt(2))*sin(theta), cos(theta));
	mat3 rot = mat3(rot_1, rot_2, rot_3);
	
	float dis = distance(vec2(.5, .5), UV);
	
	vec4 color = pixel_color_noise(UV, world_period, rot); // get world colors
	vec4 clouds = clouds(UV, cloud_period, rot); // get cloud colors
	vec4 clouds_2 = clouds_2(UV, cloud_period_2, rot); // get cloud colors
	//vec4 clouds_2 = clouds(UV, cloud_period*1.5, rot); // get secondary cloud colors
	//vec4 clouds_3 = clouds3(UV, cloud_period*2.0, rot); // get tertiary cloud colors
	
	COLOR = vec4(0., 0., 0., 1);
	COLOR = COLOR + saturn_rings(UV) * 0.5; // add the rings
	if (dis < radius) {
		COLOR.rgb = color.rgb; // draw the world
	}
    
    if (COLOR.rgb == vec3(0.)) {
        COLOR.a = .0;
    }

	
	COLOR = COLOR + clouds; // add the clouds
	COLOR = COLOR + clouds_2*0.25; // add the clouds
	//COLOR = COLOR + clouds_2 * 0.5; // add the clouds
	//COLOR = COLOR + clouds; // add the clouds
	//COLOR = COLOR + clouds_2 * 0.25; // add the secondary clouds with reduced opacity
	//COLOR = COLOR + clouds_3 * 0.5; // add the tertiary clouds with further reduced opacity
}

