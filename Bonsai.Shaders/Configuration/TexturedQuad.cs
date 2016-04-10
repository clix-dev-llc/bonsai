﻿using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Shaders.Configuration
{
    public class TexturedQuad : MeshConfiguration
    {
        [Category("State")]
        public QuadEffects QuadEffects { get; set; }

        public override Mesh CreateResource()
        {
            var mesh = base.CreateResource();
            mesh.DrawMode = PrimitiveType.Quads;
            var flipX = (QuadEffects & QuadEffects.FlipHorizontally) != 0;
            var flipY = (QuadEffects & QuadEffects.FlipVertically) != 0;
            mesh.VertexCount = VertexHelper.TexturedQuad(
                mesh.VertexBuffer,
                mesh.VertexArray,
                flipX,
                flipY);
            return mesh;
        }
    }
}
