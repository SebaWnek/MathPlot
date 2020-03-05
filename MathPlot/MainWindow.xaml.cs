using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeuralNetwork;

namespace MathPlot
{
    public partial class MainWindow : Window
    {
        CanvasConvert converter; //helper class for calculating math coordinates into screen coordinates
        Network network; //neural network 
        int[] horizontalRange; //range of values in horizontal direction 
        int[] verticalRange; //range of values in vertical direction
        double[] middleOfRange = new double[2]; //fraction pointing to position of middle of the plot on screen
        int horizontalSize; //total range in horizontal direction
        int verticalSize; //total range in horizontal direction
        int horizontalAxisPointCount; //number of axis points in horizontal direction
        int verticalAxisPointCount; //number of axis points in vertical direction
        int axisPointLenght = 5; //lenght of axis point mark
        int accuracy = 10000; //number of polyline points to be calculated - the more points the more accurate plot

        Dictionary<string, Polyline> graphList = new Dictionary<string, Polyline>();
        public MainWindow()
        {
            InitializeComponent();
            horizontalRange = new int[] { -10, 10 };
            verticalRange = new int[] { -20, 20 };
            horizontalSize = horizontalRange[1] - horizontalRange[0];
            verticalSize = verticalRange[1] - verticalRange[0];
            horizontalAxisPointCount = horizontalRange[1] - horizontalRange[0];
            verticalAxisPointCount = verticalRange[1] - verticalRange[0];
            middleOfRange[0] = Math.Abs((double)horizontalRange[0]) / (horizontalRange[1] - horizontalRange[0]);
            middleOfRange[1] = Math.Abs((double)verticalRange[0]) / (verticalRange[1] - verticalRange[0]);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            converter = new CanvasConvert(background.ActualHeight, background.ActualWidth, horizontalSize, verticalSize, middleOfRange);
            AddAxis();

            GenerateNetwork();

            //PrintGraph(Square);
            //await Task.Delay(1000);
            //UpdateGraph(Sinus);
            //await Task.Delay(1000);
            //UpdateGraph(Tangens);
            //await Task.Delay(1000);
            //PrintGraph(Sinus, Brushes.Blue, "Sinus");
            //PrintGraph(Tangens, Brushes.Green, "Tan");
        }

        private async void GenerateNetwork()
        {
            //Generate network
            Random random = new Random();
            network = new Network(new int[] { 1, 500, 1 }, FunctionTypes.TanH, FunctionTypes.Linear, 0.00005, 8, 1);
            //Training network
            double[] res;
            double[] inputs;
            int count = 1000000;
            int percent = count / 1000;

            PrintGraph(new Func<double, double>(CalculateNetwork));
            PrintGraph(x => Function(x), Brushes.Blue, "compare"); 
            iterationLabel.Content = $"Iteration: 0";
            await Task.Delay(10);

            for (int i = 1; i < count; i++)
            {
                inputs = new double[] { ((double)random.Next(-1000000, 1000000))/100000 };
                res = new double[1];
                res[0] = Function(inputs[0]);
                //res[0] = inputs[0] * inputs[0];
                await Task.Run(() => network.TrainNetwork(inputs, res));
                if (i % percent == 0)
                {
                    UpdateGraph(new Func<double, double>(CalculateNetwork));
                    iterationLabel.Content = $"Iteration: {i}";
                    MSEListBox.Items.Add(CalculateMSE());
                    //network.UpdateLearningRate(1 / (double)i);
                    await Task.Delay(10);
                }
            }

        }

        private double Function(double x)
        {
            return x+ Math.Cos(x) * Math.Pow(x,2) / 5 * Math.Pow(Math.Atan(x), 3) - 2 * x;
        }

        private double CalculateMSE()
        {
            double MSEAccuracy = 100;
            double MSEStep = (horizontalRange[1] - horizontalRange[0]) / MSEAccuracy;
            double MSE = 0;
            for(double i = horizontalRange[0]; i <= horizontalRange[1]; i += MSEStep)
            {
                MSE += Math.Pow(Function(i) - CalculateNetwork(i), 2);
            }
            MSE /= MSEAccuracy;
            return MSE;
        }

        private double CalculateNetwork(double x)
        {
            network.CalculateNetwork(new double[] { (double)x });
            return network.GetOutputs()[0];
        }

        private void AddAxis()
        {
            AddAxisLines();
            AddAxisPoints();
        }

        private void AddAxisPoints()
        {
            for (int i = 0; i <= horizontalAxisPointCount; i++)
            {
                Line tmp = new Line();
                tmp.Y1 = (background.ActualHeight - background.ActualHeight * middleOfRange[1]) - axisPointLenght;
                tmp.Y2 = (background.ActualHeight - background.ActualHeight * middleOfRange[1]) + axisPointLenght;
                tmp.X1 = tmp.X2 = background.ActualWidth * i / horizontalAxisPointCount;
                tmp.Opacity = 0.5;
                tmp.Stroke = Brushes.Black;
                tmp.StrokeThickness = 1;
                background.Children.Add(tmp);
            }
            for (int i = 0; i <= verticalAxisPointCount; i++)
            {
                Line tmp = new Line();
                tmp.X1 = background.ActualWidth * middleOfRange[0] - axisPointLenght;
                tmp.X2 = background.ActualWidth * middleOfRange[0] + axisPointLenght;
                tmp.Y1 = tmp.Y2 = background.ActualHeight * i / verticalAxisPointCount;
                tmp.Opacity = 0.5;
                tmp.Stroke = Brushes.Black;
                tmp.StrokeThickness = 1;
                background.Children.Add(tmp);
            }
        }

        private void AddAxisLines()
        {
            Line hAx = new Line();
            Line vAx = new Line();
            hAx.Stroke = Brushes.Black;
            vAx.Stroke = Brushes.Black;
            hAx.X1 = 0;
            hAx.X2 = background.ActualWidth;
            hAx.Y1 = hAx.Y2 = background.ActualHeight - background.ActualHeight * middleOfRange[1];
            vAx.Y1 = 0;
            vAx.Y2 = background.ActualHeight;
            vAx.X1 = vAx.X2 = background.ActualWidth * middleOfRange[0];
            hAx.StrokeEndLineCap = PenLineCap.Triangle;
            hAx.StrokeThickness = 1;
            vAx.StrokeStartLineCap = PenLineCap.Triangle;
            vAx.StrokeThickness = 1;
            background.Children.Add(hAx);
            background.Children.Add(vAx);
        }

        public void PrintGraph(Func<double, double> function, Brush color, string name)
        {
            Polyline line = new Polyline();
            line.Name = name;
            line.Stroke = color;
            line.StrokeThickness = 1;
            double dX = (double)horizontalSize / accuracy;
            double x, y;
            for(int i = 0; i <= accuracy; i++)
            {
                x = horizontalRange[0] + i * dX;
                y = function(x);
                line.Points.Add(converter.ToWindowsPoint(x, y));
            }
            if(!graphList.ContainsKey(name))
            {
                background.Children.Add(line);
                graphList.Add(name, line);
            }
            else
            {
                MessageBox.Show("There is already a graph with the same name. Please, specify different name.",
                    "Already contains the same name!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void PrintGraph(Func<double,double> function)
        {
            PrintGraph(function, Brushes.Red, "Name");
        }

        public void UpdateGraph(Func<double,double> function, string name)
        {
            if (graphList.ContainsKey(name))
            {
                Polyline tmp = graphList[name];
                background.Children.Remove(tmp);
                graphList.Remove(name);
                PrintGraph(function, tmp.Stroke, name);
            }
        }

        public void UpdateGraph(Func<double,double> function)
        {
            UpdateGraph(function, "Name");
        }
        
        private double Square(double x)
        {
            return x * x;
        }

        private double Sinus(double x)
        {
            return Math.Sin(x) * 20;
        }

        private double Tangens(double x)
        {
            return Math.Tan(x/3);
        }
    }
}
