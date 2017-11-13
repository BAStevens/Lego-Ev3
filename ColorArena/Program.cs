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

            brick.BrickChanged += UltrasonicChanged;
            brick.BrickChanged += PhotodiodeChanged;

			// Demo sequence
            Console.WriteLine("Moving forward.");
            Move(20, 20);

            while (proximity > 5) { }
            Console.WriteLine("Turning left.");
            Move(20, -20);

            while (proximity < 5) { }
            Stop();

            Console.WriteLine("EOF");
            Console.ReadKey();

            /* Move forward() indefinitely,
             * stop in proximity of a colored arena wall,
             * if color == base_color then stop, else continue
             * to rotate() until no obstacle in path, rendering
             * unnecessary: the drifting-prone gyroscope and brute-forcing
             * of imprecise motor power/speed parameters to
             * reproduce the desired angle.
             * Loop
             */
        }

        static async void Connect()
        {
            brick = new Brick(new UsbCommunication());
            await brick.ConnectAsync();
        }

        // Rotate both motors indefinitely to move.
        // Direction is dictated by powerA/powerD parameters.
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
