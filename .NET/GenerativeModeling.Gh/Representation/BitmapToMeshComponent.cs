using System;
using System.Collections.Generic;
using System.Drawing;
using GenerativeModeling.Io;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GenerativeModeling.Gh.Representation
{
    public class BitmapToMeshComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BitmapToMeshComponent class.
        /// </summary>
        public BitmapToMeshComponent()
          : base("Bitmap To Mesh", "Bitmap To Mesh",
              "Render a bitmap as a mesh",
              Constants.CATEGORY, Constants.REPRESENTATION)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Bitmap", "Bitmap", "Bitmap to render", GH_ParamAccess.item);
            pManager.AddNumberParameter("Cell Size", "Cell Size", "Preview Mesh Cell Size", GH_ParamAccess.item);
            pManager.AddPointParameter("Preview Origin", "Preview Origin", "Preview Origin", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Preview Mesh", "Preview Mesh", "Preview Mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Bitmap bitmap = default(Bitmap);
            double cellSize = 1;
            Point3d origin = default(Point3d);

            DA.GetData(0, ref bitmap);
            DA.GetData(1, ref cellSize);
            DA.GetData(2, ref origin);

            Mesh preview = ImageHelpers.RenderPreview(bitmap, cellSize, origin);

            DA.SetData(0, preview);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a6fbf2ef-032e-4f93-8e19-5b8ce8fc9a54"); }
        }
    }
}