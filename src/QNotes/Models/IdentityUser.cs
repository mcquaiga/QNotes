using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QNotes.API.Models
{
    public class IdentityUser : MongoEntity
    {
        public IdentityUser() { }
        public IdentityUser(string emailAddress, string firstName, string lastName, string authMethod)
        {
            EmailAddress = emailAddress;
            Name = $"{firstName} {lastName}";
            AuthenticationMethod = authMethod;
        }

        [Required]
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string FirstName => Name.Split(' ')[0];
        public string LastName => Name.Split(' ')[1];
        public string AuthenticationMethod { get; set; }

        public static IdentityUser CreateUserFromClaim(IEnumerable<Claim> securityClaims)
        {
            var allClaims = securityClaims.ToList();
            return new IdentityUser
            (
                emailAddress: allClaims.FirstOrDefault(c => c.Type.Contains("emailaddress")).Value,
                firstName: allClaims.FirstOrDefault(c => c.Type.Contains("givenname")).Value,
                lastName: allClaims.FirstOrDefault(c => c.Type.Contains("surname")).Value,
                authMethod: "google"
            );
        }
    }
}
