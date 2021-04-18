using System;

namespace DTOGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateDto : Attribute
    {
        public GenerateDto(params string[] classNames)
        {
            ClassNames = classNames;
        }
        
        public GenerateDto(bool useDynamic, params string[] classNames)
        {
            UseDynamic = useDynamic;
            ClassNames = classNames;
        }

        public bool UseDynamic { get; set; }
        public string[] ClassNames { get; set; }
    }
}