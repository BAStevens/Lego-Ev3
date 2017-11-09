using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System.Threading;

namespace Week_9
{
    class Program
    {
        public static Brick Brick { get; set; }
        static void Main(string[] args)
        {

            ConnectToBrick();
            // Turn180();
            //  System.Threading.Thread.Sleep(1000);
            // DriveBackwards();
            // System.Threading.Thread.Sleep(1000);
            // Turn180();
            // DriveForwardwards();
            //Color();
            Turn180();
            Console.Write("Sequence Complete");
            Console.ReadLine();

        }

        static async void ConnectToBrick()
        {
            Brick = new Brick(new UsbCommunication());

            await Brick.ConnectAsync();
        }

        static async void Turn45()
        {
            await Brick.DirectCommand.TurnMotorAtSpeedForTimeAsync(OutputPort.D, 50, 375, false);
        }

        static async void Turn90()
        {
            await Brick.DirectCommand.TurnMotorAtSpeedForTimeAsync(OutputPort.D, 50, 750, false);
        }
        static async void Turn180()
        {

            Brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.A, 100, 400, false);
            Brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.D, -100, 400, false);

            await Brick.BatchCommand.SendCommandAsync();
        }

        static async void DriveBackwards()
        {
            Brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.A, -100, 1000, false);
            Brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.D, -100, 1000, false);

            await Brick.BatchCommand.SendCommandAsync();
        }
        static async void DriveForwardwards()
        {
            Brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.A, 100, 1000, false);
            Brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.D, 100, 1000, false);

            await Brick.BatchCommand.SendCommandAsync();
        }

        static void Colour(object sender, BrickChangedEventArgs e)
            {
            Brick.Ports[InputPort.Two].SetMode(ColorMode.Reflective);
            Console.WriteLine(e.Ports[InputPort.Two].SIValue.ToString());
        }
    }
}
