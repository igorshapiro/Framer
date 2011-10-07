using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Framer.Code {
    public class BrightContrastEffect: ShaderEffect {
        private static readonly PixelShader s_shader = new PixelShader { UriSource = new Uri("pack://application:,,,/Framer;component/bricon.ps") };

        public BrightContrastEffect() {
            PixelShader = s_shader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BrightnessProperty);
            UpdateShaderValue(ContrastProperty);
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(BrightContrastEffect), 0);

        public float Brightness
        {
            get { return (float)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register("Brightness", typeof(double), typeof(BrightContrastEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public float Contrast
        {
            get { return (float)GetValue(ContrastProperty); }
            set { SetValue(ContrastProperty, value); }
        }

        public static readonly DependencyProperty ContrastProperty = DependencyProperty.Register("Contrast", typeof(double), typeof(BrightContrastEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(1)));
    }
}