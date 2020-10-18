using System;
using System.Collections.Generic;
using System.Drawing;
using GenerativeModeling.Gh.Helpers;
using GenerativeModeling.Io;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GenerativeModeling.Gh.Representation
{
    public class DepthMapToMeshComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DepthMapToMeshComponent class.
        /// </summary>
        public DepthMapToMeshComponent()
          : base("Depth map to mesh", "Depth map to mesh",
              "Render a depth map as a mesh",
              Constants.CATEGORY, Constants.REPRESENTATION)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("InImage", "InImage", "InImage", GH_ParamAccess.item);
            pManager.AddGenericParameter("InDepthMap", "InDepthMap", "Greyscale image", GH_ParamAccess.item);

            pManager.AddNumberParameter("Zscale", "Zscale", "Zscale must be greater than zero", GH_ParamAccess.item);
            pManager.AddNumberParameter("Threshold", "Threshold", "Threshold must be between zero and one", GH_ParamAccess.item, 0);
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Bitmap myInputBitmap = null;
            Bitmap myInputDepthmap = null;

            double zScale = 1.0;
            double threshold = 1.0;

            DA.GetData(0, ref myInputBitmap);
            DA.GetData(1, ref myInputDepthmap);

            DA.GetData(2, ref zScale);
            DA.GetData(3, ref threshold);


            if (myInputBitmap == null) return;
            if (myInputDepthmap == null) return;

            if (double.IsNaN(zScale)) return;
            if (zScale <= 0) return;
            if (double.IsNaN(threshold)) return;
            if (threshold < 0 || threshold > 1) return;



             Bitmap myBitmap = new Bitmap(myInputBitmap);
             Bitmap myDepthmap = new Bitmap(myInputDepthmap);

            myBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            myDepthmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            if (myBitmap.Size != myDepthmap.Size) return;

            Interval dx = new Interval(0, myBitmap.Width);
            Interval dy = new Interval(0, myBitmap.Height);
            Mesh myMesh = Mesh.CreateFromPlane(Plane.WorldXY, dx, dy, myBitmap.Width - 1, myBitmap.Height - 1);
            

            int index = 0;
            // read pixels
            for (int i = 0; i < myBitmap.Height; i++)
            {
                for (int j = 0; j < myBitmap.Width; j++)
                {

                    // extract color from pixel
                    Color col = myBitmap.GetPixel(j, i);
                    Color depthColor = myDepthmap.GetPixel(j, i);

                    // get mesh vertex
                    Point3d pt = myMesh.Vertices[index];

                    // set new vertex
                    myMesh.Vertices.SetVertex(index, pt.X, pt.Y, depthColor.GetBrightness() * zScale);

                    // set mesh vertex color from pixel
                    myMesh.VertexColors.SetColor(index, col);

                    index++;
                }
            }

            BoundingBox mainBox = myMesh.GetBoundingBox(false);
            double globalMin = mainBox.Min.Z;
            double globalMax = mainBox.Max.Z;

            if (threshold > 0)
            {
                Mesh joinedMesh = new Mesh();
                double worldThreshold = threshold.Remap(globalMin, globalMax);

                Mesh[] meshes = myMesh.Split(new Plane(new Point3d(0, 0, worldThreshold), Vector3d.ZAxis));
                foreach(Mesh m in meshes)
                {
                    BoundingBox bbox = m.GetBoundingBox(false);
                    if (bbox.Center.Z >= worldThreshold)
                    {
                        joinedMesh.Append(m);
                    }
                }
                if (joinedMesh.IsValid)
                    myMesh = joinedMesh;
                
            }
            


            //for (int i = myMesh.Faces.Count - 1; i >= 0; i--)
            //{

            //    //Point3f a = Point3f.Unset;
            //    //Point3f b = Point3f.Unset;
            //    //Point3f c = Point3f.Unset;
            //    //Point3f d = Point3f.Unset;

            //    //myMesh.Faces.GetFaceVertices(i, out a, out b, out c, out d);

            //    //double sum = a.Z + b.Z + c.Z + d.Z;
            //    //double average = sum / 4;

            //    //if (average > zScale * threshold)
            //    //    myMesh.Faces.RemoveAt(i);

            //}

            DA.SetData(0, myMesh);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Properties.Resources.DepthMapIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("dda77352-e7d4-4b20-ab1a-ea4da5030e9e"); }
        }
    }
}