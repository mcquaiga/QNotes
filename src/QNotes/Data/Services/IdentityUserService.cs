using MongoDB.Driver;
using QNotes.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QNotes.API.Data.Services
{
    public class IdentityUserService : EntityService<IdentityUser>
    {
        public IdentityUserService(IConnectionHandler<IdentityUser> connectionHandler) : base(connectionHandler) { }

        public async Task<IdentityUser> FindUserByEmail(string emailAddress)
        {
            var filter = Builders<IdentityUser>.Filter.Eq(user => user.EmailAddress, emailAddress);
            var users = await ConnectionHandler.Find(filter);

            return users.FirstOrDefault();
        }

        public override Task UpdateAsync(IdentityUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
