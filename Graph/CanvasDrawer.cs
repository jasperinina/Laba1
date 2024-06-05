using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Logic;
using RPN.Logic;

namespace ExpressionCalculatorWPF
{
    public class CanvasDrawer
    {
        private readonly Canvas _canvas;
        private readonly Calculator _calculator;

        public CanvasDrawer(Canvas canvas)
        {
            _canvas = canvas;
            _calculator = new Calculator();
        }

        public void DrawGraph(string expression, double start, double end, double step, double scale)
        {
            _canvas.Children.Clear();
            DrawAxes(scale);

            try
            {
                List<Token> tokens = _calculator.Parse(expression);
                List<Token> postfix = _calculator.ReversePolishNotation(tokens);

                Polyline polyline = new Polyline
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 1,
                    ClipToBounds = true
                };

                double canvasWidth = _canvas.ActualWidth;
                double canvasHeight = _canvas.ActualHeight;

                double centerX = canvasWidth / 2;
                double centerY = canvasHeight / 2;

                for (double x = start; x <= end; x += step / 10)
                {
                    double? y = null;
                    try
                    {
                        var variableValues = new Dictionary<string, double> { { "x", x } };
                        y = _calculator.EvaluatePostfix(postfix, variableValues);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

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

                _canvas.Children.Add(polyline);
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при вычислении выражения: {ex.Message}");
            }
        }

        private void DrawAxes(double scale)
        {
            _canvas.Children.Clear();

            double canvasWidth = _canvas.ActualWidth;
            double canvasHeight = _canvas.ActualHeight;

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

            Polyline horizontalArrow = new Polyline
            {
                Stroke = Brushes.Black,
                Points = new PointCollection { new Point(canvasWidth - 10, centerY - 5), new Point(canvasWidth, centerY), new Point(canvasWidth - 10, centerY + 5) }
            };

            Polyline verticalArrow = new Polyline
            {
                Stroke = Brushes.Black,
                Points = new PointCollection { new Point(centerX - 5, 10), new Point(centerX, 0), new Point(centerX + 5, 10) }
            };

            _canvas.Children.Add(horizontalLine);
            _canvas.Children.Add(verticalLine);
            _canvas.Children.Add(horizontalArrow);
            _canvas.Children.Add(verticalArrow);

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
                _canvas.Children.Add(tick);

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
                _canvas.Children.Add(tick);

                if (i != 0)
                {
                    AddAxisLabel(i.ToString(), centerX + 5, yPos - 10);
                }
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

            _canvas.Children.Add(label);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
