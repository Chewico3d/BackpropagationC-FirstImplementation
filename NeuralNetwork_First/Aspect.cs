using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;


namespace NeuralNetwork_First
{
    public static class Aspect
    {

        private static NeuralNetwork network => Program.network;

        public static CircleShape Circle = new CircleShape();
        public static CircleShape CircleT = new CircleShape();

        public static CircleShape CircleP = new CircleShape();
        public static RectangleShape Rectangle = new RectangleShape();

        static Vertex[] Vertices = new Vertex[2];

        public static void SetUp()
        {
            Circle.Radius = 20f;
            Circle.FillColor = Color.White;
            Circle.OutlineThickness = 4f;
            Circle.OutlineColor = Color.Red;
            Circle.Origin = new Vector2f(20f, 20f);

            CircleT.Radius = 10f;
            CircleT.FillColor = Color.White;
            CircleT.Origin = new Vector2f(10f, 10f);
            CircleT.OutlineThickness = 3f;
            CircleT.OutlineColor = Color.Red;

            CircleP.Radius = 5f;
            CircleP.Origin = new Vector2f(5f, 5f);
            CircleP.OutlineThickness = 3f;

            Rectangle.Size = new Vector2f(10f, 10f);

        }
        public static void RenderDebugg()
        {
            for(int x = 0; x < 80; x++)
            {
                for (int y = 0; y < 80; y++)
                {
                    int XPos = 10 * x;
                    int YPos = 10 * y;

                    float RelativeX = (float)x / 80f;
                    float RelativeY = (float)y / 80f;

                    network.Values[0][0] = RelativeX;
                    network.Values[0][1] = RelativeY;

                    network.FrontPropagation();

                    float FinalValueR = Clamp01( network.Values[network.LayersSizes.Length - 1][0]);
                    float FinalValueG = Clamp01(network.Values[network.LayersSizes.Length - 1][1]);
                    float FinalValueB = Clamp01(network.Values[network.LayersSizes.Length - 1][2]);

                    Color FinalColor = new Color((byte)(FinalValueR * 255), (byte)(FinalValueG * 255), (byte)(FinalValueB * 255));
                    Rectangle.FillColor = FinalColor;

                    Rectangle.Position = new Vector2f(XPos, YPos);
                    Rectangle.Draw(Program.window, RenderStates.Default);

                }
            }

        }
        public static void DrawProbes()
        {
            for(int x = 0; x < Program.Positions.Count; x++)
            {
                Vector3f Pos = Program.Positions[x];

                switch (Pos.Z)
                {
                    case 0:
                        CircleP.FillColor = Color.Red;
                        CircleP.OutlineColor = Color.Black;
                        break;
                    case 1:
                        CircleP.FillColor = Color.Green;
                        CircleP.OutlineColor = Color.White;
                        break;
                    case 2:
                        CircleP.FillColor = Color.Blue;
                        CircleP.OutlineColor = Color.White;
                        break;
                    case 3:
                        CircleP.FillColor = Color.Black;
                        CircleP.OutlineColor = Color.White;
                        break;
                }

                CircleP.Position = new Vector2f(Pos.X, Pos.Y);
                CircleP.Draw(Program.window, RenderStates.Default);

            }

            CircleP.FillColor = Color.Cyan;
            CircleP.OutlineColor = Color.White;
            CircleP.Position = new Vector2f(Program.DebuggNeuralNetwor.X, Program.DebuggNeuralNetwor.Y);
            CircleP.Draw(Program.window, RenderStates.Default);

            if (Program.Positions.Count == 0 || !Program.Train)
                return;

            for (int x = 0; x < 10000; x++)
            {

                int Random = (int)(NeuralNetwork.randomGenerator.NextDouble() * Program.Positions.Count);
                Vector3f Pos = Program.Positions[Random];

                network.Values[0][0] = (float)Pos.X / 800f;
                network.Values[0][1] = (float)Pos.Y / 800f;

                network.FrontPropagation();
                switch (Pos.Z)
                {
                    case 0:
                        network.BackPropagate(new float[] {1,0,0});
                        break;
                    case 1:
                        network.BackPropagate(new float[] { 0, 1, 0 });
                        break;
                    case 2:
                        network.BackPropagate(new float[] { 0, 0, 1 });
                        break;
                    case 3:
                        network.BackPropagate(new float[] { 0, 0, 0 });
                        break;
                }

            }
        }
        public static void RenderNeuralNetwork()
        {
            
            //Every Layer
            for (int x = 0; x < network.LayersSizes.Length - 1; x++)
            {

                for (int u = 0; u < network.LayersSizes[x]; u++)
                {
                    for (int v = 0; v < network.LayersSizes[x + 1]; v++)
                    {
                        Vertices[0].Position = GetPosition(x, u);
                        Vertices[1].Position = GetPosition(x + 1, v);

                        float RelativeValue = network.Weights[x][u][v];
                        float RelativeError = network.Derivatives[x][u][v];
                        RelativeError = Clamp01(RelativeError);
                        Color FinalColor;
                        byte ErCol = (byte)(RelativeError * 255);


                        if (RelativeValue < 0)
                        {
                            RelativeValue = (RelativeValue < -1) ? 1 : -RelativeValue;
                            byte ColVal = (byte)(RelativeValue * 255);
                            FinalColor = new Color(ColVal, 0, ErCol);

                        }
                        else
                        {
                            RelativeValue = (RelativeValue > 1) ? 1 : RelativeValue;
                            byte ColVal = (byte)(RelativeValue * 255);
                            FinalColor = new Color(0, ColVal, ErCol);

                        }

                        Vertices[0].Color = FinalColor;
                        Vertices[1].Color = FinalColor;

                        Program.window.Draw(Vertices, PrimitiveType.Lines);

                    }

                }

            }
            //Every Layer
            for (int x = 0; x < network.LayersSizes.Length; x++)
            {
                //Every Neuron
                for (int y = 0; y < network.LayersSizes[x]; y++)
                {
                    float Value = network.Values[x][y];
                    if(x != 0)
                    {

                        Circle.OutlineThickness = 5f;

                        float Error = network.NeuronDerivativeError[x][y];
                        Error = Clamp01(Error);

                        byte ColErr = (byte)(255 * Error);
                        Color color = new Color(ColErr, 0, 0);

                        Circle.OutlineColor = color;

                    }
                    else
                    {
                        Circle.OutlineThickness = 1f;
                        Circle.OutlineColor = Color.White;

                    }

                    byte ColVal = (byte)(Value * 255);

                    Circle.FillColor = new Color(ColVal, ColVal, ColVal);

                    Circle.Position = GetPosition(x, y);
                    Circle.Draw(Program.window, RenderStates.Default);

                }
            }
            //Every Layer
            for (int x = 0; x < network.LayersSizes.Length - 1; x++)
            {
                for (int u = 0; u < network.LayersSizes[x + 1]; u++)
                {
                    float BiasValue = network.Bias[x + 1][u];
                    if(BiasValue > 0)
                    {
                        BiasValue = (BiasValue > 1)? 1 : BiasValue;
                        Byte BiasCol = (byte)(255 * BiasValue);

                        CircleT.FillColor = new Color(0, 0, BiasCol);
                    }
                    else
                    {
                        BiasValue = (BiasValue < -1) ? 1 : -BiasValue;
                        Byte BiasCol = (byte)(255 * BiasValue);

                        CircleT.FillColor = new Color(BiasCol, 0, 0);

                    }

                    CircleT.Position = GetPosition(x + 1, u) - new Vector2f(20f, 20f);
                    CircleT.Draw(Program.window, RenderStates.Default);

                }

            }

        }

        static Vector2f GetPosition(int x, int y)
        {
            float NumberOfLayers = network.LayersSizes.Length;
            float RelativePosX = (float)x / (NumberOfLayers - 1);

            RelativePosX = Lerp(900, 1500, RelativePosX);
            int PosX = (int)RelativePosX;

            float NumberOfNeuronsInThisLayer = network.LayersSizes[x];
            float RelativeY = (float)(y + 1) / (float)(NumberOfNeuronsInThisLayer + 1);

            int PosY = (int)Lerp(0, 800, RelativeY);

            return new Vector2f(PosX, PosY);

        }

        static float Lerp(float A, float B, float Time) => (B - A) * Time + A;
        static float Clamp01(float Value)
        {
            float Val = (Value < 0) ? 0 : Value;
            return (Val > 1) ? 1 : Val;
        }

    }
}
