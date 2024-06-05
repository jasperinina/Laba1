using System.Windows;
using RPN.Logic;

namespace ExpressionCalculatorWPF
{
    public partial class MainWindow : Window
    {
        private CanvasDrawer _canvasDrawer;

        public MainWindow()
        {
            InitializeComponent();
            _canvasDrawer = new CanvasDrawer(GraphCanvas);
        }

        private void UpdateGraph_Click(object sender, RoutedEventArgs e)
        {
            DrawGraph();
        }

        private void DrawGraph()
        {
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
            _canvasDrawer.DrawGraph(expression, start, end, step, scale);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}