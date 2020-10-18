using System;
using System.Collections.Generic;
using System.Drawing;
using GenerativeModeling.Io;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GenerativeModeling.Gh.DataExtraction
{
    public class ImageToBitmapComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ImageToBitmapComponent class.
        /// </summary>
        public ImageToBitmapComponent()
          : base("Image To Bitmap", "Image To Bitmap",
              "Convert Image to Bitmap",
              Constants.CATEGORY, Constants.DATA_EXTRACTION)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path to image file", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Bitmap", "Bitmap", "Output Bitmap", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null;
            DA.GetData(0, ref path);

            Bitmap bmp = null;

            using(var image = new Bitmap(path))
            {
                bmp = image;
            }

            DA.SetData(0, bmp);
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
            get { return new Guid("c3b05c0d-7cf8-4d1b-8562-a395af23a8af"); }
        }
    }
}