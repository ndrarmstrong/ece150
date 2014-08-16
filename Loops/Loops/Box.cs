//-----------------------------------------------------------------------
// <copyright file="Box.cs" company="Nicholas Armstrong">
//     Created Sept. 2009 by Nicholas Armstrong.  Available online at http://nicholasarmstrong.com
// </copyright>
// <summary>
//     A single box displayed in the box grid.
// </summary>
//-----------------------------------------------------------------------

namespace NicholasArmstrong.Samples.ECE150.Loops
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// A single box displayed in the box grid.
    /// </summary>
    public class Box : Control
    {
        #region Fields
        /// <summary>
        /// The default highlight colour for a box.
        /// </summary>
        public static readonly Brush DefaultHighlight = Utilities.BrushFromString("#385dc9");

        /// <summary>
        /// DependencyProperty backing store for Highlighted.
        /// </summary>
        public static readonly DependencyProperty HighlightedProperty =
            DependencyProperty.Register("Highlighted", typeof(bool), typeof(Box), new UIPropertyMetadata(false));

        /// <summary>
        /// DependencyProperty backing store for HighlightColor.
        /// </summary>
        public static readonly DependencyProperty HighlightColorProperty =
            DependencyProperty.Register("HighlightColor", typeof(Brush), typeof(Box), new UIPropertyMetadata(DefaultHighlight)); 
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the box's position number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this box is highlighted.
        /// </summary>
        public bool Highlighted
        {
            get { return (bool)GetValue(HighlightedProperty); }
            set { SetValue(HighlightedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the box's highlight colour.
        /// </summary>
        public Brush HighlightColor
        {
            get { return (Brush)GetValue(HighlightColorProperty); }
            set { SetValue(HighlightColorProperty, value); }
        } 
        #endregion
    }
}
