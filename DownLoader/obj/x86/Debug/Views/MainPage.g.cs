﻿#pragma checksum "D:\Study\Programs\TestTask\DownLoader\DownLoader\Views\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5C20CDCCDD4132770AE7EB6AF350E4F2"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DownLoader
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // Views\MainPage.xaml line 27
                {
                    this.MainWindow = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 3: // Views\MainPage.xaml line 119
                {
                    this.AddLinkPopup = (global::Windows.UI.Xaml.Controls.Primitives.Popup)(target);
                }
                break;
            case 4: // Views\MainPage.xaml line 171
                {
                    this.FileDelails = (global::Windows.UI.Xaml.Controls.Primitives.Popup)(target);
                }
                break;
            case 5: // Views\MainPage.xaml line 219
                {
                    this.lvDownloads = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    ((global::Windows.UI.Xaml.Controls.ListView)this.lvDownloads).ContainerContentChanging += this.LvDownloads_ContainerContentChanging;
                }
                break;
            case 6: // Views\MainPage.xaml line 309
                {
                    this.Queue = (global::Windows.UI.Xaml.Controls.Primitives.Popup)(target);
                }
                break;
            case 7: // Views\MainPage.xaml line 310
                {
                    this.spMain = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 8: // Views\MainPage.xaml line 339
                {
                    this.AddQueue = (global::Windows.UI.Xaml.Controls.Primitives.Popup)(target);
                }
                break;
            case 9: // Views\MainPage.xaml line 354
                {
                    this.EditQueue = (global::Windows.UI.Xaml.Controls.Primitives.Popup)(target);
                }
                break;
            case 10: // Views\MainPage.xaml line 317
                {
                    this.QueuesTable = (global::Windows.UI.Xaml.Controls.ListView)(target);
                }
                break;
            case 17: // Views\MainPage.xaml line 175
                {
                    this.tbFileName = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 18: // Views\MainPage.xaml line 179
                {
                    this.tbFileSize = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 19: // Views\MainPage.xaml line 184
                {
                    this.tbFileType = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 20: // Views\MainPage.xaml line 190
                {
                    this.tbDataTime = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 21: // Views\MainPage.xaml line 196
                {
                    this.tbpDescription = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 22: // Views\MainPage.xaml line 127
                {
                    this.tbLink = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 23: // Views\MainPage.xaml line 132
                {
                    this.tbDescription = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 24: // Views\MainPage.xaml line 138
                {
                    this.cbType = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 25: // Views\MainPage.xaml line 83
                {
                    this.bnAddLink = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 26: // Views\MainPage.xaml line 100
                {
                    this.bnStop = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 27: // Views\MainPage.xaml line 112
                {
                    this.bnSetting = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 28: // Views\MainPage.xaml line 57
                {
                    this.Expander2 = (global::Microsoft.Toolkit.Uwp.UI.Controls.Expander)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

