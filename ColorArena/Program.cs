using System;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace ColorArena
{
    class Program
    {
        public static Brick brick { get; set; }
        public static float proximity { get; set; }
        public static float color { get; set; }

        static void Main(string[] args)
        {
            Connect();

            Console.Write("Base color: ");
            int base_color = Convert.ToInt32(Console.ReadLine());

            // Event handlers to store sensor proximity, color values.
            brick.BrickChanged += UltrasonicChanged;
            brick.BrickChanged += PhotodiodeChanged;

            // Demo sequence
            // Loops until base_color is detected.
            do
            {
                Move(20, 20);
                Console.WriteLine("Moving forward.");

                while (proximity > 10 && color != base_color) { }
                Console.WriteLine("Obstacle encountered.");

                Move(20, -20);
                Console.WriteLine("Turning right.");

                while (proximity < 20 && color != base_color) { }
                Console.WriteLine("Path clear.");
            } while (color != base_color);

            Stop();
            Console.WriteLine("Base found.");

            Console.ReadKey();

            /* Move forward indefinitely,
             * rotate when in close proximity until path clear,
             * loop if color != base_color, else stop.
             * 
             * (this renders unnecessary: the drifting-prone gyroscope and brute-forcing
             * of imprecise motor power/speed parameters to reproduce the desired angle.)
             */
        }

        static async void Connect()
        {
            brick = new Brick(new UsbCommunication());
            await brick.ConnectAsync();
        }

        // Rotate both motors indefinitely to move.
        // powerA/powerD parameters dictate speed and direction.
        static async void Move(int powerA, int powerD)
        {
            brick.BatchCommand.StartMotor(OutputPort.A);
            brick.BatchCommand.TurnMotorAtPower(OutputPort.A, powerA);

            brick.BatchCommand.StartMotor(OutputPort.D);
            brick.BatchCommand.TurnMotorAtPower(OutputPort.D, powerD);

            await brick.BatchCommand.SendCommandAsync();
        }

        static async void Stop()
        {
            brick.BatchCommand.StopMotor(OutputPort.A, true);
            brick.BatchCommand.StopMotor(OutputPort.D, true);

            await brick.BatchCommand.SendCommandAsync();
        }

        static void UltrasonicChanged(object sender, BrickChangedEventArgs e)
        {
            proximity = e.Ports[InputPort.Two].SIValue;
        }

        static void PhotodiodeChanged(object sender, BrickChangedEventArgs e)
        {
            brick.Ports[InputPort.Four].SetMode(ColorMode.Color);
            color = e.Ports[InputPort.Four].SIValue;
        }
    }
}