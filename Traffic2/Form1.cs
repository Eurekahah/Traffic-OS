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
        private System.Windows.Forms.Timer timer;        
        private TrafficLightSystem trafficLightSystem;
        private TrafficLightControl trafficLight;
        private TrafficLightControl trafficLight1;
        private TrafficLightControl trafficLight2;
        private TrafficLightControl trafficLight3;
        private Road road1;
        private Road road2;
        private Road road3;
        private Road road4;
        private TrafficLaneSystem trafficLaneSystem;

        private Semaphore northSemaphore = new Semaphore(0,1);
        private Semaphore southSemaphore = new Semaphore(0, 1);
        private Semaphore eastSemaphore = new Semaphore(0, 1);
        private Semaphore westSemaphore = new Semaphore(0, 1);

        public Form1()
        {

            InitializeComponent();


            //this.Load += Form1_Load1;
            //this.Shown += Form1_Load2;
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

        

        //private SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);

        //private async void Form1_Load1(object sender, EventArgs e)
        //{
        //    // 在 UI 线程上初始化 GUI
        //    await semaphore.WaitAsync(); // 等待信号量释放，确保初始化完成
        //    Init(); // 执行初始化
        //}

        //private async void Form1_Load2(object sender, EventArgs e)
        //{
        //    // 创建一个列表来存储任务
        //    List<Task> tasks = new List<Task>();

        //    await semaphore.WaitAsync(); // 等待初始化完成后释放的信号量

        //    foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        //    {
        //        // 为每个方向创建一个新的任务
        //        Task task = Task.Run(() => TaskDrive(direction));
        //        tasks.Add(task);
        //    }

        //    // 等待所有任务完成
        //    await Task.WhenAll(tasks);

        //    semaphore.Release(); // 释放信号量，表示后续任务执行完成
        //}

        //public void Init()
        //{
        //    if (InvokeRequired)
        //    {
        //        BeginInvoke(new Action(InitGUI));
        //        semaphore.Release(); // 初始化完成后释放信号量
        //        return;
        //    }

        //    semaphore.Release(); // 初始化完成后释放信号量
        //}

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
            trafficLight = new TrafficLightControl(0, 0, Direction.East);
            trafficLight.Location = new Point(this.Width / 2 -10-100-trafficLight.Width,this.Height / 2 -10 -100 -trafficLight.Height);
            this.Controls.Add(trafficLight);
            trafficLight.Passable();
            this.Controls.SetChildIndex(trafficLight, this.Controls.Count - 1);

            trafficLight1 = new TrafficLightControl(0, 0, Direction.West);
            trafficLight1.Location = new Point(this.Width / 2 + 10 + 100, this.Height / 2 + 10 + 100 );
            this.Controls.Add(trafficLight1);
            trafficLight1.Passable();
            this.Controls.SetChildIndex(trafficLight1, this.Controls.Count - 1);

            trafficLight2 = new TrafficLightControl(0, 0, Direction.North);
            trafficLight2.Location = new Point(this.Width / 2 - 10 - 100 - trafficLight2.Width, this.Height / 2 + 10 + 100 );
            this.Controls.Add(trafficLight2);
            trafficLight2.Blocked();
            this.Controls.SetChildIndex(trafficLight2, this.Controls.Count - 1);

            trafficLight3 = new TrafficLightControl(0, 0, Direction.South);
            trafficLight3.Location = new Point(this.Width / 2 + 10 + 100 , this.Height / 2 - 10 - 100 - trafficLight3.Height);
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

        public async void TaskDrive(Direction direction)
        {
            List<Task> drivingTasks = new List<Task>();

            
            for (int laneNum = 1; laneNum <= 2; laneNum++)
            {
                Queue<CarBase> carQueue = trafficLaneSystem.GetLaneWithDirectionAndLaneNum(direction, laneNum).GetCarQueue();                
                if (carQueue.Count == 0) break;
                

                foreach (CarBase car in carQueue)
                {
                    if (car != null)
                    {
                        drivingTasks.Add(car.Drive( trafficLightSystem, trafficLaneSystem));
                    }
                }

                
                
                
            }
            await Task.WhenAll(drivingTasks);
        }


        // 创建两个信号量,每个初始计数值为 1
        private static Semaphore eastWestSemaphore = new Semaphore(16, 16);
        private static Semaphore northSouthSemaphore = new Semaphore(16, 16);
        private static int crossSize = 0;

        // 根据方向获取对应的信号量
        public static Semaphore GetSemaphoreForDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                case Direction.West:
                    return eastWestSemaphore;
                case Direction.North:
                case Direction.South:
                    return northSouthSemaphore;
                default:
                    throw new ArgumentException("Invalid direction");
            }
        }
        private static Semaphore crossSemaphore = new Semaphore(1, 1);

        public static int GetCrossSize()
        {
            return crossSize;
        }
        public static void DecrementCrossSize()
        {
            crossSemaphore.WaitOne();
            crossSize--;
            crossSemaphore.Release();
        }
        public static void IncreaseCrossSize()
        {
            crossSemaphore.WaitOne();
            crossSize++;
            crossSemaphore.Release();
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
                    return eastSemaphore2;
                case Direction.West:
                    return eastSemaphore2;
                case Direction.North:
                    return northSemaphore2;
                case Direction.South:
                    return northSemaphore2;
                default:
                    throw new ArgumentException("Invalid direction");
            }
        }
    }

}


//foreach (CarBase car in carQueue)
//{
//    if (car == null) continue;
//    if (car != null)
//    {
//        if (await car.IsAbleToDrive(trafficLightSystem, trafficLaneSystem))
//        {
//            await car.Drive();
//            //Console.WriteLine("车辆正在行驶...");
//        }
//        else
//        {
//            car.Stop();
//        }
//    }
//}