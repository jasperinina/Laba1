using System;
using System.Globalization;
using System.Windows;
using ExpressionCalculator;

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
                var rpn = Utilities.ReversePolishNotation(expression);
                double result = Utilities.CalculatingValue(rpn);
                ResultLabel.Content = "Результат: " + result.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                ResultLabel.Content = "Ошибка: " + ex.Message;
            }
        }
    }
}