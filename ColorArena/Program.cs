using System;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace ColorArena
{
    class Program
    {
        public static Brick brick { get; set; }

        static void Main(string[] args)
        {
            Connect();

            brick.BrickChanged += DistanceChanged;
            brick.BrickChanged += ColorChanged;

            brick.BrickChanged += CollisionAvoidance;

            Forward();

            /* :loop
             * Move forward() indefinitely,
             * stop in proximity of a colored arena wall,
             * if color == base_color then stop, else continue
             * to rotate() until no obstacle in path,
             * goto loop
             */

            Console.WriteLine("Sequence complete!");
            Console.ReadKey();
        }

        static async void Connect()
        {
            brick = new Brick(new UsbCommunication());
            await brick.ConnectAsync();
        }

        // Rotate both motors indefinitely to move forward.
        static async void Forward()
        {
            brick.BatchCommand.StartMotor(OutputPort.A);
            brick.BatchCommand.TurnMotorAtPower(OutputPort.A, 20);

            brick.BatchCommand.StartMotor(OutputPort.D);
            brick.BatchCommand.TurnMotorAtPower(OutputPort.D, 20);

            await brick.BatchCommand.SendCommandAsync();
        }

        // Stop both motors when in close proximity of an obstacle.
        static async void CollisionAvoidance(object sender, BrickChangedEventArgs e)
        {
            if (e.Ports[InputPort.Two].SIValue < 10)
            {
                brick.BatchCommand.StopMotor(OutputPort.A, true);
                brick.BatchCommand.StopMotor(OutputPort.D, true);

                await brick.BatchCommand.SendCommandAsync();
            }
        }

        // Rotate until path ahead is clear; rendering unnecessary: the gyroscope, and
        // brute-forcing of imprecise motor power/speed parameters to reproduce the desired angle.
        //static async void Rotate() { }

        // Raw ultrasonic values
        static void DistanceChanged(object sender, BrickChangedEventArgs e)
        {
            Console.WriteLine(e.Ports[InputPort.Two].SIValue.ToString());
        }

        // Raw color sensor values
        static void ColorChanged(object sender, BrickChangedEventArgs e)
        {
            brick.Ports[InputPort.Four].SetMode(ColorMode.Color);
            Console.WriteLine(e.Ports[InputPort.Four].SIValue.ToString());
        }
    }
}