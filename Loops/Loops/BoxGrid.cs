//-----------------------------------------------------------------------
// <copyright file="BoxGrid.cs" company="Nicholas Armstrong">
//     Created Sept. 2009 by Nicholas Armstrong.  Available online at http://nicholasarmstrong.com
// </copyright>
// <summary>
//     Grid that automatically populates itself with Boxes.
// </summary>
//-----------------------------------------------------------------------

namespace NicholasArmstrong.Samples.ECE150.Loops
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Grid that automatically populates itself with Boxes.
    /// </summary>
    public class BoxGrid : Grid
    {
        #region Fields
        /// <summary>
        /// The size of one box in the grid.
        /// </summary>
        private const double BoxSize = 12;

        /// <summary>
        /// The previous capactity of the grid.
        /// </summary>
        private int oldCapacity;

        /// <summary>
        /// The number of rows in the grid the last time the grid was measured.
        /// </summary>
        private int oldRows;

        /// <summary>
        /// The number of columns in the grid the last time the grid was measured.
        /// </summary>
        private int oldColumns; 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the total number of boxes held by the grid.
        /// </summary>
        public int Capacity
        {
            get { return this.oldCapacity; }
        }

        /// <summary>
        /// Gets the total number of rows in the grid.
        /// </summary>
        public int Rows
        {
            get { return this.oldRows; }
        }

        /// <summary>
        /// Gets the total number of columns in the grid.
        /// </summary>
        public int Columns
        {
            get { return this.oldColumns; }
        } 
        #endregion

        #region Methods
        /// <summary>
        /// Populates the grid with boxes.
        /// </summary>
        /// <param name="constraint">The amount of space available to layout in.</param>
        /// <returns>The amount of space used for layout.</returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            int numRows = (int)(constraint.Height / BoxSize);
            int numColumns = (int)(constraint.Width / BoxSize);
            int capacity = numRows * numColumns;

            // Regenerate the grid if it can hold a different amount of items
            if (capacity != this.oldCapacity)
            {
                if (capacity == 0)
                {
                    this.Children.Clear();
                    this.ColumnDefinitions.Clear();
                    this.RowDefinitions.Clear();
                }
                else
                {
                    // Adjust number of rows
                    if (this.RowDefinitions.Count <= numRows)
                    {
                        for (int i = this.RowDefinitions.Count; i < numRows; i++)
                        {
                            this.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        }
                    }
                    else
                    {
                        this.RowDefinitions.RemoveRange(numRows, this.RowDefinitions.Count - numRows);
                    }

                    // Adjust number of columns
                    if (this.ColumnDefinitions.Count <= numColumns)
                    {
                        for (int i = this.ColumnDefinitions.Count; i < numColumns; i++)
                        {
                            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                        }
                    }
                    else
                    {
                        this.ColumnDefinitions.RemoveRange(numColumns, this.ColumnDefinitions.Count - numColumns);
                    }

                    // Adjust number of children
                    if (capacity < this.oldCapacity)
                    {
                        this.Children.RemoveRange(capacity, this.oldCapacity - capacity);
                    }

                    // Re-index children
                    int childIndex = 0;
                    for (int i = 0; i < numRows; i++)
                    {
                        for (int j = 0; j < numColumns; j++)
                        {
                            Box square;

                            if (childIndex >= this.oldCapacity)
                            {
                                square = new Box();
                                this.Children.Add(square);
                            }
                            else
                            {
                                square = (Box)this.Children[childIndex];
                            }

                            SetRow(square, i);
                            SetColumn(square, j);
                            square.Number = childIndex;

                            childIndex++;
                        }
                    }
                }
            }

            this.oldCapacity = capacity;
            this.oldRows = numRows;
            this.oldColumns = numColumns;

            return base.MeasureOverride(constraint);
        } 
        #endregion
    }
}
