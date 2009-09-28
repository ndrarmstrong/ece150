//-----------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Nicholas Armstrong">
//     Created Sept. 2009 by Nicholas Armstrong.  Available online at http://nicholasarmstrong.com
// </copyright>
// <summary>
//     Various utilities used throughout the demo.
// </summary>
//-----------------------------------------------------------------------

namespace NicholasArmstrong.Samples.ECE150.Loops
{
    using System.Windows.Media;

    /// <summary>
    /// Various utilities used throughout the demo.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Makes a brush from a string representation.
        /// </summary>
        /// <param name="color">The colour of the brush to make.</param>
        /// <returns>A brush of the specified colour.</returns>
        public static Brush BrushFromString(string color)
        {
            return (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString(color);
        }
    }
}
