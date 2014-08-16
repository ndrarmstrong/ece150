//-----------------------------------------------------------------------
// <copyright file="DemoContentControl.xaml.cs" company="Nicholas Armstrong">
//     Created Sept. 2009 by Nicholas Armstrong.  Available online at http://nicholasarmstrong.com
// </copyright>
// <summary>
//     Loops sample application content.  Created for the Fall 2009 offering of ECE 150.
//
//     This application uses the AvalonEdit component from SharpDevelop, licensed under LGPL.
//     The compiled version of this library is located in the Libraries/ folder.
// </summary>
//-----------------------------------------------------------------------

namespace NicholasArmstrong.Samples.ECE150.Loops
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Highlighting;

    /// <summary>
    /// ECE 150 Demo Main Window.
    /// </summary>
    public partial class DemoContentControl : UserControl
    {
        #region Fields
        /// <summary>
        /// DependencyProperty as the backing store for Samples.
        /// </summary>
        public static readonly DependencyProperty SamplesProperty =
            DependencyProperty.Register("Samples", typeof(Dictionary<string, string>), typeof(DemoContentControl), new UIPropertyMetadata(new Dictionary<string, string>()));

        /// <summary>
        /// DependencyProperty backing store for SelectedSample.
        /// </summary>
        public static readonly DependencyProperty SelectedSampleProperty =
            DependencyProperty.Register("SelectedSample", typeof(string), typeof(DemoContentControl), new UIPropertyMetadata(String.Empty, new PropertyChangedCallback(OnSelectedSampleChanged)));

        /// <summary>
        /// Command to start or stop program execution.
        /// </summary>
        public static RoutedCommand ToggleExecutionCommand = new RoutedCommand("ToggleExecution", typeof(DemoContentControl));

        /// <summary>
        /// Regex for extracting the name of a sample.
        /// </summary>
        private static readonly Regex sampleName = new Regex("[/][/][ ]Sample:[ ]?([^\r\n]*)[\r]");

        /// <summary>
        /// Value indicating whether the application is ready to execute a program.
        /// </summary>
        private bool readyToExecute = true;

        /// <summary>
        /// Value indicating whether the application should attempt to stop the running program.
        /// </summary>
        private bool stopExecuting;

        /// <summary>
        /// Value indicating whether the application is currently executing a program.
        /// </summary>
        private bool executing;

        /// <summary>
        /// Dynamic evaluation class for compiling and executing typed functions.
        /// </summary>
        private DynamicEval eval;

        /// <summary>
        /// The text editor control.
        /// </summary>
        private TextEditor editor;

        /// <summary>
        /// The grid of boxes, loading of which is delayed until the window's Loaded event.
        /// </summary>
        private BoxGrid boxesGrid;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the DemoContentControl class.
        /// </summary>
        public DemoContentControl()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(ToggleExecutionCommand, new ExecutedRoutedEventHandler(OnToggleExecutionCommand)));
            this.InputBindings.Add(new InputBinding(ToggleExecutionCommand, new KeyGesture(Key.F5)));
            this.Loaded += new RoutedEventHandler(DemoContentControl_Loaded);

            this.PreEditorTextBlock.Inlines.Add(new Run("public") { Foreground = Utilities.BrushFromString("Blue"), FontWeight = FontWeights.Bold });
            this.PreEditorTextBlock.Inlines.Add(new Run(" void ") { Foreground = Utilities.BrushFromString("Red") });
            this.PreEditorTextBlock.Inlines.Add(new Run("HighlightBoxes") { Foreground = Utilities.BrushFromString("MidnightBlue"), FontWeight = FontWeights.Bold });
            this.PreEditorTextBlock.Inlines.Add(new Run("()\n{"));
            this.PostEditorTextBlock.Text = "}";

            this.editor = new TextEditor();
            this.editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(".cs");
            this.editor.Style = (Style)this.FindResource("EditorStyle");

            this.TextEditorArea.Content = this.editor;
            this.ReadyToExecute();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the dictionary containing all of the available samples.
        /// </summary>
        public Dictionary<string, string> Samples
        {
            get { return (Dictionary<string, string>)GetValue(SamplesProperty); }
            private set { SetValue(SamplesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the currently selected sample.
        /// </summary>
        public string SelectedSample
        {
            get { return (string)GetValue(SelectedSampleProperty); }
            set { SetValue(SelectedSampleProperty, value); }
        }

        #endregion

        #region Methods

        #region User-Accessible Methods
        /// <summary>
        /// Highlights a box; this function is passed to the user's code as a delegate.
        /// </summary>
        /// <param name="position">The number of the box to highlight.</param>
        /// <param name="color">The colour to use when highlighting a box.</param>
        public void Highlight(int position, string color)
        {
            // Make sure this is executed on the UI thread
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
            {
                if (position < 0 || position >= this.boxesGrid.Children.Count)
                {
                    return;
                }

                try
                {
                    Box box = (Box)this.boxesGrid.Children[position];
                    box.Highlighted = true;

                    if (!String.IsNullOrEmpty(color.Trim()))
                    {
                        Brush highlightColor = Utilities.BrushFromString(color);
                        box.HighlightColor = highlightColor;
                    }
                }
                catch (Exception e)
                {
                    this.Exception(e.ToString());
                    return;
                }
            }));

            if (this.stopExecuting)
            {
                Thread.CurrentThread.Abort();
            }
        }

        /// <summary>
        /// Highlights a box; this function is passed to the user's code as a delegate.
        /// </summary>
        /// <param name="box">The box to highlight.</param>
        /// <param name="color">The colour to use when highlighting the box.</param>
        public void HighlightBox(Box box, string color)
        {
            // Make sure this is executed on the UI thread
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
            {
                if (box == null)
                {
                    return;
                }

                try
                {
                    box.Highlighted = true;

                    if (!String.IsNullOrEmpty(color.Trim()))
                    {
                        Brush highlightColor = Utilities.BrushFromString(color);
                        box.HighlightColor = highlightColor;
                    }
                }
                catch (Exception e)
                {
                    this.Exception(e.ToString());
                    return;
                }
            }));

            if (this.stopExecuting)
            {
                Thread.CurrentThread.Abort();
            }
        }

        /// <summary>
        /// Pauses for the specified duration; this function is passed to the user's code as a delegate.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to pause for.</param>
        public void Wait(int millisecondsTimeout)
        {
            int microSleepDuration = 25;    // Minimum resolution of the Wait function
            int currentDuration = 0;

            if (millisecondsTimeout <= 0)
            {
                return;
            }

            // Sleep in small spurts so we can interrupt the thread in the middle of a long sleep
            while (currentDuration <= millisecondsTimeout)
            {
                if (this.stopExecuting)
                {
                    Thread.CurrentThread.Abort();
                }

                currentDuration += microSleepDuration;
                Thread.Sleep(microSleepDuration);
            }
        }
        #endregion

        #region Event Methods
        /// <summary>
        /// Switches sample text when the selected sample changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Arguments describing the event.</param>
        private static void OnSelectedSampleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DemoContentControl control = sender as DemoContentControl;
            if (control != null)
            {
                control.editor.Text = control.Samples[control.SelectedSample];
            }
        }

        /// <summary>
        /// Toggles the execution state when the execute command is activated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Arguments describing the event.</param>
        private static void OnToggleExecutionCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DemoContentControl control = sender as DemoContentControl;
            if (control != null)
            {
                control.ToggleExecution();
            }
        }

        /// <summary>
        /// Loads the samples and the compiler once the window itself has loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Arguments describing the event.</param>
        private static void DemoContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            DemoContentControl control = sender as DemoContentControl;
            if (control != null)
            {
                control.boxesGrid = new BoxGrid();
                control.BoxesGridArea.Content = control.boxesGrid;

                control.LoadSamples();
                control.editor.TextChanged += new EventHandler(control.Editor_TextChanged);

                control.eval = new DynamicEval(new HighlightDelegate(control.Highlight), new HighlightBoxDelegate(control.HighlightBox), new WaitDelegate(control.Wait));
                control.editor.Focus();
            }
        }

        /// <summary>
        /// Stops program execution when the user starts typing or switches samples.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Arguments describing the event.</param>
        private void Editor_TextChanged(object sender, EventArgs e)
        {
            if (this.executing)
            {
                this.stopExecuting = true;
            }
            else if (!this.readyToExecute)
            {
                this.ReadyToExecute();
                this.readyToExecute = true;
            }
        }

        /// <summary>
        /// Toggles the execution state when the execute button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Arguments describing the event.</param>
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleExecution();
        }
        #endregion

        /// <summary>
        /// Loads the sample programs into the program.
        /// </summary>
        private void LoadSamples()
        {
            var samples = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(s => s.StartsWith("NicholasArmstrong.Samples.ECE150.Loops.Samples", StringComparison.Ordinal)).OrderBy(s => s);

            foreach (var sample in samples)
            {
                string content = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(sample)).ReadToEnd();
                string name = sampleName.Match(content).Groups[1].Value.Trim();
                this.Samples.Add(name, content);
            }

            this.SelectedSample = "How To";
        }

        /// <summary>
        /// Toggles the program execution state.
        /// </summary>
        private void ToggleExecution()
        {
            if (this.executing)
            {
                this.StopExecution();
                this.executing = false;
            }
            else
            {
                this.StartExecution();
            }
        }

        /// <summary>
        /// Compiles and starts execution of the user's program.
        /// </summary>
        private void StartExecution()
        {
            this.executing = true;
            this.stopExecuting = false;
            this.Compiling();
            this.ResetBoxes();
            this.readyToExecute = false;
            string programText = this.editor.Text;
            int capacity = this.boxesGrid.Capacity;
            int rows = this.boxesGrid.Rows;
            int columns = this.boxesGrid.Columns;
            var boxes = this.boxesGrid.Children.Cast<Box>().ToList();

            // Run on a background thread so we don't block the UI.
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    string errors;
                    if (!this.eval.CompileSnippet(programText, out errors))
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate { this.Error(errors); }));
                        return;
                    }

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate { this.Executing(); }));
                    this.eval.ExecuteSnippet(capacity, rows, columns, boxes);
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate { this.Complete(); }));
                }
                catch (ThreadAbortException)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate { this.Complete(); }));
                }
                catch (TargetInvocationException e)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate { this.Exception(e.InnerException.ToString()); this.Complete(); }));
                }
            });

            this.editor.Focus();
        }

        /// <summary>
        /// Halts execution of a user program.
        /// </summary>
        private void StopExecution()
        {
            this.stopExecuting = true;
            this.editor.Focus();
        }

        /// <summary>
        /// Resets the boxes to their default un-highlighted state.
        /// </summary>
        private void ResetBoxes()
        {
            foreach (var box in this.boxesGrid.Children.Cast<Box>())
            {
                if (box.Highlighted)
                {
                    box.Highlighted = false;
                    if (box.HighlightColor != Box.DefaultHighlight)
                    {
                        box.HighlightColor = Box.DefaultHighlight;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the application status to indicate that the user's program can be executed.
        /// </summary>
        private void ReadyToExecute()
        {
            this.StatusText.Inlines.Clear();
            this.StatusText.Inlines.Add(new Run("Ready to Execute") { Foreground = Utilities.BrushFromString("DarkGreen") });
            this.StatusText.ToolTip = null;
            this.ExecuteButton.Content = "Execute";
            this.ExecuteButton.IsEnabled = true;
        }

        /// <summary>
        /// Updates the application status to indicate that the user's program is currently being compiled.
        /// </summary>
        private void Compiling()
        {
            this.StatusText.Inlines.Clear();
            this.StatusText.Inlines.Add(new Run("Compiling...") { Foreground = Box.DefaultHighlight });
            this.StatusText.ToolTip = null;
            this.ExecuteButton.IsEnabled = false;
        }

        /// <summary>
        /// Updates the application status to indicate that errors were found during compilation.
        /// </summary>
        /// <param name="errors">Errors found during compilation.</param>
        private void Error(string errors)
        {
            this.StatusText.Inlines.Clear();
            this.StatusText.Inlines.Add(new Run("Error: " + errors) { Foreground = Utilities.BrushFromString("Red") });
            this.StatusText.ToolTip = errors;
            this.ExecuteButton.Content = "Execute";
            this.ExecuteButton.IsEnabled = true;
            this.executing = false;
        }

        /// <summary>
        /// Updates the application status to indicate an exception was encountered during execution.
        /// </summary>
        /// <param name="message">The exception that was encountered.</param>
        private void Exception(string message)
        {
            this.StatusText.Inlines.Clear();
            this.StatusText.Inlines.Add(new Run("Exception: " + message) { Foreground = Utilities.BrushFromString("Red") });
            this.StatusText.ToolTip = message;
            this.stopExecuting = true;
        }

        /// <summary>
        /// Updates the application status to indicate that the user program is executing.
        /// </summary>
        private void Executing()
        {
            this.StatusText.Inlines.Clear();
            this.StatusText.Inlines.Add(new Run("Executing...") { Foreground = Box.DefaultHighlight });
            this.StatusText.ToolTip = null;
            this.ExecuteButton.Content = "Stop";
            this.ExecuteButton.IsEnabled = true;
        }

        /// <summary>
        /// Updates the application status to indicate that the user program has finished executing.
        /// </summary>
        private void Complete()
        {
            if (!this.StatusText.Text.StartsWith("Exception: ", StringComparison.Ordinal))
            {
                this.StatusText.Inlines.Clear();
                this.StatusText.Inlines.Add(new Run("Execution complete") { Foreground = Box.DefaultHighlight });
                this.StatusText.ToolTip = null;
            }

            this.ExecuteButton.Content = "Execute";
            this.ExecuteButton.IsEnabled = true;
            this.executing = false;
        }
        #endregion
    }
}
