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

            // Raw ultrasonic, photodiode values.
            brick.BrickChanged += Sensors;

            /* There are 4 combinations of colors in each corner of the box.
             * By knowing the order of which the brick traverses by,
             * we can find a corner even by just specifying one. i.e.,
             * BASE        |CLOCKWISE|COUNTER-CLOCKWISE
             * ------------|---------|-----------------
             * red+black   |black    |red
             * black+yellow|yellow   |black
             * yellow+blue |blue     |yellow
             * blue+red    |red      |blue
             */
            Console.Write("Corner color: ");
            int corner_color = Convert.ToInt32(Console.ReadLine());

            /* Demo sequence
             * Move forward indefinitely,
             * rotate when in close proximity of an obstacle ahead until path clear,
             * loop if corner_color isn't found, else stop.
             * (this renders unnecessary: the drifting-prone gyroscope and brute-forcing of motor
             * power parameters to produce the desired angle in a seemingly unreliable fashion.)
             */
            do
            {
                Move(20, 20);
                Console.WriteLine("Moving forward");

                while (proximity > 10 && color != corner_color) { }
                Console.WriteLine("Obstacle encountered");

                Move(20, -20);
                Console.WriteLine("Rotating clockwise");

                while (proximity < 20 && color != corner_color) { }
                Console.WriteLine("Path clear");
            } while (color != corner_color);

            Stop();
            Console.WriteLine("EOF: Corner found?");
            Console.ReadKey();
        }

        static async void Connect()
        {
            brick = new Brick(new UsbCommunication());
            await brick.ConnectAsync();
        }

        // Rotate both motors indefinitely.
        // powerA/powerD parameters dictate speed and direction.
        static async void Move(int powerA, int powerD)
        {
            brick.BatchCommand.StartMotor(OutputPort.A);
            brick.BatchCommand.StartMotor(OutputPort.D);

            brick.BatchCommand.TurnMotorAtPower(OutputPort.A, powerA);
            brick.BatchCommand.TurnMotorAtPower(OutputPort.D, powerD);

            await brick.BatchCommand.SendCommandAsync();
        }

        static async void Stop()
        {
            brick.BatchCommand.StopMotor(OutputPort.A, true);
            brick.BatchCommand.StopMotor(OutputPort.D, true);

            await brick.BatchCommand.SendCommandAsync();
        }

        static void Sensors(object sender, BrickChangedEventArgs e)
        {
            proximity = e.Ports[InputPort.Two].SIValue;

            e.Ports[InputPort.Four].SetMode(ColorMode.Color);
            color = e.Ports[InputPort.Four].SIValue;
        }
    }
}