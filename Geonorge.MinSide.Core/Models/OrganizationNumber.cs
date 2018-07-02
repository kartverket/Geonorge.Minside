namespace Geonorge.MinSide.Core.Models
{
    public class OrganizationNumber
    {
        private readonly string _organizationNumber;

        public OrganizationNumber(string organizationNumber)
        {
            // Todo: trim, compact and parse number
            _organizationNumber = organizationNumber;
        }

        public override string ToString()
        {
            return _organizationNumber.Substring(0, 3) + " " +
                   _organizationNumber.Substring(3, 3) + " " +
                   _organizationNumber.Substring(6, 3);
        }
    }
}