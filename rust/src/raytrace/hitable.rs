use super::vec3;
use super::Material;
use super::Ray;
use super::Vec3;

#[derive(Copy, Clone, Default)]
pub struct HitRecord {
    pub t: f32,
    pub p: Vec3,
    pub normal: Vec3,
    pub mat: Material,
}

impl HitRecord {
    pub fn new(t: f32, p: Vec3, normal: Vec3, mat: Material) -> Self {
        HitRecord { t, p, normal, mat }
    }
}

pub trait Hitable {
    fn hit(&self, r: &Ray, t_min: f32, t_max: f32) -> Option<HitRecord>;
}

pub struct Sphere {
    center: Vec3,
    radius: f32,
    mat: Material,
}

impl Sphere {
    pub fn new(center: Vec3, radius: f32, mat: Material) -> Self {
        Sphere { center, radius, mat }
    }
}

impl Hitable for Sphere {
    fn hit(&self, r: &Ray, t_min: f32, t_max: f32) -> Option<HitRecord> {
        let oc = r.origin - self.center;
        let a = vec3::dot(r.direction, r.direction);
        let b = vec3::dot(oc, r.direction);
        let c = vec3::dot(oc, oc) - self.radius * self.radius;
        let discrimimant = b * b - a * c;
        if discrimimant > 0.0 {
            let disc_sqrt = discrimimant.sqrt();
            let temp = (-b - disc_sqrt) / a;
            if temp < t_max && temp > t_min {
                let p = r.point_at_parameter(temp);
                return Some(HitRecord::new(temp, p, (p - self.center) / self.radius, self.mat));
            }

            let temp = (-b + disc_sqrt) / a;
            if temp < t_max && temp > t_min {
                let p = r.point_at_parameter(temp);
                return Some(HitRecord::new(temp, p, (p - self.center) / self.radius, self.mat));
            }
        }
        None
    }
}

pub struct HitableList {
    pub items: Vec<Box<Hitable>>,
}

impl HitableList {
    pub fn new(items: Vec<Box<Hitable>>) -> Self {
        HitableList { items }
    }
}

impl Hitable for HitableList {
    fn hit(&self, r: &Ray, t_min: f32, t_max: f32) -> Option<HitRecord> {
        let mut temp_rec = HitRecord::default();
        let mut hit_anything = false;
        let mut closest_so_far = t_max;

        for it in self.items.iter() {
            match it.hit(r, t_min, closest_so_far) {
                Some(rec) => {
                    closest_so_far = rec.t;
                    temp_rec = rec;
                    hit_anything = true;
                }
                None => {}
            }
        }
        if hit_anything {
            Some(temp_rec)
        } else {
            None
        }
    }
}
