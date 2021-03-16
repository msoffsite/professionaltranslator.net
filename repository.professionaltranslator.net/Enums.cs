using System;

namespace Repository.ProfessionalTranslator.Net
{
    public enum Area
    {
        Root = 1,
        Admin = 2
    }

    public enum Display
    {
        Approved,
        Unapproved
    }

    public enum SaveStatus
    {
        Failed,
        PartialSuccess,
        Succeeded,
        Undetermined
    }

    internal class Enumerators
    {
        internal class Values
        {
            internal static int Area(Area input)
            {
                var output = (int)Enum.Parse(typeof(Area), input.ToString());
                return output;
            }

            private Values() { }
        }

        private Enumerators() {}
    }
    
}
