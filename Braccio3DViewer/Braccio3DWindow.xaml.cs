// Classe WPF aggiornata con 4 DOF e snodi corretti
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Braccio_Robotico
{
    public class Braccio3DWindow : UserControl
    {
        private RotateTransform3D rotBase, rot1, rot2, rot3;

        public Braccio3DWindow()
        { 
            Init3D();
        }

        private void Init3D()
        {
            var viewport = new Viewport3D();
            Content = viewport;

            var camera = new PerspectiveCamera(
                new Point3D(0, 300, 500),  // posizione della camera
                new Vector3D(0, -0.4, -1), // direzione verso cui guarda
                new Vector3D(0, 1, 0),     // "up direction"
                60                         // field of view
            );

            viewport.Camera = camera;

            var lights = new Model3DGroup();
            lights.Children.Add(new AmbientLight(Colors.Gray));
            lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-1, -2, -3)));
            viewport.Children.Add(new ModelVisual3D { Content = lights });

            // BASE ROTANTE
            var baseModel = CreateSegment(30, 60, Colors.Red);
            var mvBase = new Model3DGroup();
            foreach (var child in baseModel.Children)
                if (child is GeometryModel3D geo)
                    mvBase.Children.Add(geo);

            var jointBase = new ModelVisual3D();
            rotBase = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0), new Point3D(0, 0, 0));
            jointBase.Transform = rotBase;
            jointBase.Content = mvBase;
            viewport.Children.Add(jointBase);

            // BRACCIO 1
            var segment1 = CreateSegment(20, 80, Colors.Blue);
            var mv1 = new Model3DGroup();
            foreach (var geo in segment1.Children)
                if (geo is GeometryModel3D g) mv1.Children.Add(g);

            var joint1 = new ModelVisual3D();
            var tg1 = new Transform3DGroup();
            tg1.Children.Add(new TranslateTransform3D(0, 60, 0));
            rot1 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0), new Point3D(0, 60, 0));
            tg1.Children.Add(rot1);
            joint1.Transform = tg1;
            joint1.Content = mv1;
            jointBase.Children.Add(joint1);

            // BRACCIO 2 (agganciato alla fine del braccio 1)
            var segment2 = CreateSegment(15, 60, Colors.Green);
            var mv2 = new Model3DGroup();
            foreach (var geo in segment2.Children)
                if (geo is GeometryModel3D g) mv2.Children.Add(g);

            var joint2 = new ModelVisual3D();
            var tg2 = new Transform3DGroup();
            tg2.Children.Add(new TranslateTransform3D(0, 80, 0));
            rot2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0), new Point3D(0, 80, 0));
            tg2.Children.Add(rot2);
            joint2.Transform = tg2;
            joint2.Content = mv2;
            joint1.Children.Add(joint2);

            // PINZA (agganciata alla fine del braccio 2)
            var pinza = CreateSegment(10, 40, Colors.Orange);
            var mv3 = new Model3DGroup();
            foreach (var geo in pinza.Children)
                if (geo is GeometryModel3D g) mv3.Children.Add(g);

            var joint3 = new ModelVisual3D();
            var tg3 = new Transform3DGroup();
            tg3.Children.Add(new TranslateTransform3D(0, 60, 0));
            rot3 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0), new Point3D(0, 60, 0));
            tg3.Children.Add(rot3);
            joint3.Transform = tg3;
            joint3.Content = mv3;
            joint2.Children.Add(joint3);
        }

        private Model3DGroup CreateSegment(double radius, double height, Color color)
        {
            var material = new DiffuseMaterial(new SolidColorBrush(color));
            var mesh = new MeshGeometry3D();
            int segments = 24;

            // Estremità inferiore
            mesh.Positions.Add(new Point3D(0, 0, 0));
            for (int i = 0; i <= segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double x = radius * Math.Cos(angle);
                double z = radius * Math.Sin(angle);
                mesh.Positions.Add(new Point3D(x, 0, z));
            }

            // Estremità superiore
            mesh.Positions.Add(new Point3D(0, height, 0));
            for (int i = 0; i <= segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double x = radius * Math.Cos(angle);
                double z = radius * Math.Sin(angle);
                mesh.Positions.Add(new Point3D(x, height, z));
            }

            // Laterali
            for (int i = 1; i <= segments; i++)
            {
                int base1 = i;
                int base2 = i + 1;
                int top1 = segments + 2 + i;
                int top2 = segments + 2 + i + 1;

                mesh.TriangleIndices.Add(base1);
                mesh.TriangleIndices.Add(base2);
                mesh.TriangleIndices.Add(top1);

                mesh.TriangleIndices.Add(top1);
                mesh.TriangleIndices.Add(base2);
                mesh.TriangleIndices.Add(top2);
            }

            // Inferiore
            for (int i = 1; i < segments; i++)
            {
                mesh.TriangleIndices.Add(0);
                mesh.TriangleIndices.Add(i + 1);
                mesh.TriangleIndices.Add(i);
            }

            // Superiore
            int centerTop = segments + 2;
            for (int i = 1; i < segments; i++)
            {
                mesh.TriangleIndices.Add(centerTop);
                mesh.TriangleIndices.Add(centerTop + i);
                mesh.TriangleIndices.Add(centerTop + i + 1);
            }

            return new Model3DGroup { Children = { new GeometryModel3D(mesh, material) } };
        }


        public void UpdateAngles(double rotBaseDeg, double a1Deg, double a2Deg, double a3Deg)
        {
            ((AxisAngleRotation3D)rotBase.Rotation).Angle = NormalizeAngle(rotBaseDeg);
            ((AxisAngleRotation3D)rot1.Rotation).Angle = NormalizeAngle(a1Deg);
            ((AxisAngleRotation3D)rot2.Rotation).Angle = NormalizeAngle(a2Deg);
            ((AxisAngleRotation3D)rot3.Rotation).Angle = NormalizeAngle(a3Deg);
        }

        private double NormalizeAngle(double angle)
        { 
            angle = angle % 360;
            if (angle > 180) angle -= 360;
            if (angle < -180) angle += 360;
            return angle;
        }
    }
}
