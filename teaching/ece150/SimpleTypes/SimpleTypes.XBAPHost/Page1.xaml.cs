﻿// <copyright file="Page1.xaml.cs" company="Nicholas Armstrong">
//     Created Sept. 2009 by Nicholas Armstrong.  Available online at http://nicholasarmstrong.com
// </copyright>
// <summary>
//     Demo application XBAP host application.
// </summary>
//-----------------------------------------------------------------------

namespace NicholasArmstrong.Samples.ECE150.SimpleTypes.XBAPHost
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Demo application XBAP host application.
    /// </summary>
    public partial class Page1 : Page
    {
        /// <summary>
        /// Initializes a new instance of the Page1 class.
        /// </summary>
        public Page1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the application version number.
        /// </summary>
        public string VersionNumber
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return String.Format("Version {0}.{1}", version.Major, version.Minor);
            }
        }

        /// <summary>
        /// Gets the application's short name.
        /// </summary>
        public string AppShortName
        {
            get
            {
                return "Simple Types";
            }
        }

        /// <summary>
        /// Gets the application's long name.
        /// </summary>
        public string AppLongName
        {
            get
            {
                return this.AppShortName + " Demo Application";
            }
        }

        /// <summary>
        /// Gets the application's title string.
        /// </summary>
        public string TitleString
        {
            get
            {
                return "ECE 150 Demo Application: " + this.AppShortName + " | NicholasArmstrong.com";
            }
        }

        /// <summary>
        /// Shows or hides the about screen.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Arguments describing the event.</param>
        private void AppTitle_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.AppAboutScreen.Visibility == Visibility.Collapsed)
            {
                this.AppAboutScreen.Visibility = Visibility.Visible;
            }
            else
            {
                this.AppAboutScreen.Visibility = Visibility.Collapsed;
            }
        }
    }
}
