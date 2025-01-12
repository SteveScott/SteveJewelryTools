using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SteveJewelryToolbox
{
    public class SteveJewelryToolboxComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SteveJewelryToolboxComponent()
          : base("RoundCutter", "Round Cutter",
            "A component to make a round cutter",
            "SteveJewelry", "Cutters")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // register a list of circles.
            pManager.AddCircleParameter("Circles", "C", "A list of circles", GH_ParamAccess.list);

            // register a double representing the height of the cutter. Set the default to 5
            pManager.AddNumberParameter("Height", "H", "the height of the cutter above the stone.", GH_ParamAccess.item, 5.0);

            //register a double representing the bottom of the cutter. set default to 5.0
            pManager.AddNumberParameter("BottomHeight", "BH", "the height of the cutter below the stone.", GH_ParamAccess.item, 5.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // outputs a list of breps representing the cutters.
            pManager.AddBrepParameter("Cutters", "C", "A list of breps representing the cutters", GH_ParamAccess.list);

            // outputs a list of breps representing the stones.
            pManager.AddBrepParameter("Stones", "S", "A list of breps representing the stones", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // import a stone called unitStone.stl;
            var options = new Rhino.FileIO.FileStlReadOptions();
            Rhino.FileIO.File3dm unitStoneDoc = Rhino.FileIO.File3dm.Read("Assets/unitRoundStone.stl");
            var objectOnDefaultLayer = unitStoneDoc.Objects.FindByLayer("Default")[0];
            // declare a brep representing the unit stone.
            Brep unitStone = objectOnDefaultLayer.Geometry as Brep;


            // declare a list of circles.
            List<Circle> circles = new List<Circle>();

            // get the circles from the input.
            if (!DA.GetDataList(0, circles)) return;

            // declare a list of breps representing the cutters.
            List<Brep[]> cutters = new List<Brep[]>();

            // declare a list of breps representing the stones.
            List<Brep> stones = new List<Brep>();


            // declare a double representing the height of the cutter.
            double height = 0.0;
            double bottomHeight = 0.0;
            //get the height from the input.
            if (!DA.GetData(1, destination: ref height)) return;


            if (!DA.GetData(2, destination: ref bottomHeight)) return;



            // loop through the circles.
            foreach (Circle circle in circles)
            {

                //get the normal vector of the circle.
                Vector3d normal = circle.Plane.Normal;
                // create a brep representing the cutter.
                // 1. create a circle representing the circle above the stone.
                Circle circle1 = new Circle(circle.Plane, circle.Radius);
                circle1.Translate(normal * height);
                // 2. create a circle representing the girdle.
                Circle circle2 = circle;
                // 3. create a circle representing the midpoint between the girdle and the culet.
                Circle circle3 = new Circle(circle.Plane, circle.Radius);
                circle3.Transform(Transform.Scale(circle.Center, 0.43));
                circle3.Translate(normal * circle.Diameter * -0.341);
                // 4. create a circle representing the extension of the cutter.
                Circle circle4 = new Circle(circle.Plane, circle3.Radius);
                //translate to culit.
                circle4.Translate(normal * circle.Diameter * -0.5);
                circle4.Translate(normal * bottomHeight);
                // loft the circles to create the cutter.
                Brep[] cutter = Brep.CreateFromLoft(new Curve[] {
                    circle1.ToNurbsCurve(),
                    circle2.ToNurbsCurve(),
                    circle3.ToNurbsCurve(),
                    circle4.ToNurbsCurve() 
                }, 
                Point3d.Unset, 
                Point3d.Unset, 
                LoftType.Straight, 
                false);
                // add the cutter to the list.
                cutters.Add(cutter);

                // create a brep representing the stone.
                //
                Brep stone = unitStone.DuplicateBrep();
                // move the stone to the center of the circle.
                stone.Transform(Transform.Translation( circle.Center - Point3d.Origin));
                // scale the stone to the size of the circle.
                stone.Transform(Transform.Scale(circle.Center, circle.Radius * 2.0));
                // orient the stone to the circle's normal plane
                stone.Transform(Transform.PlaneToPlane(Plane.WorldXY, circle.Plane));


                // add the stone to the list.
                stones.Add(stone);
            }

            // set the output.
            DA.SetDataList(0, cutters);
            DA.SetDataList(1, stones);  
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("4a69e9c4-c879-4c21-8189-b41d872fbc36");
    }
}