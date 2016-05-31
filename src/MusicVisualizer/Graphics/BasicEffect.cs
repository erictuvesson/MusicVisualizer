namespace MusicVisualizer.Graphics
{
    using System;
    using OpenTK.Graphics.OpenGL;

    class BasicEffect : IDisposable
    {
        static readonly string vertexShaderSource = @"
#version 140
precision highp float;
uniform mat4 transform;
in vec3 in_position;
in vec4 in_color;
out vec4 color;
void main(void)
{
    color = in_color;
    gl_Position = transform * vec4(in_position, 1);
}";

        static readonly string fragmentShaderSource = @"
#version 140
precision highp float;
in vec4 color;
out vec4 out_color;
void main(void)
{
    out_color = color.rgba;
}";


        public int ProgramId => shaderProgramHandle;
        public int TransformLocation => transformLocation;

        private int shaderProgramHandle, transformLocation;

        public BasicEffect()
        {
            var vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            var fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.BindAttribLocation(shaderProgramHandle, 0, "in_position");
            GL.BindAttribLocation(shaderProgramHandle, 1, "in_color");

            GL.LinkProgram(shaderProgramHandle);
            GL.UseProgram(shaderProgramHandle);

            transformLocation = GL.GetUniformLocation(shaderProgramHandle, "transform");
        }

        public void Dispose()
        {
            GL.DeleteProgram(shaderProgramHandle);
        }
    }
}
