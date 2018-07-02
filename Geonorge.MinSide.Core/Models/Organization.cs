namespace Geonorge.MinSide.Core.Models
{
    public class Organization
    {
        public OrganizationName Name { get; set; }
        public OrganizationNumber OrganizationNumber { get; set; }

        public Organization(string name, string organizationNumber)
        {
            Name = new OrganizationName(name);
            OrganizationNumber = new OrganizationNumber(organizationNumber);
        }
    }
}