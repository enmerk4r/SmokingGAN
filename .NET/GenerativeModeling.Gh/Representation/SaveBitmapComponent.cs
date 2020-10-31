using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GenerativeModeling.Io;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GenerativeModeling.Gh.Representation
{
    public class SaveBitmapComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SaveBitmapComponent class.
        /// </summary>
        public SaveBitmapComponent()
          : base("Save Bitmap", "Save Bitmap",
              "Save Bitmap to disk",
              Constants.CATEGORY, Constants.REPRESENTATION)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Bitmap", "Bitmap", "Bitmap to save", GH_ParamAccess.item);
            pManager.AddTextParameter("Path", "Path", "Filepath to save", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Save", "Save", "Save to disk", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Bitmap _bmp = null;
            string path = null;
            bool save = false;

            DA.GetData(0, ref _bmp);
            DA.GetData(1, ref path);
            DA.GetData(2, ref save);

            Bitmap bmp = new Bitmap(_bmp);

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            if (save)
            {
                Encoder myEncoder = Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, 100L);
                bmp.Save(path, GetEncoderInfo("image/jpeg"), myEncoderParameters);
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
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
            get { return new Guid("3cdd5924-5664-44cd-92eb-f217a8ba28ce"); }
        }
    }
}