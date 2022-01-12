﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HRM_Viewer.Utilities
{
    public class BrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = 
            DependencyProperty.RegisterAttached( "Html", typeof(string), typeof(BrowserBehavior), new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser webBrowser = dependencyObject as WebBrowser;
            string hcode = e.NewValue.ToString();
            if (string.IsNullOrEmpty(hcode))
                hcode = "&nbsp;";

            if (webBrowser != null) 
                webBrowser.NavigateToString(hcode);
        }
    }
}
