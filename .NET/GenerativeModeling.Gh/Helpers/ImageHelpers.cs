using GenerativeModeling.Io;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerativeModeling.Gh
{
    public static class ImageHelpers
    {
        public static Bitmap ReadBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }

        public static Mesh RenderPreview(Bitmap bitmap, double cellSize, Point3d origin)
        {
            Plane pl = new Plane(origin, new Vector3d(new Point3d(0, 0, 1)));
            Mesh preview = Mesh.CreateFromPlane(pl, new Interval(0, cellSize * (bitmap.Size.Width - 1)), new Interval(0, cellSize * (bitmap.Size.Height - 1)), bitmap.Size.Width - 1, bitmap.Size.Height - 1);
            ColorMesh(preview, bitmap);
            return preview;
        }

        public static void ColorMesh(Mesh mesh, Bitmap bmp)
        {
            int count = 0;
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            List<Color> colors = new List<Color>();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    colors.Add(bmp.GetPixel(x, y));
                    //mesh.VertexColors.SetColor(count++, bmp.GetPixel(x, y));
                }
            }
            mesh.VertexColors.SetColors(colors.ToArray());
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        

    }
}
