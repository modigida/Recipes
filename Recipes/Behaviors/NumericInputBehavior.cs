using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Recipes.Behaviors;
public class NumericInputBehavior : Behavior<TextBox>
{
    private static readonly Regex NumericRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.PreviewTextInput += OnPreviewTextInput;
        DataObject.AddPastingHandler(AssociatedObject, OnPaste);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
        DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextValid(e.Text, ((TextBox)sender).Text);
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            var pastedText = e.DataObject.GetData(DataFormats.Text) as string;
            var currentText = ((TextBox)sender).Text;

            if (!IsTextValid(pastedText, currentText))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private bool IsTextValid(string newText, string existingText)
    {
        // Kombinera ny text och befintlig text för att validera det fullständiga resultatet
        var combinedText = existingText.Insert(AssociatedObject.CaretIndex, newText);

        // Tillåt tomt fält
        if (string.IsNullOrWhiteSpace(combinedText))
            return true;

        // Validera med regex
        return NumericRegex.IsMatch(combinedText);
    }
}
