using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace normalcalculator
{
    static class Game
    {
        static int WIDTH = 1000,
                   HEIGHT = 720;

        static GameWindow window;
        static Plain plain;

        public static void Main(string[] args) {
            window = new GameWindow(WIDTH,HEIGHT);
            handleWindow();
        }

        static void handleWindow() {

            //-------------------------------------//
            // PLAIN TRIANGLES
            //-------------------------------------//
            float size = 10;

            float[] y = {
                -2,-2,
                -2,-2,
            };

            Vector3[] triangleA = {
                // Bottom Left
                new Vector3(-size, y[0], size),
                new Vector3(-size, y[1],-size),
                new Vector3( size, y[2],-size),
            },
            triangleB = {
                // Top Right
                new Vector3(-size, y[0], size),
                new Vector3( size, y[3], size),
                new Vector3( size, y[2],-size)
            };

            plain = new Plain(triangleA, triangleB);

            window.RenderFrame += render;
            window.Resize += resize;
            window.Load += load;
            window.Run(60);
        }

        static void render(object sender, FrameEventArgs e) {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //-------------------------------------//
            // LIGHT
            //-------------------------------------//

            float[] lightPos = { 100f, 100, 100f };
            float[] lightDiffuse = { 1f, 1f, 1f };
            float[] lightAmbient = { .4f, .4f, .4f };

            GL.Light(LightName.Light0, LightParameter.Position, lightPos);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightDiffuse);
            GL.Light(LightName.Light0, LightParameter.Ambient, lightAmbient);

            GL.Rotate(.1, 0, 10, 0);
            plain.render();

            window.SwapBuffers();
        }

        static void resize(object sender, EventArgs e) {

            GL.Viewport(0, 0, window.Width, window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            Matrix4 perspectiveMatrix =
                Matrix4.CreatePerspectiveFieldOfView(1, window.Width / window.Height, 1.0f, 2000.0f);

            GL.LoadMatrix(ref perspectiveMatrix);
            GL.MatrixMode(MatrixMode.Modelview);

            GL.End();
        }

        static void load(object sender, EventArgs e) {
            GL.ClearColor(0, 0, 0, 0);
            GL.Enable(EnableCap.DepthTest);

            //-------------------------------------//
            // IMPORTANT TO ENABLE LIGHTING
            //-------------------------------------//
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Light0);
        }
    }

    //-------------------------------------//
    // PLAIN CLASS (EXAMPLE RENDERING)
    //-------------------------------------//
    class Plain {

        Vector3[] triangleA, triangleB;
        Vector3[] normals = new Vector3[2];

        public Plain(Vector3[] triangleA, Vector3[] triangleB) {
            this.triangleA = triangleA;
            this.triangleB = triangleB;

            normals[0] = calculateNormals(triangleA[0], triangleA[1], triangleA[2]);
            normals[1] = calculateNormals(triangleB[0], triangleB[1], triangleB[2]);
        }

        public void render()
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(1.0, 1.0, 1.0);

            GL.Normal3(normals[0]);
            GL.Vertex3(triangleA[0]);
            GL.Vertex3(triangleA[1]);
            GL.Vertex3(triangleA[2]);

            GL.Normal3(normals[1]);
            GL.Vertex3(triangleB[0]);
            GL.Vertex3(triangleB[1]);
            GL.Vertex3(triangleB[2]);
            GL.End();
        }

        //-------------------------------------//
        // NORMAL CALCULATION
        //-------------------------------------//
        Vector3 calculateNormals(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC) {

            Vector3 tangentA = Vector3.Subtract(vertexB, vertexA);
            Vector3 tangentB = Vector3.Subtract(vertexC, vertexA);

            Vector3 normal = Vector3.Cross(tangentA, tangentB);
            normal.Normalize();
            return normal;
        }
    }
}
