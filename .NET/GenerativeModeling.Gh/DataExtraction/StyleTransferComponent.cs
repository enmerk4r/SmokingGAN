using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GenerativeModeling.Http;
using GenerativeModeling.Io;
using Grasshopper.Kernel;
using Newtonsoft.Json;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace GenerativeModeling.Gh
{
    public class StyleTransferComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public StyleTransferComponent()
          : base("Style transfer", "ST",
              "Style Transfer Between Two Images",
              Constants.CATEGORY, Constants.DATA_EXTRACTION)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Input Image", "Input Image", "Input Image Pass", GH_ParamAccess.item);
            pManager.AddTextParameter("Style", "Style", "Style Checkpoint", GH_ParamAccess.item, "la_muse");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Output Image", "Output Image", "Output Image Bitmap", GH_ParamAccess.item);
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string imgPath = string.Empty;
            string styleName = string.Empty;
            

            DA.GetData(0, ref imgPath);
            DA.GetData(1, ref styleName);

            Bitmap img = ImageHelpers.ReadBitmap(imgPath);
            Bitmap result;

            try
            {
                StyleGanInput input = new StyleGanInput(styleName, img);
                string computed = Transponder.Post(RoutesController.StyleTransferRoute, input);
                StyleGanOutput output = JsonConvert.DeserializeObject<StyleGanOutput>(computed);

                result = output.ToBitmap();
            }
            catch
            {
                result = default(Bitmap);
            }
            
            
            DA.SetData(0, result);
        }

        
        

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e386c9ea-fc45-48e6-a772-008614960eff"); }
        }
    }
}
