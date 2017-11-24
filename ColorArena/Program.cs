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
        public static float angle { get; set; }

        static void Main(string[] args)
        {
            BluetoothConnect();

            brick.BrickChanged += DistanceChanged;
            brick.BrickChanged += ColorChanged;
            brick.BrickChanged += GyroChanged;

            

            

            

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