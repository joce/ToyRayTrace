//#define COMPLEX_SCENE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ToyRayTrace
{
    static class Program
    {
        static Stopwatch s_Stopwatch = new Stopwatch();
        static List<long> s_Times = new List<long>();
        const int k_ImageWidth = 400;
        const int k_ImageHeight = 200;

        public static void Main(string[] args)
        {
            for (var i = 0; i < 1; i++)
            {
                s_Stopwatch.Restart();
                Write(k_ImageWidth, k_ImageHeight);
                s_Stopwatch.Stop();
                s_Times.Add(s_Stopwatch.ElapsedMilliseconds);
            }

            Console.WriteLine($"Pixels where rays got lost: {s_RaysLost}");
            Console.WriteLine($"{s_RaysCast} rays cast");
            Console.WriteLine($"Avg: {s_Times.Average()}, Med: {s_Times.Median()}, Min: {s_Times.Min()}, Max: {s_Times.Max()}");
            Console.WriteLine($"Rays/ms: {s_RaysCast/s_Times.Average()}");
        }

        static long s_RaysCast;
        static long s_RaysLost;

        static readonly Vec3 k_Bluish = new Vec3(0.5f, 0.7f, 1f);

        const int k_MaxDepth = 50;

        static Vec3 Color(in Ray r, IHitable world, int depth)
        {
            s_RaysCast++;
            var rec = new HitRecord();
            if (world.Hit(r, 0.001f, float.MaxValue, ref rec))
            {
                if (depth < k_MaxDepth && rec.material.Scatter(r, rec, out var attenuation, out var scattered))
                {
                    return attenuation * Color(scattered, world, depth + 1);
                }

                if (depth >= k_MaxDepth)
                {
                    s_RaysLost++;
                }

                return Vec3.Zero;
            }

            var unitDirection = r.Direction;
            var t = 0.5f * unitDirection.y + 1.0f;
            return Vec3.One * (1f-t) + k_Bluish * t;
        }

#if COMPLEX_SCENE
        static IHitable RandomScene()
        {
            var hitables = new HitableList();
            hitables.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(new Vec3(0.5f, 0.5f, 0.5f))));

            for (var a = -11; a < 11; ++a)
            {
                for (var b = -11; b < 11; ++b)
                {
                    var center = new Vec3(a + 0.9f * Rng.Next(), 0.2f, b + 0.9f * Rng.Next());
                    if ((center - new Vec3(4, 0.2f, 0)).Length <= 0.9)
                        continue;
                    IMaterial mat;
                    var chooseMat = Rng.Next();
                    if (chooseMat < 0.8f)
                        mat = new Lambertian(new Vec3(Rng.Next()*Rng.Next(), Rng.Next()*Rng.Next(), Rng.Next()*Rng.Next()));
                    else if (chooseMat < 0.95f)
                        mat = new Metal(new Vec3(0.5f*(1+Rng.Next()), 0.5f*(1+Rng.Next()), 0.5f*(1+Rng.Next())), 0.5f*Rng.Next());
                    else
                        mat = new Dielectric(1.5f);
                    hitables.Add(new Sphere(center, 0.2f, mat));
                }
            }

            hitables.Add(new Sphere(new Vec3(0, 1, 0), 1, new Dielectric(1.5f)));
            hitables.Add(new Sphere(new Vec3(-4, 1, 0), 1, new Lambertian(new Vec3(0.4f, 0.2f, 0.1f))));
            hitables.Add(new Sphere(new Vec3(4, 1, 0), 1, new Metal(new Vec3(0.7f, 0.6f, 0.5f))));
            return hitables;
        }
#endif

        static void Write(int nx, int ny)
        {
            s_RaysCast = 0;
            const float ns = 100f;
            const float invNs = 1f / ns;

#if COMPLEX_SCENE
            var lookFrom = new Vec3(13, 2, 3);
            var lookAt = new Vec3(0,0.5f,0);
            var distToFocus = (lookFrom - lookAt).Length;
            var aperture = 0.1f;

            var world = RandomScene();
#else
            var lookFrom = new Vec3(0, 1, 3); // new Vec3(3f, 3f, 2f);
            var lookAt = new Vec3(0, 0.25f, 0f);
            var distToFocus = 1; //(lookFrom - lookAt).Length;
            var aperture = 0; //1f;

            var world = new HitableList(new []
            {
                new Sphere(new Vec3(0, 0, -1), 0.5f, new Lambertian(new Vec3(0.8f, 0.3f, 0.3f))),
                new Sphere(new Vec3(0, -100.5f, -1), 100f, new Lambertian(new Vec3(0.8f, 0.8f, 0))),
                new Sphere(new Vec3(1, 0, -1), 0.5f, new Metal(new Vec3(0.8f, 0.6f, 0.2f), 0.0f)),
                new Sphere(new Vec3(-1, 0, -1), 0.5f, new Dielectric(1.5f)),
                new Sphere(new Vec3(-1, 0, -1), -0.45f, new Dielectric(1.5f)),
            });
#endif

            var camera = new Camera(lookFrom, lookAt, new Vec3(0, 1, 0), 20, ((float)nx)/ny, aperture, distToFocus);

            var image = new Vec3[nx,ny];
            for (var y = 0; y < ny; y++)
            {
                for (var x = 0; x < nx; x++)
                {
                    var col = Vec3.Zero;
                    for (var s = 0; s < ns; s++)
                    {
                        var u = (x + Rng.Next()) / nx;
                        var v = (y + Rng.Next()) / ny;
                        var r = camera.GetRay(u, v);
                        col += Color(r, world, 0);
                    }

                    col = new Vec3((int)(255.99f *MathF.Sqrt(col.x*invNs)), (int)(255.99f *MathF.Sqrt(col.y*invNs)), (int)(255.99f *MathF.Sqrt(col.z*invNs)));
                    image[x, y] = col;
                }
            }

            using (var fs = new StreamWriter(@"c:\dump\backgroundHitables.ppm"))
            {
                fs.WriteLine($"P3\n{nx} {ny}\n255");
                for (var y = ny - 1; y >= 0; y--)
                {
                    for (var x = 0; x < nx; x++)
                    {
                        ref Vec3 col = ref image[x, y];
                        fs.WriteLine($"{col.x} {col.y} {col.z}");
                    }
                }
            }
        }
    }
}
