using System;

namespace DTOGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcludeProperty : Attribute
    {
        public ExcludeProperty(params string[] classNames)
        {
            ClassNames = classNames;
        }

        public string[] ClassNames { get; set; }
    }
}