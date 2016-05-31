namespace MusicVisualizer.Graphics
{
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using System;

    public class Line : IDisposable
    {
        public float LineThickness
        {
            get { return lineThickness; }
            set
            {
                if (value < 0)
                    value = 0.1f;

                lineThickness = value;
            }
        }
        private float lineThickness = 1.0f;

        public bool LoopLines = false;

        private int vertexBufferId;
        private Vertex[] vertices;

        public Line(int size = 256)
        {
            vertices = new Vertex[size];

            vertexBufferId = GL.GenBuffer();
        }

        public void SetAtPosition(int index, Vector2 position, Color4 color)
        {
            if (index >= vertices.Length) return;

            vertices[index] = new Vertex()
            {
                Position = new Vector3(position.X, position.Y, 0),
                Color = color
            };
        }

        public void Resize(int newSize)
        {
            if (vertices.Length != newSize)
            {
                Array.Resize(ref vertices, newSize);
            }
        }

        internal void Draw(BasicEffect basicEffect, Matrix4 matrix)
        {
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(lineThickness);
            
            var vertexCount = vertices.Length - 1;

            GL.UseProgram(basicEffect.ProgramId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 12);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.UniformMatrix4(basicEffect.TransformLocation, false, ref matrix);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexCount * Vertex.SizeInBytes), vertices, BufferUsageHint.StaticDraw);

            GL.DrawArrays(LoopLines ? PrimitiveType.LineLoop : PrimitiveType.LineStrip, 0, vertexCount);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vertexBufferId);
        }
    }
}
