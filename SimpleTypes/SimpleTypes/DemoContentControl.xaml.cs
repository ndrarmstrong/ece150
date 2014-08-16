//-----------------------------------------------------------------------
// <copyright file="DemoContentControl.xaml.cs" company="Nicholas Armstrong">
//     Created Sept. 2009 by Nicholas Armstrong.  Available online at http://nicholasarmstrong.com
// </copyright>
// <summary>
//     Simple types sample application content.  Created for the Fall 2009 offering of ECE 150.
// </summary>
//-----------------------------------------------------------------------

namespace NicholasArmstrong.Samples.ECE150.SimpleTypes
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// ECE 150 Demo Main Window.
    /// </summary>
    public partial class DemoContentControl : UserControl
    {
        #region Fields
        #region Dependency Properties
        /// <summary>
        /// DependencyProperty backing store for BasicTypesValue.
        /// </summary>
        public static readonly DependencyProperty BasicTypesValueProperty =
            DependencyProperty.Register("BasicTypesValue", typeof(string), typeof(DemoContentControl), new UIPropertyMetadata(String.Empty, new PropertyChangedCallback(OnBasicTypesValueChanged)));

        /// <summary>
        /// DependencyProperty backing store for Types.
        /// </summary>
        public static readonly DependencyProperty TypesProperty =
            DependencyProperty.Register("Types", typeof(List<string>), typeof(DemoContentControl), new UIPropertyMetadata(new List<string>()));

        /// <summary>
        /// DependencyProperty backing store for FirstTypeValue.
        /// </summary>
        public static readonly DependencyProperty FirstTypeValueProperty =
            DependencyProperty.Register("FirstTypeValue", typeof(string), typeof(DemoContentControl), new UIPropertyMetadata("int", new PropertyChangedCallback(OnTypeConversionValueChanged)));

        /// <summary>
        /// DependencyProperty backing store for SecondTypeValue.
        /// </summary>
        public static readonly DependencyProperty SecondTypeValueProperty =
            DependencyProperty.Register("SecondTypeValue", typeof(string), typeof(DemoContentControl), new UIPropertyMetadata("long", new PropertyChangedCallback(OnTypeConversionValueChanged)));
        #endregion

        #region Regular Expressions
        /// <summary>
        /// Regular expression for matching strings.
        /// </summary>
        private static readonly Regex stringRegex = new Regex("^[@]?[\"]([^\"\\\\]|[\\\\]['\"\\\\0abfnrtuUxv])*[\"]$");

        /// <summary>
        /// Regular expression for matching characters.
        /// </summary>
        private static readonly Regex charRegex = new Regex("^[']([^'\\\\]|[\\\\]['\"\\\\0abfnrtv]|[\\\\][x][0-9a-fA-F]{1,4})[']$");

        /// <summary>
        /// Regular expression for matching integers.
        /// </summary>
        private static readonly Regex intRegex = new Regex("^([-+]?[0-9]+|[0][x][0-9a-fA-F]+)$");

        /// <summary>
        /// Regular expression for selecting integer numbers.
        /// </summary>
        private static readonly Regex intNumbersRegex = new Regex("[0-9]+");

        /// <summary>
        /// Regular expression for matching unsigned integers.
        /// </summary>
        private static readonly Regex uintRegex = new Regex("^([0-9]+|[0][x][0-9a-fA-F]+)[uU]$");

        /// <summary>
        /// Regular expression for matching long integers.
        /// </summary>
        private static readonly Regex longRegex = new Regex("^([-+]?[0-9]+|[0][x][0-9a-fA-F]+)[lL]$");

        /// <summary>
        /// Regular expression for matching unsigned long integers.
        /// </summary>
        private static readonly Regex ulongRegex = new Regex("^([0-9]+|[0][x][0-9a-fA-F]+)([uU][lL]|[lL][uU])$");

        /// <summary>
        /// Regular expression for matching single precision floating point numbers.
        /// </summary>
        private static readonly Regex floatRegex = new Regex("^[-+]?([0-9]*[.])?[0-9]+([eE][-+]?[0-9]+)?[fF]?$");

        /// <summary>
        /// Regular expression for matching double precision floating point numbers.
        /// </summary>
        private static readonly Regex doubleRegex = new Regex("^[-+]?([0-9]*[.])?[0-9]+([eE][-+]?[0-9]+)?[dD]?$");

        /// <summary>
        /// Regular expression for matching decimal numbers.
        /// </summary>
        private static readonly Regex decimalRegex = new Regex("^[-+]?([0-9]*[.])?[0-9]+([eE][-+]?[0-9]+)?[mM]?$");
        #endregion

        #region Type Tables
        /// <summary>
        /// Types that can be implicitly converted.
        /// </summary>
        private static readonly Dictionary<string, List<string>> implicitConversions = new Dictionary<string, List<string>>()
        {
            { "string", new List<string>() },
            { "bool", new List<string>() },
            { "sbyte", new List<string>() { "short", "int", "long", "float", "double", "decimal" } },
            { "byte", new List<string>() { "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal" } },
            { "short", new List<string>() { "int", "long", "float", "double", "decimal" } },
            { "ushort", new List<string>() { "int", "uint", "long", "ulong", "float", "double", "decimal" } },
            { "int", new List<string>() { "long", "float", "double", "decimal" } },
            { "uint", new List<string>() { "long", "ulong", "float", "double", "decimal" } },
            { "long", new List<string>() { "float", "double", "decimal" } },
            { "ulong", new List<string>() { "float", "double", "decimal" } },
            { "char", new List<string>() { "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal" } },
            { "float", new List<string>() { "double" } },
            { "double", new List<string>() },
            { "decimal", new List<string>() },
        };

        /// <summary>
        /// Imprecise implicit float conversions.
        /// </summary>
        private static readonly List<string> impreciseFloatConversions = new List<string>() { "int", "uint", "long", "ulong" };

        /// <summary>
        /// Imprecise implicit double conversions.
        /// </summary>
        private static readonly List<string> impreciseDoubleConversions = new List<string>() { "long", "ulong" };

        /// <summary>
        /// Types that can be explicitly converted.
        /// </summary>
        private static readonly Dictionary<string, List<string>> explicitConversions = new Dictionary<string, List<string>>()
        {
            { "string", new List<string>() },
            { "bool", new List<string>() },
            { "sbyte", new List<string>() { "byte", "ushort", "uint", "ulong", "char" } },
            { "byte", new List<string>() { "sbyte", "char" } },
            { "short", new List<string>() { "sbyte", "byte", "ushort", "uint", "ulong", "char" } },
            { "ushort", new List<string>() { "sbyte", "byte", "short", "char" } },
            { "int", new List<string>() { "sbyte", "byte", "short", "ushort", "uint", "ulong", "char" } },
            { "uint", new List<string>() { "sbyte", "byte", "short", "ushort", "int", "char" } },
            { "long", new List<string>() { "sbyte", "byte", "short", "ushort", "int", "uint", "ulong", "char" } },
            { "ulong", new List<string>() { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "char" } },
            { "char", new List<string>() { "sbyte", "byte", "short" } },
            { "float", new List<string>() { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "decimal" } },
            { "double", new List<string>() { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "float", "decimal" } },
            { "decimal", new List<string>() { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double" } },
        };
        #endregion

        /// <summary>
        /// FontFamily for program code.
        /// </summary>
        private static readonly FontFamily programTextFamily = new FontFamily("Consolas, Courier New, Courier, Monospace"); 
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the DemoContentControl class.
        /// </summary>
        public DemoContentControl()
        {
            InitializeComponent();

            this.Types.Add("bool");
            this.Types.Add("byte");
            this.Types.Add("char");
            this.Types.Add("decimal");
            this.Types.Add("double");
            this.Types.Add("float");
            this.Types.Add("int");
            this.Types.Add("long");
            this.Types.Add("sbyte");
            this.Types.Add("short");
            this.Types.Add("string");
            this.Types.Add("uint");
            this.Types.Add("ulong");
            this.Types.Add("ushort");

            this.UpdateBasicTypesText(null);
            this.UpdateTypeConversionText();
        }
        #endregion

        #region Enumerations
        /// <summary>
        /// The list of types supported by the basic type detector.
        /// </summary>
        private enum SupportedTypes
        {
            /// <summary>
            /// String type.
            /// </summary>
            String,

            /// <summary>
            /// Boolean type.
            /// </summary>
            Boolean,

            /// <summary>
            /// Character type.
            /// </summary>
            Character,

            /// <summary>
            /// Integer type.
            /// </summary>
            Integer,

            /// <summary>
            /// Unsigned integer type.
            /// </summary>
            UnsignedInteger,

            /// <summary>
            /// Long integer type.
            /// </summary>
            Long,

            /// <summary>
            /// Unsigned long type.
            /// </summary>
            UnsignedLong,

            /// <summary>
            /// Single precision floating point type.
            /// </summary>
            SingleFloat,

            /// <summary>
            /// Double precision floating point type.
            /// </summary>
            DoubleFloat,

            /// <summary>
            /// Decimal type.
            /// </summary>
            DecimalFloat,
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the basic types value.
        /// </summary>
        public string BasicTypesValue
        {
            get { return (string)GetValue(BasicTypesValueProperty); }
            set { SetValue(BasicTypesValueProperty, value); }
        }

        /// <summary>
        /// Gets the list of convertable types.
        /// </summary>
        public List<string> Types
        {
            get { return (List<string>)GetValue(TypesProperty); }
            private set { SetValue(TypesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the first type to be converted.
        /// </summary>
        public string FirstTypeValue
        {
            get { return (string)GetValue(FirstTypeValueProperty); }
            set { SetValue(FirstTypeValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the second type to be converted.
        /// </summary>
        public string SecondTypeValue
        {
            get { return (string)GetValue(SecondTypeValueProperty); }
            set { SetValue(SecondTypeValueProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates the basic types text when the value in the basic types box changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Arguments describing the event.</param>
        private static void OnBasicTypesValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DemoContentControl window = sender as DemoContentControl;
            if (window != null)
            {
                string newValue = window.BasicTypesValue.Trim();

                if (newValue.Length == 0)
                {
                    window.UpdateBasicTypesText(null);
                }
                else if (stringRegex.IsMatch(newValue))
                {
                    window.UpdateBasicTypesText(SupportedTypes.String);
                }
                else if (newValue.ToUpperInvariant() == "TRUE")
                {
                    window.UpdateBasicTypesText(SupportedTypes.Boolean);
                }
                else if (charRegex.Match(newValue).Success)
                {
                    window.UpdateBasicTypesText(SupportedTypes.Character);
                }
                else if (newValue.ToUpperInvariant() == "FALSE")
                {
                    window.UpdateBasicTypesText(SupportedTypes.Boolean);
                }
                else if (intRegex.IsMatch(newValue))
                {
                    int intValue;
                    uint uintValue;
                    long longValue;
                    ulong ulongValue;

                    if (int.TryParse(intNumbersRegex.Match(newValue).Value, out intValue))
                    {
                        window.UpdateBasicTypesText(SupportedTypes.Integer);
                    }
                    else if (uint.TryParse(intNumbersRegex.Match(newValue).Value, out uintValue))
                    {
                        window.UpdateBasicTypesText(SupportedTypes.UnsignedInteger);
                    }
                    else if (long.TryParse(intNumbersRegex.Match(newValue).Value, out longValue))
                    {
                        window.UpdateBasicTypesText(SupportedTypes.Long);
                    }
                    else if (ulong.TryParse(intNumbersRegex.Match(newValue).Value, out ulongValue))
                    {
                        window.UpdateBasicTypesText(SupportedTypes.UnsignedLong);
                    }
                    else
                    {
                        window.UpdateBasicTypesText(null);
                    }
                }
                else if (uintRegex.IsMatch(newValue))
                {
                    uint uintValue;
                    ulong ulongValue;

                    if (uint.TryParse(intNumbersRegex.Match(newValue).Value, out uintValue))
                    {
                        window.UpdateBasicTypesText(SupportedTypes.UnsignedInteger);
                    }
                    else if (ulong.TryParse(intNumbersRegex.Match(newValue).Value, out ulongValue))
                    {
                        window.UpdateBasicTypesText(SupportedTypes.UnsignedLong);
                    }
                    else
                    {
                        window.UpdateBasicTypesText(null);
                    }
                }
                else if (longRegex.IsMatch(newValue))
                {
                    long longValue;
                    ulong ulongValue;

                    if (long.TryParse(intNumbersRegex.Match(newValue).Value, out longValue))
                    {
                        window.UpdateBasicTypesText(SupportedTypes.Long);
                    }
                    else if (ulong.TryParse(intNumbersRegex.Match(newValue).Value, out ulongValue))
                    {
                        window.UpdateBasicTypesText(SupportedTypes.UnsignedLong);
                    }
                    else
                    {
                        window.UpdateBasicTypesText(null);
                    }
                }
                else if (ulongRegex.IsMatch(newValue))
                {
                    ulong ulongValue;

                    if (ulong.TryParse(intNumbersRegex.Match(newValue).Value, out ulongValue))
                    {
                        window.UpdateBasicTypesText(SupportedTypes.UnsignedLong);
                    }
                    else
                    {
                        window.UpdateBasicTypesText(null);
                    }
                }
                else if (floatRegex.IsMatch(newValue))
                {
                    window.UpdateBasicTypesText(SupportedTypes.SingleFloat);
                }
                else if (doubleRegex.IsMatch(newValue))
                {
                    window.UpdateBasicTypesText(SupportedTypes.DoubleFloat);
                }
                else if (decimalRegex.IsMatch(newValue))
                {
                    window.UpdateBasicTypesText(SupportedTypes.DecimalFloat);
                }
                else
                {
                    window.UpdateBasicTypesText(null);
                }
            }
        }

        /// <summary>
        /// Updates the type conversion text when one of the types involved changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Arguments describing the event.</param>
        private static void OnTypeConversionValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DemoContentControl window = sender as DemoContentControl;
            if (sender != null)
            {
                window.UpdateTypeConversionText();
            }
        }

        /// <summary>
        /// Updates the basic types text to reflect the provided type.
        /// </summary>
        /// <param name="type">The type to update the basic types text to.</param>
        private void UpdateBasicTypesText(SupportedTypes? type)
        {
            BasicTypesProgramTextBlock.Inlines.Clear();
            BasicTypesDetailsTextBlock.Inlines.Clear();

            switch (type)
            {
                case SupportedTypes.String:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is a "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("string") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.String") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Strings are ordered groupings of unicode characters, can be of any length, and are specified by surrounding a group of characters in double quotes ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("\"") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(")."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("string") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" stringVariable = "));
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(this.BasicTypesValue.Trim()) { Foreground = new SolidColorBrush(Colors.DarkRed) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(";"));
                    break;
                case SupportedTypes.Boolean:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is a "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("bool") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.Boolean") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Booleans represent boolean logical quantities - values that are either "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("true") { FontStyle = FontStyles.Italic });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" or "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("false") { FontStyle = FontStyles.Italic });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("bool") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" booleanVariable = "));
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(this.BasicTypesValue.Trim()) { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(";"));
                    break;
                case SupportedTypes.Character:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is a "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("char") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.Char") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Characters are unsigned 16-bit numbers (values between 0 and 65535), with legal values corresponding to the two-byte Unicode character set."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("char") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" characterVariable = "));
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(this.BasicTypesValue.Trim()) { Foreground = new SolidColorBrush(Colors.DarkRed) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(";"));
                    break;
                case SupportedTypes.Integer:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is an "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("int") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.Int32") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Integers are signed 32-bit integral numbers, and represent values between -2,147,483,648 and 2,147,483,647."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("int") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" integerVariable = " + this.BasicTypesValue.Trim() + ";"));
                    break;
                case SupportedTypes.UnsignedInteger:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is a "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("uint") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.UInt32") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Unsigned integers are unsigned 32-bit integral numbers, and represent values between 0 and 4,294,967,295."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("uint") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" unsignedIntegerVariable = " + this.BasicTypesValue.Trim() + ";"));
                    break;
                case SupportedTypes.Long:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is a "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("long") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.Int64") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Long integers are signed 64-bit integral numbers, and represent values between -9,223,372,036,854,775,808 and 9,223,372,036,854,775,807."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("long") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" longIntegerVariable = " + this.BasicTypesValue.Trim() + ";"));
                    break;
                case SupportedTypes.UnsignedLong:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is a "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("ulong") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.UInt64") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Unsigned long integers are unsigned 64-bit integral numbers, and represent values between 0 and 18,446,744,073,709,551,615."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("ulong") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" unsignedLongIntegerVariable = " + this.BasicTypesValue.Trim() + ";"));
                    break;
                case SupportedTypes.SingleFloat:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is a "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("float") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.Single") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Single-precision floating point numbers are 32 bits in length, and can represent values ranging from approximately 1.5e-45 to 3.4e38 with a precision of 7 digits, as well as the special quantities Not-a-Number (NaN), positive and negative infinity, and positive and negative zero."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("float") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" singlePrecisionFloatingPointVariable = " + this.BasicTypesValue.Trim() + ";"));
                    break;
                case SupportedTypes.DoubleFloat:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is a "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("double") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.Double") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Double-precision floating point numbers are 64 bits in length, and can represent values ranging from approximately 5.0e-324 to 1.7e308 with a precision of 15-16 digits, as well as the special quantities Not-a-Number (NaN), positive and negative infinity, and positive and negative zero."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("double") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" doublePrecisionFloatingPointVariable = " + this.BasicTypesValue.Trim() + ";"));
                    break;
                case SupportedTypes.DecimalFloat:
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("This is a "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("decimal") { FontWeight = FontWeights.Bold });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(" ("));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("System.Decimal") { FontFamily = programTextFamily });
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run(").  "));
                    BasicTypesDetailsTextBlock.Inlines.Add(new Run("Decimal numbers are 128 bits in length, are used for financial calculations, and can represent values ranging from approximately 1.0e-28 to 7.9e28 with a precision of 28-29 digits.  The decimal type does not support signed zeros, infinities, or NaN, and uses banker's rounding to minimize round-off errors."));

                    BasicTypesProgramTextBlock.Inlines.Add(new Run("decimal") { Foreground = new SolidColorBrush(Colors.Blue) });
                    BasicTypesProgramTextBlock.Inlines.Add(new Run(" decimalFloatingPointVariable = " + this.BasicTypesValue.Trim() + ";"));
                    break;
                default:
                    if (this.BasicTypesValue.Trim() != String.Empty)
                    {
                        BasicTypesProgramTextBlock.Inlines.Add(new Run("compiler error") { Foreground = new SolidColorBrush(Colors.Red) });
                        BasicTypesDetailsTextBlock.Inlines.Add(new Run("The C# compiler does not recognize the provided value as a legal simple type."));
                    }
                    else
                    {
                        BasicTypesDetailsTextBlock.Inlines.Add(new Run("Enter a value in the box above to see what type the C# compiler interprets the value as."));
                    }

                    break;
            }
        }

        /// <summary>
        /// Updates the type conversion text.
        /// </summary>
        private void UpdateTypeConversionText()
        {
            this.TypeConversionDetailsTextBlock.Inlines.Clear();
            this.TypeConversionProgramTextBlock.Inlines.Clear();

            if (this.FirstTypeValue == this.SecondTypeValue)
            {
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("Identity Conversion") { FontWeight = FontWeights.Bold });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(". Any type is type-convertable with itself; you can assign any "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.FirstTypeValue) { FontStyle = FontStyles.Italic });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" to any other "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.FirstTypeValue) { FontStyle = FontStyles.Italic });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("."));

                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { Foreground = new SolidColorBrush(Colors.Blue) });
                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(" convertedValue = " + this.FirstTypeValue + "Variable;"));
            }
            else if (implicitConversions[this.FirstTypeValue].Contains(this.SecondTypeValue))
            {
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("Implicit Conversion") { FontWeight = FontWeights.Bold });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(". This conversion is performed automatically by C# since a "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.FirstTypeValue) { FontStyle = FontStyles.Italic });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" can intuitively be treated as a "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { FontStyle = FontStyles.Italic });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(".  Implicit conversions always succeed and never cause exceptions to be thrown.  Converting from "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.FirstTypeValue) { FontStyle = FontStyles.Italic });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" to "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { FontStyle = FontStyles.Italic });
                if ((this.SecondTypeValue == "float" && impreciseFloatConversions.Contains(this.FirstTypeValue)) || (this.SecondTypeValue == "double" && impreciseDoubleConversions.Contains(this.FirstTypeValue)))
                {
                    this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" may cause a loss of precision, but never causes a loss of magnitude."));
                }
                else
                {
                    this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" never loses any information."));
                }

                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { Foreground = new SolidColorBrush(Colors.Blue) });
                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(" convertedValue = " + this.FirstTypeValue + "Variable;"));
            }
            else if (explicitConversions[this.FirstTypeValue].Contains(this.SecondTypeValue))
            {
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("Explicit Conversion") { FontWeight = FontWeights.Bold });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(". This conversion must be declared explicitly by casting since a "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.FirstTypeValue) { FontStyle = FontStyles.Italic });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" value may lose information when put in a "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { FontStyle = FontStyles.Italic });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(".  By default, a "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("System.OverflowException") { FontFamily = programTextFamily });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" is thrown when the value of the source operand is outside of the range of the destination type."));

                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { Foreground = new SolidColorBrush(Colors.Blue) });
                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(" convertedValue = ("));
                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { Foreground = new SolidColorBrush(Colors.Blue) });
                this.TypeConversionProgramTextBlock.Inlines.Add(new Run());
                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(")" + this.FirstTypeValue + "Variable;"));
            }
            else if (this.SecondTypeValue == "string")
            {
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("By calling ") { FontWeight = FontWeights.Bold });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("ToString()") { FontWeight = FontWeights.Bold, FontFamily = programTextFamily });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(". The "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("ToString()") { FontFamily = programTextFamily });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" function is defined for all types and returns a human-readable version of the type using the current culture settings (or the fully qualified type name if no implementation is provided). "));

                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { Foreground = new SolidColorBrush(Colors.Blue) });
                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(" convertedValue = " + this.FirstTypeValue + "Variable.ToString();"));
            }
            else if (this.FirstTypeValue == "string")
            {
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("Through Custom Conversion or Parsing") { FontWeight = FontWeights.Bold });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(". A string value containing an appropriate value can be converted to a "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { FontStyle = FontStyles.Italic });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" by calling a specific conversion function (e.g. "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("Convert.ToInt32()") { FontFamily = programTextFamily });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(", which returns an "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("int") { FontFamily = programTextFamily });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(") or by calling the type's "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("Parse()") { FontFamily = programTextFamily });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" or "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("TryParse()") { FontFamily = programTextFamily });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" method ("));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(this.SecondTypeValue + ".Parse()") { FontFamily = programTextFamily });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("). A "));
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("System.FormatException") { FontFamily = programTextFamily });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(" is thrown when the value cannot be converted."));

                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(this.SecondTypeValue) { Foreground = new SolidColorBrush(Colors.Blue) });
                this.TypeConversionProgramTextBlock.Inlines.Add(new Run(" convertedValue = " + this.SecondTypeValue + ".Parse(" + this.FirstTypeValue + "Variable);"));
            }
            else
            {
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run("No direct procedure exists") { FontWeight = FontWeights.Bold });
                this.TypeConversionDetailsTextBlock.Inlines.Add(new Run(". If this operation makes sense in your program, you'll have have to write your own function to perform this conversion."));
                this.TypeConversionProgramTextBlock.Inlines.Add(new Run("no built-in conversion") { Foreground = new SolidColorBrush(Colors.Red) });
            }
        }
        #endregion
    }
}
