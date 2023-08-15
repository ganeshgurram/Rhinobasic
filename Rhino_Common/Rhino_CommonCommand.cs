using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Rhino_Common
{
    public class Rhino_CommonCommand : Command
    {
        public Rhino_CommonCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static Rhino_CommonCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "DrawRectangle";








        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            using (var window = new RectangularSurfaceForm())
            {
                var result = window.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var corner1 = window.Corner1;
                    var corner2 = window.Corner2;
                    var corner3 = window.Corner3;
                    var corner4 = window.Corner4;
                    var surface = CreateRectangularSurface(corner1, corner2,corner3,corner4);

                    if (surface != null)
                    {
                        doc.Objects.AddBrep(surface);
                        doc.Views.Redraw();
                        return Result.Success;
                    }
                }

                return Result.Cancel;
            }
        }
        public Brep CreateRectangularSurface(Point3d corner1, Point3d corner2,Point3d corner3,Point3d corner4)
        {

            var surface = Brep.CreateFromCornerPoints(corner1, corner2, corner3, corner4, 0);

            return surface;
        }
    }

    public class RectangularSurfaceForm : Form
    {
        public Point3d Corner1 { get; private set; }
        public Point3d Corner2 { get; private set; }
        public Point3d Corner3 { get; private set; }
        public Point3d Corner4 { get; private set; }

        public RectangularSurfaceForm()
        {
            Text = "Rectangular Surface Generator";
            AutoSize = true;


            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 1,
                AutoSize = true,
            };
            var mainpanel = new TableLayoutPanel
            {
                Padding = new Padding(10),
                ColumnCount = 2,
                AutoSize = true
            };
            var panel1 = new TableLayoutPanel
            {
                Padding = new Padding(10),
                ColumnCount = 2,  
                AutoSize = true
            };

            var panel2 = new TableLayoutPanel
            {
                Padding = new Padding(10),
                ColumnCount = 2,
                AutoSize = true
            };

            var labellen = new Label
            {
                Text = "Length (Lx):",
                Anchor = AnchorStyles.Left
            };
            panel1.Controls.Add(labellen);

            var lengthbox = new TextBox
            {
                Anchor = AnchorStyles.Left
            };
            panel1.Controls.Add(lengthbox);

            var labelwidth = new Label
            {
                Text = "Width (Ly):",
                Anchor = AnchorStyles.Left
            };
            panel2.Controls.Add(labelwidth);

            var widthbox = new TextBox
            {
                Anchor = AnchorStyles.Right
            };
            panel2.Controls.Add(widthbox);


            var labelX = new Label
            {
                Text = "X",
                Anchor = AnchorStyles.Left
            };
            panel1.Controls.Add(labelX);

            var Xbox = new TextBox
            {
                Anchor = AnchorStyles.Right
            };

            panel1.Controls.Add(Xbox);


            var labelY = new Label
            {
                Text = "Y ",
                Anchor = AnchorStyles.Left
            };
            panel1.Controls.Add(labelY);

            var Ybox = new TextBox
            {
                Anchor = AnchorStyles.Right
            };
            panel1.Controls.Add(Ybox);


            var labelZ = new Label
            {
                Text = "Z:",
                Anchor = AnchorStyles.Left
            };
            panel1.Controls.Add(labelZ);

            var Zbox = new TextBox
            {
                Anchor = AnchorStyles.Right
            };
            panel1.Controls.Add(Zbox);

            mainpanel.Controls.Add(panel1);
            mainpanel.Controls.Add(panel2);

            mainpanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            mainpanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));



            var okButton = new Button
            {
                Text = "Generate Plate",
                Margin = new Padding(0, 10, 0, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
            };
            okButton.Click += (sender, e) =>
            {
                if (double.TryParse(lengthbox.Text, out double length) &&
                   double.TryParse(widthbox.Text, out double width) &&
                   double.TryParse(Xbox.Text, out double x) &&
                   double.TryParse(Ybox.Text, out double y) &&
                   double.TryParse(Zbox.Text, out double z))
                {
                    var midpoint = new Point3d(x, y, z);

                    double halfLength = length / 2.0;
                    double halfBreadth = width / 2.0;
                    Vector3d halfLengthVector = new Vector3d(halfLength, 0, 0);
                    Vector3d halfBreadthVector = new Vector3d(0, halfBreadth, 0);

                    Corner1 = midpoint - halfLengthVector - halfBreadthVector;
                    Corner2 = midpoint + halfLengthVector - halfBreadthVector;
                    Corner3 = midpoint + halfLengthVector + halfBreadthVector;
                    Corner4 = midpoint - halfLengthVector + halfBreadthVector;

                    DialogResult = DialogResult.OK;
                }
                else
                {
                    System.Windows.MessageBox.Show("Invalid  input Please Enter Valid Values ");
                }

            };
            panel.Controls.Add(mainpanel);
            panel.Controls.Add(okButton);
            //  panel.Controls.Add(rowpanel);

            Controls.Add(panel);
        }
    }
}
