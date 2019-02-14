use rand::Rng;
use std::f32;
use std::fs::File;
use std::io::{BufWriter, Write};
use std::path::Path;

mod raytrace;

use raytrace::*;

struct Scene {
    pub world: HitableList,
    pub camera: Camera,
}

impl Scene {
    pub fn new(world: HitableList, camera: Camera) -> Self {
        Scene { world, camera }
    }

    pub fn get_simple(nx: i32, ny: i32) -> Self {
        let look_from = Vec3::new(3.0, 3.0, 2.0);
        let look_at = Vec3::new(0.0, 0.0, -1.0);
        let dist_to_focus = (look_from - look_at).length();
        let aperture = 2.0;
        let camera = Camera::new(
            look_from,
            look_at,
            Vec3::Y,
            20.0,
            (nx as f32) / (ny as f32),
            aperture,
            dist_to_focus,
        );

        let world = HitableList::new(vec![
            Box::new(Sphere::new(
                Vec3::new(0.0, 0.0, -1.0),
                0.5,
                Material::Lambertian {
                    albedo: Vec3::new(0.8, 0.3, 0.3),
                },
            )),
            Box::new(Sphere::new(
                Vec3::new(0.0, -100.5, -1.0),
                100.0,
                Material::Lambertian {
                    albedo: Vec3::new(0.8, 0.8, 0.0),
                },
            )),
            Box::new(Sphere::new(
                Vec3::new(1.0, 0.0, -1.0),
                0.5,
                Material::Metal {
                    albedo: Vec3::new(0.8, 0.6, 0.2),
                    fuzz: 0.5,
                },
            )),
            Box::new(Sphere::new(
                Vec3::new(-1.0, 0.0, -1.0),
                0.5,
                Material::Dielectric { ref_idx: 1.5 },
            )),
            Box::new(Sphere::new(
                Vec3::new(-1.0, 0.0, -1.0),
                -0.45,
                Material::Dielectric { ref_idx: 1.5 },
            )),
        ]);

        Scene { world, camera }
    }

    pub fn get_complex(nx: i32, ny: i32) -> Self {
        let look_from = Vec3::new(11.0, 2.0, 2.5);
        let look_at = Vec3::new(0.0, 0.25, 0.0);
        let dist_to_focus = (look_from - Vec3::new(4.0, 1.0, 0.0)).length();
        let aperture = 0.1;
        let camera = Camera::new(
            look_from,
            look_at,
            Vec3::Y,
            25.0,
            (nx as f32) / (ny as f32),
            aperture,
            dist_to_focus,
        );

        let mut world: Vec<Box<Hitable>> = vec![Box::new(Sphere::new(
            Vec3::new(0.0, -1000.0, -1.0),
            1000.0,
            Material::Lambertian {
                albedo: Vec3::new(0.5, 0.5, 0.5),
            },
        ))];

        let mut rng = rand::thread_rng();

        for a in -11..11 {
            for b in -11..11 {
                let choose_mat: f32 = rng.gen();
                let mat: Material;
                if choose_mat < 0.8 {
                    mat = Material::Lambertian {
                        albedo: Vec3::new(
                            rng.gen::<f32>() * rng.gen::<f32>(),
                            rng.gen::<f32>() * rng.gen::<f32>(),
                            rng.gen::<f32>() * rng.gen::<f32>(),
                        ),
                    }
                } else if choose_mat < 0.95 {
                    mat = Material::Metal {
                        albedo: Vec3::new(
                            0.5 * (1.0 + rng.gen::<f32>()),
                            0.5 * (1.0 + rng.gen::<f32>()),
                            0.5 * (1.0 + rng.gen::<f32>()),
                        ),
                        fuzz: 0.5 * rng.gen::<f32>(),
                    }
                } else {
                    mat = Material::Dielectric { ref_idx: 1.5 };
                }

                let center = Vec3::new(
                    (a as f32) + 0.9 * rng.gen::<f32>(),
                    0.2,
                    (b as f32) + 0.9 * rng.gen::<f32>(),
                );

                if (center - Vec3::new(4.0, 0.2, 0.0)).length() > 0.9 {
                    world.push(Box::new(Sphere::new(center, 0.2, mat)));
                }
            }
        }

        world.push(Box::new(Sphere::new(
            Vec3::new(0.0, 1.0, 0.0),
            1.0,
            Material::Dielectric { ref_idx: 1.5 },
        )));
        world.push(Box::new(Sphere::new(
            Vec3::new(-4.0, 1.0, 0.0),
            1.0,
            Material::Lambertian {
                albedo: Vec3::new(0.4, 0.2, 0.1),
            },
        )));
        world.push(Box::new(Sphere::new(
            Vec3::new(4.0, 1.0, 0.0),
            1.0,
            Material::Metal {
                albedo: Vec3::new(0.7, 0.6, 0.5),
                fuzz: 0.0,
            },
        )));

        Scene {
            world: HitableList::new(world),
            camera,
        }
    }
}

fn color(r: &Ray, world: &Hitable, depth: i32) -> Vec3 {
    match world.hit(r, 0.0001, f32::MAX) {
        Some(rec) => {
            if depth < 50 {
                match rec.mat.scatter(r, &rec) {
                    Some(scat) => scat.attenuation * color(&scat.scattered, world, depth + 1),
                    None => Vec3::ZERO,
                }
            } else {
                Vec3::ZERO
            }
        }
        None => {
            let unit_direction = vec3::normalize(r.direction);
            let t = 0.5 * (unit_direction.y + 1.0);
            Vec3::ONE * (1.0 - t) + Vec3::new(0.5, 0.7, 1.0) * t
        }
    }
}

fn main() {
    let path = Path::new("c:\\Dump\\rust_background.ppm");
    let f = File::create(path).expect("Unable to open file");
    let mut f = BufWriter::new(f);

    let nx = 1200;
    let ny = 800;
    let ns = 100;
    writeln!(f, "P3\n{} {}\n255", nx, ny).expect("Can't write header");

    let scene = Scene::get_complex(nx, ny);

    let mut rng = rand::thread_rng();

    for y in (0..ny).rev() {
        for x in 0..nx {
            let mut col = Vec3::default();
            for _s in 0..ns {
                let u = ((x as f32) + rng.gen::<f32>()) / (nx as f32);
                let v = ((y as f32) + rng.gen::<f32>()) / (ny as f32);

                let r = scene.camera.get_ray(u, v);

                col = col + color(&r, &scene.world, 0);
            }
            col = col / (ns as f32);

            let r = (col.x.sqrt() * 255.99) as i32;
            let g = (col.y.sqrt() * 255.99) as i32;
            let b = (col.z.sqrt() * 255.99) as i32;

            writeln!(f, "{} {} {}", r, g, b).expect("Can't write triplet");
        }
    }
}
