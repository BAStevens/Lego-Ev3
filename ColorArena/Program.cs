using System;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System.Threading;

namespace ColorArena
{
    class Program
    {
        public static Brick brick { get; set; }
        public static float proximity { get; set; }
        public static float color { get; set; }
        public static float angle { get; set; }

        static void Main(string[] args)
        {
            BluetoothConnect();

            brick.BrickChanged += DistanceChanged;
            brick.BrickChanged += ColorChanged;
            brick.BrickChanged += GyroChanged;

            Console.WriteLine("Enter base color: ");
            int basecolor1 = Int32.Parse(Console.ReadLine());
            while (color != basecolor1)
            {
                Move(20, 20);

                while (proximity > 3) { }

                Move(-30, -50);

                while (proximity < 20) { }
                Move(20, 20);



            }

            Stop();

            //    Console.WriteLine("Moving forward.");

            //    while (proximity > 10 && color != base_color) { }
            //    Console.WriteLine("Obstacle encountered.");

            //    Move(20, -20);
            //    Console.WriteLine("Turning right.");

            //    while (proximity < 20 && color != base_color) { }
            //    Console.WriteLine("Path clear.");





            Console.WriteLine("Sequence complete!");
            Console.ReadKey();
        }

        static async void Connect()
        {
            brick = new Brick(new UsbCommunication());
            await brick.ConnectAsync();
        }

        static async void BluetoothConnect()
        {
            brick = new Brick(new BluetoothCommunication("com4"));
            await brick.ConnectAsync();
        }
        // Rotate both motors indefinitely to move forward.
        static async void Move(int powerA, int powerD)
        {
            brick.BatchCommand.StartMotor(OutputPort.A);
            brick.BatchCommand.TurnMotorAtPower(OutputPort.A, powerA);

            brick.BatchCommand.StartMotor(OutputPort.D);
            brick.BatchCommand.TurnMotorAtPower(OutputPort.D, powerD);

            await brick.BatchCommand.SendCommandAsync();
        }



        static async void Reverse()
        {
            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.A, -20, 3000, false);
            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.D, -20, 3000, false);

            await brick.BatchCommand.SendCommandAsync();
        }

        static async void Stop()
        {
            brick.BatchCommand.StopMotor(OutputPort.A, true);
            brick.BatchCommand.StopMotor(OutputPort.D, true);

            await brick.BatchCommand.SendCommandAsync();
        }





        // Rotate until path ahead is clear; rendering unnecessary: the gyroscope, and
        // brute-forcing of imprecise motor power/speed parameters to reproduce the desired angle.
        //static async void Rotate() { }

        // Raw ultrasonic values
        static void DistanceChanged(object sender, BrickChangedEventArgs e)
        {
            proximity = e.Ports[InputPort.Two].SIValue;
        }

        // Raw color sensor values
        static void ColorChanged(object sender, BrickChangedEventArgs e)
        {
            brick.Ports[InputPort.Four].SetMode(ColorMode.Color);
            color = e.Ports[InputPort.Four].SIValue;
        }

        static void GyroChanged(object sender, BrickChangedEventArgs e)
        {
            angle = e.Ports[InputPort.One].SIValue;
        }
    }
}