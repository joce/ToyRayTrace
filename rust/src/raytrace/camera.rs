use super::vec3;
use super::Ray;
use super::Vec3;
use std::f32::consts;

pub struct Camera {
    origin: Vec3,
    lower_left_corner: Vec3,
    horizontal: Vec3,
    vertical: Vec3,
    u: Vec3,
    v: Vec3,
    w: Vec3,
    lens_radius: f32,
}

impl Camera {
    pub fn new(
        look_from: Vec3,
        look_at: Vec3,
        v_up: Vec3,
        v_fov: f32,
        aspect: f32,
        aperture: f32,
        focus_dist: f32,
    ) -> Self {
        let lens_radius = aperture / 2.0;
        let theta = v_fov * consts::PI / 180.0;
        let half_height = (theta / 2.0).tan();
        let half_width = aspect * half_height;

        let w = vec3::normalize(look_from - look_at);
        let u = vec3::normalize(vec3::cross(v_up, w));
        let v = vec3::cross(w, u);
        let origin = look_from;
        let lower_left_corner = origin - u * focus_dist * half_width - v * focus_dist * half_height - w * focus_dist;
        let horizontal = u * 2.0 * focus_dist * half_width;
        let vertical = v * 2.0 * focus_dist * half_height;
        Camera {
            origin,
            lower_left_corner,
            horizontal,
            vertical,
            u,
            v,
            w,
            lens_radius,
        }
    }

    pub fn get_ray(&self, s: f32, t: f32) -> Ray {
        let rd = vec3::random_in_unit_disc() * self.lens_radius;
        let offset = self.u * rd.x + self.v * rd.y;
        Ray::new(
            self.origin + offset,
            self.lower_left_corner + self.horizontal * s + self.vertical * t - self.origin - offset,
        )
    }
}
