using System.Globalization;
using System.Windows;

namespace ExpressionCalculatorWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string expression = InputExpression.Text.Replace(",", ".");
                double xValue = double.Parse(InputXValue.Text, CultureInfo.InvariantCulture);

                var rpn = Utilities.ReversePolishNotation(expression);
                double result = Utilities.CalculatingValue(rpn, xValue);

                ResultLabel.Content = "Результат: " + result.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                ResultLabel.Content = "Ошибка: " + ex.Message;
            }
        }
    }
}