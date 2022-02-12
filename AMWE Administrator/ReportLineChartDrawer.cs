// LineChart Draw code taken from: https://github.com/kareemsulthan07/Charts/blob/master/LineChart/MainWindow.xaml.cs
// Code modified for AMWE working, optimized for c# 9
using ReportHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AMWE_Administrator
{
    public partial class ReportLineChartDrawer
    {
        private Line xAxisLine, yAxisLine;
        private readonly double xAxisStart = 40, yAxisStart = 10, interval = 10;
        private Polyline chartPolyline;

        private Point origin;
        private readonly List<Holder> holders;
        public m3md2.UpgList<Value> Values { get; set; }

        private readonly UserReports UserWindow;

        public ReportLineChartDrawer(UserReports window, List<Report> userReports)
        {
            UserWindow = window;

            holders = new List<Holder>();

            Values = new();

            for (int i = 0; i < userReports.Count; i++)
            {
                Values.Add(new Value(i, userReports[i].OverallRating * 100));
            }

            Paint();

            Values.OnAdd += (sender, e) => Paint();
            UserWindow.StateChanged += (sender, e) => Paint();
            UserWindow.SizeChanged += (sender, e) => Paint();
        }


        public void Paint()
        {
            try
            {
                if (UserWindow.ActualWidth > 0 && UserWindow.ActualHeight > 0)
                {
                    double xinterval;
                    if (Values.Count == 0)
                        xinterval = Convert.ToInt32((UserWindow.chartCanvas.ActualWidth - xAxisStart) / (Values.Count + 1));
                    else
                        xinterval = Convert.ToInt32((UserWindow.chartCanvas.ActualWidth - xAxisStart) / Values.Count);
                    UserWindow.chartCanvas.Children.Clear();
                    holders.Clear();

                    // axis lines
                    xAxisLine = new Line()
                    {
                        X1 = xAxisStart,
                        Y1 = UserWindow.chartCanvas.ActualHeight - yAxisStart,
                        X2 = UserWindow.chartCanvas.ActualWidth - xAxisStart,
                        Y2 = UserWindow.chartCanvas.ActualHeight - yAxisStart,
                        Stroke = App.FontColor,
                        StrokeThickness = 1,
                    };
                    yAxisLine = new Line()
                    {
                        X1 = xAxisStart,
                        Y1 = yAxisStart - 5,
                        X2 = xAxisStart,
                        Y2 = UserWindow.chartCanvas.ActualHeight - yAxisStart,
                        Stroke = App.FontColor,
                        StrokeThickness = 1,
                    };

                    _ = UserWindow.chartCanvas.Children.Add(xAxisLine);
                    _ = UserWindow.chartCanvas.Children.Add(yAxisLine);

                    origin = new Point(xAxisLine.X1, yAxisLine.Y2);

                    // y axis lines
                    double xValue = xAxisStart;
                    double xPoint = origin.X + xinterval;
                    while (xPoint < xAxisLine.X2)
                    {
                        Line line = new()
                        {
                            X1 = xPoint,
                            Y1 = yAxisStart - 5,
                            X2 = xPoint,
                            Y2 = UserWindow.chartCanvas.ActualHeight - yAxisStart,
                            Stroke = Brushes.LightGray,
                            StrokeThickness = 1,
                            Opacity = 1,
                        };

                        _ = UserWindow.chartCanvas.Children.Add(line);

                        xPoint += xinterval;
                        xValue += xinterval;
                    }


                    TextBlock yTextBlock0 = new() { Text = $"{0}", Foreground = App.FontColor };
                    _ = UserWindow.chartCanvas.Children.Add(yTextBlock0);
                    Canvas.SetLeft(yTextBlock0, origin.X - 20);
                    Canvas.SetTop(yTextBlock0, origin.Y - 10);

                    // x axis lines
                    double yValue = yAxisStart;
                    double yPoint = origin.Y - interval;
                    while (yPoint > yAxisLine.Y1)
                    {
                        Line line = new()
                        {
                            X1 = xAxisStart,
                            Y1 = yPoint,
                            X2 = UserWindow.chartCanvas.ActualWidth - xAxisStart,
                            Y2 = yPoint,
                            Stroke = Brushes.LightGray,
                            StrokeThickness = 1,
                            Opacity = 1,
                        };

                        _ = UserWindow.chartCanvas.Children.Add(line);

                        if (yValue / 100 <= 1)
                        {
                            TextBlock textBlock = new() { Text = $"{yValue / 100}", Foreground = App.FontColor };
                            _ = UserWindow.chartCanvas.Children.Add(textBlock);
                            Canvas.SetLeft(textBlock, line.X1 - 30);
                            Canvas.SetTop(textBlock, yPoint - 10);
                        }

                        yPoint -= interval;
                        yValue += interval;
                    }

                    // connections
                    double x = 0, y = 0;
                    xPoint = origin.X;
                    yPoint = origin.Y;
                    while (xPoint < xAxisLine.X2)
                    {
                        while (yPoint > yAxisLine.Y1)
                        {
                            Holder holder = new()
                            {
                                X = x,
                                Y = y,
                                Point = new Point(xPoint, yPoint),
                            };

                            holders.Add(holder);

                            yPoint -= interval;
                            y += interval;
                        }

                        xPoint += xinterval;
                        yPoint = origin.Y;
                        x += xinterval;
                        y = 0;
                    }

                    // showing where are the connections points
                    foreach (Holder holder in holders)
                    {
                        Ellipse oEllipse = new()
                        {
                            Fill = Brushes.Red,
                            Width = 1,
                            Height = 1,
                            Opacity = 0,
                        };

                        _ = UserWindow.chartCanvas.Children.Add(oEllipse);
                        //Canvas.SetLeft(oEllipse, holder.Point.X);
                        //Canvas.SetTop(oEllipse, holder.Point.Y);
                    }

                    // polyline
                    chartPolyline = new Polyline()
                    {
                        Stroke = App.LineChartColor,
                        StrokeThickness = 2,
                    };
                    _ = UserWindow.chartCanvas.Children.Add(chartPolyline);

                    // add connection points to polyline
                    foreach (Value value in Values)
                    {
                        Holder holder = holders.FirstOrDefault(h => h.X == value.X * xinterval && h.Y == value.Y);
                        if (holder != null)
                            chartPolyline.Points.Add(holder.Point);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }
    }

    public class Holder
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point Point { get; set; }

        public Holder()
        {
        }
    }

    public class Value
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Value(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}