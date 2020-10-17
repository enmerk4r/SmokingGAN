using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace GenerativeModeling.Gh
{
    public class GenerativeModelingGhInfo : GH_AssemblyInfo
  {
    public override string Name
    {
        get
        {
            return "GenerativeModelingGh";
        }
    }
    public override Bitmap Icon
    {
        get
        {
            //Return a 24x24 pixel bitmap to represent this GHA library.
            return null;
        }
    }
    public override string Description
    {
        get
        {
            //Return a short string describing the purpose of this GHA library.
            return "";
        }
    }
    public override Guid Id
    {
        get
        {
            return new Guid("6d637008-bb16-4b5b-bf39-38d0fe1ad290");
        }
    }

    public override string AuthorName
    {
        get
        {
            //Return a string identifying you or your company.
            return "";
        }
    }
    public override string AuthorContact
    {
        get
        {
            //Return a string representing your preferred contact details.
            return "";
        }
    }
}
}
