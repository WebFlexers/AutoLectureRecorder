﻿#pragma checksum "..\..\..\..\..\Pages\RecordPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "706353D4C95CF724EBC7BD353FCED2C62AFEBFD6"
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
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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
    /// RecordPage
    /// </summary>
    public partial class RecordPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 95 "..\..\..\..\..\Pages\RecordPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonRecord;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\..\..\Pages\RecordPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path RecordEllipse;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\..\..\..\Pages\RecordPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel StackPanelNextLecture;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\..\..\..\Pages\RecordPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBlockNextLecture;
        
        #line default
        #line hidden
        
        
        #line 140 "..\..\..\..\..\Pages\RecordPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel StackPanelStartTime;
        
        #line default
        #line hidden
        
        
        #line 144 "..\..\..\..\..\Pages\RecordPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBlockTime;
        
        #line default
        #line hidden
        
        
        #line 149 "..\..\..\..\..\Pages\RecordPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonEndLecture;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.6.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AutoLectureRecorder;component/pages/recordpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Pages\RecordPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.6.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ButtonRecord = ((System.Windows.Controls.Button)(target));
            
            #line 95 "..\..\..\..\..\Pages\RecordPage.xaml"
            this.ButtonRecord.Click += new System.Windows.RoutedEventHandler(this.ButtonRecord_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.RecordEllipse = ((System.Windows.Shapes.Path)(target));
            return;
            case 3:
            this.StackPanelNextLecture = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 4:
            this.TextBlockNextLecture = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.StackPanelStartTime = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.TextBlockTime = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.ButtonEndLecture = ((System.Windows.Controls.Button)(target));
            
            #line 159 "..\..\..\..\..\Pages\RecordPage.xaml"
            this.ButtonEndLecture.Click += new System.Windows.RoutedEventHandler(this.ButtonEndLecture_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

