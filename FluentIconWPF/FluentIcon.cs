using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Path = System.Windows.Shapes.Path;

namespace FluentIconWPF
{

    public class FluentIcon : Control
    {
        private Path _iconDisplay;

        public FluentIcon() => DefaultStyleKey = typeof(FluentIcon);

        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
            "Symbol", typeof(string), typeof(FluentIcon), new PropertyMetadata("", OnSymbolChanged));

        public string Symbol
        {
            get => (string)GetValue(SymbolProperty);
            set
            {
                SetValue(SymbolProperty, value);
                OnSymbolChanged(this, new DependencyPropertyChangedEventArgs());
            }
        }

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size", typeof(double), typeof(FluentIcon), new PropertyMetadata(1d, OnSizeChanged));

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FluentIcon self) || self._iconDisplay is null) return;
            double scale = self.Size;
            self._iconDisplay.SetValue(RenderTransformProperty,
                new TransformGroup
                {
                    Children =
                    {
                        new ScaleTransform(scale, scale,
                            12, 12)
                    }
                });
            Thickness padding = (Thickness)self.GetValue(PaddingProperty);
            self.SetValue(WidthProperty, 24 * scale + padding.Left + padding.Right);
            self.SetValue(HeightProperty, 24 * scale + padding.Top + padding.Bottom);
        }

        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set
            {
                SetValue(SizeProperty, value);
                OnSizeChanged(this, new DependencyPropertyChangedEventArgs());
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!(GetTemplateChild("IconDisplay") is Path pi)) return;
            _iconDisplay = pi;
            Symbol = Symbol;
            Size = Size;
        }

        private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FluentIcon self) || self.Symbol is null || self._iconDisplay is null) return;
            if (!FluentIconDataSource.TryGetValue(self.Symbol, out string value)) return;
            self._iconDisplay.Data = Geometry.Parse(value);
        }

        #region Data Source

        private static Dictionary<string, string> _fluentIconDataSource;

        private static Dictionary<string, string> FluentIconDataSource
        {
            get
            {
                if (!(_fluentIconDataSource is null)) return _fluentIconDataSource;

                Stream stream = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/Icons.json"))?.Stream;
                if (stream == null) return _fluentIconDataSource;

                string data = new StreamReader(stream).ReadToEnd();

                if (!string.IsNullOrEmpty(data))
                    _fluentIconDataSource = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                return _fluentIconDataSource;
            }
        }

        #endregion
    }

    public class FluentMenuIcon : FluentIcon
    {

    }

}
