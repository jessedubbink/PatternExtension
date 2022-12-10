using System;
using System.Collections.Generic;
using System.Text;

namespace InspectorPatterns.Core.Models
{
    public class ClassSyntaxModel
    {
        public string[] Modifiers { get; set; }
        public string[] Members { get; set; }
        public string[] Attributes { get; set; }
        public string Keyword { get; set; }
        public string Identifier { get; set; }
        public bool IsInterface { get; set; } = false;
        public bool IsAbstract { get; set; } = false;

        private List<string> _methods = new List<string>();
        private List<string> _properties = new List<string>();
        public List<ClassSyntaxModel> Parents = new List<ClassSyntaxModel>();
        public List<ClassSyntaxModel> ObjectCreations = new List<ClassSyntaxModel>();
    }
}
