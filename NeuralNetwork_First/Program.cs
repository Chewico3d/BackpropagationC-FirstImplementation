using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SFML;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace NeuralNetwork_First
{
    static class Program
    {

        public static int ColorRel;
        public static bool Train = false;

        public static RenderWindow window;
        public static NeuralNetwork network;

        public static List<Vector3f> Positions = new List<Vector3f>();
        public static Vector2f DebuggNeuralNetwor;

        private static bool LastButtonPressed;

        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static void Main()
        {
            ContextSettings settings = new ContextSettings();
            settings.AntialiasingLevel = 8;

            // Create the main window
            window = new RenderWindow(new VideoMode(1600, 800), "SFML Works!", Styles.Default, settings);
            window.Closed += new EventHandler(OnClose);

            network = new NeuralNetwork(new int[] { 2, 5,3});
            Aspect.SetUp();

            MainLoop();

        } //End Main()

        static void MainLoop()
        {

            // Start the game loop
            while (window.IsOpen)
            {
                // Process events
                window.DispatchEvents();
                //Buttons
                RecordProbes();

                //Train

                // Clear screen
                window.Clear(Color.Black);

                //Everithing
                Aspect.RenderDebugg();
                Aspect.DrawProbes();

                network.Values[0][0] = DebuggNeuralNetwor.X / 800f;
                network.Values[0][1] = DebuggNeuralNetwor.Y / 800f;
                network.FrontPropagation();
                //network.BackPropagate(new float[] {1});

                Aspect.RenderNeuralNetwork();

                // Update the window
                window.Display();
            } //End game loop

        }

        static void RecordProbes()
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left) & ColorRel != 3)
            {
                Vector2i CurrentPos = Mouse.GetPosition(window);
                if(!LastButtonPressed)
                    Positions.Add(new Vector3f(CurrentPos.X, CurrentPos.Y , ColorRel));

                LastButtonPressed = true;
            }
            else if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                ColorRel = 3;
                LastButtonPressed = true;
            }
            else
                LastButtonPressed = false;

            if (Mouse.IsButtonPressed(Mouse.Button.Middle)){
                DebuggNeuralNetwor = (Vector2f)Mouse.GetPosition(window);
                Train = true;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.R))
                ColorRel = 0;
            else if(Keyboard.IsKeyPressed(Keyboard.Key.G))
                ColorRel = 1;
            else if(Keyboard.IsKeyPressed(Keyboard.Key.B))
                ColorRel = 2;

        }

    } //End Program

}