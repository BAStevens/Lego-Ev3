using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace WorkingWithSensors
{
    public class Program
    {
        public static Brick brick { get; set; }

        static void Main(string[] args)
        {
            ConnectToBrick();


            brick.BrickChanged += ButtonChanged;
            brick.BrickChanged += DistanceChanged;

            Console.WriteLine(brick.Ports[InputPort.One].SIValue.ToString());

            Console.ReadLine();
        }

        static async void ConnectToBrick()
        {
            brick = new Brick(new UsbCommunication());
            await brick.ConnectAsync();
        }

        static void ButtonChanged(object sender, BrickChangedEventArgs e)
        {
            Console.WriteLine(e.Ports[InputPort.One].SIValue.ToString());
        }

        static  void DistanceChanged(object sender, BrickChangedEventArgs e)
        {
            brick.Ports[InputPort.Two].SetMode(ColorMode.Reflective);
            Console.WriteLine(e.Ports[InputPort.Two].SIValue.ToString());
        }
    }
}