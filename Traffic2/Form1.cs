using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Traffic2
{
    public partial class Form1 : Form
    {       
        private TrafficLightSystem trafficLightSystem;                
        private TrafficLaneSystem trafficLaneSystem;
        public Form1()
        {

            InitializeComponent();

            InitGUI();
            // 创建一个列表来存储任务
            List<Task> tasks = new List<Task>();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                // 为每个方向创建一个新的任务
                
                Task task = Task.Run(() => trafficLaneSystem.DriveTask(direction,trafficLightSystem.GetTrafficLight(direction)));
                tasks.Add(task);
            }
            Task.WhenAll();
        }


        public void InitGUI()
        {            
            this.Size = new Size(800, 800);
            trafficLaneSystem = new TrafficLaneSystem();
            trafficLaneSystem.PutCar(this);
            InitTrafficLight();
            InitRoad();
        }

        public void InitTrafficLight()
        {
            TrafficLightControl trafficLight;
            TrafficLightControl trafficLight1;
            TrafficLightControl trafficLight2;
            TrafficLightControl trafficLight3;
            trafficLight = new TrafficLightControl(0, 0, Direction.East);
            trafficLight.Location = new Point(this.Width / 2 - 10 - 100 - trafficLight.Width, this.Height / 2 - 10 - 100 - trafficLight.Height);
            this.Controls.Add(trafficLight);
            trafficLight.Passable();
            this.Controls.SetChildIndex(trafficLight, this.Controls.Count - 1);

            trafficLight1 = new TrafficLightControl(0, 0, Direction.West);
            trafficLight1.Location = new Point(this.Width / 2 + 10 + 100, this.Height / 2 + 10 + 100);
            this.Controls.Add(trafficLight1);
            trafficLight1.Passable();
            this.Controls.SetChildIndex(trafficLight1, this.Controls.Count - 1);

            trafficLight2 = new TrafficLightControl(0, 0, Direction.North);
            trafficLight2.Location = new Point(this.Width / 2 - 10 - 100 - trafficLight2.Width, this.Height / 2 + 10 + 100);
            this.Controls.Add(trafficLight2);
            trafficLight2.Blocked();
            this.Controls.SetChildIndex(trafficLight2, this.Controls.Count - 1);

            trafficLight3 = new TrafficLightControl(0, 0, Direction.South);
            trafficLight3.Location = new Point(this.Width / 2 + 10 + 100, this.Height / 2 - 10 - 100 - trafficLight3.Height);
            this.Controls.Add(trafficLight3);
            trafficLight3.Blocked();
            this.Controls.SetChildIndex(trafficLight3, this.Controls.Count - 1);

            trafficLightSystem = new TrafficLightSystem();
            trafficLightSystem.AddTrafficLight(trafficLight.GetDirection(), trafficLight);
            trafficLightSystem.AddTrafficLight(trafficLight1.GetDirection(), trafficLight1);
            trafficLightSystem.AddTrafficLight(trafficLight2.GetDirection(), trafficLight2);
            trafficLightSystem.AddTrafficLight(trafficLight3.GetDirection(), trafficLight3);

        }

        public void InitRoad()
        {
            Road road1;
            Road road2;
            Road road3;
            Road road4;
            road1 = new Road(0, (int)(this.Height / 2 - 10 - 100), this.Width, 100);
            this.Controls.Add(road1);
            this.Controls.SetChildIndex(road1, this.Controls.Count - 1);

            road2 = new Road(0, (int)(this.Height / 2 + 10), this.Width, 100);
            this.Controls.Add(road2);
            this.Controls.SetChildIndex(road2, this.Controls.Count - 1);

            road3 = new Road(this.Width / 2 - 10 - 100, 0, 100, this.Height);
            this.Controls.Add(road3);
            this.Controls.SetChildIndex(road3, this.Controls.Count - 1);

            road4 = new Road(this.Width / 2 + 10, 0, 100, this.Height);
            this.Controls.Add(road4);
            this.Controls.SetChildIndex(road4, this.Controls.Count - 1);
        }



        private static Semaphore eastSemaphore2 = new Semaphore(1, 1);
        private static Semaphore northSemaphore2 = new Semaphore(1, 1);
        //private static Semaphore westSemaphore2 = new Semaphore(1, 1);
        //private static Semaphore southSemaphore2 = new Semaphore(1, 1);
        public static Semaphore GetSemaphore2ForDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    //return eastSemaphore2;
                case Direction.West:
                    return eastSemaphore2;
                case Direction.North:
                    //return northSemaphore2;
                case Direction.South:
                    return northSemaphore2;
                default:
                    throw new ArgumentException("Invalid direction");
            }
        }
    }

}
