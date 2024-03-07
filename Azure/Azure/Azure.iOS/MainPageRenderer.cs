using Azure.iOS;
using Azure;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]
namespace Azure.iOS
{
    class MainPageRenderer : PageRenderer
    {
        MainPage Page;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            Page = e.NewElement as MainPage;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
    }
}