﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InspectorPatterns {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("InspectorPatterns.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Type names should be all uppercase..
        /// </summary>
        internal static string AnalyzerDescription {
            get {
                return ResourceManager.GetString("AnalyzerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type name &apos;{0}&apos; contains lowercase letters.
        /// </summary>
        internal static string AnalyzerMessageFormat {
            get {
                return ResourceManager.GetString("AnalyzerMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type name contains lowercase letters.
        /// </summary>
        internal static string AnalyzerTitle {
            get {
                return ResourceManager.GetString("AnalyzerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Is a flyweight.
        /// </summary>
        internal static string FlyweightDescription {
            get {
                return ResourceManager.GetString("FlyweightDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It seems you have implemented the Flyweight pattern here. If you like more information about this pattern visit: https://refactoring.guru/design-patterns/flyweight.
        /// </summary>
        internal static string FlyweightMessage {
            get {
                return ResourceManager.GetString("FlyweightMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Flyweight.
        /// </summary>
        internal static string FlyweightTitle {
            get {
                return ResourceManager.GetString("FlyweightTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Is a singleton.
        /// </summary>
        internal static string SingletonDescription {
            get {
                return ResourceManager.GetString("SingletonDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It seems you have implemented the Singleton pattern here. If you like more information about this pattern visit: https://refactoring.guru/design-patterns/singleton.
        /// </summary>
        internal static string SingletonMessage {
            get {
                return ResourceManager.GetString("SingletonMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Singleton.
        /// </summary>
        internal static string SingletonTitle {
            get {
                return ResourceManager.GetString("SingletonTitle", resourceCulture);
            }
        }
    }
}
