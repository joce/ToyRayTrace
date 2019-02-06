//#define COMPLEX_SCENE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

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
            var imageWidth = k_ImageWidth;
            var imageHeight = k_ImageHeight;

#if COMPLEX_SCENE
            var world = RandomScene();

            var lookFrom = new Vector3(13, 2, 3);
            var lookAt = new Vector3(0,0.5f,0);
            var distToFocus = (lookFrom - lookAt).Length();
            var aperture = 0.1f;

#else
            var world = new HitableList(new []
            {
                new Sphere(new Vector3(0, 0, -1), 0.5f, new Lambertian(new Vector3(0.8f, 0.3f, 0.3f))),
                new Sphere(new Vector3(0, -100.5f, -1), 100f, new Lambertian(new Vector3(0.8f, 0.8f, 0))),
                new Sphere(new Vector3(1, 0, -1), 0.5f, new Metal(new Vector3(0.8f, 0.6f, 0.2f), 0.2f)),
                new Sphere(new Vector3(-1, 0, -1), 0.5f, new Dielectric(1.5f)),
                new Sphere(new Vector3(-1, 0, -1), -0.45f, new Dielectric(1.5f)),
            });

            var lookFrom = new Vector3(0, 1, 3); // new Vector3(3f, 3f, 2f);
            var lookAt = new Vector3(0, 0.25f, 0f);
            var distToFocus = 1; //(lookFrom - lookAt).Length;
            var aperture = 0; //1f;
#endif

            var camera = new Camera(lookFrom, lookAt, new Vector3(0, 1, 0), 20, ((float)imageWidth)/imageHeight, aperture, distToFocus);

            for (var i = 0; i < 1; i++)
            {
                s_Stopwatch.Restart();
                RenderImage(imageWidth, imageHeight, world, camera);
                s_Stopwatch.Stop();
                s_Times.Add(s_Stopwatch.ElapsedMilliseconds);
            }

            Console.WriteLine($"{s_RaysCast} rays cast");
            Console.WriteLine($"Avg: {s_Times.Average()}, Med: {s_Times.Median()}, Min: {s_Times.Min()}, Max: {s_Times.Max()}");
            Console.WriteLine($"Rays/ms: {s_RaysCast/s_Times.Average()}");
        }

        static long s_RaysCast;

        static readonly Vector3 k_Bluish = new Vector3(0.5f, 0.7f, 1f);

        const int k_MaxDepth = 50;

        static Vector3 Trace(in Ray r, IHitable world, ref int depth, ref uint state)
        {
            depth++;
            var rec = new HitRecord();
            if (world.Hit(r, 0.001f, float.MaxValue, ref rec))
            {
                if (depth <= k_MaxDepth && rec.material.Scatter(r, rec, ref state, out var attenuation, out var scattered))
                {
                    return attenuation * Trace(scattered, world, ref depth, ref state);
                }

                return Vector3.Zero;
            }

            var unitDirection = r.Direction;
            var t = 0.5f * unitDirection.Y + 1.0f;
            return Vector3.One * (1f-t) + k_Bluish * t;
        }

#if COMPLEX_SCENE
        static IHitable RandomScene()
        {
            var hitables = new HitableList();
            hitables.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(new Vector3(0.5f, 0.5f, 0.5f))));

            for (var a = -11; a < 11; ++a)
            {
                for (var b = -11; b < 11; ++b)
                {
                    var center = new Vector3(a + 0.9f * Rng.Next(), 0.2f, b + 0.9f * Rng.Next());
                    if ((center - new Vector3(4, 0.2f, 0)).Length() <= 0.9)
                        continue;
                    IMaterial mat;
                    var chooseMat = Rng.Next();
                    if (chooseMat < 0.8f)
                        mat = new Lambertian(new Vector3(Rng.Next()*Rng.Next(), Rng.Next()*Rng.Next(), Rng.Next()*Rng.Next()));
                    else if (chooseMat < 0.95f)
                        mat = new Metal(new Vector3(0.5f*(1+Rng.Next()), 0.5f*(1+Rng.Next()), 0.5f*(1+Rng.Next())), 0.5f*Rng.Next());
                    else
                        mat = new Dielectric(1.5f);
                    hitables.Add(new Sphere(center, 0.2f, mat));
                }
            }

            hitables.Add(new Sphere(new Vector3(0, 1, 0), 1, new Dielectric(1.5f)));
            hitables.Add(new Sphere(new Vector3(-4, 1, 0), 1, new Lambertian(new Vector3(0.4f, 0.2f, 0.1f))));
            hitables.Add(new Sphere(new Vector3(4, 1, 0), 1, new Metal(new Vector3(0.7f, 0.6f, 0.5f))));
            return hitables;
        }
#endif

        static int RenderLine(int imageWidth, int imageHeight, int lineIdx, IHitable world, in Camera camera, out Vector3[] line)
        {
            const float ns = 100f;
            const float invNs = 1f / ns;

            line = new Vector3[imageWidth];
            int rayCount = 0;
            int frameCount = 1; // Until we have frames...
            uint state = (uint)(lineIdx * 9781 + frameCount * 6271) | 1;
            for (var x = 0; x < imageWidth; x++)
            {
                var col = Vector3.Zero;
                for (var s = 0; s < ns; s++)
                {
                    var u = (x + Rng.Next(ref state)) / imageWidth;
                    var v = (lineIdx + Rng.Next(ref state)) / imageHeight;
                    var r = camera.GetRay(u, v, ref state);
                    int rayDepth = 0;
                    col += Trace(r, world, ref rayDepth, ref state);
                    rayCount += rayDepth;
                }

                col = new Vector3((int)(255.99f *MathF.Sqrt(col.X*invNs)), (int)(255.99f *MathF.Sqrt(col.Y*invNs)), (int)(255.99f *MathF.Sqrt(col.Z*invNs)));
                line[x] = col;
            }

            return rayCount;
        }

        static void RenderImage(int imageWidth, int imageHeight, IHitable world, Camera camera)
        {
            s_RaysCast = 0;

            var image = new Vector3[imageHeight][];

            Parallel.For(0, imageHeight,
                () => 0,
                (y, loop, counter) => counter + RenderLine(imageWidth, imageHeight, y, world, in camera, out image[y]),
                x => Interlocked.Add(ref s_RaysCast, x));

            using (var fs = new StreamWriter(@"c:\dump\backgroundHitables.ppm"))
            {
                fs.WriteLine($"P3\n{imageWidth} {imageHeight}\n255");
                for (var y = imageHeight - 1; y >= 0; y--)
                {
                    for (var x = 0; x < imageWidth; x++)
                    {
                        ref Vector3 col = ref image[y][x];
                        fs.WriteLine($"{col.X} {col.Y} {col.Z}");
                    }
                }
            }
        }
    }
}
