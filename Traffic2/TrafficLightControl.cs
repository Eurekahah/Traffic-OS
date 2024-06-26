﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Traffic2
{
    public class TrafficLightSystem
    {
        // 存储方向与交通信号灯之间的对应关系
        private Dictionary<Direction, TrafficLightControl> _trafficLights;

        // 定时器用于切换方向
        private System.Windows.Forms.Timer _directionSwitchTimer;

        // 构造函数初始化字典
        public TrafficLightSystem()
        {
            _trafficLights = new Dictionary<Direction, TrafficLightControl>();
            // 创建定时器,每隔8秒执行一次SwitchDirection方法
            _directionSwitchTimer = new System.Windows.Forms.Timer();
            _directionSwitchTimer.Tick += SwitchDirection;
            _directionSwitchTimer.Interval = 8000;
            _directionSwitchTimer.Start();
        }

        // 添加交通信号灯到系统中
        public void AddTrafficLight(Direction direction, TrafficLightControl trafficLight)
        {
            _trafficLights[direction] = trafficLight;

        }

        // 获取特定方向的交通信号灯
        public TrafficLightControl GetTrafficLight(Direction direction)
        {
            if (_trafficLights.ContainsKey(direction))
            {
                return _trafficLights[direction];
            }
            else
            {
                return null; // 或者抛出异常，表示未找到对应方向的交通信号灯
            }
        }

        public void SwitchDirection(object sender, EventArgs e)
        {

            if (_trafficLights[Direction.East].CanPass()&& _trafficLights[Direction.West].CanPass())
            {
                _trafficLights[Direction.East].Blocked();
                _trafficLights[Direction.West].Blocked();
                _trafficLights[Direction.South].Passable();
                _trafficLights[Direction.North].Passable();
            }
            else if (_trafficLights[Direction.South].CanPass() && _trafficLights[Direction.North].CanPass())
            {
                _trafficLights[Direction.South].Blocked();
                _trafficLights[Direction.North].Blocked();
                _trafficLights[Direction.East].Passable();
                _trafficLights[Direction.West].Passable();
                
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    public class TrafficLightControl : Control
    {
        private Light _redLight;

        private Light _greenLight;

        private static int _radius = 15;

        private Direction _direction;


        public TrafficLightControl(int left,int top, Direction dir) : base("", left,top, 5 * 2 + _radius * 2, 5 * 2 + _radius * 4 + 5)
        {

            _direction = dir; 

            this.BackColor = Color.Black;            

            // 根据方向调整灯位置
            if (_direction == Direction.North || _direction == Direction.South)
            {
                this.Size = new Size(5 * 2 + _radius * 4 + 5, 5 * 2 + _radius * 2);

                this._redLight = new Light(Color.Red, 5, 5, _radius);
                this._greenLight = new Light(Color.Green, _radius * 2 + 5 + 5, 5, _radius);

            }
            else
            {
                this.Size = new Size(5 * 2 + _radius * 2, 5 * 2 + _radius * 4 + 5);

                this._redLight = new Light(Color.Red, 5, 5, _radius);
                this._greenLight = new Light(Color.Green, 5, _radius * 2 + 5 + 5, _radius);

                
            }

            this.Controls.Add(_greenLight);

            this.Controls.Add(_redLight);                       
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            
            // 绘制外壳
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);

            _redLight.Invalidate();
            _greenLight.Invalidate();
        }

        public void Passable()
        {
            this._redLight.TurnOff();
            this._greenLight.TurnOn();
        }

        public void Blocked()
        {
            this._redLight.TurnOn();
            this._greenLight.TurnOff();
        }

        public bool CanPass()
        {
            return _redLight.GetCurrentColor() != _redLight.GetBrightColor()
                &&  _greenLight.GetCurrentColor() == _greenLight.GetBrightColor();            
        }

        public Direction GetDirection() { return _direction; }

    }

    public class Light : Control
    {

        protected Color _currentColor; //当前的颜色
        protected Color _brightColor; //灯亮起的颜色
        private int _radius;//半径

        public Light(Color brightcolor, int left, int top ,int radius = 15) : base("",left,top,radius * 2,radius * 2)
        {
            this._currentColor = Color.Gray;           
            this._brightColor = brightcolor;
            this.BackColor = Color.Black;
            this._radius = radius;
            
        }
        public void SetCurrentColor(Color color) { _currentColor = color; }
        public Color GetCurrentColor() { return _currentColor; }

        public void SetBrightColor(Color color) { _brightColor = color; }
        public Color GetBrightColor() { return _brightColor; }

        public void TurnOn()
        {            
            setColor(this._brightColor);
        }
        public void TurnOff()
        {
            
            setColor(Color.Gray);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // 绘制交通灯                        
            e.Graphics.FillEllipse(new SolidBrush(_currentColor), 0, 0, this._radius * 2, this._radius * 2);
            
            
        }


        private void setColor(Color color)
        {
            this._currentColor = color;
            this.Invalidate(); // 重新绘制控件
        }     
        
    }

    public class Road : Control
    {
        private Color _roadColor;
        public Road(int left, int top, int width, int height): base("",left, top, width, height)
        {
            Color darkGray = Color.Gray;/*Color.FromArgb(255, 128, 128, 128);*/

            this._roadColor = darkGray;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // 绘制交通灯                        
            e.Graphics.FillRectangle (new SolidBrush(_roadColor), 0, 0, this.Width, this.Height);

        }
    }
    
    public enum Direction
    {
        East,
        West,
        North,
        South
    }
    
    public abstract class CarBase : Control
    {
        protected string _carName;

        protected Color _carColor;

        protected int _privilege; // 优先级,越高越优先

        protected int _speed; // 车速

        protected Direction _carDirection; //车的方向

        protected int _laneNum; //车道

        private System.Windows.Forms.Timer animationTimer;

        private static Random random = new Random(Guid.NewGuid().GetHashCode());

        protected Font _font = new Font("Arial", 10);//字体
        protected CarBase(Direction direction, int lane)
        {                       
            _carDirection = direction;
            _laneNum = lane;      
            //_lane = new Lane(direction, lane);
            _carColor = GetRandomColor();
            this.SetSizeWithDirection();

            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 50; // 设置动画更新频率

        }

        public static Color GetRandomColor()
        {
            // 创建 Random 实例
            
            
            // 生成随机的 R、G、B 值
            int red = random.Next(256); // 0 到 255 之间的随机整数
            int green = random.Next(256);
            int blue = random.Next(256);

            // 使用生成的颜色值创建颜色对象
            Color randomColor = Color.FromArgb(red, green, blue);
            
            
            return randomColor;
        }

        protected void SetColor(Color color)
        {
            _carColor = color;
            this.Invalidate();
        }

        private void SetSizeWithDirection() 
        {
            if (_carDirection == Direction.North || _carDirection == Direction.South) 
            {               
                this.Size = new Size(40, 60);
            }
            else if(_carDirection == Direction.East || _carDirection == Direction.West)
            {
                this.Size = new Size(60, 40);
            }
        }

        private bool isTaskCompleted = false;
        
        

        public void Drive()
        {
            TaskCompletionSource<bool> animationTaskCompletionSource = new TaskCompletionSource<bool>();

            //Console.WriteLine("开始行驶...");

            animationTimer.Tick += async (sender, e) => Timer_Tick(sender, e);

            this.Invoke(new MethodInvoker(delegate
            {
                animationTimer.Start();
            }));
        }
        protected void Timer_Tick(object sender, EventArgs e)
        {
            // 在每个计时周期移动小球的位置
            Form parentForm = this.FindForm();
            
            if (parentForm != null) {             

                switch (_carDirection)
                {
                    case Direction.North:
                        this.Location = new Point(this.Left, this.Top - this._speed);
                        if(this.Top + this.Height <= 0)
                        {
                            this.Hide();
                            this.Dispose();
                            
                        }
                        break;
                    case Direction.South:
                        this.Location = new Point(this.Left, this.Top + this._speed);
                        if (this.Top >= parentForm.Height)
                        {
                            this.Hide();
                            this.Dispose();
                            
                        }
                        break;
                    case Direction.East:
                        this.Location = new Point(this.Left + this._speed, this.Top);
                        if (this.Left >= parentForm.Width)
                        {
                            this.Hide();
                            this.Dispose();
                            
                        }
                        break;
                    case Direction.West:
                        this.Location = new Point(this.Left - this._speed, this.Top);
                        if (this.Left + this.Width <= 0)
                        {
                            this.Hide();
                            this.Dispose();
                            
                        }
                        break;
                }

                // 重新绘制窗体
                this.Invalidate();
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //                         
            e.Graphics.FillRectangle(new SolidBrush(_carColor), 0, 0, this.Width, this.Height);

        }

        public static CarBase CreateRandomCar(Direction direction,int lane)
        {
            // 定义权重字典
            var weightDict = new Dictionary<Type, int>
                {
                    { typeof(Car), 6 },
                    { typeof(PoliceCar), 1 },
                    { typeof(FireEngine), 1 },
                    { typeof(Ambulance), 1 }
                };

            // 计算总权重
            int totalWeight = weightDict.Sum(kvp => kvp.Value);
            

            // 生成随机数
            int randomWeight = random.Next(totalWeight);

            int currWeight = 0;
            foreach (var kvp in weightDict)
            {
                currWeight += kvp.Value;
                if (randomWeight < currWeight)
                {
                    // 获取构造函数
                    var constructor = kvp.Key.GetConstructor(new[] { typeof(Direction), typeof(int) });
                    if (constructor != null)
                    {
                        // 创建实例并返回
                        return (CarBase)constructor.Invoke(new object[] { direction, lane });
                    }
                }
            }

            return null;
        }
    }

    public class Car : CarBase
    {       
        public Car(Direction direction, int lane):base(direction,lane)
        {
            _carName = "Car";
            _privilege = 0;
            _speed = 10;
                       
        }
        
    }

    public abstract class SpecialCar : CarBase
    {
        protected SpecialCar(Direction direction,int lane):base(direction,lane)
        {
            
            _privilege = 1;
            _speed = 20;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawString(_carName, _font, Brushes.Black, 0, 0);
        }
        protected Lane GetCurrentLane(TrafficLaneSystem trafficLaneSystem)
        {

            return new Lane (0,1);
        }

    }

    public class PoliceCar : SpecialCar
    {
        public PoliceCar(Direction direction, int lane):base(direction,lane)
        { 
            _carName = "PoliceCar";
        }
        
    }
    public class Ambulance : SpecialCar
    {
        public Ambulance(Direction direction, int lane) : base(direction, lane)
        {
            _carName = "Ambulance";
        }
    }
    public class FireEngine : SpecialCar
    {
        public FireEngine(Direction direction, int lane) : base( direction, lane)
        {
            _carName = "FireEngine";
        }
    }


    public class TrafficLaneSystem
    {
        private List<Lane> lanes;
        
        // 存储方向和车道之间的对应关系
        private Dictionary<Direction, Dictionary<int, Lane>> _trafficLaneDict;

        public TrafficLaneSystem()
        {
            lanes = new List<Lane>();
            _trafficLaneDict = new Dictionary<Direction, Dictionary<int, Lane>>();
            // 初始化lanes列表,添加所有车道
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                _trafficLaneDict[dir] = new Dictionary<int, Lane>(); // 初始化内部字典
                for (int i = 1; i <= 2; i++)
                {
                    Lane lane =new Lane(dir,i);
                    lanes.Add(lane);
                    // 添加车道到字典中
                    _trafficLaneDict[dir][i] = lane;
                }
            }
        }
        public void PutCar(Form form)
        {
            Size formSize = form.Size;
            foreach (Lane lane in lanes)
            {
                Direction direction = lane.GetDirection();
                int laneNum =lane.GetLaneNum();
                List<Point> points = CalculateCarPositions(direction, laneNum, formSize);
                foreach (Point point in points)
                {
                    CarBase car = CarBase.CreateRandomCar(direction, laneNum);

                    if (car != null) 
                    { 

                    lane.EnterLane(car);

                    car.Location = point;

                    form.Controls.Add(car);

                    }
                }
            } 
        }
        public List<Lane> GetLanes() { return lanes; }

        public Lane GetLaneWithDirectionAndLaneNum(Direction direction,int laneNum)
        {
            if (_trafficLaneDict.ContainsKey(direction) && _trafficLaneDict[direction].ContainsKey(laneNum))
            {
                return _trafficLaneDict[direction][laneNum];
            }
            else
            {
                return null;
            }
        }


        public async Task DriveTask(Direction direction, TrafficLightControl trafficLightControl)
        {

            while (true)
            {
                Semaphore semaphoreEast = Form1.GetSemaphore2ForDirection(Direction.North);
                Semaphore semaphoreNorth = Form1.GetSemaphore2ForDirection(Direction.East);


                for (int laneNum = 1; laneNum <= 2; laneNum++)
                {


                    int status = IsAbleToPass(direction, laneNum, trafficLightControl);
                    if (status != 0)
                    {
                        //检查可以通行，执行下面的；否则等待50ms

                        semaphoreEast.WaitOne();
                        semaphoreNorth.WaitOne();

                        try
                        {
                            if (status == 2)
                            {
                                await Task.Delay(2000);
                            }
                            Lane lane = GetLaneWithDirectionAndLaneNum(direction, laneNum);//获取车道上的车
                            lane.ExitLane();//进行行驶
                            if (status == 2)
                            {
                                await Task.Delay(1000);
                            }
                        }
                        finally
                        {
                            semaphoreNorth.Release();
                            semaphoreEast.Release();
                        }
                    }
                    else
                    {
                        await Task.Delay(50);
                    }


                }
            }
        }

        public int IsAbleToPass(Direction direction,int laneNum, TrafficLightControl trafficLightControl)
        { 
            Lane lane = GetLaneWithDirectionAndLaneNum(direction, laneNum); ;
            if (trafficLightControl.CanPass())
            {
                return 1;                                    
            }
            else
            {
                if(lane.GetCarQueue().Peek() is SpecialCar)
                {
                    return 2;
                }
                return 0;
            }
            
        }

        public List<Point> CalculateCarPositions(Direction dir, int lane, Size formSize)
        {
            List<Point> positions = new List<Point>();
            int maxCars = 0;//计算最大车辆数目
            if (dir == Direction.East || dir == Direction.West)
            {
                Car car = new Car(Direction.East, 1);
                maxCars = (int)((formSize.Width / 2 - 10 - 100) / (car.Width + 5));

            }
            else
            {
                Car car = new Car(Direction.North, 1);
                maxCars = (int)((formSize.Height / 2 - 10 - 100) / (car.Height + 5));

            }
            int StartX = formSize.Width / 2, StartY = formSize.Height / 2;//初始化为中心点的位置
            switch (dir)//按照方向以及车道计算可放置的Point
            {
                case Direction.East:
                    Car car = new Car(Direction.East, 1);
                    StartX = StartX - 10 - 100;
                    StartY = StartY + 10 + 5;
                    for (int i = 1; i < lane; i++) //根据车道切换
                    {
                        StartY = StartY + 10 + car.Height;
                    }
                    for (int i = 1; i <= maxCars; i++) //根据每个车道放置车辆
                    {
                        Point point = new Point(StartX - i * (car.Width + 5), StartY);
                        positions.Add(point);
                    }
                    break;
                case Direction.West:
                    car = new Car(Direction.West, 1);
                    StartX = StartX + 10 + 100 + 5;
                    StartY = StartY - 10 - 5 - car.Height;
                    for (int i = 1; i < lane; i++)
                    {
                        StartY = StartY - 10 - car.Height;
                    }
                    for (int i = 1; i <= maxCars; i++)
                    {
                        Point point = new Point(StartX + (i - 1) * (car.Width + 5), StartY);
                        positions.Add(point);
                    }
                    break;
                case Direction.North:
                    car = new Car(Direction.North, 1);
                    StartX = StartX + 10 + 5;
                    StartY = StartY + 10 + 100 + 5;
                    for (int i = 1; i < lane; i++)
                    {
                        StartX = StartX + 10 + car.Width;
                    }
                    for (int i = 1; i <= maxCars; i++)
                    {
                        Point point = new Point(StartX, StartY + (i - 1) * (car.Height + 5));
                        positions.Add(point);
                    }
                    break;
                case Direction.South:
                    car = new Car(Direction.South, 1);
                    StartX = StartX - 10 - 5 - car.Width;
                    StartY = StartY - 10 - 100;
                    for (int i = 1; i < lane; i++)
                    {
                        StartX = StartX - 10 - car.Width;
                    }
                    for (int i = 1; i <= maxCars; i++)
                    {
                        Point point = new Point(StartX, StartY - i * (car.Height + 5));
                        positions.Add(point);
                    }
                    break;
            }
            return positions;
        }

    }
    public class Lane
    {
        public Queue<CarBase> _carQueue { get; set; }

        protected Direction _direction;

        protected int _laneNum;

        public Lane(Direction direction,int laneNum)
        {
            _carQueue = new Queue<CarBase>();

            this._direction = direction;

            this._laneNum = laneNum;
        }

        public Direction GetDirection() { return this._direction; }
        public int GetLaneNum() {  return this._laneNum; }

        public void EnterLane(CarBase car)
        {
            _carQueue.Enqueue(car);
        }

        public async void ExitLane()
        {
            if (_carQueue.Count > 0)
            {
                CarBase car = _carQueue.Dequeue();

                car.Drive();

            }
        }
        public Queue<CarBase> GetCarQueue() { return this._carQueue; }

        
    }
}
