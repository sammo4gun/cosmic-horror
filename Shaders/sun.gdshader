shader_type canvas_item;

/*
2D version of the sun shader (use on a Sprite/TextureRect with a square texture).
Renders a spherical-looking sun + corona inside the quad.
*/

uniform vec4 surface_color : source_color = vec4(1.0, 0.85, 0.35, 1.0);
uniform float surface_intensity : hint_range(0.0, 50.0) = 12.0;
uniform float corona_intensity : hint_range(0.0, 200.0) = 40.0;
uniform float corona_power : hint_range(0.1, 8.0) = 2.5;

uniform float granulation_scale : hint_range(0.1, 20.0) = 6.0;
uniform float granulation_contrast : hint_range(0.0, 5.0) = 1.6;
uniform float turbulence_scale : hint_range(0.1, 10.0) = 2.5;
uniform float flow_speed : hint_range(0.0, 5.0) = 0.25;

uniform float limb_darkening : hint_range(0.0, 1.0) = 0.55;
uniform float limb_power : hint_range(0.1, 5.0) = 1.4;

uniform float spot_scale : hint_range(0.1, 10.0) = 1.2;
uniform float spot_threshold : hint_range(0.0, 1.0) = 0.58;
uniform float spot_darkness : hint_range(0.0, 1.0) = 0.55;

uniform float flicker_strength : hint_range(0.0, 1.0) = 0.15;

uniform bool enable_spots = true;
uniform bool enable_time = true;
uniform float corona_outer : hint_range(0.0, 1.0) = 0.3; // how far corona extends past disc
uniform float antialias : hint_range(0.0, 0.02) = 0.003;

float hash(vec3 p) {
    p = fract(p * 0.3183099 + vec3(0.11,0.17,0.23));
    p *= 17.0;
    return fract(p.x * p.y * p.z * (p.x + p.y + p.z));
}

float value_noise(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    vec3 u = f*f*(3.0 - 2.0*f);
    float n000 = hash(i + vec3(0,0,0));
    float n100 = hash(i + vec3(1,0,0));
    float n010 = hash(i + vec3(0,1,0));
    float n110 = hash(i + vec3(1,1,0));
    float n001 = hash(i + vec3(0,0,1));
    float n101 = hash(i + vec3(1,0,1));
    float n011 = hash(i + vec3(0,1,1));
    float n111 = hash(i + vec3(1,1,1));
    float nx00 = mix(n000, n100, u.x);
    float nx10 = mix(n010, n110, u.x);
    float nx01 = mix(n001, n101, u.x);
    float nx11 = mix(n011, n111, u.x);
    float nxy0 = mix(nx00, nx10, u.y);
    float nxy1 = mix(nx01, nx11, u.y);
    return mix(nxy0, nxy1, u.z);
}

float fbm(vec3 p) {
    float a = 0.0;
    float amp = 0.5;
    for (int i=0; i<5; i++) {
        a += value_noise(p) * amp;
        p *= 2.02;
        amp *= 0.5;
    }
    return a;
}

void fragment() {
    // Centered UV in -1..1
    vec2 uv = (UV - 0.5) * 5.0;
    float r = length(uv);

    // Sphere projection normal (z up toward viewer)
    float z = sqrt(max(0.0, 1.0 - r*r));
    vec3 normal = normalize(vec3(uv.x, uv.y, z));

    float ndv = z; // like dot(NORMAL, view_dir)

    float t = enable_time ? TIME : 0.0;

    // Longitude/latitude (match 3D version roughly)
    float lon = atan(normal.x, normal.z);
    float lat = asin(clamp(normal.y, -1.0, 1.0));
    vec2 suv = vec2(lon / PI, lat / (0.5 * PI)); // -1..1 approx

    // Flow rotation
    float rot = t * flow_speed;
    mat2 R = mat2(vec2(cos(rot), sin(rot)), vec2(-sin(rot), cos(rot)));
    vec2 flow_uv = R * suv * granulation_scale;

    vec3 p = vec3(flow_uv, t * 0.25);
    vec3 q = normal * turbulence_scale + vec3(0.0, t * 0.15, t * 0.07);

    float g_base = fbm(p);
    float g_turb = fbm(q + g_base * 2.0);
    float granulation = g_base * 0.6 + g_turb * 0.4;
    granulation = pow(clamp(granulation, 0.0, 1.0), granulation_contrast);

    // Limb darkening
    float limb = mix(limb_darkening, 1.0, pow(ndv, limb_power));

    // Sunspots
    float spot_mask = 1.0;
    if (enable_spots) {
        float spot_noise = fbm(normal * spot_scale + vec3(0.0, t * 0.05, 0.0));
        float s_mask = smoothstep(spot_threshold, spot_threshold + 0.05, spot_noise);
        spot_mask = mix(spot_darkness, 1.0, s_mask);
    }

    // Flicker
    float flicker = 1.0 + flicker_strength * (value_noise(vec3(t * 2.3, t * 1.7, t * 3.1)) - 0.5) * 2.0;

    float surface = granulation * limb * spot_mask * flicker;

    // Disc mask (soft edge)
    float disc = 1.0 - smoothstep(1.0 - antialias, 1.0 + antialias, r);

    // Corona: allow outside up to corona_outer
    float rim = pow(1.0 - ndv, corona_power); // ndv->0 at rim
    float corona_region = smoothstep(1.0 + corona_outer, 1.0, r); // 0 outside, 1 at rim
    float corona = rim * corona_region * (0.6 + granulation * 0.4);

    vec3 base_col = surface_color.rgb;
    vec3 col = base_col * (surface_intensity * surface * disc +
                           corona_intensity * corona);

    float alpha = clamp(disc + corona * 0.5, 0.0, 1.0);
    if (alpha <= 0.001) {
        discard;
    }

    COLOR = vec4(col, alpha);
}