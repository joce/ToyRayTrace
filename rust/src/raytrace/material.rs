use super::vec3;
use super::HitRecord;
use super::Ray;
use super::Vec3;
use rand::Rng;

#[derive(Copy, Clone)]
pub enum Material {
    Lambertian { albedo: Vec3 },
    Metal { albedo: Vec3, fuzz: f32 },
    Dielectric { ref_idx: f32 },
}

impl Default for Material {
    fn default() -> Material {
        Material::Lambertian { albedo: Vec3::ZERO }
    }
}

#[derive(Default)]
pub struct ScatterRecord {
    pub attenuation: Vec3,
    pub scattered: Ray,
}

impl ScatterRecord {
    pub fn new(attenuation: Vec3, scattered: Ray) -> Self {
        ScatterRecord { attenuation, scattered }
    }
}

impl Material {
    fn schlick(cosine: f32, ref_idx: f32) -> f32 {
        let r0 = (1.0 - ref_idx) / (1.0 + ref_idx);
        let r0 = r0 * r0;
        r0 + (1.0 - r0) * (1.0 - cosine).powf(5.0)
    }

    pub fn scatter(&self, r: &Ray, rec: &HitRecord) -> Option<ScatterRecord> {
        match self {
            Material::Lambertian { albedo } => {
                let target = rec.p + rec.normal + vec3::random_in_unit_sphere();
                let scat = ScatterRecord::new(*albedo, Ray::new(rec.p, target - rec.p));
                return Some(scat);
            }

            Material::Metal { albedo, fuzz } => {
                let reflected = vec3::reflect(r.direction, rec.normal);
                let scattered = Ray::new(rec.p, reflected + vec3::random_in_unit_sphere() * (*fuzz));
                if vec3::dot(scattered.direction, rec.normal) > 0.0 {
                    return Some(ScatterRecord::new(*albedo, scattered));
                }
            }

            Material::Dielectric { ref_idx } => {
                let outward_normal: Vec3;
                let ni_over_nt: f32;
                let cosine: f32;
                let dot = vec3::dot(r.direction, rec.normal);
                if dot > 0.0 {
                    outward_normal = -rec.normal;
                    ni_over_nt = *ref_idx;
                    cosine = *ref_idx * dot / r.direction.length()
                } else {
                    outward_normal = rec.normal;
                    ni_over_nt = 1.0 / (*ref_idx);
                    cosine = -dot / r.direction.length();
                }

                let reflect_prob: f32;
                let refracted: Vec3;
                match vec3::refract(r.direction, outward_normal, ni_over_nt) {
                    Some(refract) => {
                        reflect_prob = Material::schlick(cosine, *ref_idx);
                        refracted = refract;
                    }
                    None => {
                        reflect_prob = 1.0;
                        refracted = Vec3::ZERO;
                    }
                }

                let mut rng = rand::thread_rng();

                if rng.gen::<f32>() < reflect_prob {
                    return Some(ScatterRecord::new(
                        Vec3::ONE,
                        Ray::new(rec.p, vec3::reflect(r.direction, rec.normal)),
                    ));
                } else {
                    return Some(ScatterRecord::new(Vec3::ONE, Ray::new(rec.p, refracted)));
                }
            }
        }
        None
    }
}
