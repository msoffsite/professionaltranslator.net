using System;

namespace Repository.ProfessionalTranslator.Net
{
    public enum Area
    {
        Root = 1,
        Admin = 2,
        Blog = 3
    }

    public enum Display
    {
        Approved,
        Unapproved
    }

    public enum ResultStatus
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
