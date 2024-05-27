using System;
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
                ErrorMessage.Text = string.Empty;

                string expression = InputExpression.Text.Replace(",", ".");
                double xValue = 0;
                bool isXValid = double.TryParse(InputXValue.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out xValue);

                var rpn = Utilities.ReversePolishNotation(expression);
                double? result;

                if (isXValid)
                {
                    result = Utilities.CalculatingValue(rpn, xValue);
                }
                else
                {
                    result = Utilities.CalculatingValue(rpn, 0);
                }

                if (result.HasValue)
                {
                    OutputResult.Text = result.Value.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    OutputResult.Text = "Результат не может быть вычислен.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
    }
}