using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ExpressionCalculatorWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateGraph_Click(object sender, RoutedEventArgs e)
        {
            DrawGraph();
        }

        private void DrawGraph()
        {
            GraphCanvas.Children.Clear();

            if (!double.TryParse(InputStart.Text, out double start))
            {
                ShowError("Некорректное значение начала диапазона.");
                return;
            }

            if (!double.TryParse(InputEnd.Text, out double end))
            {
                ShowError("Некорректное значение конца диапазона.");
                return;
            }

            if (!double.TryParse(InputStep.Text, out double step) || step <= 0)
            {
                ShowError("Некорректное значение шага вычислений.");
                return;
            }

            if (!double.TryParse(InputScale.Text, out double scale) || scale <= 0)
            {
                ShowError("Некорректное значение масштаба.");
                return;
            }

            string expression = InputExpression.Text;

            DrawAxes(scale);

            List<Token> tokens = Utilities.ReversePolishNotation(expression);

            Polyline polyline = new Polyline
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                ClipToBounds = true
            };

            double canvasWidth = GraphCanvas.ActualWidth;
            double canvasHeight = GraphCanvas.ActualHeight;

            double centerX = canvasWidth / 2;
            double centerY = canvasHeight / 2;

            for (double x = start; x <= end; x += step / 10) // Увеличиваем разрешение графика
            {
                double? y = Utilities.CalculatingValue(tokens, x);
                if (y.HasValue)
                {
                    double canvasX = scale * x + centerX;
                    double canvasY = -scale * y.Value + centerY;

                    if (canvasX >= 0 && canvasX <= canvasWidth && canvasY >= 0 && canvasY <= canvasHeight)
                    {
                        polyline.Points.Add(new Point(canvasX, canvasY));
                    }
                }
            }

            GraphCanvas.Children.Add(polyline);
        }

        private void DrawAxes(double scale)
        {
            GraphCanvas.Children.Clear();

            double canvasWidth = GraphCanvas.ActualWidth;
            double canvasHeight = GraphCanvas.ActualHeight;

            double centerX = canvasWidth / 2;
            double centerY = canvasHeight / 2;

            Line horizontalLine = new Line
            {
                X1 = 0,
                Y1 = centerY,
                X2 = canvasWidth,
                Y2 = centerY,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Line verticalLine = new Line
            {
                X1 = centerX,
                Y1 = 0,
                X2 = centerX,
                Y2 = canvasHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            GraphCanvas.Children.Add(horizontalLine);
            GraphCanvas.Children.Add(verticalLine);

            // Добавление стрелок поверх линий осей
            AddArrow(horizontalLine, Direction.Right);
            AddArrow(verticalLine, Direction.Up);

            AddAxisLabel("x", canvasWidth - 20, centerY - 20);
            AddAxisLabel("y", centerX + 10, 10);

            for (int i = (int)(-canvasWidth / (2 * scale)); i <= canvasWidth / (2 * scale); i++)
            {
                double xPos = i * scale + centerX;

                Line tick = new Line
                {
                    X1 = xPos,
                    Y1 = centerY - 5,
                    X2 = xPos,
                    Y2 = centerY + 5,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                GraphCanvas.Children.Add(tick);

                if (i != 0)
                {
                    AddAxisLabel(i.ToString(), xPos - 10, centerY + 5);
                }
            }

            for (int i = (int)(-canvasHeight / (2 * scale)); i <= canvasHeight / (2 * scale); i++)
            {
                double yPos = -i * scale + centerY;

                Line tick = new Line
                {
                    X1 = centerX - 5,
                    Y1 = yPos,
                    X2 = centerX + 5,
                    Y2 = yPos,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                GraphCanvas.Children.Add(tick);

                if (i != 0)
                {
                    AddAxisLabel(i.ToString(), centerX + 5, yPos - 10);
                }
            }
        }


        private void AddArrow(Line axis, Direction direction)
        {
            double arrowSize = 10;

            double x1 = axis.X2, y1 = axis.Y2;
            double x2 = x1, y2 = y1;

            if (direction == Direction.Right)
            {
                // Стрелка вправо (по оси x)
                x2 = x1 - arrowSize;
                y2 = y1 + arrowSize / 2;

                Line arrow1 = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                x2 = x1 - arrowSize;
                y2 = y1 - arrowSize / 2;

                Line arrow2 = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                GraphCanvas.Children.Add(arrow1);
                GraphCanvas.Children.Add(arrow2);
            }
            else if (direction == Direction.Up) // Изменено для стрелки по оси y
            {
                // Стрелка вверх (по оси y)
                x2 = x1 - arrowSize / 2;
                y2 = y1 - arrowSize;

                Line arrow1 = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                x2 = x1 + arrowSize / 2; // изменено здесь
                y2 = y1 - arrowSize; // изменено здесь

                Line arrow2 = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                GraphCanvas.Children.Add(arrow1);
                GraphCanvas.Children.Add(arrow2);
            }
        }




        private void AddAxisLabel(string text, double x, double y)
        {
            TextBlock label = new TextBlock
            {
                Text = text,
                FontSize = 10,
                Foreground = Brushes.Black
            };

            Canvas.SetLeft(label, x);
            Canvas.SetTop(label, y);

            GraphCanvas.Children.Add(label);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
