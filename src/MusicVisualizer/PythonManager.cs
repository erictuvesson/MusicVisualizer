namespace MusicVisualizer
{
    using Audio;
    using IronPython.Hosting;
    using Microsoft.Scripting.Hosting;
    using System.IO;
    using Visualizations;
    using System;
    using Graphics;
    using System.Collections.Generic;
    using OpenTK;
    using System.Diagnostics;
    public interface IPythonObjectRegister
    {
        Line CreateLine(int size);

        void Draw(Matrix4 matrix); // TODO: Remove
    }

    public class PythonRegister : IPythonObjectRegister
    {
        public List<Line> lines;

        private BasicEffect basicEffect;

        public PythonRegister()
        {
            lines = new List<Line>();

            basicEffect = new BasicEffect();
        }

        public Line CreateLine(int size)
        {
            var line = new Line(size);
            lines.Add(line);
            return line;
        }

        public void Draw(Matrix4 matrix)
        {
            foreach (var line in lines)
            {
                line.Draw(basicEffect, matrix);
            }
        }
    }

    public class PythonVisualization : Visualization
    {
        public override string Title => "Python Visualization";

        private dynamic pyInstance;
        private IPythonObjectRegister register;

        public PythonVisualization(dynamic pyInstance, IPythonObjectRegister register)
        {
            this.pyInstance = pyInstance;
            this.register = register;

            // Check instance
        }

        public override void Draw(float elapsedTime, AnalyzedAudio data)
        {
            pyInstance.Update(elapsedTime, data);
            
            var transform = Matrix4.CreateOrthographicOffCenter(0, AppShell.Width, AppShell.Height, 0, -1.0f, 1.0f);
            register.Draw(transform);
        }
    }

    public class PythonManager
    {
        public readonly IApplicationShell AppShell;

        private ScriptEngine engine;
        private ScriptScope scope;

        private IPythonObjectRegister register;

        public PythonManager(IApplicationShell appShell)
        {
            this.AppShell = appShell;
            this.register = new PythonRegister();

            const string initPath = "Python\\init.py";

            engine = Python.CreateEngine();
            engine.SetSearchPaths(new[] { "MusicVisualizer" });

            scope = engine.ExecuteFile(initPath);
            scope.ImportModule("clr");
            scope.SetVariable("AppShell", this.AppShell);
            scope.SetVariable("Shell", register);
        }

        public PythonVisualization CreateVisualization(string file, string className)
        {
            var source = engine.CreateScriptSourceFromFile(file);
            source.Execute(scope);

            try
            {
                dynamic pyClass = scope.GetVariable(className);
                dynamic pyInstance = pyClass();

                //var test = (System.Collections.Generic.IDictionary<string, object>)pyInstance;

                return new PythonVisualization(pyInstance, register);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
    }
}
