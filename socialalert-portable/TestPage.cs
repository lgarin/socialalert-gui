using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public class TestPage : ContentPage
    {
        Label label;

        public TestPage()
        {
            label = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            Content = label;
            SizeChanged += (s, e) => label.Text = $"{Width} \u00D7 {Height}";
        }
    }
}
