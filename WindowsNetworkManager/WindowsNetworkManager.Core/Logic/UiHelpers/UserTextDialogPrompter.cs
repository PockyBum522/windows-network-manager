using System.Windows;
using System.Windows.Controls;

namespace WindowsNetworkManager.Core.Logic.UiHelpers;

public class UserTextDialogPrompter
{
    public string GetTextFromUser(string title, string body = "")
    {
        if (string.IsNullOrWhiteSpace(body))
            body = title;

        // Create the input dialog window
        var inputDialog = new Window
        {
            Width = 300,
            Height = 300,
            SizeToContent = SizeToContent.Height,
            Title = title,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize
        };

        // Create the main grid
        var grid = new Grid();
        grid.Margin = new Thickness(10);
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) });
        
        // Create and add the TextBlock for the body
        var textBlockBody = new TextBlock();
        textBlockBody.Margin = new Thickness(0, 0, 0, 10);
        textBlockBody.Text = body;
        textBlockBody.TextWrapping = TextWrapping.Wrap;
        
        Grid.SetRow(textBlockBody, 0);
        grid.Children.Add(textBlockBody);
        
        // Create and add the TextBox
        var textBox = new TextBox();
        textBox.Margin = new Thickness(0, 0, 0, 10);
        
        Grid.SetRow(textBox, 1);
        grid.Children.Add(textBox);

        // Create and add the StackPanel with buttons
        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        Grid.SetRow(buttonPanel, 2);
        grid.Children.Add(buttonPanel);

        // Create and add the OK button
        var okButton = new Button
        {
            Content = "OK",
            Width = 75,
            Margin = new Thickness(0, 0, 5, 0)
        };
        
        okButton.Click += (sender, args) => { inputDialog.DialogResult = true; };
        buttonPanel.Children.Add(okButton);

        // Create and add the Cancel button
        var cancelButton = new Button
        {
            Content = "Cancel",
            Width = 75
        };
        cancelButton.Click += (sender, args) => { inputDialog.DialogResult = false; };
        buttonPanel.Children.Add(cancelButton);

        inputDialog.Content = grid;
        inputDialog.Owner = System.Windows.Application.Current.MainWindow;

        // Show the dialog and get the result
        var dialogResult = inputDialog.ShowDialog();

        // Return the input if the OK button was pressed
        if (dialogResult.HasValue && dialogResult.Value)
        {
            return textBox.Text;
        }

        return string.Empty;
    }
}