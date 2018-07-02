namespace Geonorge.MinSide.Core.Models
{
    // Todo: localization
    public class OrganizationName
    {
        private readonly string _name;

        public OrganizationName(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}