﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IgorekBot.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("IgorekBot.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, введите код активации.
        /// </summary>
        internal static string AuthenticationDialog_Code_Prompt {
            get {
                return ResourceManager.GetString("AuthenticationDialog_Code_Prompt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Укажите рабочий email.
        /// </summary>
        internal static string AuthenticationDialog_EMail_Prompt {
            get {
                return ResourceManager.GetString("AuthenticationDialog_EMail_Prompt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Отмена.
        /// </summary>
        internal static string CancelCommand {
            get {
                return ResourceManager.GetString("CancelCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ввод отсутствия.
        /// </summary>
        internal static string EnterAbsenceCommand {
            get {
                return ResourceManager.GetString("EnterAbsenceCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Количество часов.
        /// </summary>
        internal static string HoursCommand {
            get {
                return ResourceManager.GetString("HoursCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Меню.
        /// </summary>
        internal static string MenuCommand {
            get {
                return ResourceManager.GetString("MenuCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Уведомления.
        /// </summary>
        internal static string NotificationsCommand {
            get {
                return ResourceManager.GetString("NotificationsCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Регистрация.
        /// </summary>
        internal static string RegistrationCommand {
            get {
                return ResourceManager.GetString("RegistrationCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Вам необходимо зарегистрироваться.
        /// </summary>
        internal static string RootDialog_Authentication_Message {
            get {
                return ResourceManager.GetString("RootDialog_Authentication_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ничего не понял.
        /// </summary>
        internal static string RootDialog_Didnt_Understand_Message {
            get {
                return ResourceManager.GetString("RootDialog_Didnt_Understand_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Игорек готов к работе!.
        /// </summary>
        internal static string RootDialog_Main_Message {
            get {
                return ResourceManager.GetString("RootDialog_Main_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Приветствую, {0}.
        /// </summary>
        internal static string RootDialog_Welcome_Message {
            get {
                return ResourceManager.GetString("RootDialog_Welcome_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Задачи.
        /// </summary>
        internal static string TasksCommand {
            get {
                return ResourceManager.GetString("TasksCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ТШ.
        /// </summary>
        internal static string TimeSheetCommand {
            get {
                return ResourceManager.GetString("TimeSheetCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Работаем с ТШ &amp;#128209;.
        /// </summary>
        internal static string TimeSheetDialog_Main_Message {
            get {
                return ResourceManager.GetString("TimeSheetDialog_Main_Message", resourceCulture);
            }
        }
    }
}
