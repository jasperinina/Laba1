using System;
using System.Globalization;
using System.Windows;
using RPN.Logic;

namespace ExpressionCalculatorWPF
{
    public partial class MainWindow
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

                var rpn = Calculator.ReversePolishNotation(expression);
                double? result;

                if (isXValid)
                {
                    result = Calculator.CalculatingValue(rpn, xValue);
                }
                else
                {
                    result = Calculator.CalculatingValue(rpn, 0);
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