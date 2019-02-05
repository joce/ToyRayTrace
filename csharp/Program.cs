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
        }

        static long s_RaysCast;
        static long s_RaysLost;

        static Vec3 Color(in Ray r, IHitable world, int depth)
        {
            s_RaysCast++;
            var rec = new HitRecord();
            if (world.Hit(r, 0.001f, float.MaxValue, ref rec))
            {
                if (depth < 50 && rec.material.Scatter(r, rec, out var attenuation, out var scattered))
                {
                    return attenuation * Color(scattered, world, depth + 1);
                }

                if (depth >= 50)
                {
                    s_RaysLost++;
                }

                return new Vec3(0, 0, 0);
            }

            var unitDirection = r.Direction;
            var t = 0.5f * unitDirection.Y + 1.0f;
            return (1f-t) * new Vec3(1f, 1f, 1f)  + t * new Vec3(0.5f, 0.7f, 1f) ;
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
            var ns = 100;

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

            using (var fs = new StreamWriter(@"c:\dump\backgroundHitables.ppm"))
            {
                fs.WriteLine($"P3\n{nx} {ny}\n255");
                for (var y = ny - 1; y >= 0; y--)
                {
                    for (var x = 0; x < nx; x++)
                    {
                        var col = new Vec3(0, 0, 0);
                        for (var s = 0; s < ns; s++)
                        {
                            var u = (x + Rng.Next()) / nx;
                            var v = (y + Rng.Next()) / ny;
                            var r = camera.GetRay(u, v);
                            col += Color(r, world, 0);
                        }

                        col /= (float)ns;
                        col = new Vec3(MathF.Sqrt(col[0]), MathF.Sqrt(col[1]), MathF.Sqrt(col[2]));
                        var ir = (int)(255.99f * col[0]);
                        var ig = (int)(255.99f * col[1]);
                        var ib = (int)(255.99f * col[2]);

                        fs.WriteLine($"{ir} {ig} {ib}");
                    }
                }
            }
        }
    }
}
