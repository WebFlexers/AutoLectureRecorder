﻿#pragma checksum "..\..\..\..\..\..\Pages\Lectures.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3D697D14EECC7F2593277A186D1A787FDB4882B0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AutoLectureRecorder.Pages;
using AutoLectureRecorder.Structure;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace AutoLectureRecorder.Pages {
    
    
    /// <summary>
    /// Lectures
    /// </summary>
    public partial class Lectures : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\..\..\..\Pages\Lectures.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListViewItem ListViewItemMonday;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\..\..\Pages\Lectures.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Frame FrameDayLectures;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AutoLectureRecorder;component/pages/lectures.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\Pages\Lectures.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ListViewItemMonday = ((System.Windows.Controls.ListViewItem)(target));
            
            #line 29 "..\..\..\..\..\..\Pages\Lectures.xaml"
            this.ListViewItemMonday.Selected += new System.Windows.RoutedEventHandler(this.ListViewItemMonday_Selected);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 30 "..\..\..\..\..\..\Pages\Lectures.xaml"
            ((System.Windows.Controls.ListViewItem)(target)).Selected += new System.Windows.RoutedEventHandler(this.ListViewItemTuesday_Selected);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 31 "..\..\..\..\..\..\Pages\Lectures.xaml"
            ((System.Windows.Controls.ListViewItem)(target)).Selected += new System.Windows.RoutedEventHandler(this.ListViewItemWednesday_Selected);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 32 "..\..\..\..\..\..\Pages\Lectures.xaml"
            ((System.Windows.Controls.ListViewItem)(target)).Selected += new System.Windows.RoutedEventHandler(this.ListViewItemThursday_Selected);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 33 "..\..\..\..\..\..\Pages\Lectures.xaml"
            ((System.Windows.Controls.ListViewItem)(target)).Selected += new System.Windows.RoutedEventHandler(this.ListViewItemFriday_Selected);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 34 "..\..\..\..\..\..\Pages\Lectures.xaml"
            ((System.Windows.Controls.ListViewItem)(target)).Selected += new System.Windows.RoutedEventHandler(this.ListViewItemSaturday_Selected);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 35 "..\..\..\..\..\..\Pages\Lectures.xaml"
            ((System.Windows.Controls.ListViewItem)(target)).Selected += new System.Windows.RoutedEventHandler(this.ListViewItemSunday_Selected);
            
            #line default
            #line hidden
            return;
            case 8:
            this.FrameDayLectures = ((System.Windows.Controls.Frame)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

