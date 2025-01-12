using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;


namespace SteveJewelryToolbox
{
    public class SteveJewelryToolboxInfo : GH_AssemblyInfo
    {
        public override string Name => "SteveJewelryToolbox";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("657cf4d6-5b4f-4f10-833f-9c7759475786");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}