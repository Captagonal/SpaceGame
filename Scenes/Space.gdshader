shader_type sky;

uniform vec3 space_color : source_color = vec3(0.02, 0.01, 0.05);
uniform float star_density : hint_range(0.0, 1.0) = 0.5;

void sky() {
    vec3 final_color = space_color;
    
    // 1. Scale up EYEDIR to create a 'grid' in space
    // Higher number = smaller, more frequent stars
    vec3 grid_dir = EYEDIR * 250.0; 
    
    // 2. Floor the direction to get a stable ID for this 'chunk' of sky
    vec3 id = floor(grid_dir);
    
    // 3. Generate a stable random value for this specific chunk
    float star_rand = fract(sin(dot(id, vec3(12.9898, 78.233, 45.164))) * 43758.5453);
    
    // 4. Determine star presence based on density
    if (star_rand > 1.0 - (star_density * 0.02)) {
        // Calculate distance from center of the 'grid cell' to make them round
        float dist = distance(fract(grid_dir), vec3(0.5));
        
        // Use smoothstep so stars aren't flickering 1-pixel dots
        float star_intensity = smoothstep(0.4, 0.2, dist);
        final_color += vec3(star_intensity);
    }

    COLOR = final_color;
}

