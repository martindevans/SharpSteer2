﻿using Microsoft.Xna.Framework;
using SharpSteer2.Pathway;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpSteer2.WinDemo.PlugIns.MeshPathFollowing
{
    public class MeshPathFollowingPlugin
        :PlugIn
    {
        private TrianglePathway _path;
        private List<PathWalker> _walkers = new List<PathWalker>();

        public override bool RequestInitialSelection
        {
            get
            {
                return true;
            }
        }

        public MeshPathFollowingPlugin(IAnnotationService annotations)
            :base(annotations)
        {
        }

        public override void Open()
        {
            GeneratePath();

            Random r = new Random();
            for (int i = 0; i < 20; i++)
            {
                _walkers.Add(new PathWalker(_path, Annotations)
                {
                    Position = new Vector3(20 * (float)r.NextDouble(), 0, 0)
                });
            }
        }

        private void GeneratePath()
        {
            var rand = new Random();

            float xOffsetDeriv = 0;
            float xOffset = 0;

            var points = new List<Vector3>();
            for (var i = 0; i < 200; i++)
            {
                xOffsetDeriv = MathHelper.Clamp((float)rand.NextDouble() - (xOffsetDeriv * 0.0125f), -15, 15);
                xOffset += xOffsetDeriv;

                points.Add(new Vector3(xOffset + 1, 0, i) * 5);
                points.Add(new Vector3(xOffset - 1, 0, i) * 5);
            }

            _path = new TrianglePathway(points);
        }

        public override void Update(float currentTime, float elapsedTime)
        {
            foreach (var walker in _walkers)
                walker.Update(elapsedTime);
        }

        public override void Redraw(float currentTime, float elapsedTime)
        {
            Demo.UpdateCamera(elapsedTime, _walkers[0]);
            foreach (var walker in _walkers)
                walker.Draw();

            var tri = _path.Triangles.ToArray();
            for (int i = 0; i < tri.Length; i++)
            {
                var triangle = tri[i];

                Drawing.Draw2dLine(triangle.A, triangle.A + triangle.Edge0, Color.Black);
                Drawing.Draw2dLine(triangle.A, triangle.A + triangle.Edge1, Color.Black);
                Drawing.Draw2dLine(triangle.A + triangle.Edge0, triangle.A + triangle.Edge1, Color.Black);
            }
        }

        public override void Close()
        {
            
        }

        public override string Name
        {
            get { return "Nav Mesh Path Following"; }
        }

        public override IEnumerable<IVehicle> Vehicles
        {
            get
            {
                return _walkers;
            }
        }
    }
}
